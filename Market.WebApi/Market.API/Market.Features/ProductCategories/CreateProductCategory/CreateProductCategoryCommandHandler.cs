using Market.Core.Entities;
using Market.Infrastructure.Database;
using MediatR;

namespace Market.Features.ProductCategories.CreateProductCategory
{
    /// <summary>
    /// Handler for creating new product category.
    /// </summary>
    public class CreateProductCategoryCommandHandler(DatabaseContext context) : IRequestHandler<CreateProductCategoryCommand, int>
    {
        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<int> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new ProductCategoryEntity { Name = request.Name };
            context.ProductCategories.Add(category);
            await context.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}