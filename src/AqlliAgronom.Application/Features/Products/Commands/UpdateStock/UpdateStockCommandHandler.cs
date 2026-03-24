using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Application.Features.Products.DTOs;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.Features.Products.Commands.UpdateStock;

public class UpdateStockCommandHandler(IUnitOfWork uow, ILogger<UpdateStockCommandHandler> logger)
    : IRequestHandler<UpdateStockCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateStockCommand request, CancellationToken ct)
    {
        var product = await uow.Products.GetByIdAsync(request.ProductId, ct)
            ?? throw new NotFoundException("Product", request.ProductId.ToString());

        product.UpdateStock(request.Quantity);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Stock updated: {ProductId} → {Quantity}", product.Id, request.Quantity);

        return new ProductDto(
            product.Id, product.Name, product.Description,
            product.Manufacturer, product.ActiveIngredient, product.Category,
            product.UsageInstructions, product.Price.Amount, product.Price.Currency,
            product.StockQuantity, product.IsAvailable, product.ImageUrl);
    }
}
