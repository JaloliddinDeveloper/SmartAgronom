using AqlliAgronom.Application.Features.MathGame.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.MathGame.Commands.SubmitScore;

public record SubmitMathScoreCommand(
    string PlayerName,
    int Score,
    int CorrectAnswers,
    int BestStreak,
    int LevelReached,
    string Difficulty) : IRequest<MathScoreDto>;
