using Market.Domain.Models;

namespace Market.Application.Modules.Reviews.Commands.Create;

public sealed class CreateReviewCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateReviewCommand, bool>
{
    public async Task<bool> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        //  checking if user has purchased the game
        var hasPurchased = await ctx.Orders
            .AnyAsync(o => o.UserId == request.UserId &&
                           o.OrderGames.Any(og => og.GameId == request.GameId), ct);

        if (!hasPurchased)
            throw new Exception("Možete ocijeniti samo igre koje ste kupili.");

        // checiking if user has already reviewed the game
        var alreadyReviewed = await ctx.Comments
            .AnyAsync(c => c.UserId == request.UserId && c.GameId == request.GameId, ct);

        if (alreadyReviewed)
            throw new Exception("Već ste ostavili recenziju za ovu igru.");

        
        var rating = new Rating
        {
            UserId = request.UserId,
            GameId = request.GameId,
            Stars = request.RatingValue,
            CreatedAt = DateTime.Now

        };
        ctx.Ratings.Add(rating);

        
        var comment = new Comment
        {
            UserId = request.UserId,
            GameId = request.GameId,
            Content = request.Text,
            CreatedAt = DateTime.Now 
        };
        ctx.Comments.Add(comment);

        await ctx.SaveChangesAsync(ct);
        return true;
    }
}