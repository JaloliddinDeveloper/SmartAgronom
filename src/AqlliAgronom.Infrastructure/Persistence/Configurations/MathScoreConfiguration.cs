using AqlliAgronom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqlliAgronom.Infrastructure.Persistence.Configurations;

public class MathScoreConfiguration : IEntityTypeConfiguration<MathScore>
{
    public void Configure(EntityTypeBuilder<MathScore> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.PlayerName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Difficulty)
            .HasMaxLength(10)
            .IsRequired();

        builder.HasIndex(s => new { s.Difficulty, s.Score })
            .HasDatabaseName("ix_math_scores_difficulty_score");

        builder.HasIndex(s => s.Score)
            .HasDatabaseName("ix_math_scores_score");
    }
}
