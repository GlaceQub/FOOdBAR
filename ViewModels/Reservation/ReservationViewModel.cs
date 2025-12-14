using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Restaurant.ViewModels.Reservation
{
    public class ReservationViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Datum is verplicht")]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }

        [Required(ErrorMessage = "Tijdslot is verplicht")]
        public int TijdSlotId { get; set; }

        [Required(ErrorMessage = "Aantal personen is verplicht")]
        [Range(1, 20, ErrorMessage = "Aantal personen moet tussen 1 en 20 zijn")]
        public int AantalPersonen { get; set; }

        [Display(Name = "Speciale verzoeken")]
        public string? Opmerking { get; set; }

        public List<TijdslotDto> LunchTijdsloten { get; set; } = new();
        public List<TijdslotDto> DinerTijdsloten { get; set; } = new();

        // Model-level validatie (server-side)
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Datum.Date < DateTime.Today)
            {
                yield return new ValidationResult("Datum kan niet in het verleden liggen.", new[] { nameof(Datum) });
            }

            if (TijdSlotId <= 0)
            {
                yield return new ValidationResult("Kies een geldig tijdslot.", new[] { nameof(TijdSlotId) });
            }
        }
    }

    // Eenvoudige DTO voor tijdslot-selectie
    public class TijdslotDto
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
    }
}