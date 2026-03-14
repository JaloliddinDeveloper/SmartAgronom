using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Events.Knowledge;

public sealed record KnowledgeEntryCreatedEvent(Guid EntryId, string CropName, string ProblemName) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
