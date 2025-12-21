using Market.Application.Modules.Users.Commands.ChangePassword;
using Market.Application.Modules.Users.Queries.GetProfile;

namespace Market.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    
    [HttpGet("profile/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<GetProfileDto>> GetProfile(int id)
        => Ok(await mediator.Send(new GetProfileQuery(id)));

    
    [HttpPost("change-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await mediator.Send(command);
        return Ok("Lozinka uspješno promijenjena.");
    }
}