namespace Market.Features.ProductCategories.Commands.Create;

public class CreateProductCategoryCommandHandler(DatabaseContext context)
    : IRequestHandler<CreateProductCategoryCommand, int>
{
    public async Task<int> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        // Provjera da li već postoji kategorija sa istim nazivom
        bool exists = await context.ProductCategories
            .AnyAsync(x => x.Name == request.Name, cancellationToken);

        if (exists)
        {
            throw new ConflictException("Naziv već postoji.");
        }

        var category = new ProductCategoryEntity
        {
            Name = request.Name.Trim()
        };

        context.ProductCategories.Add(category);
        await context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
