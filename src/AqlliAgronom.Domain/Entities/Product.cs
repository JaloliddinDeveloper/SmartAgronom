using AqlliAgronom.Domain.Common;
using AqlliAgronom.Domain.ValueObjects;

namespace AqlliAgronom.Domain.Entities;

public class Product : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string? Manufacturer { get; private set; }
    public string? ActiveIngredient { get; private set; }
    public string? Category { get; private set; }
    public string UsageInstructions { get; private set; } = default!;
    public Money Price { get; private set; } = default!;
    public int StockQuantity { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public string? ImageUrl { get; private set; }
    public string? SafetyDataSheetUrl { get; private set; }

    private Product() { }

    public static Product Create(
        string name, string description, string usageInstructions,
        decimal price, string currency = "UZS",
        string? manufacturer = null, string? activeIngredient = null,
        string? category = null, string? imageUrl = null,
        Guid? createdById = null)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            UsageInstructions = usageInstructions,
            Price = Money.Create(price, currency),
            Manufacturer = manufacturer,
            ActiveIngredient = activeIngredient,
            Category = category,
            ImageUrl = imageUrl,
            StockQuantity = 0
        };

        if (createdById.HasValue) product.SetCreatedBy(createdById.Value);
        return product;
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("STOCK_NEGATIVE", "Stock quantity cannot be negative.");
        StockQuantity = quantity;
        IsAvailable = quantity > 0;
        SetUpdatedAt();
    }

    public void Update(string name, string description, string usageInstructions,
        decimal price, string currency, string? manufacturer,
        string? activeIngredient, string? category, string? imageUrl)
    {
        Name = name;
        Description = description;
        UsageInstructions = usageInstructions;
        Price = Money.Create(price, currency);
        Manufacturer = manufacturer;
        ActiveIngredient = activeIngredient;
        Category = category;
        ImageUrl = imageUrl;
        SetUpdatedAt();
    }
}
