
using Microsoft.EntityFrameworkCore;
using Market.API.Models; // Ovdje koristimo tvoj namespace

namespace Market.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly LoadoutDbContext _context;

        // Konstruktor: Prima pristup bazi (Dependency Injection)
        public GamesController(LoadoutDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/Games
        // Vraća listu svih igara
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            // .Include(g => g.Category) znači: "Kad povučeš igru, povuci i njenu Kategoriju"
            // Tako na frontendu nećeš imati samo CategoryId, nego i ime kategorije.
            return await _context.Games
                                 .Include(g => g.Category)
                                 .ToListAsync();
        }

        // 2. GET: api/Games/5
        // Vraća samo jednu igru po ID-u
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
            var game = await _context.Games
                                     .Include(g => g.Category) // Uključi kategoriju i ovdje
                                     .Include(g => g.Tags)     // Možda želiš i tagove za detaljan prikaz
                                     .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            return game;
        }

        // 3. POST: api/Games
        // Dodaje novu igru (ovo će ti trebati za Admin panel)
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Game>> PostGame(Game game)
        {
            // Ako frontend pokuša poslati Category objekat, ignoriši ga, gledamo samo CategoryId
            game.Category = null;

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, game);
        }

        // 4. PUT: api/Games/5
        // Ažurira postojeću igru (mijenja cijenu, naslov itd.)
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutGame(int id, Game game)
        {
            if (id != game.Id)
            {
                return BadRequest();
            }

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // 5. DELETE: api/Games/5
        // Briše igru
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Pomoćna metoda da provjerimo postoji li igra
        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}