namespace Market.Application.Modules.Catalog.ProductCategories.Queries.List;

public sealed class ListProductCategoriesQuery : BasePagedQuery<ListProductCategoriesQueryDto>
{
    public string? Search { get; init; }

    /// <summary>Used by dropdowns in the game forms (only enabled categories).</summary>
    public bool? OnlyEnabled { get; init; }

    // admin list filters
    /// <summary>true = only active, false = only inactive, null = all</summary>
    public bool? IsEnabled { get; init; }
    public int? MinGames { get; init; }
    public int? MaxGames { get; init; }
    /// <summary>true = has a running discount (on the category or one of its games), false = none, null = all</summary>
    public bool? OnSale { get; init; }
}
