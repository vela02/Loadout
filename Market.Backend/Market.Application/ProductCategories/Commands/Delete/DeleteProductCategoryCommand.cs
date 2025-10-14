namespace Market.Application.ProductCategories.Commands.Delete;

public class DeleteProductCategoryCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
