namespace Market.Application.Modules.Catalog.Promotions.Queries.GetOptions;

public sealed class GetPromotionOptionsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetPromotionOptionsQuery, GetPromotionOptionsQueryDto>
{
    public async Task<GetPromotionOptionsQueryDto> Handle(GetPromotionOptionsQuery request, CancellationToken ct)
    {
        var games = await ctx.Products
            .AsNoTracking()
            .Where(g => !g.IsDeleted)
            .OrderBy(g => g.Title)
            .Select(g => new PromotionGameDto
            {
                Id = g.Id,
                Title = g.Title,
                Price = g.Price ?? 0,
                CategoryId = g.CategoryId,
                Genre = g.Genre,
                Publisher = g.Publisher,
                Developer = g.Developer,
                TagIds = g.Tags.Select(t => t.Id).ToList()
            })
            .ToListAsync(ct);

        var categories = await ctx.ProductCategories
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new PromotionOptionDto { Id = c.Id, Name = c.Name })
            .ToListAsync(ct);

        var tags = await ctx.GameTags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new PromotionOptionDto { Id = t.Id, Name = t.Name })
            .ToListAsync(ct);

        return new GetPromotionOptionsQueryDto
        {
            Categories = categories,
            Tags = tags,
            Genres = DistinctValues(games.Select(g => g.Genre)),
            Publishers = DistinctValues(games.Select(g => g.Publisher)),
            Developers = DistinctValues(games.Select(g => g.Developer)),
            Games = games
        };
    }

    private static List<string> DistinctValues(IEnumerable<string?> values) =>
        values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .Distinct()
            .OrderBy(v => v)
            .ToList();
}
