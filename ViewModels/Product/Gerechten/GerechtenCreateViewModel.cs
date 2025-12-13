namespace Restaurant.ViewModels.Product.Gerechten
{
    public class GerechtenCreateViewModel
    {
        [Required(ErrorMessage = "Naam is verplicht")]
        public string Naam { get; set; }
        [Required(ErrorMessage = "Beschrijving is verplicht")]
        public string Beschrijving { get; set; }
        public string AllergenenInfo { get; set; }
        [Required(ErrorMessage = "Categorie is verplicht")]
        public int CategorieId { get; set; }
        public bool Actief { get; set; }
        [Required(ErrorMessage = "Prijs is verplicht")]
        public decimal Prijs { get; set; }

        // Voor dropdown
        public List<Categorie> CategorieList { get; set; } = new();
    }
}
