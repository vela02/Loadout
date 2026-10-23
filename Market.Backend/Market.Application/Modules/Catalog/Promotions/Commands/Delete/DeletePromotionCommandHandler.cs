namespace Market.Application.Modules.Catalog.Promotions.Commands.Delete;

public sealed class DeletePromotionCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeletePromotionCommand, Unit>
{
    public async Task<Unit> Handle(DeletePromotionCommand request, CancellationToken ct)
    {
        var promotion = await ctx.Promotions
            .Include(p => p.Discounts)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (promotion is null)
            throw new MarketNotFoundException($"Promocija s ID-em {request.Id} nije pronađena.");

        // Discounts are removed together with the promotion (cascade)
        ctx.Promotions.Remove(promotion);
        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
