namespace Market.Application.Modules.Catalog.Products.Queries.List;

public sealed class ListProductsQuery : BasePagedQuery<ListProductsQueryDto>
{
    // 1) title contains
    public string? Search { get; init; }

    // 2) category
    public int? CategoryId { get; init; }

    // 3) status: true = only active, false = only inactive, null = all
    public bool? IsEnabled { get; init; }

    // 4) + 5) price range (regular price, without discount)
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
}
