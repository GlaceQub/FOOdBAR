namespace Restaurant.ViewModels.Reservation
{
    public class ReservationViewModel
    {
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
    }

    // Eenvoudige DTO voor tijdslot-selectie
    public class TijdslotDto
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
    }
}

