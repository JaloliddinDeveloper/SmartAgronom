using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Application.Features.Orders.DTOs;
using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.Features.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandHandler(IUnitOfWork uow, ILogger<PlaceOrderCommandHandler> logger)
    : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        // Validate all products exist and are available
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await uow.Products.FindAsync(p => productIds.Contains(p.Id), ct);

        if (products.Count != productIds.Count)
            throw new NotFoundException("One or more products", "requested products");

        var unavailable = products.Where(p => !p.IsAvailable).Select(p => p.Name).ToList();
        if (unavailable.Count > 0)
            throw new ValidationException(
                [new FluentValidation.Results.ValidationFailure("Products",
                    $"The following products are out of stock: {string.Join(", ", unavailable)}")]);

        // Create order aggregate
        var order = Order.Place(
            userId: request.UserId,
            farmerName: request.FarmerName,
            phone: request.Phone,
            region: request.Region,
            cropDescription: request.CropDescription,
            problemDescription: request.ProblemDescription,
            notes: request.Notes);

        // Add items with unit prices from DB
        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            order.AddItem(product.Id, product.Name, item.Quantity, product.Price);
        }

        await uow.Orders.AddAsync(order, ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Order {OrderId} placed by farmer {UserId}", order.Id, request.UserId);

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order o) => new(
        o.Id, o.UserId, o.FarmerName, o.FarmerPhone,
        o.Region, o.Status.ToString(), o.TotalAmount.Amount, o.TotalAmount.Currency,
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Quantity,
            i.UnitPrice.Amount, i.UnitPrice.Currency)).ToList(),
        o.Notes, o.CreatedAt);
}
