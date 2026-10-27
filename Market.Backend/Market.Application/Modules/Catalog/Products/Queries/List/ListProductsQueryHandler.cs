namespace Market.Application.Modules.Catalog.Products.Queries.List;

public sealed class ListProductsQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListProductsQuery, PageResult<ListProductsQueryDto>>
{
    public async Task<PageResult<ListProductsQueryDto>> Handle(
        ListProductsQuery request, CancellationToken ct)
    {
        // games are soft-deleted
        var q = ctx.Products.AsNoTracking().Where(x => !x.IsDeleted);

        // 1) search by title
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x => x.Title.ToLower().Contains(searchTerm));
        }

        // 2) category
        if (request.CategoryId.HasValue)
            q = q.Where(x => x.CategoryId == request.CategoryId);

        // 3) status
        if (request.IsEnabled.HasValue)
            q = q.Where(x => x.IsEnabled == request.IsEnabled);

        var now = DateTime.Now;

        // best running discount + the price the customer actually pays
        var priced = q
            .Select(x => new
            {
                Game = x,
                Discount = x.Discounts
                    .Where(d => d.StartDate <= now && d.EndDate >= now)
                    .Max(d => (decimal?)d.DiscountPercentage)
            })
            .Select(x => new
            {
                x.Game,
                x.Discount,
                FinalPrice = Math.Round((x.Game.Price ?? 0) * (100 - (x.Discount ?? 0)) / 100, 2)
            });

        // 4) + 5) price range (on the discounted price)
        if (request.MinPrice.HasValue)
            priced = priced.Where(x => x.FinalPrice >= request.MinPrice);

        if (request.MaxPrice.HasValue)
            priced = priced.Where(x => x.FinalPrice <= request.MaxPrice);

        var projectedQuery = priced.OrderByDescending(x => x.Game.Id)
            .Select(x => new ListProductsQueryDto
            {
                Id = x.Game.Id,
                Name = x.Game.Title,
                IsEnabled = x.Game.IsEnabled,
                Description = x.Game.Description,
                Price = x.Game.Price ?? 0,
                StockQuantity = 999,
                CategoryName = x.Game.Category != null ? x.Game.Category.Name : "-",
                ActiveDiscountPercentage = x.Discount
            });

        return await PageResult<ListProductsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
