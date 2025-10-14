namespace Market.Application.ProductCategories.Commands.Create;

public class CreateProductCategoryCommand : IRequest<int>
{
    public required string Name { get; set; }
}