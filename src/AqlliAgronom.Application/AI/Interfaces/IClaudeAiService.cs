namespace AqlliAgronom.Application.AI.Interfaces;

public interface IClaudeAiService
{
    Task<AiCompletionResult> CompleteAsync(
        IReadOnlyList<AiMessage> messages,
        string systemPrompt,
        int maxTokens = 4096,
        CancellationToken ct = default);

    IAsyncEnumerable<string> StreamAsync(
        IReadOnlyList<AiMessage> messages,
        string systemPrompt,
        int maxTokens = 4096,
        CancellationToken ct = default);
}

public record AiMessage(string Role, string Content);

public record AiCompletionResult(
    string Content,
    int InputTokens,
    int OutputTokens,
    string ModelVersion,
    string StopReason);
