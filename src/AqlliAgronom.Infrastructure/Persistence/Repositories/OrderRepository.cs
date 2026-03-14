using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AqlliAgronom.Infrastructure.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext dbContext)
    : GenericRepository<Order>(dbContext), IOrderRepository
{
    public async Task<Order?> GetWithItemsAsync(Guid orderId, CancellationToken ct = default)
        => await DbSet
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);

    public async Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await DbSet
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default)
        => await DbSet
            .Include(o => o.Items)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, OrderStatus? status = null,
        Guid? userId = null, CancellationToken ct = default)
    {
        var q = DbSet.Include(o => o.Items).AsNoTracking().AsQueryable();

        if (status.HasValue)
            q = q.Where(o => o.Status == status.Value);
        if (userId.HasValue)
            q = q.Where(o => o.UserId == userId.Value);

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
