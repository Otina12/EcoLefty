using EcoLefty.Domain.Entities;
using System.Linq.Expressions;

namespace EcoLefty.Domain.Common.IncludeExpressions;

public static class ProductIncludes
{
    public static Expression<Func<Product, object>> Company => company => company.Company;
    public static Expression<Func<Product, object>> Categories => company => company.Categories;
    public static Expression<Func<Product, object>> Offers => company => company.Offers;
}
