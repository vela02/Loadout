using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsSavedForLater { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public bool IsPreOrder => ReleaseDate.HasValue && ReleaseDate.Value > DateOnly.FromDateTime(DateTime.Today);
        public decimal Total => Price * Quantity;
    }
}
