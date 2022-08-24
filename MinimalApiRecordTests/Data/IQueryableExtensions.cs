using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace MinimalApiRecordTests.Data;

public static class IQueryableExtensions
{
    public static async Task<ImmutableArray<TSource>> ToImmutableArrayAsync<TSource>(
        this IQueryable<TSource> source,
        CancellationToken cancellationToken = default)
    {
        var builder = ImmutableArray.CreateBuilder<TSource>();
        await foreach (var element in source.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            builder.Add(element);
        }

        return builder.MoveToImmutable();
    }
}
