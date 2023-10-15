using System.Linq.Expressions;

namespace AppCore.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool isDescending)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);

        string methodName = isDescending ? "OrderByDescending" : "OrderBy";
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.Type },
            source.Expression,
            Expression.Quote(lambda)
        );

        return source.Provider.CreateQuery<T>(resultExpression);
    }
}