using MediatR;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;

namespace Market.Application.Modules.Users.Queries.GetProfile;

public sealed class GetProfileHandler(IAppDbContext ctx,IAppCurrentUser currentUser) : IRequestHandler<GetProfileQuery, GetProfileDto>
{
    // OrderStatus.Id of a cancelled order - not counted in spent money / owned games
    private const int CancelledStatusId = 5;

    public async Task<GetProfileDto> Handle(GetProfileQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new Exception("Korisnik nije prijavljen.");

        var profile = await ctx.Users
            .Where(u => u.Id == userId)
            .Select(u => new GetProfileDto
            {
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLogin,
                Role = u.Role != null ? u.Role.Name : null
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new Exception("Korisnik nije pronađen.");

        var orders = ctx.Orders.Where(o => o.UserId == userId);
        var validOrders = orders.Where(o => o.StatusId != CancelledStatusId);

        profile.OrdersCount = await orders.CountAsync(ct);
        profile.TotalSpent = await validOrders.SumAsync(o => o.TotalAmount ?? 0, ct);
        profile.OwnedGamesCount = await validOrders
            .SelectMany(o => o.OrderGames)
            .Select(og => og.GameId)
            .Distinct()
            .CountAsync(ct);
        profile.WishlistCount = await ctx.Wishlists.CountAsync(w => w.UserId == userId, ct);

        profile.RecentOrders = await orders
            .OrderByDescending(o => o.Date)
            .Take(5)
            .Select(o => new GetProfileRecentOrderDto
            {
                Id = o.Id,
                Date = o.Date,
                TotalAmount = o.TotalAmount ?? 0,
                StatusId = o.StatusId,
                Status = o.Status != null ? o.Status.Name : "-",
                ItemsCount = o.OrderGames.Sum(og => og.Quantity)
            })
            .ToListAsync(ct);

        return profile;
    }
}
