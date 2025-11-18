namespace Restaurant.ViewModels.Rekening
{
    public class BestellingInfoRekeningViewModel
    {
        public DateTime? TijdstipBestelling { get; set; } = DateTime.Now;
        public string ProductNaam { get; set; } = string.Empty;
        public decimal PrijsPerEenheid { get; set; } = 0.0m;
        public int Aantal { get; set; } = 0;
    }
}
