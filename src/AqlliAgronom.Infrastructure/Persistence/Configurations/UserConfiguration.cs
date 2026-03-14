using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqlliAgronom.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .HasMaxLength(200)
            .IsRequired();

        // PhoneNumber value object stored as owned
        builder.OwnsOne(u => u.Phone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("phone")
                .HasMaxLength(20)
                .IsRequired();
            phone.HasIndex(p => p.Value).IsUnique().HasDatabaseName("ix_users_phone");
        });

        builder.Property(u => u.Email).HasMaxLength(320);
        builder.Property(u => u.PasswordHash).HasMaxLength(128).IsRequired();
        builder.Property(u => u.Role).HasConversion<int>();
        builder.Property(u => u.PreferredLanguage).HasConversion<int>();
        builder.Property(u => u.Region).HasMaxLength(200);

        builder.HasIndex(u => u.TelegramChatId)
            .IsUnique()
            .HasFilter("telegram_chat_id IS NOT NULL")
            .HasDatabaseName("ix_users_telegram_chat_id");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("email IS NOT NULL")
            .HasDatabaseName("ix_users_email");
    }
}
