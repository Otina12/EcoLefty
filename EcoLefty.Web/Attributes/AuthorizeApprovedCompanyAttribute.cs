using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Persistence.Context;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcoLefty.Web.Attributes;

public class AuthorizeApprovedCompanyAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var account = context.HttpContext.User;

        if (!account.Identity?.IsAuthenticated ?? true || account.FindFirst(ClaimTypes.Role)?.Value != "Company")
            throw new UnauthorizedException();

        var accountId = account.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (accountId is null)
            throw new UnauthorizedException();

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<EcoLeftyDbContext>();

        var company = await dbContext.Companies.FirstOrDefaultAsync(c => c.Id == accountId);

        if (company is null)
            throw new UnauthorizedException();

        if (!company.IsApproved)
            throw new CompanyNotApprovedException(accountId);

        await next();
    }
}