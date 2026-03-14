using AqlliAgronom.Domain.Enums;

namespace AqlliAgronom.Application.Features.Auth.DTOs;

public record AuthResponseDto(
    Guid UserId,
    string FullName,
    string Phone,
    UserRole Role,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);
