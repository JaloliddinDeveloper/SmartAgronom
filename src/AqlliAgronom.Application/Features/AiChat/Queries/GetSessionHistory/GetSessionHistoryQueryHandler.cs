using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Application.Features.AiChat.DTOs;
using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.AiChat.Queries.GetSessionHistory;

public class GetSessionHistoryQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetSessionHistoryQuery, IReadOnlyList<ChatMessageDto>>
{
    public async Task<IReadOnlyList<ChatMessageDto>> Handle(
        GetSessionHistoryQuery request, CancellationToken ct)
    {
        var session = await uow.FarmerSessions.GetWithMessagesAsync(request.SessionId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.FarmerSession), request.SessionId);

        if (session.UserId != request.UserId)
            throw new ForbiddenException();

        return session.Messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatMessageDto(m.Id, m.Role.ToString(), m.Content, m.TokensUsed, m.CreatedAt))
            .ToList();
    }
}
