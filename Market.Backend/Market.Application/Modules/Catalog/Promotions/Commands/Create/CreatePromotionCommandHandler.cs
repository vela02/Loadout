using Market.Domain.Models;

namespace Market.Application.Modules.Catalog.Promotions.Commands.Create;

public sealed class CreatePromotionCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreatePromotionCommand, int>
{
    public async Task<int> Handle(CreatePromotionCommand request, CancellationToken ct)
    {
        var games = ctx.Products.Where(g => !g.IsDeleted);
        string? targetValue = null;

        switch (request.TargetType)
        {
            case PromotionTargetTypes.Games:
                var ids = request.GameIds!.Distinct().ToList();
                games = games.Where(g => ids.Contains(g.Id));
                break;

            case PromotionTargetTypes.Category:
                targetValue = await ctx.ProductCategories
                    .Where(c => c.Id == request.TargetId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync(ct)
                    ?? throw new MarketNotFoundException($"Kategorija s ID-em {request.TargetId} ne postoji.");
                games = games.Where(g => g.CategoryId == request.TargetId);
                break;

            case PromotionTargetTypes.Tag:
                targetValue = await ctx.GameTags
                    .Where(t => t.Id == request.TargetId)
                    .Select(t => t.Name)
                    .FirstOrDefaultAsync(ct)
                    ?? throw new MarketNotFoundException($"Tag s ID-em {request.TargetId} ne postoji.");
                games = games.Where(g => g.Tags.Any(t => t.Id == request.TargetId));
                break;

            case PromotionTargetTypes.Genre:
                targetValue = request.TargetText!.Trim();
                games = games.Where(g => g.Genre == targetValue);
                break;

            case PromotionTargetTypes.Publisher:
                targetValue = request.TargetText!.Trim();
                games = games.Where(g => g.Publisher == targetValue);
                break;

            case PromotionTargetTypes.Developer:
                targetValue = request.TargetText!.Trim();
                games = games.Where(g => g.Developer == targetValue);
                break;
        }

        var gameIds = await games.Select(g => g.Id).ToListAsync(ct);

        if (gameIds.Count == 0)
            throw new ValidationException("Nijedna igra ne odgovara odabranom cilju promocije.");

        var name = request.Name.Trim();

        var promotion = new Promotion
        {
            Name = name,
            Description = request.Description?.Trim(),
            DiscountPercentage = request.DiscountPercentage,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TargetType = request.TargetType,
            TargetValue = targetValue,
            CreatedAt = DateTime.Now,
            Discounts = gameIds.Select(gameId => new Discount
            {
                GameId = gameId,
                DiscountPercentage = request.DiscountPercentage,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Description = name
            }).ToList()
        };

        ctx.Promotions.Add(promotion);
        await ctx.SaveChangesAsync(ct);

        return promotion.Id;
    }
}
