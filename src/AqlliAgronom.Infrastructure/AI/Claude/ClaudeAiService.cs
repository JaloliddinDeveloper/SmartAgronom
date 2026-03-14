using System.Runtime.CompilerServices;
using AqlliAgronom.Application.AI.Interfaces;
using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
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
    private readonly AnthropicClient _client = new(new APIAuthentication(options.Value.ApiKey));

    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TaskCanceledException>()
        .WaitAndRetryAsync(
            retryCount: options.Value.MaxRetries,
            sleepDurationProvider: attempt =>
                TimeSpan.FromMilliseconds(options.Value.RetryDelayMs * Math.Pow(2, attempt - 1)),
            onRetry: (ex, delay, attempt, _) =>
                Console.WriteLine($"Claude API retry {attempt} after {delay.TotalSeconds:F1}s: {ex.Message}"));

    public async Task<AiCompletionResult> CompleteAsync(
        IReadOnlyList<AiMessage> messages,
        string systemPrompt,
        int maxTokens = 4096,
        CancellationToken ct = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
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
                System = [new SystemMessage { Text = systemPrompt }],
                Messages = claudeMessages
            };

            logger.LogDebug("Sending request to Claude API. Model: {Model}, Messages: {Count}",
                _options.ModelId, messages.Count);

            var response = await _client.Messages.GetClaudeMessageAsync(request, ct);

            var content = response.Content
                .OfType<TextContent>()
                .Select(c => c.Text)
                .FirstOrDefault() ?? string.Empty;

            return new AiCompletionResult(
                Content: content,
                InputTokens: response.Usage.InputTokens,
                OutputTokens: response.Usage.OutputTokens,
                ModelVersion: response.Model,
                StopReason: response.StopReason ?? "end_turn");
        });
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
            System = [new SystemMessage { Text = systemPrompt }],
            Messages = claudeMessages
        };

        await foreach (var streamEvent in _client.Messages.StreamClaudeMessageAsync(request, ct))
        {
            if (streamEvent is ContentBlockDeltaEvent delta &&
                delta.Delta is TextDelta textDelta)
            {
                yield return textDelta.Text;
            }
        }
    }
}
