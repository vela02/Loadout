using MediatR;

namespace Market.Application.Modules.Wish_List.Queries.GetByUser;

public record GetWishlistQuery(int UserId) : IRequest<List<GetWishlistDto>>;