using AqlliAgronom.Domain.Interfaces;
using AqlliAgronom.Domain.Interfaces.Repositories;
using AqlliAgronom.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace AqlliAgronom.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public IUserRepository Users { get; } = new UserRepository(dbContext);
    public IFarmerSessionRepository FarmerSessions { get; } = new FarmerSessionRepository(dbContext);
    public IKnowledgeEntryRepository KnowledgeEntries { get; } = new KnowledgeEntryRepository(dbContext);
    public IProductRepository Products { get; } = new ProductRepository(dbContext);
    public IOrderRepository Orders { get; } = new OrderRepository(dbContext);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await dbContext.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await dbContext.Database.BeginTransactionAsync(ct);

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        dbContext.Dispose();
    }
}
