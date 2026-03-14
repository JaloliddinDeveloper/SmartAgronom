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
        // Reuse active session if exists
        var existing = await uow.FarmerSessions
            .GetActiveSessionByTelegramAsync(request.TelegramChatId, ct);

        if (existing is not null)
        {
            return MapToDto(existing);
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
