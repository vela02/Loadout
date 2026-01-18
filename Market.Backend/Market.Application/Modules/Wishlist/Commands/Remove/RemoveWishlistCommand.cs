using MediatR;
namespace Market.Application.Modules.Wishlist.Commands.Remove;
public record RemoveWishlistCommand(int GameId) : IRequest<bool>;