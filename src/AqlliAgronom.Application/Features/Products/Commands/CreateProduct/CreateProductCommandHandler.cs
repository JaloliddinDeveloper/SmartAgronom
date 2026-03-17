using AqlliAgronom.Application.Features.Products.DTOs;
using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IUnitOfWork uow, ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = Product.Create(
            name: request.Name,
            description: request.Description,
            usageInstructions: request.UsageInstructions,
            price: request.Price,
            currency: request.Currency,
            manufacturer: request.Manufacturer,
            activeIngredient: request.ActiveIngredient,
            category: request.Category,
            imageUrl: request.ImageUrl,
            createdById: request.CreatedById);

        await uow.Products.AddAsync(product, ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Product created: {ProductId} — {Name}", product.Id, product.Name);

        return new ProductDto(
            product.Id, product.Name, product.Description,
            product.Manufacturer, product.ActiveIngredient, product.Category,
            product.UsageInstructions, product.Price.Amount, product.Price.Currency,
            product.StockQuantity, product.IsAvailable, product.ImageUrl);
    }
}
