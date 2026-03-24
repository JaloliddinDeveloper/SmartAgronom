using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Application.Features.Products.DTOs;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IUnitOfWork uow, ILogger<UpdateProductCommandHandler> logger)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await uow.Products.GetByIdAsync(request.ProductId, ct)
            ?? throw new NotFoundException("Product", request.ProductId.ToString());

        product.Update(
            name: request.Name,
            description: request.Description,
            usageInstructions: request.UsageInstructions,
            price: request.Price,
            currency: request.Currency,
            manufacturer: request.Manufacturer,
            activeIngredient: request.ActiveIngredient,
            category: request.Category,
            imageUrl: request.ImageUrl);

        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Product updated: {ProductId} — {Name}", product.Id, product.Name);

        return new ProductDto(
            product.Id, product.Name, product.Description,
            product.Manufacturer, product.ActiveIngredient, product.Category,
            product.UsageInstructions, product.Price.Amount, product.Price.Currency,
            product.StockQuantity, product.IsAvailable, product.ImageUrl);
    }
}
