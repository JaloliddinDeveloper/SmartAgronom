using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Events.Sessions;

public sealed record FarmerSessionStartedEvent(Guid SessionId, Guid UserId, long TelegramChatId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
