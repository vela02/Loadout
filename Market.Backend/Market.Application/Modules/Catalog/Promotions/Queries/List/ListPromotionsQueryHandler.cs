namespace Market.Application.Modules.Catalog.Promotions.Queries.List;

public sealed class ListPromotionsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListPromotionsQuery, List<ListPromotionsQueryDto>>
{
    public async Task<List<ListPromotionsQueryDto>> Handle(ListPromotionsQuery request, CancellationToken ct)
    {
        return await ctx.Promotions
            .AsNoTracking()
            .OrderByDescending(p => p.StartDate)
            .Select(p => new ListPromotionsQueryDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DiscountPercentage = p.DiscountPercentage,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                TargetType = p.TargetType,
                TargetValue = p.TargetValue,
                GameCount = p.Discounts.Count,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(ct);
    }
}
