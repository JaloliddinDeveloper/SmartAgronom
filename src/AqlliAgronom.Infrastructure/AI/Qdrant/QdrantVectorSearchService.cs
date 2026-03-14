using AqlliAgronom.Application.AI.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace AqlliAgronom.Infrastructure.AI.Qdrant;

public class QdrantVectorSearchService(
    QdrantClient client,
    IOptions<QdrantOptions> options,
    ILogger<QdrantVectorSearchService> logger)
    : IVectorSearchService
{
    private readonly QdrantOptions _options = options.Value;

    public async Task<IReadOnlyList<VectorSearchResult>> SearchAsync(
        float[] queryVector,
        string collectionName,
        int limit = 5,
        float scoreThreshold = 0.7f,
        Dictionary<string, object>? filters = null,
        CancellationToken ct = default)
    {
        try
        {
            Filter? qdrantFilter = null;
            if (filters is not null && filters.Count > 0)
            {
                // Build Qdrant filter from dictionary
                var conditions = filters.Select(kvp =>
                    new Condition
                    {
                        Field = new FieldCondition
                        {
                            Key = kvp.Key,
                            Match = new Match { Integer = Convert.ToInt64(kvp.Value) }
                        }
                    }).ToList();

                qdrantFilter = new Filter();
                qdrantFilter.Must.AddRange(conditions);
            }

            var results = await client.SearchAsync(
                collectionName: collectionName,
                vector: queryVector,
                filter: qdrantFilter,
                limit: (ulong)limit,
                scoreThreshold: scoreThreshold,
                withPayload: true,
                cancellationToken: ct);

            return results.Select(r => new VectorSearchResult(
                Id: Guid.Parse(r.Id.Uuid),
                Score: r.Score,
                Payload: r.Payload.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object)ExtractPayloadValue(kvp.Value))
            )).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Qdrant search failed in collection {Collection}", collectionName);
            return [];
        }
    }

    public async Task<Guid> UpsertAsync(
        float[] vector,
        Dictionary<string, object> payload,
        string collectionName,
        Guid? existingId = null,
        CancellationToken ct = default)
    {
        var id = existingId ?? Guid.NewGuid();

        var qdrantPayload = payload.ToDictionary(
            kvp => kvp.Key,
            kvp => BuildQdrantValue(kvp.Value));

        var point = new PointStruct
        {
            Id = new PointId { Uuid = id.ToString() },
            Vectors = vector,
            Payload = { qdrantPayload }
        };

        await client.UpsertAsync(collectionName, [point], cancellationToken: ct);

        logger.LogDebug("Upserted vector {Id} to collection {Collection}", id, collectionName);
        return id;
    }

    public async Task DeleteAsync(Guid vectorId, string collectionName, CancellationToken ct = default)
    {
        await client.DeleteAsync(
            collectionName,
            new PointId { Uuid = vectorId.ToString() },
            cancellationToken: ct);
    }

    public async Task EnsureCollectionExistsAsync(
        string collectionName, int vectorDimension, CancellationToken ct = default)
    {
        var collections = await client.ListCollectionsAsync(ct);
        if (collections.Any(c => c == collectionName)) return;

        await client.CreateCollectionAsync(
            collectionName,
            new VectorsConfig
            {
                Params = new VectorParams
                {
                    Size = (ulong)vectorDimension,
                    Distance = Distance.Cosine
                }
            },
            cancellationToken: ct);

        // Create payload indexes for filtering
        await client.CreatePayloadIndexAsync(collectionName, "language",
            PayloadSchemaType.Integer, cancellationToken: ct);
        await client.CreatePayloadIndexAsync(collectionName, "crop_name",
            PayloadSchemaType.Keyword, cancellationToken: ct);

        logger.LogInformation("Created Qdrant collection: {Collection} ({Dim}D)", collectionName, vectorDimension);
    }

    private static string ExtractPayloadValue(Value value) => value.KindCase switch
    {
        Value.KindOneofCase.StringValue => value.StringValue,
        Value.KindOneofCase.IntegerValue => value.IntegerValue.ToString(),
        Value.KindOneofCase.DoubleValue => value.DoubleValue.ToString(),
        Value.KindOneofCase.BoolValue => value.BoolValue.ToString(),
        _ => string.Empty
    };

    private static Value BuildQdrantValue(object obj) => obj switch
    {
        string s => new Value { StringValue = s },
        int i => new Value { IntegerValue = i },
        long l => new Value { IntegerValue = l },
        double d => new Value { DoubleValue = d },
        float f => new Value { DoubleValue = f },
        bool b => new Value { BoolValue = b },
        _ => new Value { StringValue = obj.ToString() ?? string.Empty }
    };
}
