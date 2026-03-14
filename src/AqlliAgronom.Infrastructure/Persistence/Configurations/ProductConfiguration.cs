using AqlliAgronom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqlliAgronom.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).HasMaxLength(300).IsRequired();
        builder.Property(p => p.Description).HasColumnType("text").IsRequired();
        builder.Property(p => p.UsageInstructions).HasColumnType("text").IsRequired();
        builder.Property(p => p.Manufacturer).HasMaxLength(200);
        builder.Property(p => p.ActiveIngredient).HasMaxLength(200);
        builder.Property(p => p.Category).HasMaxLength(100);
        builder.Property(p => p.ImageUrl).HasMaxLength(500);
        builder.Property(p => p.SafetyDataSheetUrl).HasMaxLength(500);

        builder.OwnsOne(p => p.Price, money =>
        {
            money.Property(m => m.Amount).HasColumnName("price").HasColumnType("decimal(18,2)");
            money.Property(m => m.Currency).HasColumnName("price_currency").HasMaxLength(3);
        });

        builder.HasIndex(p => p.Name).HasDatabaseName("ix_products_name");
        builder.HasIndex(p => p.IsAvailable).HasDatabaseName("ix_products_is_available");
    }
}
