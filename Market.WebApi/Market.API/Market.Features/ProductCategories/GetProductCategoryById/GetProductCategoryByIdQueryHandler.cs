using Market.Core.Entities;
using Market.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Features.ProductCategories.GetProductCategoryById
{
    public class GetProductCategoryByIdQueryHandler(DatabaseContext context) : IRequestHandler<GetProductCategoryByIdQuery, ProductCategoryEntity>
    {
        public async Task<ProductCategoryEntity> Handle(GetProductCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await context.ProductCategories
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category == null)
            {
                throw new KeyNotFoundException($"Product category with Id {request.Id} not found.");
            }

            return category;
        }
    }
}