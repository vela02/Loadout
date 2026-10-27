namespace Market.Application.Modules.Users.Queries.GetProfile;

public class GetProfileDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? Role { get; set; }

    // account stats
    public int OrdersCount { get; set; }
    public decimal TotalSpent { get; set; }
    public int OwnedGamesCount { get; set; }
    public int WishlistCount { get; set; }

    public List<GetProfileRecentOrderDto> RecentOrders { get; set; } = new();
}

public class GetProfileRecentOrderDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public int? StatusId { get; set; }
    public string Status { get; set; } = "-";
    public int ItemsCount { get; set; }
}
