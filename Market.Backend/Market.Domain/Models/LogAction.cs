using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Models
{
    public class LogAction
    {
        public int Id { get; set; }
        public string AdminUsername { get; set; } = null!;

        // Tip akcije: npr. "DELETE", "UPDATE", "CREATE", "LOGIN"
        public string ActionType { get; set; } = null!;

        // Na koju tabelu se odnosi: npr. "Comments", "Games"
        public string EntityName { get; set; } = null!;

        // ID reda koji je izmijenjen
        public int? EntityId { get; set; }

        public string Message { get; set; } = null!;
        public DateTime Timestamp { get; set; }

        // Opcionalno: IP adresa administratora (izgleda jako profi)
        public string? IpAddress { get; set; }
    }
}
