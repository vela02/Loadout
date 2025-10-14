namespace Market.Application.Modules.Catalog.ProductCategories.Commands.Status.Enable;

public sealed class EnableProductCategoryCommandHandler(IAppDbContext ctx)
    : IRequestHandler<EnableProductCategoryCommand, Unit>
{
    public async Task<Unit> Handle(EnableProductCategoryCommand request, CancellationToken ct)
    {
        var entity = await ctx.ProductCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null)
            throw new MarketNotFoundException($"Kategorija (ID={request.Id}) nije pronađena.");

        if (!entity.IsEnabled)
        {
            entity.IsEnabled = true;
            await ctx.SaveChangesAsync(ct);
        }

        // ako je već enabled — ništa se ne mijenja, idempotentno
        return Unit.Value;
    }
}
