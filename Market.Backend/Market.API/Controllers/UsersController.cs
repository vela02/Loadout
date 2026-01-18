using Market.Application.Modules.Users.Commands.ChangePassword;
using Market.Application.Modules.Users.Queries.GetProfile;

namespace Market.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<GetProfileDto>> GetProfile()
        => Ok(await mediator.Send(new GetProfileQuery()));

    
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await mediator.Send(command);
        return Ok("Lozinka uspješno promijenjena.");
    }
}