using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Application.Features.Auth.DTOs;
using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.Auth.Commands.LoginFarmer;

public class LoginFarmerCommandHandler(
    IUnitOfWork uow,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtService)
    : IRequestHandler<LoginFarmerCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginFarmerCommand request, CancellationToken ct)
    {
        var user = await uow.Users.FindByPhoneAsync(request.Phone, ct)
            ?? throw new UnauthorizedException("Invalid phone number or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("Account is deactivated.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid phone number or password.");

        user.RecordLogin();
        uow.Users.Update(user);
        await uow.SaveChangesAsync(ct);

        var (accessToken, refreshToken, expiresAt) = await jwtService.GenerateTokenPairAsync(user, ct);

        return new AuthResponseDto(
            UserId: user.Id,
            FullName: user.FullName,
            Phone: user.Phone,
            Role: user.Role,
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: expiresAt);
    }
}
