namespace AqlliAgronom.Application.AI.Pipeline.Steps;

public class ContextRankingStep : IRagStep
{
    public int Order => 3;

    // Approximate token budget for context window
    private const int MaxContextTokens = 6000;
    private const int AvgCharsPerToken = 4;

    public Task ExecuteAsync(RagPipelineContext context, CancellationToken ct)
    {
        var chunks = context.RetrievedChunks.ToList();

        // Boost score if chunk crop matches detected crop
        if (context.DetectedCrop is not null)
        {
            chunks = chunks
                .Select(c => c with
                {
                    Score = c.CropName.Contains(context.DetectedCrop, StringComparison.OrdinalIgnoreCase)
                        ? c.Score * 1.2f
                        : c.Score
                })
                .ToList();
        }

        // Sort by boosted score descending
        chunks.Sort((a, b) => b.Score.CompareTo(a.Score));

        // Trim to token budget
        var selected = new List<RetrievedKnowledgeChunk>();
        var usedChars = 0;
        foreach (var chunk in chunks)
        {
            var chunkChars = chunk.Content.Length;
            if (usedChars + chunkChars > MaxContextTokens * AvgCharsPerToken) break;
            selected.Add(chunk);
            usedChars += chunkChars;
        }

        context.RankedChunks = selected;
        return Task.CompletedTask;
    }
}
