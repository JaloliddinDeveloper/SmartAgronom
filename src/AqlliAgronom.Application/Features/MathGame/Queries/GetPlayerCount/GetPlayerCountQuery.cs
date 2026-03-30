using MediatR;

namespace AqlliAgronom.Application.Features.MathGame.Queries.GetPlayerCount;

public record GetPlayerCountQuery : IRequest<int>;
