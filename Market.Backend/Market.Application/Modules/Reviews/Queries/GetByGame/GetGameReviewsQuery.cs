
namespace Market.Application.Modules.Reviews.Queries.GetByGame;

public record GetGameReviewsQuery(int GameId) : IRequest<List<GetGameReviewsDto>>;