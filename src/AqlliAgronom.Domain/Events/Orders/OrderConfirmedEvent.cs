using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Events.Orders;

public sealed record OrderConfirmedEvent(Guid OrderId, Guid UserId, Guid AgronomiystId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
