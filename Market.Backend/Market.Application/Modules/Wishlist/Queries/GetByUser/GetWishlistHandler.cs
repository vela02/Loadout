using MediatR;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;
using Market.Application.Modules.Wish_List.Queries.GetByUser;

namespace Market.Application.Modules.Wish_List.Queries.GetByUser;

public sealed class GetWishlistHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<GetWishlistQuery, List<GetWishlistDto>>
{
    public async Task<List<GetWishlistDto>> Handle(GetWishlistQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        
        if (userId== null) return new List<GetWishlistDto>();
        return await ctx.Wishlists
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .Select(w => new GetWishlistDto
            {
                GameId = (int)w.GameId,
                GameTitle = w.Game.Title,
                Price = (decimal)w.Game.Price,
                ImageUrl = w.Game.ImageUrl
            })
            .ToListAsync(ct);
    }
}