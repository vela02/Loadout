using Market.Application.Modules.Refunds.Commands.Create;
using Market.Application.Modules.Refunds.Queries.GetByUser;

namespace Market.API.Controllers;

[Authorize] 
[ApiController]
[Route("api/refunds")]
public class RefundsController(IMediator mediator) : ControllerBase
{
    // GET: api/refunds
    [HttpGet]
    public async Task<IActionResult> GetMyRefunds()
    {
        var result = await mediator.Send(new GetUserRefundsQuery());
        return Ok(result);
    }

    // POST: api/refunds
    [HttpPost]
    public async Task<IActionResult> RequestRefund([FromBody] CreateRefundDto dto)
    {
        try
        {
            var result = await mediator.Send(new CreateRefundCommand(dto));

            
            if (!result) return BadRequest("Order not found or invalid.");

            return Ok("Refund request submitted successfully.");
        }
        catch (InvalidOperationException ex)
        {
            // Returns 400 for business logic errors (e.g. "Refund period expired", "Duplicate request")
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Returns 401/403 if trying to refund someone else's order
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            
            return StatusCode(500, "An error occurred while processing your request."); //could be anything unexpected but its aight
        }
    }
}