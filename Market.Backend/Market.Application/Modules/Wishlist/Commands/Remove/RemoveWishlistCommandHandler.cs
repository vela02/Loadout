using MediatR;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;

namespace Market.Application.Modules.Wishlist.Commands.Remove;

public sealed class RemoveWishlistCommandHandler(IAppDbContext ctx, IAppCurrentUser currentUser) : IRequestHandler<RemoveWishlistCommand, bool>
{
    public async Task<bool> Handle(RemoveWishlistCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId == null) return false;

        var item = await ctx.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId && w.GameId == request.GameId, ct);
        if (item == null) return false;

        ctx.Wishlists.Remove(item);
        await ctx.SaveChangesAsync(ct);
        return true;
    }
}