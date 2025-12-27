using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class OrderGameDto
    {
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public int Quantity { get; set; }
        public List<string> LicenseKeys { get; set; } = new();
    }
}
