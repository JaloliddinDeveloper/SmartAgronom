using AqlliAgronom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqlliAgronom.Infrastructure.Persistence.Configurations;

public class FarmerSessionConfiguration : IEntityTypeConfiguration<FarmerSession>
{
    public void Configure(EntityTypeBuilder<FarmerSession> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TelegramChatId).IsRequired();
        builder.Property(s => s.Status).HasConversion<int>();
        builder.Property(s => s.CurrentTopic).HasMaxLength(500);

        builder.HasMany(s => s.Messages)
            .WithOne()
            .HasForeignKey(m => m.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(FarmerSession.Messages))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(s => new { s.TelegramChatId, s.Status })
            .HasDatabaseName("ix_farmer_sessions_telegram_status");
        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("ix_farmer_sessions_user_id");
    }
}

public class ConversationMessageConfiguration : IEntityTypeConfiguration<ConversationMessage>
{
    public void Configure(EntityTypeBuilder<ConversationMessage> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Content).HasColumnType("text").IsRequired();
        builder.Property(m => m.Role).HasConversion<int>();
        builder.Property(m => m.ModelVersion).HasMaxLength(100);
        builder.HasIndex(m => new { m.SessionId, m.CreatedAt })
            .HasDatabaseName("ix_conversation_messages_session_created");
    }
}
