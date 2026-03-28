using AqlliAgronom.Domain.Common;
using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Infrastructure.Identity;
using AqlliAgronom.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace AqlliAgronom.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    AuditableEntitySaveChangesInterceptor auditInterceptor,
    DomainEventDispatchInterceptor domainEventInterceptor)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshTokenRecord> RefreshTokens => Set<RefreshTokenRecord>();
    public DbSet<FarmerSession> FarmerSessions => Set<FarmerSession>();
    public DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();
    public DbSet<KnowledgeEntry> KnowledgeEntries => Set<KnowledgeEntry>();
    public DbSet<KnowledgeVersion> KnowledgeVersions => Set<KnowledgeVersion>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<MathScore> MathScores => Set<MathScore>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Interceptors are null when running via IDesignTimeDbContextFactory (migrations tooling)
        if (auditInterceptor is not null && domainEventInterceptor is not null)
            optionsBuilder.AddInterceptors(auditInterceptor, domainEventInterceptor);

        optionsBuilder.EnableSensitiveDataLogging(false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all IEntityTypeConfiguration<T> from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Snake_case naming is applied via UseSnakeCaseNamingConvention() in DependencyInjection.cs
        // (EFCore.NamingConventions package). No manual renaming needed here.

        base.OnModelCreating(modelBuilder);
    }

}
