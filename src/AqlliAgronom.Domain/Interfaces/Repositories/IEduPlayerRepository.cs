namespace AqlliAgronom.Domain.Interfaces.Repositories;

public interface IEduPlayerRepository
{
    Task<int> CountAsync(CancellationToken ct = default);
    Task RegisterIfNewAsync(string name, CancellationToken ct = default);
}
