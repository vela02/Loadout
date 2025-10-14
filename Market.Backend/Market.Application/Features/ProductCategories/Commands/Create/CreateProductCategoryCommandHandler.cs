namespace Market.Application.Features.ProductCategories.Commands.Create;

public class CreateProductCategoryCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateProductCategoryCommand, int>
{
    public async Task<int> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var normalized = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ValidationException("Naziv je obavezan.");

        // Provjera da li već postoji kategorija sa istim nazivom
        bool exists = await context.ProductCategories
            .AnyAsync(x => x.Name == normalized, cancellationToken);

        if (exists)
        {
            throw new MarketConflictException("Naziv već postoji.");
        }

        var category = new ProductCategoryEntity
        {
            Name = request.Name!.Trim(),
            IsEnabled = true // defaultno IsEnabled
        };

        context.ProductCategories.Add(category);
        await context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
