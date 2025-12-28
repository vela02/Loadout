namespace Market.Application.Modules.Wishlist.Commands.Add;
public record AddWishlistCommand(int UserId, int GameId) : IRequest<bool>;