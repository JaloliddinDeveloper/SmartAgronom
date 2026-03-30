using AqlliAgronom.Domain.Entities;

namespace AqlliAgronom.Domain.Interfaces.Repositories;

public interface IMathScoreRepository : IRepository<MathScore>
{
    Task<IReadOnlyList<MathScore>> GetTopScoresAsync(
        string? difficulty, int limit, CancellationToken ct = default);

    Task<int> GetPlayerCountAsync(CancellationToken ct = default);
}
