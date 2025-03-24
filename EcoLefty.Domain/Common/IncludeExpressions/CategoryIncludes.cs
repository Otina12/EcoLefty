using EcoLefty.Domain.Entities;
using System.Linq.Expressions;

namespace EcoLefty.Domain.Common.IncludeExpressions;

public static class CategoryIncludes
{
    public static Expression<Func<Category, object>> Products => company => company.Products;
    public static Expression<Func<Category, object>> Users => company => company.FollowingUsers;
}
