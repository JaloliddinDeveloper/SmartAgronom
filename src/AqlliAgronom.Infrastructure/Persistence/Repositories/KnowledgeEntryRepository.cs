using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AqlliAgronom.Infrastructure.Persistence.Repositories;

public class KnowledgeEntryRepository(ApplicationDbContext dbContext)
    : GenericRepository<KnowledgeEntry>(dbContext), IKnowledgeEntryRepository
{
    public async Task<IReadOnlyList<KnowledgeEntry>> SearchByTextAsync(
        string query, Language? language = null, int limit = 20, CancellationToken ct = default)
    {
        var q = DbSet.AsNoTracking()
            .Where(e => e.Status == KnowledgeEntryStatus.Published);

        if (language.HasValue)
            q = q.Where(e => e.Language == language.Value);

        // PostgreSQL full-text search using EF LIKE (for basic implementation)
        // In production, consider using NpgsqlTsVector/TsQuery for FTS
        q = q.Where(e =>
            EF.Functions.ILike(e.Title, $"%{query}%") ||
            EF.Functions.ILike(e.CropName, $"%{query}%") ||
            EF.Functions.ILike(e.ProblemName, $"%{query}%") ||
            EF.Functions.ILike(e.Symptoms, $"%{query}%"));

        return await q.Take(limit).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<KnowledgeEntry>> GetByStatusAsync(
        KnowledgeEntryStatus status, CancellationToken ct = default)
        => await DbSet.AsNoTracking()
            .Where(e => e.Status == status)
            .OrderByDescending(e => e.UpdatedAt ?? e.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<KnowledgeEntry>> GetUnindexedPublishedAsync(CancellationToken ct = default)
        => await DbSet.AsNoTracking()
            .Where(e => e.Status == KnowledgeEntryStatus.Published && e.VectorId == null)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<KnowledgeEntry>> GetByCropAndCategoryAsync(
        string cropName, ProblemCategory? category = null, CancellationToken ct = default)
    {
        var q = DbSet.AsNoTracking()
            .Where(e => e.Status == KnowledgeEntryStatus.Published &&
                        EF.Functions.ILike(e.CropName, $"%{cropName}%"));

        if (category.HasValue)
            q = q.Where(e => e.Category == category.Value);

        return await q.ToListAsync(ct);
    }

    public async Task<(IReadOnlyList<KnowledgeEntry> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? searchTerm = null,
        KnowledgeEntryStatus? status = null, Language? language = null,
        CancellationToken ct = default)
    {
        var q = DbSet.AsNoTracking().AsQueryable();

        if (status.HasValue)
            q = q.Where(e => e.Status == status.Value);
        if (language.HasValue)
            q = q.Where(e => e.Language == language.Value);
        if (!string.IsNullOrWhiteSpace(searchTerm))
            q = q.Where(e =>
                EF.Functions.ILike(e.Title, $"%{searchTerm}%") ||
                EF.Functions.ILike(e.CropName, $"%{searchTerm}%") ||
                EF.Functions.ILike(e.ProblemName, $"%{searchTerm}%"));

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(e => e.UpdatedAt ?? e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
