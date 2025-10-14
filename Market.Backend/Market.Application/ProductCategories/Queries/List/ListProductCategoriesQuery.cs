using Market.Application.Common;

namespace Market.Application.ProductCategories.Queries.List;

public sealed class ListProductCategoriesQuery : BasePagedQuery<ListProductCategoriesQueryDto>
{
    public string? Search { get; init; }
    public bool? OnlyEnabled { get; init; }
}
