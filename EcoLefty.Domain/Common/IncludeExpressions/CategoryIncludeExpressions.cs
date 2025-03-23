using System.Linq.Expressions;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Domain.Common.IncludeExpressions;

public static class CategoryIncludeExpressions
{
    public static Expression<Func<Category, object>> Products => company => company.Products;
    public static Expression<Func<Category, object>> Users => company => company.FollowingUsers;
}
