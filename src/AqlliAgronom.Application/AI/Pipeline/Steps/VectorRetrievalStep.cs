using AqlliAgronom.Application.AI.Interfaces;

namespace AqlliAgronom.Application.AI.Pipeline.Steps;

public class VectorRetrievalStep(
    IEmbeddingService embeddingService,
    IVectorSearchService vectorSearchService) : IRagStep
{
    private const string CollectionName = "agronomic_knowledge";

    public int Order => 2;

    public async Task ExecuteAsync(RagPipelineContext context, CancellationToken ct)
    {
        // Generate embedding for normalized query (fall back to raw query if preprocessing was skipped)
        var queryText = string.IsNullOrWhiteSpace(context.NormalizedQuery)
            ? context.UserQuery
            : context.NormalizedQuery;
        var embedding = await embeddingService.GenerateEmbeddingAsync(queryText, ct);
        context.QueryEmbedding = embedding;

        // Build optional filters (e.g. language filter)
        var filters = new Dictionary<string, object>
        {
            ["language"] = (int)context.UserLanguage
        };

        // Search vector DB
        var results = await vectorSearchService.SearchAsync(
            queryVector: embedding,
            collectionName: CollectionName,
            limit: 8,
            scoreThreshold: 0.65f,
            filters: filters,
            ct: ct);

        // Map to domain chunks
        context.RetrievedChunks = results.Select(r => new RetrievedKnowledgeChunk(
            KnowledgeEntryId: r.Id,
            Content: r.Payload.TryGetValue("content", out var c) ? c?.ToString() ?? "" : "",
            Score: r.Score,
            CropName: r.Payload.TryGetValue("crop_name", out var cn) ? cn?.ToString() ?? "" : "",
            ProblemName: r.Payload.TryGetValue("problem_name", out var pn) ? pn?.ToString() ?? "" : "",
            Category: r.Payload.TryGetValue("category", out var cat) ? cat?.ToString() ?? "" : ""
        )).ToList();
    }
}
