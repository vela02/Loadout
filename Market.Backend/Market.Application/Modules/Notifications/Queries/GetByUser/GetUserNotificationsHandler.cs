using MediatR;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;

namespace Market.Application.Modules.Notifications.Queries.GetByUser;

public sealed class GetUserNotificationsHandler(IAppDbContext ctx, IAppCurrentUser currentUser) 
                                                           : IRequestHandler<GetUserNotificationsQuery, List<NotificationDto>>
{
    public async Task<List<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null)
        {
            throw new UnauthorizedAccessException("You are not logged in");
        }
        return await ctx.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToListAsync(ct);
    }
}