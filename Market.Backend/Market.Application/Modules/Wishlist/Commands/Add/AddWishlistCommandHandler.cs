using MediatR;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;
using Market.Domain.Models;

namespace Market.Application.Modules.Wishlist.Commands.Add;

public sealed class AddWishlistCommandHandler(IAppDbContext ctx) : IRequestHandler<AddWishlistCommand, bool>
{
    public async Task<bool> Handle(AddWishlistCommand request, CancellationToken ct)
    {
        // Provjera duplikata
        var exists = await ctx.Wishlists.AnyAsync(w => w.UserId == request.UserId && w.GameId == request.GameId, ct);
        if (exists) return false;

        var item = new Domain.Models.Wishlist { UserId = request.UserId, GameId = request.GameId, AddedAt=DateTime.Now }; // ?
        ctx.Wishlists.Add(item);
        await ctx.SaveChangesAsync(ct);
        return true;
    }
}