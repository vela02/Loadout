namespace Market.Application.Modules.Wish_List.Queries.GetByUser;

public class GetWishlistDto
{
    public int GameId { get; set; }
    public string GameTitle { get; set; } = null!;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}