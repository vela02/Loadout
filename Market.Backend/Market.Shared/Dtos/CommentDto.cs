using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Shared.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string? Username { get; set; } // Da se zna koji korisnik je napisao komentar
        public int? RatingValue { get; set; } // Opcionalno, ako je komentar vezan za ocjenu
        public DateTime DateCreated { get; set; }
    }
}
