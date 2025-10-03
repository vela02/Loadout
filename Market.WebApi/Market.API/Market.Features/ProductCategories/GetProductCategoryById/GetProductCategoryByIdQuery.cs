using Market.Core.Entities;
using MediatR;

namespace Market.Features.ProductCategories.GetProductCategoryById
{
    public class GetProductCategoryByIdQuery : IRequest<ProductCategoryEntity>
    {
        public int Id { get; set; }
    }
}