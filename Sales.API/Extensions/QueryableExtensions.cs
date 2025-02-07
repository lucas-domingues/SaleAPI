using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sales.API.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string order)
        {
            if (string.IsNullOrEmpty(order))
                return query;

            var orderParams = order.Split(',');
            var validProperties = typeof(T).GetProperties()
                                           .Select(prop => prop.Name)
                                           .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var param in orderParams)
            {
                var trimmedParam = param.Trim();
                var isDescending = trimmedParam.EndsWith("desc", StringComparison.OrdinalIgnoreCase);
                var propertyName = isDescending ? trimmedParam[..^4].Trim() : trimmedParam;

                if (validProperties.Contains(propertyName))
                {
                    var parameter = Expression.Parameter(typeof(T), "p");
                    var property = Expression.Property(parameter, propertyName);
                    var lambda = Expression.Lambda(property, parameter);

                    var methodName = isDescending ? "OrderByDescending" : "OrderBy";
                    var method = typeof(Queryable).GetMethods()
                                                  .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                                                  .Single()
                                                  .MakeGenericMethod(typeof(T), property.Type);

                    query = (IQueryable<T>)method.Invoke(null, new object[] { query, lambda });
                }
            }

            return query;
        }
    }
}
