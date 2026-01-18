namespace Market.Application.Modules.Wishlist.Commands.Add;
public record AddWishlistCommand(int GameId) : IRequest<bool>;