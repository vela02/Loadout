namespace Market.Application.Modules.Users.Queries.GetProfile;

public class GetProfileDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}