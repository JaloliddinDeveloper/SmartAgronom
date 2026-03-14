namespace AqlliAgronom.Application.Features.Products.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string? Manufacturer,
    string? ActiveIngredient,
    string? Category,
    string UsageInstructions,
    decimal Price,
    string Currency,
    int StockQuantity,
    bool IsAvailable,
    string? ImageUrl);

public record ProductSummaryDto(
    Guid Id,
    string Name,
    string? Category,
    decimal Price,
    string Currency,
    bool IsAvailable,
    string? ImageUrl);
