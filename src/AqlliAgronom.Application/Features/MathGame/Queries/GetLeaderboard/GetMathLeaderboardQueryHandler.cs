using AqlliAgronom.Application.Features.MathGame.DTOs;
using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.MathGame.Queries.GetLeaderboard;

public class GetMathLeaderboardQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMathLeaderboardQuery, IReadOnlyList<MathScoreDto>>
{
    public async Task<IReadOnlyList<MathScoreDto>> Handle(
        GetMathLeaderboardQuery request, CancellationToken ct)
    {
        var limit  = Math.Clamp(request.Limit, 1, 100);
        var scores = await uow.MathScores.GetTopScoresAsync(request.Difficulty, limit, ct);
        return scores.Select(ToDto).ToList();
    }

    private static MathScoreDto ToDto(MathScore s) =>
        new(s.Id, s.PlayerName, s.Score, s.CorrectAnswers, s.BestStreak, s.LevelReached, s.Difficulty, s.CreatedAt);
}
