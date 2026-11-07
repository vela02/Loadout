using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Market.Shared.Dtos
{
    public class CreateNotificationDto : IValidatableObject
    {
        [Required(ErrorMessage = "Naslov je obavezan.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Naslov mora imati 3–150 znakova.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Tekst vijesti je obavezan.")]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "Tekst mora imati 10–4000 znakova.")]
        public string Message { get; set; } = null!;

        [Required(ErrorMessage = "Tip je obavezan.")]
        [RegularExpression("^(Sale|Info|Maintenance|Update)$", ErrorMessage = "Tip mora biti Sale, Info, Maintenance ili Update.")]
        public string Type { get; set; } = "Info"; // Defaultni tip

        // Nova vijest je po defaultu odmah vidljiva u feedu
        public bool IsActive { get; set; } = true;

        // Zakazivanje (opcionalno)
        public DateTime? PublishAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        /// <summary>Validacija koja zavisi od više polja: kraj mora biti poslije početka.</summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PublishAt.HasValue && ExpiresAt.HasValue && ExpiresAt <= PublishAt)
                yield return new ValidationResult("Datum isteka mora biti poslije datuma objave.", new[] { nameof(ExpiresAt) });
        }
    }
}
