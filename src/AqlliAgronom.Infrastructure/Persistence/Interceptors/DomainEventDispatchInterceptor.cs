using AqlliAgronom.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AqlliAgronom.Infrastructure.Persistence.Interceptors;

public class DomainEventDispatchInterceptor(IMediator mediator) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken ct = default)
    {
        await DispatchDomainEventsAsync(eventData.Context, ct);
        return await base.SavedChangesAsync(eventData, result, ct);
    }

    private async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken ct)
    {
        if (context is null) return;

        // Collect all domain events from tracked aggregates
        var aggregates = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Count > 0)
            .ToList();

        var events = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        // Clear events before dispatch to prevent re-dispatch
        foreach (var aggregate in aggregates)
            aggregate.ClearDomainEvents();

        // Dispatch all events
        foreach (var domainEvent in events)
            await mediator.Publish(domainEvent, ct);
    }
}
