
using System.Linq;

namespace Market.Application.Modules.Reviews.Queries.GetByGame;

public sealed class GetGameReviewsHandler(IAppDbContext ctx)
                          : IRequestHandler<GetGameReviewsQuery,PageResult<GetGameReviewsDto>>
{
    public async Task<PageResult<GetGameReviewsDto>> Handle(GetGameReviewsQuery request, CancellationToken ct)
    {
        //base query
        var query= ctx.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.GameId == request.GameId)
            .AsQueryable();


        if (request.MinStars.HasValue)
            {
            query = query.Where(c => ctx.Ratings.Any(r=>
            r.UserId == c.UserId &&
            r.GameId == c.GameId &&
            r.Stars >= request.MinStars.Value
            ));
        }
        var projectedQuery = query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new GetGameReviewsDto
            {
                Username = c.User != null ? c.User.Username : "Unknown",
                Text = c.Content,
                Date= c.CreatedAt,

                Rating = ctx.Ratings
                    .Where(r => r.UserId == c.UserId && r.GameId == c.GameId)
                    .Select(r => r.Stars)
                    .FirstOrDefault(),
                
            });

        return await PageResult<GetGameReviewsDto>
            .FromQueryableAsync(projectedQuery, request.Paging, ct, includeTotal: true);

    }
}