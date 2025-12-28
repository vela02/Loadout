using MediatR;


namespace Market.Application.Modules.Wishlist.Commands.Add;

public sealed class AddWishlistCommandHandler(IAppDbContext ctx) : IRequestHandler<AddWishlistCommand, bool>
{
    public async Task<bool> Handle(AddWishlistCommand request, CancellationToken ct)
    {
        
        var gameExists = await ctx.Products.AnyAsync(g => g.Id == request.GameId, ct);

        if (!gameExists)
        {
            return false; 
        }
     
        var existsInWishlist = await ctx.Wishlists
            .AnyAsync(w => w.UserId == request.UserId && w.GameId == request.GameId, ct);

        if (existsInWishlist) return false;
     
        var item = new Domain.Models.Wishlist
        {
            UserId = request.UserId,
            GameId = request.GameId,
            AddedAt = DateTime.UtcNow 
        };

        ctx.Wishlists.Add(item);
        await ctx.SaveChangesAsync(ct);
        return true;
    }
}