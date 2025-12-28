namespace Market.Application.Modules.Reviews.Queries.GetByGame;

public class GetGameReviewsDto
{
    public string Username { get; set; } = null!;
    public string Text { get; set; } = null!;
    public int Rating { get; set; }
    public DateTime Date { get; set; }
}