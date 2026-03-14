using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Events.Orders;

public sealed record OrderCancelledEvent(Guid OrderId, Guid UserId, string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
