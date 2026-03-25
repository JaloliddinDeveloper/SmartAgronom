using MediatR;

namespace AqlliAgronom.Application.Features.Knowledge.Commands.PublishKnowledgeEntry;

public record PublishKnowledgeEntryCommand(Guid EntryId, Guid? PublishedById = null) : IRequest;
