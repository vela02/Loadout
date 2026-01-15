
namespace Market.Application.Modules.Catalog.Products.Commands.Create;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Naziv igre je obavezan.")
            .MaximumLength(100).WithMessage("Naziv je predug.");

        
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Cijena ne može biti negativna.");

        
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Kategorija je obavezna.");

        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Opis je obavezan.");

       
        RuleFor(x => x.ImageUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("Neispravan format linka za sliku.");
    }
}