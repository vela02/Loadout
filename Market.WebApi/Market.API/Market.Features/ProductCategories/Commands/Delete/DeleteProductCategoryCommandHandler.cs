namespace Market.Features.ProductCategories.Commands.Delete;

public class DeleteProductCategoryCommandHandler(DatabaseContext context)
      : IRequestHandler<DeleteProductCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.ProductCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (category is null)
            throw new NotFoundException("Kategorija nije pronađena.");

        context.ProductCategories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
