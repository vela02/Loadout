namespace Market.Features.Common;

public sealed class PageResult<T>
{
    public required int Total { get; init; }
    public required IReadOnlyList<T> Items { get; init; }

    /// <summary>
    /// Kreira PageResult iz IQueryable-a pomoću EF Core asinkronih metoda.
    /// </summary>
    public static async Task<PageResult<T>> FromQueryableAsync(
        IQueryable<T> query,
        PageRequest paging,
        CancellationToken ct = default,
        bool includeTotal = true)
    {
        int total = 0;
        if (includeTotal)
            total = await query.CountAsync(ct);

        var items = await query
            .Skip(paging.SkipCount)
            .Take(paging.PageSize)
            .ToListAsync(ct);

        return new PageResult<T> { Total = total, Items = items };
    }
}
