using MediatR;

namespace Market.Application.Modules.Wish_List.Queries.GetByUser;

public record GetWishlistQuery() : IRequest<List<GetWishlistDto>>;