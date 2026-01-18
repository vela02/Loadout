using Market.Application.Modules.Reviews.Commands.Create;
using Market.Application.Modules.Reviews.Commands.Report;
using Market.Application.Modules.Reviews.Queries.GetByGame;

[ApiController]
[Route("api/reviews")]
public class ReviewsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{gameId}")]
    [AllowAnonymous]
    public async Task<ActionResult<PageResult<GetGameReviewsDto>>>   GetByGame(
        int gameId,
        [FromQuery] int page=1,
        [FromQuery] int pageSize=10,
        [FromQuery] int? minStars= null
        )
    {
       var query= new GetGameReviewsQuery
        {
            GameId = gameId,
            MinStars = minStars,
            Paging =new PageRequest { Page=page, PageSize=pageSize}
           
        };
       
        return Ok(await mediator.Send(query));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReviewCommand command)
    {
        try
        {
            await mediator.Send(command);
            return Ok("Recenzija uspješno dodana.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); 
        }
    }


    [HttpPost("report/{commentId}")]
    [Authorize]
    public async Task<IActionResult> Report(int commentId)
    {
        var result = await mediator.Send(new ReportCommentCommand(commentId));

        if (!result)
            return NotFound("Komentar nije pronađen.");

        return Ok("Komentar je uspješno prijavljen administratoru.");
    }
}