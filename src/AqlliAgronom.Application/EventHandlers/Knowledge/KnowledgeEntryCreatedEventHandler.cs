using AqlliAgronom.Application.AI.Interfaces;
using AqlliAgronom.Domain.Events.Knowledge;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.EventHandlers.Knowledge;

/// <summary>
/// Triggered after a KnowledgeEntry is created or updated.
/// Generates an embedding and stores it in Qdrant for semantic search.
/// </summary>
public class KnowledgeEntryCreatedEventHandler(
    IUnitOfWork uow,
    IEmbeddingService embeddingService,
    IVectorSearchService vectorSearchService,
    ILogger<KnowledgeEntryCreatedEventHandler> logger)
    : INotificationHandler<KnowledgeEntryCreatedEvent>
{
    private const string CollectionName = "agronomic_knowledge";

    public async Task Handle(KnowledgeEntryCreatedEvent notification, CancellationToken ct)
    {
        var entry = await uow.KnowledgeEntries.GetByIdAsync(notification.EntryId, ct);
        if (entry is null)
        {
            logger.LogWarning("KnowledgeEntry {EntryId} not found for indexing", notification.EntryId);
            return;
        }

        try
        {
            // Build the document text for embedding
            var document = entry.BuildDocument();

            // Generate embedding via Claude/Voyage
            var embedding = await embeddingService.GenerateEmbeddingAsync(document, ct);

            // Upsert to Qdrant with payload metadata
            var payload = new Dictionary<string, object>
            {
                ["knowledge_entry_id"] = entry.Id.ToString(),
                ["crop_name"] = entry.CropName,
                ["problem_name"] = entry.ProblemName,
                ["category"] = entry.Category.ToString(),
                ["language"] = (int)entry.Language,
                ["content"] = document,
                ["version"] = entry.Version,
                ["status"] = entry.Status.ToString()
            };

            var vectorId = await vectorSearchService.UpsertAsync(
                vector: embedding,
                payload: payload,
                collectionName: CollectionName,
                existingId: entry.VectorId,
                ct: ct);

            entry.SetVectorId(vectorId);
            uow.KnowledgeEntries.Update(entry);
            await uow.SaveChangesAsync(ct);

            logger.LogInformation(
                "KnowledgeEntry {EntryId} indexed successfully. VectorId: {VectorId}",
                entry.Id, vectorId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to index KnowledgeEntry {EntryId}", notification.EntryId);
            // Do not rethrow — indexing is async background work
        }
    }
}
