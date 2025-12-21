namespace Market.Application.Modules.Notifications.Queries.GetByUser;

public class GetNotificationsDto
{
    public int Id { get; set; }
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}