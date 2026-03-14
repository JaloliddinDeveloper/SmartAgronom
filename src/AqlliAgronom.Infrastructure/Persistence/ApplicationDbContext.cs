using AqlliAgronom.Domain.Common;
using AqlliAgronom.Domain.Entities;
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
    public DbSet<FarmerSession> FarmerSessions => Set<FarmerSession>();
    public DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();
    public DbSet<KnowledgeEntry> KnowledgeEntries => Set<KnowledgeEntry>();
    public DbSet<KnowledgeVersion> KnowledgeVersions => Set<KnowledgeVersion>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .AddInterceptors(auditInterceptor, domainEventInterceptor)
            .EnableSensitiveDataLogging(false); // Disable in production
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all IEntityTypeConfiguration<T> from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // PostgreSQL-specific: use snake_case naming
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Owned types share their owner's table — skip table/key renaming to avoid
            // PK constraint name conflicts (e.g. Order.FarmerPhone#PhoneNumber vs Order).
            if (!entity.IsOwned())
            {
                entity.SetTableName(ToSnakeCase(entity.GetTableName() ?? entity.ClrType.Name));

                foreach (var key in entity.GetKeys())
                    key.SetName(ToSnakeCase(key.GetName() ?? "pk"));
            }

            foreach (var property in entity.GetProperties())
            {
                var colName = property.GetColumnName();
                if (colName is not null)
                    property.SetColumnName(ToSnakeCase(colName));
            }

            foreach (var fk in entity.GetForeignKeys())
                fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName() ?? "fk"));

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName() ?? "idx"));
        }

        base.OnModelCreating(modelBuilder);
    }

    private static string ToSnakeCase(string name) =>
        string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? $"_{c}" : $"{c}")).ToLower();
}
