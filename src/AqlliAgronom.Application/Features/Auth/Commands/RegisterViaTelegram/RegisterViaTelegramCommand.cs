using MediatR;

namespace AqlliAgronom.Application.Features.Auth.Commands.RegisterViaTelegram;

public record RegisterViaTelegramCommand(
    string FullName,
    string Phone,
    long TelegramChatId,
    string? TelegramUsername) : IRequest<bool>;
