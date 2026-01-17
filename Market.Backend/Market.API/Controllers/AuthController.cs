using Market.Application.Modules.Auth.Commands.Login;
using Market.Application.Modules.Auth.Commands.Logout;
using Market.Application.Modules.Auth.Commands.Refresh;
using Market.Application.Modules.Auth.Commands.Register;


namespace Market.API.Controllers;

[Route("api/auth")]
[ApiController]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")] //the method registers a new user with a customer role, to register as admin use DataSeeder
    [AllowAnonymous]
    public async Task<ActionResult<RegisterCommandDto>> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        return Ok(await mediator.Send(command, ct));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginCommandDto>> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        return Ok(await mediator.Send(command, ct));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginCommandDto>> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        return Ok(await mediator.Send(command, ct));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task Logout([FromBody] LogoutCommand command, CancellationToken ct)
    {
        await mediator.Send(command, ct);
    }
}


//[Route("api/[controller]")]
//[ApiController]
//public class AuthController : ControllerBase
//{
//    private readonly IUserService _userService;

//    public AuthController(IUserService userService)
//    {
//        _userService = userService;
//    }

//    // POST: api/Auth/login
//    [HttpPost("login")]
//    [AllowAnonymous] // Dozvoljava pristup bez tokena 
//    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
//    {
//        var result = await _userService.LoginAsync(dto);

//        if (result == null)
//        {
//            return Unauthorized(new { Message = "Pogrešan email ili lozinka!" });
//        }

//        // Ako je sve u redu, vraća tokene (Access i Refresh), username i ulogu
//        return Ok(result);
//    }
//}