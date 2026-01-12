using Market.Application.Abstractions;
using Market.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    // POST: api/Auth/login
    [HttpPost("login")]
    [AllowAnonymous] // Dozvoljava pristup bez tokena 
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _userService.LoginAsync(dto);

        if (result == null)
        {
            return Unauthorized(new { Message = "Pogrešan email ili lozinka!" });
        }

        // Ako je sve u redu, vraća tokene (Access i Refresh), username i ulogu
        return Ok(result);
    }
}