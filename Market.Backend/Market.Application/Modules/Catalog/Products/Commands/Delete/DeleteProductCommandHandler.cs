namespace Market.Application.Modules.Catalog.Products.Commands.Delete;

public class DeleteProductCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
      : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken ct)
    {      
        if (!appCurrentUser.IsAdmin)
        {
            throw new MarketBusinessRuleException("UNAUTHORIZED", "Samo admin može brisati proizvode.");
        }  
        var product = await context.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        
        if (product is null)
        {
            throw new MarketNotFoundException("Proizvod nije pronađen.");
        }

        // Soft delete
        product.IsDeleted = true;
        product.IsEnabled = false;

        
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
