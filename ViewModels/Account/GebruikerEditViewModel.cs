namespace Restaurant.ViewModels.Account
{
    public class GebruikerEditViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;
        public string? Voornaam { get; set; }
        public string? Achternaam { get; set; }

        [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
        [Required(ErrorMessage = "E-mailadres is verplicht!")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mailadres")]
        public string? Email { get; set; }
        public string? Adres { get; set; }
        public string? Huisnummer { get; set; }
        public string? Postcode { get; set; }
        public string? Gemeente { get; set; }
        public bool Actief { get; set; }
        public int LandId { get; set; }
        public string wachtwoord { get; set; } = string.Empty;

        // List of role names the user currently has
        [Required]
        public IEnumerable<string> Rollen { get; set; } = new List<string>();

        public IEnumerable<SelectListItem> AllRollen { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Landen { get; set; } = new List<SelectListItem>();
    }
}
