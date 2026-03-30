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
        // Per-player best score only: DISTINCT ON groups by player_name and picks
        // the row with the highest score, so a lower attempt never displaces a
        // player's personal best on the leaderboard.
        var filterByDifficulty = !string.IsNullOrWhiteSpace(difficulty) && difficulty != "all";

        if (filterByDifficulty)
        {
            return await DbSet.FromSqlRaw(
                """
                SELECT * FROM (
                    SELECT DISTINCT ON (player_name) *
                    FROM math_scores
                    WHERE difficulty = {0}
                    ORDER BY player_name, score DESC, correct_answers DESC
                ) best
                ORDER BY score DESC, correct_answers DESC
                LIMIT {1}
                """, difficulty!, limit)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        return await DbSet.FromSqlRaw(
            """
            SELECT * FROM (
                SELECT DISTINCT ON (player_name) *
                FROM math_scores
                ORDER BY player_name, score DESC, correct_answers DESC
            ) best
            ORDER BY score DESC, correct_answers DESC
            LIMIT {0}
            """, limit)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
