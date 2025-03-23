using System.Linq.Expressions;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Domain.Common.IncludeExpressions;

public static class OfferIncludeExpressions
{
    public static Expression<Func<Offer, object>> Company => offer => offer.Company;
    public static Expression<Func<Offer, object>> Products => offer => offer.Product;
}
