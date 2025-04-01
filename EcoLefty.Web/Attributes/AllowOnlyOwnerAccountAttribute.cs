using EcoLefty.Domain.Common.Exceptions.Base;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace EcoLefty.Web.Attributes;

public class AllowOnlyOwnerAccountAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var account = context.HttpContext.User;

        if (!account.Identity?.IsAuthenticated ?? true || account.FindFirst(ClaimTypes.Role)?.Value != "Company")
            throw new UnauthorizedException();

        if (context.ActionArguments.TryGetValue("id", out var idObj))
        {
            string id = idObj!.ToString()!;
            string currentAccountId = account.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if (id != currentAccountId)
                throw new UnauthorizedException("You are not authorized to access this account.");
        }
        else
        {
            throw new UnauthorizedException("Id not provided.");
        }

        await next();
    }
}