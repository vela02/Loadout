using Market.Application;
using Market.Application.Abstractions;
using Market.Infrastructure.Database;
using Market.Shared.Dtos;
using Market.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Services;

public class GameService : IGameService
{
    private readonly LoadoutDbContext _context; 

    public GameService(LoadoutDbContext context)
    {
        _context = context;
    }
    public async Task<List<GameListDto>> GetFilteredGamesAsync(
    string? searchTerm,
    decimal? minPrice,
    decimal? maxPrice,
    string? genre,
    int? categoryId,
    Market.Shared.Enums.GameContentType? type,
    int? minRating)
    {
        // 1. Osnovni upit sa potrebnim relacijama
        var query = _context.Games
            .Include(g => g.Category)
            .Include(g => g.Ratings)
            .Where(g => !g.IsDeleted && g.IsEnabled)
            .AsQueryable();

        // 2. GLOBALNA PRETRAGA (Search) - Naslov ili Opis
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // .Contains generiše SQL LIKE '%pojam%'
            query = query.Where(g => g.Title.Contains(searchTerm) ||
                                     (g.Description != null && g.Description.Contains(searchTerm)));
        }

        // 3. Filtriranje po CIJENI
        if (minPrice.HasValue)
        {
            query = query.Where(g => g.Price != null && g.Price.Value >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(g => g.Price != null && g.Price.Value <= maxPrice.Value);
        }

        // 4. Filtriranje po ŽANRU
        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(g => g.Genre == genre);
        }

        // 5. Filtriranje po KATEGORIJI
        if (categoryId.HasValue)
        {
            query = query.Where(g => g.CategoryId == categoryId);
        }

        // 6. Filtriranje po TIPU SADRŽAJA (Enum)
        if (type.HasValue)
        {
            query = query.Where(g => g.ContentType == type);
        }

        // 7. Mapiranje na DTO i računanje prosječne ocjene (SQL strana)
        var games = await query.Select(g => new GameListDto
        {
            Id = g.Id,
            Title = g.Title,
            Price = g.Price ?? 0,
            ImageUrl = g.ImageUrl,
            Genre = g.Genre,
            CategoryName = g.Category != null ? g.Category.Name : "N/A",
            // Pretpostavka: polje u tabeli Ratings se zove Value
            AverageRating = g.Ratings.Any() ? g.Ratings.Average(r => (double)r.Stars) : 0
        }).ToListAsync();

        // 8. Naknadno filtriranje po OCJENI (Memorijska strana, jer je AverageRating izračunat)
        if (minRating.HasValue)
        {
            games = games.Where(g => g.AverageRating >= minRating.Value).ToList();
        }

        return games;
    }


    public async Task<GameDetailsDto?> GetGameDetailsAsync(int id)
    {
        var game = await _context.Games
            .Include(g => g.Category)
            .Include(g => g.Comments)
            .Include(g => g.Ratings)
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted);

        if (game == null) return null;

        return new GameDetailsDto
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            Price = game.Price ?? 0,
            ImageUrl = game.ImageUrl,
            Genre = game.Genre,
            Developer = game.Developer,
            Publisher = game.Publisher,
            ReleaseDate = game.ReleaseDate,
            AverageRating = game.Ratings.Any() ? game.Ratings.Average(r => (double)r.Stars) : 0,
            Reviews = game.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Text = c.Content,
                Username = c.User.Username,
                DateCreated = DateTime.Now
            }).ToList()
        };
    }
}