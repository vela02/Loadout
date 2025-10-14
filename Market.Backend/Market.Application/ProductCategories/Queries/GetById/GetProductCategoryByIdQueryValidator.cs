using Market.Application.ProductCategories.Queries.GetById;

public sealed class GetProductCategoryByIdQueryValidator : AbstractValidator<GetProductCategoryByIdQuery>
{
    public GetProductCategoryByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id mora biti pozitivna vrijednost.");
    }
}
