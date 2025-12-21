using MediatR;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;
using Market.Application.Modules.Wish_List.Queries.GetByUser;

namespace Market.Application.Modules.Wish_List.Queries.GetByUser;

public sealed class GetWishlistHandler(IAppDbContext ctx)
    : IRequestHandler<GetWishlistQuery, List<GetWishlistDto>>
{
    public async Task<List<GetWishlistDto>> Handle(GetWishlistQuery request, CancellationToken ct)
    {
        return await ctx.Wishlists
            .Where(w => w.UserId == request.UserId)
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