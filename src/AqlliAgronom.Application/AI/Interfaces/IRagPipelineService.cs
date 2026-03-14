using AqlliAgronom.Application.AI.Pipeline;

namespace AqlliAgronom.Application.AI.Interfaces;

public interface IRagPipelineService
{
    Task<RagPipelineResult> ExecuteAsync(RagPipelineContext context, CancellationToken ct = default);
}

public record RagPipelineResult(
    string Response,
    int TotalTokensUsed,
    IReadOnlyList<RetrievedKnowledgeChunk> UsedChunks,
    bool AskingClarification,
    IReadOnlyList<string>? SuggestedProductIds);
