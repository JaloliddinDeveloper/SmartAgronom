using AqlliAgronom.Application.Features.AiChat.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.AiChat.Commands.SendChatMessage;

public record SendChatMessageCommand(
    Guid SessionId,
    string Message,
    Guid UserId) : IRequest<ChatResponseDto>;
