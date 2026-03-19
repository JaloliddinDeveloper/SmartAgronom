using AqlliAgronom.Application.Features.AiChat.DTOs;
using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.AiChat.Commands.StartSession;

public class StartSessionCommandHandler(IUnitOfWork uow)
    : IRequestHandler<StartSessionCommand, SessionDto>
{
    public async Task<SessionDto> Handle(StartSessionCommand request, CancellationToken ct)
    {
        // Reuse active session only if it belongs to the same user.
        // A stale session (different userId) is closed and replaced.
        var existing = await uow.FarmerSessions
            .GetActiveSessionByTelegramAsync(request.TelegramChatId, ct);

        if (existing is not null)
        {
            if (existing.UserId == request.UserId)
                return MapToDto(existing);

            // Stale session from a different user — close it before creating a new one
            existing.Close();
            await uow.SaveChangesAsync(ct);
        }

        var session = FarmerSession.Start(request.UserId, request.TelegramChatId);
        await uow.FarmerSessions.AddAsync(session, ct);
        await uow.SaveChangesAsync(ct);

        return MapToDto(session);
    }

    private static SessionDto MapToDto(FarmerSession s) => new(
        s.Id, s.UserId, s.Status.ToString(),
        s.StartedAt, s.ClosedAt, s.MessageCount);
}
