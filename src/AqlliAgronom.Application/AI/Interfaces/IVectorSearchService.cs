namespace AqlliAgronom.Application.AI.Interfaces;

public interface IVectorSearchService
{
    Task<IReadOnlyList<VectorSearchResult>> SearchAsync(
        float[] queryVector,
        string collectionName,
        int limit = 5,
        float scoreThreshold = 0.7f,
        Dictionary<string, object>? filters = null,
        CancellationToken ct = default);

    Task<Guid> UpsertAsync(
        float[] vector,
        Dictionary<string, object> payload,
        string collectionName,
        Guid? existingId = null,
        CancellationToken ct = default);

    Task DeleteAsync(Guid vectorId, string collectionName, CancellationToken ct = default);

    Task EnsureCollectionExistsAsync(string collectionName, int vectorDimension, CancellationToken ct = default);
}

public record VectorSearchResult(
    Guid Id,
    float Score,
    Dictionary<string, object> Payload);
