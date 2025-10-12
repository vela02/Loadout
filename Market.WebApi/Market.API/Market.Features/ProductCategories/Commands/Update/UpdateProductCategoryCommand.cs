namespace Market.Features.ProductCategories.Commands.Update;

public sealed class UpdateProductCategoryCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public required string Name { get; set; }
}
