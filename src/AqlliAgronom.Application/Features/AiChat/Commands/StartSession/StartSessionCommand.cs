using AqlliAgronom.Application.Features.AiChat.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.AiChat.Commands.StartSession;

public record StartSessionCommand(Guid UserId, long TelegramChatId) : IRequest<SessionDto>;
