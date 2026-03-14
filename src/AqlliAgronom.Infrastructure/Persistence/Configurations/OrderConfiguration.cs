using AqlliAgronom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqlliAgronom.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.FarmerName).HasMaxLength(200).IsRequired();

        builder.OwnsOne(o => o.FarmerPhone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("farmer_phone")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(o => o.Region).HasMaxLength(200).IsRequired();
        builder.Property(o => o.Status).HasConversion<int>();
        builder.Property(o => o.CropDescription).HasColumnType("text");
        builder.Property(o => o.ProblemDescription).HasColumnType("text");
        builder.Property(o => o.Notes).HasColumnType("text");

        builder.OwnsOne(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("total_amount").HasColumnType("decimal(18,2)");
            money.Property(m => m.Currency).HasColumnName("total_currency").HasMaxLength(3);
        });

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Order.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(o => o.UserId).HasDatabaseName("ix_orders_user_id");
        builder.HasIndex(o => o.Status).HasDatabaseName("ix_orders_status");
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ProductName).HasMaxLength(300).IsRequired();
        builder.Property(i => i.Quantity).IsRequired();

        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("unit_price").HasColumnType("decimal(18,2)");
            money.Property(m => m.Currency).HasColumnName("unit_currency").HasMaxLength(3);
        });
    }
}
