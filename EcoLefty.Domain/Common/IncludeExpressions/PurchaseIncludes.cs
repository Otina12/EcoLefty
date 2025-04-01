using EcoLefty.Domain.Entities;
using System.Linq.Expressions;

namespace EcoLefty.Domain.Common.IncludeExpressions;

public class PurchaseIncludes
{
    public static Expression<Func<Purchase, object>> Customer => purchase => purchase.Customer;
    public static Expression<Func<Purchase, object>> Customer_Account => purchase => purchase.Customer.Account;
    public static Expression<Func<Purchase, object>> Offer => purchase => purchase.Offer;
    public static Expression<Func<Purchase, object>> Offer_Product_Company => purchase => purchase.Offer.Product.Company;
}
