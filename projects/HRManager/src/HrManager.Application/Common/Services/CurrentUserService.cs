using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HrManager.Application.Common.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId => httpContextAccessor.HttpContext?.User.GetUserId() ?? Guid.Empty;

    public string[] UserRoles => httpContextAccessor.HttpContext.User.GetUserRoles();

    public string? Email => httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
}

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : Guid.Empty;
    }

    public static string[] GetUserRoles(this ClaimsPrincipal principal)
    {
        var roles = principal?.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray();

        return roles;
    }
}