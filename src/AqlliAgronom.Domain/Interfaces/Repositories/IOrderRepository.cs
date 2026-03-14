using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Enums;

namespace AqlliAgronom.Domain.Interfaces.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithItemsAsync(Guid orderId, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default);
    Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, OrderStatus? status = null,
        Guid? userId = null, CancellationToken ct = default);
}
