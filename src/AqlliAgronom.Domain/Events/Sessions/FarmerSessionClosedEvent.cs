using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Events.Sessions;

public sealed record FarmerSessionClosedEvent(Guid SessionId, Guid UserId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
