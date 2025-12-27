namespace Market.Application.Modules.Reviews.Commands.Create;

public record CreateReviewCommand(
    int UserId,
    int GameId,
    string Text,
    int RatingValue
) : IRequest<bool>;