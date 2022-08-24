using MinimalApiRecordTests.Models;

namespace MinimalApiRecordTests.Internal;

public static class QueryableExtensions
{
    public static IQueryable<TSource> ApplyRange<TSource>(this IQueryable<TSource> source, RangeFilter range)
    {
        if (range is null or { Skip: <= 0, Take: <= 0 })
        {
            return source;
        }

        if (range.Skip is int skip and > 0)
        {
            source = source.Skip(skip);
        }

        if (range.Take is int take and > 0)
        {
            source = source.Take(take);
        }

        return source;
    }
}
