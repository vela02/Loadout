namespace Market.Features.ProductCategories.Commands.Update;

public sealed class UpdateProductCategoryCommandValidator
    : AbstractValidator<UpdateProductCategoryCommand>
{
    public UpdateProductCategoryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naziv je obavezan.")
            .MaximumLength(100).WithMessage("Naziv može imati najviše 100 znakova.");
    }
}
