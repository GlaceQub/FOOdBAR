namespace Restaurant.ViewModels
{
    public class CategorieCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naam is verplicht")]
        public string? Naam { get; set; }

        [Required(ErrorMessage = "Type is verplicht")]
        public int TypeId { get; set; }

        public bool Actief { get; set; }

        // Voor de dropdown
        public IEnumerable<CategorieType>? TypeList { get; set; }
    }
}
