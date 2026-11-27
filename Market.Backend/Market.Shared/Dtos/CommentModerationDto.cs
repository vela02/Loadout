using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class CommentModerationDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string GameTitle { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Dodatno za moderaciju
        public int? GameId { get; set; }
        public int? UserId { get; set; }
        public int? Rating { get; set; }       // ocjena koju je autor dao igri (1–5), ako postoji
        public bool IsReported { get; set; }   // korisnik je prijavio komentar kao neprikladan
    }
}
