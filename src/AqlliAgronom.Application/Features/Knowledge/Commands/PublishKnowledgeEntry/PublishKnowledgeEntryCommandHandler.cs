using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.Knowledge.Commands.PublishKnowledgeEntry;

public class PublishKnowledgeEntryCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<PublishKnowledgeEntryCommand>
{
    public async Task Handle(PublishKnowledgeEntryCommand request, CancellationToken ct)
    {
        var entry = await uow.KnowledgeEntries.GetByIdAsync(request.EntryId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.KnowledgeEntry), request.EntryId);

        var userId = currentUser.UserId
            ?? throw new UnauthorizedException("User is not authenticated.");
        entry.Publish(userId);
        uow.KnowledgeEntries.Update(entry);
        await uow.SaveChangesAsync(ct);
    }
}
