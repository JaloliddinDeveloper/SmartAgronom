using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AqlliAgronom.Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext dbContext)
    : GenericRepository<Product>(dbContext), IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAvailableAsync(CancellationToken ct = default)
        => await DbSet.AsNoTracking()
            .Where(p => p.IsAvailable)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(string category, CancellationToken ct = default)
        => await DbSet.AsNoTracking()
            .Where(p => p.Category == category && p.IsAvailable)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Product>> SearchByNameAsync(string name, CancellationToken ct = default)
        => await DbSet.AsNoTracking()
            .Where(p => EF.Functions.ILike(p.Name, $"%{name}%"))
            .Take(50)
            .ToListAsync(ct);

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? searchTerm = null,
        bool? availableOnly = null, string? category = null,
        CancellationToken ct = default)
    {
        var q = DbSet.AsNoTracking().AsQueryable();

        if (availableOnly == true)
            q = q.Where(p => p.IsAvailable);
        if (!string.IsNullOrWhiteSpace(category))
            q = q.Where(p => p.Category == category);
        if (!string.IsNullOrWhiteSpace(searchTerm))
            q = q.Where(p =>
                EF.Functions.ILike(p.Name, $"%{searchTerm}%") ||
                EF.Functions.ILike(p.Description, $"%{searchTerm}%"));

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
