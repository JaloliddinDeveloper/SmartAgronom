using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Enums;

namespace AqlliAgronom.Domain.Interfaces.Repositories;

public interface IKnowledgeEntryRepository : IRepository<KnowledgeEntry>
{
    Task<IReadOnlyList<KnowledgeEntry>> SearchByTextAsync(string query, Language? language = null, int limit = 20, CancellationToken ct = default);
    Task<IReadOnlyList<KnowledgeEntry>> GetByStatusAsync(KnowledgeEntryStatus status, CancellationToken ct = default);
    Task<IReadOnlyList<KnowledgeEntry>> GetUnindexedPublishedAsync(CancellationToken ct = default);
    Task<IReadOnlyList<KnowledgeEntry>> GetByCropAndCategoryAsync(string cropName, ProblemCategory? category = null, CancellationToken ct = default);
    Task<(IReadOnlyList<KnowledgeEntry> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? searchTerm = null,
        KnowledgeEntryStatus? status = null, Language? language = null,
        CancellationToken ct = default);
}
