using FluentValidation;
using Market.Application.Common.Exceptions;
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
    [Authorize]
    public async Task<IActionResult> GetMyRefunds()
    {
        var result = await mediator.Send(new GetUserRefundsQuery());
        return Ok(result);
    }

    // POST: api/refunds
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RequestRefund([FromBody] CreateRefundDto dto)
    {
        try
        {
            await mediator.Send(new CreateRefundCommand(dto));
            return Ok("Refund request submitted successfully.");
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Validacija nije prošla", errors = errors });
        }  

        catch (MarketNotFoundException ex)
        {

            return NotFound(ex.Message);
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