using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AqlliAgronom.Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context is null) return;

        var now = DateTime.UtcNow;
        var userId = currentUser.UserId;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is AuditableEntity auditable && userId.HasValue)
                {
                    // Use reflection to set protected property
                    typeof(AuditableEntity)
                        .GetProperty(nameof(AuditableEntity.CreatedById))!
                        .SetValue(auditable, userId.Value);
                }
            }

            if (entry.State == EntityState.Modified)
            {
                // Set UpdatedAt via reflection (protected setter)
                typeof(BaseEntity)
                    .GetMethod("SetUpdatedAt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .Invoke(entry.Entity, null);

                if (entry.Entity is AuditableEntity auditable && userId.HasValue)
                {
                    typeof(AuditableEntity)
                        .GetProperty(nameof(AuditableEntity.UpdatedById))!
                        .SetValue(auditable, userId.Value);
                }
            }
        }
    }
}
