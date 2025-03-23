using EcoLefty.Domain.Entities;
using System.Linq.Expressions;

namespace EcoLefty.Domain.Common.IncludeExpressions;

public static class CompanyIncludeExpressions
{
    public static Expression<Func<Company, object>> Account => company => company.Account;
    public static Expression<Func<Company, object>> Products => company => company.Products;
    public static Expression<Func<Company, object>> Offers => company => company.Offers;
}