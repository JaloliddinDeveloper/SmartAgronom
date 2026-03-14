using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Events.Users;

public sealed record UserProfileUpdatedEvent(Guid UserId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
