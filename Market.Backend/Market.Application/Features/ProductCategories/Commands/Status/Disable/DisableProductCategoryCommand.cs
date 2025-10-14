namespace Market.Application.Features.ProductCategories.Commands.Status.Disable;

public sealed class DisableProductCategoryCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
