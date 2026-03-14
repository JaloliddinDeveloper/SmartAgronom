using AqlliAgronom.Domain.Common;
using AqlliAgronom.Domain.ValueObjects;

namespace AqlliAgronom.Domain.Events.Users;

public sealed record UserRegisteredEvent(Guid UserId, string FullName, PhoneNumber Phone) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
