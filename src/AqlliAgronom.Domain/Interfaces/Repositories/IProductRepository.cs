using AqlliAgronom.Domain.Entities;

namespace AqlliAgronom.Domain.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetAvailableAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> SearchByNameAsync(string name, CancellationToken ct = default);
    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? searchTerm = null,
        bool? availableOnly = null, string? category = null,
        CancellationToken ct = default);
}
