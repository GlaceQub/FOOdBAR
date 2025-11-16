namespace Restaurant.ViewModels.Rekening
{
    public class OverzichtViewModel
    {
        //Bestellingen
        [Required]
        public IEnumerable<Bestelling> GeleverdeBestellingen { get; set; } = new List<Bestelling>();
        [Required]
        public decimal TotaalPrijs { get; set; }
        [Required]
        public int TafelNummer { get; set; }

        //Klant
        [Required]
        public string KlantNaam { get; set; } = string.Empty;
        [Required]
        public string KlantVoornaam { get; set; } = string.Empty;

    }
}