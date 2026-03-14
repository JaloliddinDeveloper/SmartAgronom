using System.Runtime.CompilerServices;
using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using AqlliAgronom.Application.AI.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace AqlliAgronom.Infrastructure.AI.Claude;

public class ClaudeAiService(
    IOptions<ClaudeOptions> options,
    ILogger<ClaudeAiService> logger)
    : IClaudeAiService
{
    private readonly ClaudeOptions _options = options.Value;
    private readonly AnthropicClient _client = new(options.Value.ApiKey);

    private readonly ResiliencePipeline _retryPipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder()
                .Handle<HttpRequestException>()
                .Handle<TaskCanceledException>(),
            MaxRetryAttempts = options.Value.MaxRetries,
            Delay = TimeSpan.FromMilliseconds(options.Value.RetryDelayMs),
            BackoffType = DelayBackoffType.Exponential,
            OnRetry = args =>
            {
                Console.WriteLine($"Claude API retry {args.AttemptNumber} after {args.RetryDelay.TotalSeconds:F1}s");
                return ValueTask.CompletedTask;
            }
        })
        .Build();

    public async Task<AiCompletionResult> CompleteAsync(
        IReadOnlyList<AiMessage> messages,
        string systemPrompt,
        int maxTokens = 4096,
        CancellationToken ct = default)
    {
        return await _retryPipeline.ExecuteAsync(async token =>
        {
            var claudeMessages = messages
                .Select(m => new Message
                {
                    Role = m.Role == "user" ? RoleType.User : RoleType.Assistant,
                    Content = [new TextContent { Text = m.Content }]
                })
                .ToList();

            var request = new MessageParameters
            {
                Model = _options.ModelId,
                MaxTokens = maxTokens,
                System = [new SystemMessage(systemPrompt)],
                Messages = claudeMessages
            };

            logger.LogDebug("Sending request to Claude API. Model: {Model}, Messages: {Count}",
                _options.ModelId, messages.Count);

            var response = await _client.Messages.GetClaudeMessageAsync(request, token);

            var content = response.Content
                .OfType<TextContent>()
                .Select(c => c.Text)
                .FirstOrDefault() ?? string.Empty;

            return new AiCompletionResult(
                Content: content,
                InputTokens: response.Usage.InputTokens,
                OutputTokens: response.Usage.OutputTokens,
                ModelVersion: response.Model ?? _options.ModelId,
                StopReason: response.StopReason ?? "end_turn");
        }, ct);
    }

    public async IAsyncEnumerable<string> StreamAsync(
        IReadOnlyList<AiMessage> messages,
        string systemPrompt,
        int maxTokens = 4096,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var claudeMessages = messages
            .Select(m => new Message
            {
                Role = m.Role == "user" ? RoleType.User : RoleType.Assistant,
                Content = [new TextContent { Text = m.Content }]
            })
            .ToList();

        var request = new MessageParameters
        {
            Model = _options.ModelId,
            MaxTokens = maxTokens,
            System = [new SystemMessage(systemPrompt)],
            Messages = claudeMessages,
            Stream = true
        };

        await foreach (var streamEvent in _client.Messages.StreamClaudeMessageAsync(request, ct))
        {
            var text = streamEvent.Delta?.Text;
            if (!string.IsNullOrEmpty(text))
                yield return text;
        }
    }
}
