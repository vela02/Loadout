namespace Market.Application.Modules.Catalog.ProductCategories.Queries.List;

public sealed class ListProductCategoriesQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListProductCategoriesQuery, PageResult<ListProductCategoriesQueryDto>>
{
    public async Task<PageResult<ListProductCategoriesQueryDto>> Handle(
        ListProductCategoriesQuery request, CancellationToken ct)
    {
        var q = ctx.ProductCategories.AsNoTracking().Where(x => !x.IsDeleted);
        var now = DateTime.Now;

        // 1) search by name
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x => x.Name.ToLower().Contains(searchTerm));
        }

        if (request.OnlyEnabled == true)
            q = q.Where(x => x.IsEnabled);

        // 2) status
        if (request.IsEnabled.HasValue)
            q = q.Where(x => x.IsEnabled == request.IsEnabled);

        // 3) + 4) number of games
        if (request.MinGames.HasValue)
            q = q.Where(x => x.Games.Count(g => !g.IsDeleted) >= request.MinGames);

        if (request.MaxGames.HasValue)
            q = q.Where(x => x.Games.Count(g => !g.IsDeleted) <= request.MaxGames);

        // 5) running discount on the category itself or on one of its games
        if (request.OnSale.HasValue)
        {
            q = q.Where(x =>
                (x.Discounts.Any(d => d.StartDate <= now && d.EndDate >= now) ||
                 x.Games.Any(g => !g.IsDeleted && g.Discounts.Any(d => d.StartDate <= now && d.EndDate >= now)))
                == request.OnSale);
        }

        var projectedQuery = q.OrderBy(x => x.Name)
            .Select(x => new ListProductCategoriesQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                IsEnabled = x.IsEnabled,
                GamesCount = x.Games.Count(g => !g.IsDeleted),
                OnSale = x.Discounts.Any(d => d.StartDate <= now && d.EndDate >= now) ||
                         x.Games.Any(g => !g.IsDeleted && g.Discounts.Any(d => d.StartDate <= now && d.EndDate >= now))
            });

        return await PageResult<ListProductCategoriesQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
