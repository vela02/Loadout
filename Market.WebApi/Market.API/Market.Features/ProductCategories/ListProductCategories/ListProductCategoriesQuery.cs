using Market.Features.Common;
using MediatR;

namespace Market.Features.ProductCategories.ListProductCategories;

public sealed class ListProductCategoriesQuery : BasePagedQuery<ListProductCategoriesItem>
{
    public string? Search { get; init; }
    public bool? OnlyEnabled { get; init; }
}

public sealed class ListProductCategoriesItem
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required bool IsEnabled { get; init; }
}
