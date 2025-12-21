using MediatR;
namespace Market.Application.Modules.Notifications.Commands.MarkAsRead;

public record MarkNotificationAsReadCommand(int NotificationId) : IRequest<bool>;