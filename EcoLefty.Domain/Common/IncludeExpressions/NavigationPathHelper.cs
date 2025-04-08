namespace EcoLefty.Domain.Common.IncludeExpressions;

public static class NavigationPathHelper
{
    public static string FormatNavigationPath(params string[] fieldNames)
    {
        return string.Join('.', fieldNames);
    }
}
