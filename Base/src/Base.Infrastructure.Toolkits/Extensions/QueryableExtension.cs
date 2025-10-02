using System.Linq.Expressions;

namespace Base.Infrastructure.Toolkits.Extensions;

public static class QueryableExtension
{
    public static IQueryable<T> Where<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, bool IsSearch)
    {
        if (IsSearch)
        {
            query = query.Where(predicate);
        }
        return query;
    }
}
