using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Events.Knowledge;

public sealed record KnowledgeEntryUpdatedEvent(Guid EntryId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
