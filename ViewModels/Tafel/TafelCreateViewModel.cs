namespace Restaurant.ViewModels.Tafel
{
    public class TafelCreateViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Tafelnummer is verplicht")]
        public string? TafelNummer { get; set; }
        [Required(ErrorMessage = "Aantal personen is verplicht")]
        public int AantalPersonen { get; set; }
        [Required(ErrorMessage = "Minimum aantal personen is verplicht")]
        public int MinAantalPersonen { get; set; }
        public bool Actief { get; set; }
        [Required(ErrorMessage = "QR Barcode is verplicht")]
        public string? QrBarcode { get; set; }

    }
}
