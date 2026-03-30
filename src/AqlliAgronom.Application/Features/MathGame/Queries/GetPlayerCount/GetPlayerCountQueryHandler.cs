using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.MathGame.Queries.GetPlayerCount;

public class GetPlayerCountQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetPlayerCountQuery, int>
{
    public Task<int> Handle(GetPlayerCountQuery request, CancellationToken ct) =>
        uow.MathScores.GetPlayerCountAsync(ct);
}
