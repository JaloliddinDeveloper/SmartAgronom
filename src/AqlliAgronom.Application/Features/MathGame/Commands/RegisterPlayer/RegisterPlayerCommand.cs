using MediatR;

namespace AqlliAgronom.Application.Features.MathGame.Commands.RegisterPlayer;

public record RegisterPlayerCommand(string Name) : IRequest;
