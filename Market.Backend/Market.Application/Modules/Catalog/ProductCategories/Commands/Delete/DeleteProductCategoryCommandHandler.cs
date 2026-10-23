namespace Market.Application.Modules.Catalog.ProductCategories.Commands.Delete;

public class DeleteProductCategoryCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
      : IRequestHandler<DeleteProductCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
    {
        if (appCurrentUser.UserId is null)
            throw new MarketBusinessRuleException("123", "Korisnik nije autentifikovan.");

        var category = await context.ProductCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (category is null)
            throw new MarketNotFoundException("Kategorija nije pronađena.");

        var isUsed = await context.Products
       .AnyAsync(p => p.CategoryId == request.Id, cancellationToken);

        if (isUsed)
        {
            
            throw new MarketBusinessRuleException("category.delete.blocked",
                "Ne možete obrisati ovu kategoriju jer postoje igre koje joj pripadaju. Prvo obrišite ili premjestite te igre.");
        }

        context.ProductCategories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
