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
    public static string Account_string => NavigationPathHelper.FormatNavigationPath(nameof(ApplicationUser.Account));
    public static string Purchases_string => NavigationPathHelper.FormatNavigationPath(nameof(ApplicationUser.Purchases));
    public static string Purchases_Offer_string => NavigationPathHelper.FormatNavigationPath(nameof(ApplicationUser.Purchases), nameof(Purchase.Offer));
    public static string Purchases_Customer_string => NavigationPathHelper.FormatNavigationPath(nameof(ApplicationUser.Purchases), nameof(Purchase.Customer));
    public static string Categories_string => NavigationPathHelper.FormatNavigationPath(nameof(ApplicationUser.FollowedCategories));
}
