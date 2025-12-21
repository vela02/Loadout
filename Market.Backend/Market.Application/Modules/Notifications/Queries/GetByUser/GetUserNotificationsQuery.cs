using MediatR;
namespace Market.Application.Modules.Notifications.Queries.GetByUser;
public record GetUserNotificationsQuery(int UserId) : IRequest<List<NotificationDto>>;

public class NotificationDto
{
    public int Id { get; set; }
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}