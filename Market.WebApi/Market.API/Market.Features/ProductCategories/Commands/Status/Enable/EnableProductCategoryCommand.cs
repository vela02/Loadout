namespace Market.Features.ProductCategories.Commands.Status.Enable;

public sealed class EnableProductCategoryCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
