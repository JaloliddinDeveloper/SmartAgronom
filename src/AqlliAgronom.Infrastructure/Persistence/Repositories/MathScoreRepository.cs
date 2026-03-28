using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AqlliAgronom.Infrastructure.Persistence.Repositories;

public class MathScoreRepository(ApplicationDbContext dbContext)
    : GenericRepository<MathScore>(dbContext), IMathScoreRepository
{
    public async Task<IReadOnlyList<MathScore>> GetTopScoresAsync(
        string? difficulty, int limit, CancellationToken ct = default)
    {
        var query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(difficulty) && difficulty != "all")
            query = query.Where(s => s.Difficulty == difficulty);

        return await query
            .OrderByDescending(s => s.Score)
            .ThenByDescending(s => s.CorrectAnswers)
            .Take(limit)
            .ToListAsync(ct);
    }
}
