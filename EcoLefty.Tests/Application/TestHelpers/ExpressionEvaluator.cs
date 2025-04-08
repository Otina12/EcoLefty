using System.Linq.Expressions;

namespace EcoLefty.Tests.Application.TestHelpers;

public static class ExpressionEvaluator
{
    public static bool CheckPredicate<T>(Expression<Func<T, bool>> expression, T objectToTest)
    {
        var compiledExpression = expression.Compile();
        return compiledExpression(objectToTest);
    }

    public static string? GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        var memberExpression = propertyExpression.Body as MemberExpression;
        if (memberExpression == null)
        {
            var unaryExpression = propertyExpression.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
        }

        return memberExpression?.Member.Name;
    }
}