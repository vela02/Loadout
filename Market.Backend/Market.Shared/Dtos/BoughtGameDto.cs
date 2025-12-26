using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class BoughtGameDto
    {
        public string Title { get; set; } = null!;
        public decimal PriceAtPurchase { get; set; }
        public List<string> LicenseKeys { get; set; } = new();
    }
}
