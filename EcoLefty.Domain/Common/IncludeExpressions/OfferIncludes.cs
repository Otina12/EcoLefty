using EcoLefty.Domain.Entities;
using System.Linq.Expressions;

namespace EcoLefty.Domain.Common.IncludeExpressions;

public static class OfferIncludes
{
    public static Expression<Func<Offer, object>> Product => offer => offer.Product;
    public static Expression<Func<Offer, object>> Purchases => offer => offer.Purchases;
    public static Expression<Func<Offer, object>> Product_Company => offer => offer.Product.Company;
    public static Expression<Func<Offer, object>> Product_Categories => offer => offer.Product.Categories;
}
