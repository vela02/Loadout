using MediatR;
namespace Market.Application.Modules.Wishlist.Commands.Remove;
public record RemoveWishlistCommand(int UserId, int GameId) : IRequest<bool>;