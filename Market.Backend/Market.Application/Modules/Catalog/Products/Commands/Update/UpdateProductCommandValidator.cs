namespace Market.Application.Modules.Catalog.Products.Commands.Update;

// same rules as the edit form on the frontend (ProductFormService)
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Naziv igre je obavezan.")
            .MinimumLength(3).WithMessage("Naziv mora imati najmanje 3 znaka.")
            .MaximumLength(100).WithMessage("Naziv je predug.");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("Cijena je obavezna.")
            .GreaterThan(0).WithMessage("Cijena mora biti veća od 0.")
            .LessThanOrEqualTo(1_000_000).WithMessage("Cijena je prevelika.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Kategorija je obavezna.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis može imati najviše 500 znakova.");

        RuleFor(x => x.ImageUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("Neispravan format linka za sliku.");
    }
}
