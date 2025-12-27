
namespace Market.Application.Modules.Reviews.Queries.GetByGame;

public sealed class GetGameReviewsHandler(IAppDbContext ctx)
    : IRequestHandler<GetGameReviewsQuery, List<GetGameReviewsDto>>
{
    public async Task<List<GetGameReviewsDto>> Handle(GetGameReviewsQuery request, CancellationToken ct)
    {
        return await ctx.Comments
            .Where(c => c.GameId == request.GameId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new GetGameReviewsDto
            {
                Username = c.User.Username,
                Text = c.Content,
                Date = c.CreatedAt,
                // seeking for the rating given by the same user for the same game
                Rating = ctx.Ratings
                    .Where(r => r.UserId == c.UserId && r.GameId == c.GameId)
                    .Select(r => r.Stars)
                    .FirstOrDefault()
            })
            .ToListAsync(ct);
    }
}