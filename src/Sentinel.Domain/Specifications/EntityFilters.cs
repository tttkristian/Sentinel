using Sentinel.Domain.Common;
using Sentinel.Domain.Interfaces;

namespace Sentinel.Domain.Specifications;

public static class EntityFilters
{
    public static IQueryable<T> WhereNotDeleted<T>(this IQueryable<T> query)
        where T : ISoftDelete
    {
        return query.Where(x => !x.IsDeleted);
    }

    public static IQueryable<T> WhereActive<T>(this IQueryable<T> query)
        where T : IActivatable
    {
        return query.Where(x => x.IsActive);
    }
}