namespace Market.Application.Features.ProductCategories.Commands.Update;

public sealed class UpdateProductCategoryCommandHandler(IAppDbContext ctx)
            : IRequestHandler<UpdateProductCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCategoryCommand request, CancellationToken ct)
    {
        var entity = await ctx.ProductCategories
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(ct);

        if (entity is null)
            throw new MarketNotFoundException($"Kategorija (ID={request.Id}) nije pronađena.");

        // Provjera duplikata naziva (case-insensitive, osim na isti ID)
        var exists = await ctx.ProductCategories
            .AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == request.Name.ToLower(), ct);

        if (exists)
        {
            throw new MarketConflictException("Naziv već postoji.");
        }


        entity.Name = request.Name.Trim();

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
