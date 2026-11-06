using Market.Application.Abstractions;
using Market.Shared.Dtos;
using Microsoft.AspNetCore.Authorization; // OVO JE BITNO
using Microsoft.AspNetCore.Mvc;
using Market.Application.Common.Exceptions;

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

        // 1b. Lista s 5 filtera, sortiranjem i backend pagingom
        // GET api/admin/users/paged?search=&roleId=&isEnabled=&createdFrom=&createdTo=&sortBy=&sortDir=&page=&pageSize=
        [HttpGet("paged")]
        public async Task<IActionResult> GetUsersPaged([FromQuery] UserFilterDto filter)
        {
            return Ok(await _userService.GetUsersPagedAsync(filter));
        }

        // Lista uloga za padajući meni
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _userService.GetRolesAsync());
        }

        // 2. Dodavanje korisnika
        [HttpPost]
        [AllowAnonymous] // Privremeno ostavljamo da možeš napraviti prvog Admina bez tokena
        public async Task<IActionResult> Create([FromBody] UserUpdateDto dto)
        {
            try
            {
                var result = await _userService.CreateUserAsync(dto);
                return Ok(result);
            }
            catch (MarketConflictException ex)
            {
                return Conflict(new { message = ex.Message }); // 409: email/username zauzet
            }
        }

        // 3. Uređivanje korisnika
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(id, dto);
                return Ok(result);
            }
            catch (MarketConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
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