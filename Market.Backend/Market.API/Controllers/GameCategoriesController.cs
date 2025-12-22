using Market.Application;
using Market.Domain.Models;
using Market.Shared.Dtos; // Dodaj ovo
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameCategoriesController : ControllerBase
    {
        private readonly LoadoutDbContext _context;

        public GameCategoriesController(LoadoutDbContext context)
        {
            _context = context;
        }

        // POPRAVLJENO: Vraća samo listu DTO-ova (kratak JSON)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            return await _context.GameCategories
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        // POPRAVLJENO: Prima samo ime, ne cijelu bazu podataka
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> PostCategory(CategoryDTO dto)
        {
            var category = new GameCategory
            {
                Name = dto.Name,
                IsEnabled = true, // Postavi defaultne vrijednosti
                IsDeleted = false
            };

            _context.GameCategories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { id = category.Id, name = category.Name });
        }
    }
}