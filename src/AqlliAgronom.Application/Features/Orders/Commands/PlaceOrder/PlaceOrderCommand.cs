using AqlliAgronom.Application.Features.Orders.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.Orders.Commands.PlaceOrder;

public record PlaceOrderCommand(
    Guid UserId,
    string FarmerName,
    string Phone,
    string Region,
    IReadOnlyList<OrderItemRequest> Items,
    string? CropDescription = null,
    string? ProblemDescription = null,
    string? Notes = null) : IRequest<OrderDto>;

public record OrderItemRequest(Guid ProductId, int Quantity);
