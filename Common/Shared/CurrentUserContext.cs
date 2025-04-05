using EcoLefty.Domain.Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Shared;
public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true ?
            _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;

    public string? UserRole =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true ?
            _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)
            : null;

    public bool IsInRole(string roleName) => _httpContextAccessor.HttpContext?.User?.IsInRole(roleName) ?? false;
}