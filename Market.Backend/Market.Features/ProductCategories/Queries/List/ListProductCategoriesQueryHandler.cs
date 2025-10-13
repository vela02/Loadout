namespace Market.Features.ProductCategories.Queries.List;

public sealed class ListProductCategoriesQueryHandler(DatabaseContext ctx)
        : IRequestHandler<ListProductCategoriesQuery, PageResult<ListProductCategoriesQueryDto>>
{
    public async Task<PageResult<ListProductCategoriesQueryDto>> Handle(
        ListProductCategoriesQuery request, CancellationToken ct)
    {
        var q = ctx.ProductCategories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            if (ctx.Database.IsRelational())
            {
                // Radi samo na relacijskim bazama podataka (npr. SQL Server, PostgreSQL, MySQL)
                q = q.Where(x => EF.Functions.Like(x.Name, $"%{request.Search}%"));
            }
            else
            {
                // Fallback for non-relational databases (e.g., InMemory), za testiranje
                q = q.Where(x => x.Name.Contains(request.Search));
            }
        }

        if (request.OnlyEnabled is not null)
            q = q.Where(x => x.IsEnabled == request.OnlyEnabled);

        var projectedQuery = q.OrderBy(x => x.Name)
            .Select(x => new ListProductCategoriesQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                IsEnabled = x.IsEnabled
            });

        return await PageResult<ListProductCategoriesQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
