
namespace Market.Application.Modules.Catalog.Discounts.Commands.Delete
{
    public sealed class DeleteDiscountCommandHandler(
     IAppDbContext ctx,
     IAppCurrentUser user) 
     : IRequestHandler<DeleteDiscountCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteDiscountCommand request, CancellationToken ct)
        {
            
            if (!user.IsAdmin)
            {
                throw new MarketBusinessRuleException("UNAUTHORIZED", "Samo admin može brisati popuste.");
            }
            
            var discount = await ctx.Discounts
                .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

            if (discount is null)
            {
                throw new MarketNotFoundException($"Popust s ID-em {request.Id} nije pronađen.");
            }
      
            ctx.Discounts.Remove(discount);
            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
