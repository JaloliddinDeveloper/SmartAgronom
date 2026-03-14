using System.Security.Claims;
using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace AqlliAgronom.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var sub = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User?.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? UserName => User?.FindFirstValue(ClaimTypes.Name);

    public UserRole? Role
    {
        get
        {
            var roleStr = User?.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(roleStr, out var role) ? role : null;
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public bool IsInRole(UserRole role) =>
        Role == role || Role == UserRole.Admin;
}
