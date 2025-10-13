namespace Market.Features.ProductCategories.Commands.Delete;

public class DeleteProductCategoryCommandHandler(DatabaseContext context)
      : IRequestHandler<DeleteProductCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.ProductCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (category is null)
            throw new MarketNotFoundException("Kategorija nije pronađena.");

        category.IsDeleted = true; // Soft delete
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
