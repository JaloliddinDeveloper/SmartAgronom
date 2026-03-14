using AqlliAgronom.Domain.Interfaces.Repositories;

namespace AqlliAgronom.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IFarmerSessionRepository FarmerSessions { get; }
    IKnowledgeEntryRepository KnowledgeEntries { get; }
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
