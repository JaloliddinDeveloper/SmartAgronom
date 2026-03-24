using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Application.Features.Products.DTOs;
using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await uow.Products.GetByIdAsync(request.ProductId, ct)
            ?? throw new NotFoundException("Product", request.ProductId.ToString());

        return new ProductDto(
            product.Id, product.Name, product.Description,
            product.Manufacturer, product.ActiveIngredient, product.Category,
            product.UsageInstructions, product.Price.Amount, product.Price.Currency,
            product.StockQuantity, product.IsAvailable, product.ImageUrl);
    }
}
