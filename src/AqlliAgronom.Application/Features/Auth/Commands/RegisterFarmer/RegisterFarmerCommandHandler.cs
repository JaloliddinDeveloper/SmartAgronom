using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Application.Features.Auth.DTOs;
using AqlliAgronom.Domain.Entities;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.Features.Auth.Commands.RegisterFarmer;

public class RegisterFarmerCommandHandler(
    IUnitOfWork uow,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtService,
    ILogger<RegisterFarmerCommandHandler> logger)
    : IRequestHandler<RegisterFarmerCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterFarmerCommand request, CancellationToken ct)
    {
        // Check uniqueness
        if (await uow.Users.PhoneExistsAsync(request.Phone, ct))
            throw new ConflictException($"Phone number '{request.Phone}' is already registered.");

        // Hash password
        var passwordHash = passwordHasher.Hash(request.Password);

        // Create user aggregate
        var user = User.Register(
            fullName: request.FullName,
            phone: request.Phone,
            passwordHash: passwordHash,
            language: request.Language,
            email: request.Email,
            region: request.Region);

        await uow.Users.AddAsync(user, ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("New farmer registered: {UserId} — {FullName}", user.Id, user.FullName);

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
