namespace Restaurant.ViewModels.Rekening
{
    public class AfrekenenViewModel
    {
        [Required(ErrorMessage = "Gelieve een reservatie ID op te geven.")]
        public int ReservatieId { get; set; } = 0;
        [Required(ErrorMessage = "Gelieve een betaalmethode te selecteren.")]
        public string BetaalMethode { get; set; } = string.Empty;
        
        public IEnumerable<BestellingInfoRekeningViewModel> BestellingenInfoRekening { get; set; } = new List<BestellingInfoRekeningViewModel>();
        public decimal TotaalPrijs { get; set; } = 0;
        public string TafelNummer { get; set; } = string.Empty;
        public string KlantNaam { get; set; } = string.Empty;
        public string KlantVoornaam { get; set; } = string.Empty;


    }
}