using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Events.Orders;

public sealed record OrderPlacedEvent(Guid OrderId, Guid UserId, string FarmerName, string Phone) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
