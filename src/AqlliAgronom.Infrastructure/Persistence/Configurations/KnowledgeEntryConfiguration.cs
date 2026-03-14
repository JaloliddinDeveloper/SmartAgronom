using AqlliAgronom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqlliAgronom.Infrastructure.Persistence.Configurations;

public class KnowledgeEntryConfiguration : IEntityTypeConfiguration<KnowledgeEntry>
{
    public void Configure(EntityTypeBuilder<KnowledgeEntry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).HasMaxLength(500).IsRequired();
        builder.Property(e => e.CropName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.ProblemName).HasMaxLength(300).IsRequired();
        builder.Property(e => e.Category).HasConversion<int>();
        builder.Property(e => e.Language).HasConversion<int>();
        builder.Property(e => e.Status).HasConversion<int>();

        // Long text columns
        builder.Property(e => e.Symptoms).HasColumnType("text").IsRequired();
        builder.Property(e => e.DetailedExplanation).HasColumnType("text").IsRequired();
        builder.Property(e => e.RecommendedProducts).HasColumnType("text").IsRequired();
        builder.Property(e => e.DosagePerHectare).HasColumnType("text").IsRequired();
        builder.Property(e => e.ApplicationInstructions).HasColumnType("text").IsRequired();
        builder.Property(e => e.SafetyPrecautions).HasColumnType("text").IsRequired();
        builder.Property(e => e.GrowthStage).HasMaxLength(200);
        builder.Property(e => e.RegionConsiderations).HasColumnType("text");

        // Tags stored as PostgreSQL text array
        builder.Property(e => e.Tags)
            .HasColumnType("text[]")
            .HasDefaultValue(Array.Empty<string>());

        // Qdrant vector ID
        builder.HasIndex(e => e.VectorId)
            .IsUnique()
            .HasFilter("vector_id IS NOT NULL")
            .HasDatabaseName("ix_knowledge_entries_vector_id");

        // Versions relationship
        builder.HasMany(e => e.Versions)
            .WithOne()
            .HasForeignKey(v => v.KnowledgeEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for search
        builder.HasIndex(e => new { e.CropName, e.Category })
            .HasDatabaseName("ix_knowledge_entries_crop_category");
        builder.HasIndex(e => e.Status)
            .HasDatabaseName("ix_knowledge_entries_status");

        // Ignore navigation (loaded via HasMany)
        builder.Metadata.FindNavigation(nameof(KnowledgeEntry.Versions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
