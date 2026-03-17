using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.Features.Auth.Commands.RegisterViaTelegram;

public class RegisterViaTelegramCommandHandler(
    IUnitOfWork uow,
    IPasswordHasher passwordHasher,
    ILogger<RegisterViaTelegramCommandHandler> logger)
    : IRequestHandler<RegisterViaTelegramCommand, bool>
{
    public async Task<bool> Handle(RegisterViaTelegramCommand request, CancellationToken ct)
    {
        // If this Telegram account is already linked to a user — nothing to do
        var existingByTelegram = await uow.Users.FindByTelegramChatIdAsync(request.TelegramChatId, ct);
        if (existingByTelegram is not null)
        {
            logger.LogInformation("Telegram {ChatId} already linked to user {UserId}", request.TelegramChatId, existingByTelegram.Id);
            return true;
        }

        // If phone already registered — link telegram to that account
        var existingByPhone = await uow.Users.FindByPhoneAsync(request.Phone, ct);
        if (existingByPhone is not null)
        {
            existingByPhone.LinkTelegram(request.TelegramChatId, request.TelegramUsername);
            await uow.SaveChangesAsync(ct);
            logger.LogInformation("Linked Telegram {ChatId} to existing user {UserId}", request.TelegramChatId, existingByPhone.Id);
            return true;
        }

        // New user — register with random password (Telegram users don't need password login)
        var passwordHash = passwordHasher.Hash(Guid.NewGuid().ToString());
        var user = User.Register(
            fullName: request.FullName,
            phone: request.Phone,
            passwordHash: passwordHash);

        user.LinkTelegram(request.TelegramChatId, request.TelegramUsername);

        await uow.Users.AddAsync(user, ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("New user registered via Telegram: {UserId} — {FullName}", user.Id, user.FullName);
        return true;
    }
}
