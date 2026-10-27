namespace Market.Application.Modules.Catalog.ProductCategories.Queries.List;

public sealed class ListProductCategoriesQueryValidator : AbstractValidator<ListProductCategoriesQuery>
{
    public ListProductCategoriesQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(50).WithMessage("Pretraga može imati najviše 50 znakova.");

        RuleFor(x => x.MinGames)
            .GreaterThanOrEqualTo(0).When(x => x.MinGames.HasValue)
            .WithMessage("Minimalan broj igara ne može biti negativan.");

        RuleFor(x => x.MaxGames)
            .GreaterThanOrEqualTo(0).When(x => x.MaxGames.HasValue)
            .WithMessage("Maksimalan broj igara ne može biti negativan.");

        RuleFor(x => x)
            .Must(x => x.MinGames <= x.MaxGames)
            .When(x => x.MinGames.HasValue && x.MaxGames.HasValue)
            .WithMessage("Minimalan broj igara ne može biti veći od maksimalnog.");
    }
}
