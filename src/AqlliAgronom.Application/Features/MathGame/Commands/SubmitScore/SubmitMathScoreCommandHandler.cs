using AqlliAgronom.Application.Features.MathGame.DTOs;
using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.MathGame.Commands.SubmitScore;

public class SubmitMathScoreCommandHandler(IUnitOfWork uow)
    : IRequestHandler<SubmitMathScoreCommand, MathScoreDto>
{
    public async Task<MathScoreDto> Handle(SubmitMathScoreCommand request, CancellationToken ct)
    {
        var score = MathScore.Create(
            playerName:     request.PlayerName,
            score:          request.Score,
            correctAnswers: request.CorrectAnswers,
            bestStreak:     request.BestStreak,
            levelReached:   request.LevelReached,
            difficulty:     request.Difficulty);

        await uow.MathScores.AddAsync(score, ct);
        await uow.SaveChangesAsync(ct);

        return ToDto(score);
    }

    private static MathScoreDto ToDto(MathScore s) =>
        new(s.Id, s.PlayerName, s.Score, s.CorrectAnswers, s.BestStreak, s.LevelReached, s.Difficulty, s.CreatedAt);
}
