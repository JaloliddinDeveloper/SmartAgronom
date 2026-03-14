using AqlliAgronom.Domain.Enums;

namespace AqlliAgronom.Application.AI.Pipeline;

public class RagPipelineContext
{
    public required string UserQuery { get; init; }
    public required Guid SessionId { get; init; }
    public required Guid UserId { get; init; }
    public required Language UserLanguage { get; init; }

    // Mutable pipeline state
    public string NormalizedQuery { get; set; } = string.Empty;
    public string? DetectedCrop { get; set; }
    public IReadOnlyList<string> DetectedSymptoms { get; set; } = [];
    public float[]? QueryEmbedding { get; set; }
    public IReadOnlyList<RetrievedKnowledgeChunk> RetrievedChunks { get; set; } = [];
    public IReadOnlyList<RetrievedKnowledgeChunk> RankedChunks { get; set; } = [];
    public string AssembledSystemPrompt { get; set; } = string.Empty;
    public IReadOnlyList<ConversationTurn> RecentHistory { get; set; } = [];
    public string FinalResponse { get; set; } = string.Empty;
    public int TotalTokensUsed { get; set; }
    public bool AskingClarification { get; set; }
    public IReadOnlyList<string> SuggestedProductIds { get; set; } = [];
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public record RetrievedKnowledgeChunk(
    Guid KnowledgeEntryId,
    string Content,
    float Score,
    string CropName,
    string ProblemName,
    string Category);

public record ConversationTurn(string Role, string Content);
