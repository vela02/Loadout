using Market.Application.Modules.Reviews.Commands.Create;
using Market.Application.Modules.Reviews.Commands.Report;
using Market.Application.Modules.Reviews.Queries.GetByGame;

[ApiController]
[Route("api/reviews")]
public class ReviewsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetByGame(int gameId)
        => Ok(await mediator.Send(new GetGameReviewsQuery(gameId)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewCommand command)
        => Ok(await mediator.Send(command));

   
    [HttpPost("report/{commentId}")]
    public async Task<IActionResult> Report(int commentId)
    {
        var result = await mediator.Send(new ReportCommentCommand(commentId));

        if (!result)
            return NotFound("Komentar nije pronađen.");

        return Ok("Komentar je uspješno prijavljen administratoru.");
    }
}