namespace Market.Features.ProductCategories.Commands.Status.Disable;

public sealed class DisableProductCategoryCommandHandler(DatabaseContext ctx)
    : IRequestHandler<DisableProductCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DisableProductCategoryCommand request, CancellationToken ct)
    {
        var entity = await ctx.ProductCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null)
            throw new MarketNotFoundException($"Kategorija (ID={request.Id}) nije pronađena.");

        if (entity.IsEnabled)
        {
            entity.IsEnabled = false;
            await ctx.SaveChangesAsync(ct);
        }

        // idempotentno: ako je već disabled, ne dižemo grešku
        return Unit.Value;
    }
}
