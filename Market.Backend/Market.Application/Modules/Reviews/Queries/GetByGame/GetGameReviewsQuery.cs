
namespace Market.Application.Modules.Reviews.Queries.GetByGame;

public class GetGameReviewsQuery() :BasePagedQuery<GetGameReviewsDto>
{
       public int GameId { get; set; }
       public int? MinStars { get; set; }

}