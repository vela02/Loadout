using Market.Shared.Dtos;
using Market.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Abstractions
{
    public interface IGameService
    {
        Task<List<GameListDto>> GetFilteredGamesAsync(
        string? searchTerm,
        decimal? minPrice,
        decimal? maxPrice,
        string? genre,
        int? categoryId,
        GameContentType? type,
        int? minRating);

        Task<GameDetailsDto?> GetGameDetailsAsync(int id);
    }
}
