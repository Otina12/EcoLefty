using EcoLefty.Domain.Entities;
using System.Linq.Expressions;

namespace EcoLefty.Domain.Common.IncludeExpressions;

public static class CompanyIncludes
{
    public static Expression<Func<Company, object>> Account => company => company.Account;
    public static Expression<Func<Company, object>> Products => company => company.Products;
}