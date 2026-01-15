
using Market.Domain.Models;

namespace Market.Application.Modules.Catalog.Discounts.Commands.Create
{
    public sealed class CreateDiscountCommandHandler(IAppDbContext ctx)
     : IRequestHandler<CreateDiscountCommand, int>
    {
        public async Task<int> Handle(CreateDiscountCommand request, CancellationToken ct)
        {
            // checks the game/category existence if provided
            if (request.GameId.HasValue)
            {
                var gameExists = await ctx.Products.AnyAsync(x => x.Id == request.GameId, ct);
                if (!gameExists)
                    throw new ValidationException($"Igra s ID-em {request.GameId} ne postoji.");
            }
          
            if (request.CategoryId.HasValue)
            {
                var categoryExists = await ctx.ProductCategories.AnyAsync(x => x.Id == request.CategoryId, ct);
                if (!categoryExists)
                    throw new ValidationException($"Kategorija s ID-em {request.CategoryId} ne postoji.");
            }

            
            var discount = new Discount
            {
                GameId = request.GameId,
                CategoryId = request.CategoryId,
                DiscountPercentage = request.DiscountPercentage,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Description = request.Description
            };

            
            ctx.Discounts.Add(discount);
            await ctx.SaveChangesAsync(ct);

            return discount.Id;
        }
    }
}
