using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.Features.Users.Commands.PromoteToAgronom;

public class PromoteToAgronomiystCommandHandler(IUnitOfWork uow, ILogger<PromoteToAgronomiystCommandHandler> logger)
    : IRequestHandler<PromoteToAgronomiystCommand, string>
{
    public async Task<string> Handle(PromoteToAgronomiystCommand request, CancellationToken ct)
    {
        var user = await uow.Users.FindByPhoneAsync(request.Phone, ct)
            ?? throw new NotFoundException("User", request.Phone);

        user.AssignRole(UserRole.Agronom);
        uow.Users.Update(user);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} ({FullName}) promoted to Agronom", user.Id, user.FullName);
        return user.FullName;
    }
}
