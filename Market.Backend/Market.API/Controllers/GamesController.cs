using Market.Application;
using Market.Application.Abstractions;
using Market.Domain.Models;
using Market.Shared.Dtos;
using Market.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly LoadoutDbContext _context;
        private readonly IGameService _gameService;

        public GamesController(LoadoutDbContext context, IGameService gameService)
        {
            _context = context;
            _gameService = gameService;
        }

        // 1. GET: api/Games
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<GameListDto>>> GetGames(
            [FromQuery] string? searchTerm,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? genre,
            [FromQuery] int? categoryId,
            [FromQuery] Market.Shared.Enums.GameContentType? type,
            [FromQuery] int? minRating)
        {
            var games = await _gameService.GetFilteredGamesAsync(searchTerm,minPrice, maxPrice, genre, categoryId, type, minRating);
            return Ok(games);
        }

        // 2. GET: api/Games/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<GameDetailsDto>> GetGame(int id)
        {
            var game = await _gameService.GetGameDetailsAsync(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        // 3. POST: api/Games
        // POPRAVLJENO: Koristi CreateGameDto za čist Swagger JSON
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> PostGame(CreateGameDTO dto)
        {
            var game = new Game
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                Genre = dto.Genre,
                ReleaseDate = dto.ReleaseDate,
                Developer = dto.Developer,
                Publisher = dto.Publisher,
                CategoryId = dto.CategoryId,
                ContentType = dto.ContentType,
                TrailerUrl = dto.TrailerUrl,
                SystemRequirements = dto.SystemRequirements,
                IsEnabled = true,
                IsDeleted = false
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, new { id = game.Id, title = game.Title });
        }

        // 4. PUT: api/Games/5
        // POPRAVLJENO: Koristi CreateGameDto za ažuriranje
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutGame(int id, CreateGameDTO dto)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();

            // Ažuriranje polja
            game.Title = dto.Title;
            game.Description = dto.Description;
            game.Price = dto.Price;
            game.ImageUrl = dto.ImageUrl;
            game.Genre = dto.Genre;
            game.ReleaseDate = dto.ReleaseDate;
            game.Developer = dto.Developer;
            game.Publisher = dto.Publisher;
            game.CategoryId = dto.CategoryId;
            game.ContentType = dto.ContentType;
            game.TrailerUrl = dto.TrailerUrl;
            game.SystemRequirements = dto.SystemRequirements;

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // 5. DELETE: api/Games/5
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();

            // Umjesto fizičkog brisanja, možemo raditi soft delete
            game.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}