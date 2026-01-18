using MediatR;


namespace Market.Application.Modules.Wishlist.Commands.Add;

public sealed class AddWishlistCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser) : IRequestHandler<AddWishlistCommand, bool>
{
    public async Task<bool> Handle(AddWishlistCommand request, CancellationToken ct)
    {
        
        var userId= currentUser.UserId;

        if (userId is null)
        {
            return false;
        }

        var gameExists = await ctx.Products.AnyAsync(g => g.Id == request.GameId, ct);

        if (!gameExists)
        {
            return false; 
        }
     
        var existsInWishlist = await ctx.Wishlists
            .AnyAsync(w => w.UserId == userId && w.GameId == request.GameId, ct);

        if (existsInWishlist) return false;
     
        var item = new Domain.Models.Wishlist
        {
            UserId = userId.Value,
            GameId = request.GameId,
            AddedAt = DateTime.UtcNow 
        };

        ctx.Wishlists.Add(item);
        await ctx.SaveChangesAsync(ct);
        return true;
    }
}