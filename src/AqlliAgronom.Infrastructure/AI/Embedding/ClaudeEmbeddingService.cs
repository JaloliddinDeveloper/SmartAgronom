using AqlliAgronom.Application.AI.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace AqlliAgronom.Infrastructure.AI.Embedding;

/// <summary>
/// Uses Voyage AI API (voyageai.com) for text embeddings —
/// Anthropic's recommended embedding model optimized for RAG.
/// Falls back to a simple TF-IDF hash embedding if API is unavailable.
/// </summary>
public class ClaudeEmbeddingService(
    HttpClient httpClient,
    IOptions<EmbeddingOptions> options,
    ILogger<ClaudeEmbeddingService> logger)
    : IEmbeddingService
{
    private readonly EmbeddingOptions _options = options.Value;

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct = default)
    {
        try
        {
            var request = new
            {
                input = text,
                model = _options.ModelId,
                input_type = "document"
            };

            var response = await httpClient.PostAsJsonAsync("/v1/embeddings", request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<VoyageEmbeddingResponse>(ct);
            return json?.Data?.FirstOrDefault()?.Embedding
                   ?? throw new InvalidOperationException("No embedding returned from Voyage API.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to generate embedding for text (length: {Length}). Using fallback.", text.Length);
            // Fallback: generate a deterministic hash-based pseudo-embedding
            return GenerateFallbackEmbedding(text, _options.VectorDimension);
        }
    }

    public async Task<IReadOnlyList<float[]>> GenerateBatchEmbeddingsAsync(
        IReadOnlyList<string> texts, CancellationToken ct = default)
    {
        // Process in batches of 96 (Voyage limit)
        const int batchSize = 96;
        var results = new List<float[]>();

        for (int i = 0; i < texts.Count; i += batchSize)
        {
            var batch = texts.Skip(i).Take(batchSize).ToList();
            var request = new { input = batch, model = _options.ModelId, input_type = "document" };

            var response = await httpClient.PostAsJsonAsync("/v1/embeddings", request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<VoyageEmbeddingResponse>(ct);
            results.AddRange(json?.Data?.Select(d => d.Embedding) ?? []);
        }

        return results;
    }

    private static float[] GenerateFallbackEmbedding(string text, int dimension)
    {
        var embedding = new float[dimension];
        var words = text.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            var hash = Math.Abs(word.GetHashCode());
            var idx = hash % dimension;
            embedding[idx] += 1.0f / words.Length;
        }
        // L2 normalize
        var norm = (float)Math.Sqrt(embedding.Sum(v => v * v));
        if (norm > 0) for (int i = 0; i < dimension; i++) embedding[i] /= norm;
        return embedding;
    }

    private record VoyageEmbeddingResponse(List<VoyageEmbeddingData> Data);
    private record VoyageEmbeddingData(float[] Embedding);
}
