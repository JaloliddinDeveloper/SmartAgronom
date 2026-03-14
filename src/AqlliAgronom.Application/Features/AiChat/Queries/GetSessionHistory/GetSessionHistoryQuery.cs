using AqlliAgronom.Application.Features.AiChat.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.AiChat.Queries.GetSessionHistory;

public record GetSessionHistoryQuery(Guid SessionId, Guid UserId) : IRequest<IReadOnlyList<ChatMessageDto>>;

public record ChatMessageDto(Guid Id, string Role, string Content, int TokensUsed, DateTime CreatedAt);
