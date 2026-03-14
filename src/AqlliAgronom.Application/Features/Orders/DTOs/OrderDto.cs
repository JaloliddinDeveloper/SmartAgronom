namespace AqlliAgronom.Application.Features.Orders.DTOs;

public record OrderDto(
    Guid Id,
    Guid UserId,
    string FarmerName,
    string FarmerPhone,
    string Region,
    string Status,
    decimal TotalAmount,
    string Currency,
    IReadOnlyList<OrderItemDto> Items,
    string? Notes,
    DateTime CreatedAt);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    string Currency);
