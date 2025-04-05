using EcoLefty.Domain.Entities;
using System.Linq.Expressions;

namespace EcoLefty.Domain.Common.IncludeExpressions;

public static class ApplicationUserIncludes
{
    public static Expression<Func<ApplicationUser, object>> Account => user => user.Account;
    public static Expression<Func<ApplicationUser, object>> Purchases => user => user.Purchases;
    public static Expression<Func<ApplicationUser, object>> Categories => user => user.FollowedCategories;
}

public static class ApplicationUserStringIncludes
{
    public static string Account_string => "Account";
    public static string Purchases_string => "Purchases";
    public static string Purchases_Offer_string => "Purchases.Offer";
    public static string Purchases_Customer_string => "Purchases.Customer";
    public static string Categories_string => "FollowedCategories";
}
