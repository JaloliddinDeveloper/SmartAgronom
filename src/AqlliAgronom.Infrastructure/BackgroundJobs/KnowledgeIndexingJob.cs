using AqlliAgronom.Application.AI.Interfaces;
using AqlliAgronom.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Infrastructure.BackgroundJobs;

/// <summary>
/// Background job that periodically finds published KnowledgeEntries without
/// a VectorId and indexes them into Qdrant.
/// </summary>
public class KnowledgeIndexingJob(
    IServiceProvider serviceProvider,
    ILogger<KnowledgeIndexingJob> logger)
    : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);
    private const string CollectionName = "agronomic_knowledge";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Knowledge Indexing Job started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await IndexPendingEntriesAsync(stoppingToken);
            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    private async Task IndexPendingEntriesAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();
        var vectorService = scope.ServiceProvider.GetRequiredService<IVectorSearchService>();

        var entries = await uow.KnowledgeEntries.GetUnindexedPublishedAsync(ct);
        if (entries.Count == 0) return;

        logger.LogInformation("Indexing {Count} unindexed knowledge entries...", entries.Count);

        var indexed = 0;
        foreach (var entry in entries)
        {
            try
            {
                var document = entry.BuildDocument();
                var embedding = await embeddingService.GenerateEmbeddingAsync(document, ct);

                var payload = new Dictionary<string, object>
                {
                    ["knowledge_entry_id"] = entry.Id.ToString(),
                    ["crop_name"] = entry.CropName,
                    ["problem_name"] = entry.ProblemName,
                    ["category"] = entry.Category.ToString(),
                    ["language"] = (int)entry.Language,
                    ["content"] = document,
                    ["version"] = entry.Version
                };

                var vectorId = await vectorService.UpsertAsync(embedding, payload, CollectionName, ct: ct);
                entry.SetVectorId(vectorId);
                uow.KnowledgeEntries.Update(entry);

                logger.LogDebug("Indexed entry {EntryId} → vector {VectorId}", entry.Id, vectorId);
                indexed++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to index entry {EntryId}", entry.Id);
            }
        }

        if (indexed > 0)
            await uow.SaveChangesAsync(ct);

        logger.LogInformation("Indexing batch completed: {Indexed}/{Total} entries", indexed, entries.Count);
    }
}
