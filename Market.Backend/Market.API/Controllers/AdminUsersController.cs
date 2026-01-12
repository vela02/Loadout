using Market.Application.Abstractions;
using Market.Shared.Dtos;
using Microsoft.AspNetCore.Authorization; // OVO JE BITNO
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Administrator")] // Zaključava cijeli kontroler samo za Admine
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUsersController(IUserService userService)
        {
            _userService = userService;
        }

        // 1. Pregled i pretraga korisnika
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] string? search)
        {
            var users = await _userService.GetAllUsersAsync(search);
            return Ok(users);
        }

        // 2. Dodavanje korisnika
        [HttpPost]
        [AllowAnonymous] // Privremeno ostavljamo da možeš napraviti prvog Admina bez tokena
        public async Task<IActionResult> Create([FromBody] UserUpdateDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);
            return Ok(result);
        }

        // 3. Uređivanje korisnika
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            var result = await _userService.UpdateUserAsync(id, dto);
            if (!result) return NotFound("Korisnik nije pronađen.");
            return Ok(result);
        }

        // 4. Brisanje korisnika (Soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result) return NotFound("Korisnik nije pronađen.");
            return Ok(result);
        }

        // 5. Pregled historije po korisniku
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetUserHistory(int id)
        {
            var history = await _userService.GetUserPurchaseHistoryAsync(id);
            return Ok(history);
        }
    }
}