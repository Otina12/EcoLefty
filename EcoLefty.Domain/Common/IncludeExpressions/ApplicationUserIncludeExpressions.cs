using EcoLefty.Domain.Entities;
using System.Linq.Expressions;

namespace EcoLefty.Domain.Common.IncludeExpressions;

public static class ApplicationUserIncludeExpressions
{
    public static Expression<Func<ApplicationUser, object>> Account => user => user.Account;
    public static Expression<Func<ApplicationUser, object>> Categories => user => user.FollowedCategories;
}
