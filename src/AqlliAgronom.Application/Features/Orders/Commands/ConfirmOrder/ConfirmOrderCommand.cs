using MediatR;

namespace AqlliAgronom.Application.Features.Orders.Commands.ConfirmOrder;

/// <summary>Returns the farmer's UserId so the caller can notify them.</summary>
public record ConfirmOrderCommand(Guid OrderId, Guid AgronomiystId) : IRequest<Guid>;
