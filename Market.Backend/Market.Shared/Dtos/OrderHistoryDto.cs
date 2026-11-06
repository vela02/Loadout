using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class OrderHistoryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int? StatusId { get; set; }
        public string? ReferenceNumber { get; set; }
        // true samo za aktivan pre-order čije igre još nisu izašle
        public bool CanCancel { get; set; }
        public List<BoughtGameDto> Games { get; set; } = new();
    }
}
