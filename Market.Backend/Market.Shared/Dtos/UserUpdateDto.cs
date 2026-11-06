using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Market.Shared.Dtos
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati 3–50 znakova.")]
        [RegularExpression(@"^[A-Za-z0-9._-]+$", ErrorMessage = "Dozvoljena su slova, brojevi i . _ -")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Email nije ispravan.")]
        [StringLength(255)]
        public string Email { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Uloga je obavezna.")]
        public int RoleId { get; set; }

        public bool IsEnabled { get; set; }

        // Opcionalno: pri izmjeni prazno = šifra se ne mijenja
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Šifra mora imati najmanje 6 znakova.")]
        public string? Password { get; set; }

    }
}
