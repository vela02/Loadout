using MediatR;
using Microsoft.EntityFrameworkCore;
using Market.Application.Abstractions;

namespace Market.Application.Modules.Notifications.Commands.MarkAsRead;

public sealed class MarkNotificationAsReadHandler(IAppDbContext ctx)
    : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken ct)
    {
        var notification = await ctx.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId, ct);

        if (notification == null) return false;

        notification.IsRead = true; 
        

        await ctx.SaveChangesAsync(ct);
        return true;
    }
}