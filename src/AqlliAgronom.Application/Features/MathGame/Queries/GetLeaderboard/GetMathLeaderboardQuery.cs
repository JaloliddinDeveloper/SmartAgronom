using AqlliAgronom.Application.Features.MathGame.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.MathGame.Queries.GetLeaderboard;

public record GetMathLeaderboardQuery(
    string? Difficulty = null,
    int Limit = 20) : IRequest<IReadOnlyList<MathScoreDto>>;
