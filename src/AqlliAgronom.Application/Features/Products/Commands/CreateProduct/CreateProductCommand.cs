using AqlliAgronom.Application.Features.Products.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    string UsageInstructions,
    decimal Price,
    string Currency = "UZS",
    string? Manufacturer = null,
    string? ActiveIngredient = null,
    string? Category = null,
    string? ImageUrl = null,
    Guid? CreatedById = null) : IRequest<ProductDto>;
