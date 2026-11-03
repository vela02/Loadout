using Market.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class GameListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public string? Genre { get; set; }
        public double AverageRating { get; set; }
        public GameContentType ContentType { get; set; }
        public DateOnly? ReleaseDate { get; set; }

        // Pre-order = igra još nije izašla (datum izlaska je u budućnosti)
        public bool IsPreOrder => ReleaseDate.HasValue && ReleaseDate.Value > DateOnly.FromDateTime(DateTime.Today);
    }
}
