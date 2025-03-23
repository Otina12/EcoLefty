using EcoLefty.Domain.Contracts;
using Microsoft.AspNetCore.Http;

namespace Common.Shared;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true ?
            _httpContextAccessor.HttpContext.User.Identity.Name
            : null;
}
