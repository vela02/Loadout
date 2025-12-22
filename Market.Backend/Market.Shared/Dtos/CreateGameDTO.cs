using Market.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class CreateGameDTO
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Genre { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }
        public int CategoryId { get; set; }
        public GameContentType ContentType { get; set; }
        public string? TrailerUrl { get; set; }
        public string? SystemRequirements { get; set; }
    }
}
