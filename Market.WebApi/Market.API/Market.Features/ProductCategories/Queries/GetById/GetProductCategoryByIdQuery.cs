namespace Market.Features.ProductCategories.Queries.GetById;

public class GetProductCategoryByIdQuery : IRequest<GetProductCategoryByIdQueryDto>
{
    public int Id { get; set; }
}