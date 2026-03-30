using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.MathGame.Commands.RegisterPlayer;

public class RegisterPlayerCommandHandler(IUnitOfWork uow)
    : IRequestHandler<RegisterPlayerCommand>
{
    public Task Handle(RegisterPlayerCommand request, CancellationToken ct) =>
        uow.EduPlayers.RegisterIfNewAsync(request.Name.Trim(), ct);
}
