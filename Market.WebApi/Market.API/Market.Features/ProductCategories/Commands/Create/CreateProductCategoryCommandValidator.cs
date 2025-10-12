namespace Market.Features.ProductCategories.Commands.Create;

public sealed class CreateProductCategoryCommandValidator
    : AbstractValidator<CreateProductCategoryCommand>
{
    public CreateProductCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naziv je obavezan.")
            .MaximumLength(100).WithMessage("Naziv može imati najviše 100 znakova.");
    }
}
