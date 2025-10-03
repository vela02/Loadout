using MediatR;

namespace Market.Features.ProductCategories.CreateProductCategory
{
    /// <summary>
    /// Command for creating new product category.
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Int32&gt;" />
    public class CreateProductCategoryCommand : IRequest<int>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}