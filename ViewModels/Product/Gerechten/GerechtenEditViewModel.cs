namespace Restaurant.ViewModels.Product.Gerechten
{
    public class GerechtenEditViewModel
    {
        
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Beschrijving { get; set; }
        public string AllergenenInfo { get; set; }
        [Required]
        [Range(1.00, 100.00, ErrorMessage = "Prijs moet tussen 1.00 en 100.00 liggen")]
        public decimal Prijs { get; set; }
        public int CategorieId { get; set; }
        public bool Actief { get; set; }
        public bool IsSuggestie { get; set; }

        // Voor de dropdown
        public IEnumerable<Restaurant.Models.Categorie>? CategorieList { get; set; }

    }
}
