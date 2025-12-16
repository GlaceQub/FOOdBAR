using System.ComponentModel.DataAnnotations;

namespace Restaurant.ViewModels.Account
{
    public class RegistratieViewModel
    {
        [StringLength(50)]
        public string? Voornaam { get; set; }

        [StringLength(50)]
        public string? Naam { get; set; }

        [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
        [Required(ErrorMessage = "E-mailadres is verplicht!")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mailadres")]
        public string Email { get; set; } = "";

        [StringLength(100)]
        public string? Adres { get; set; }

        [StringLength(10)]
        public string? Huisnummer { get; set; }

        [StringLength(10)]
        public string? Postcode { get; set; }

        [StringLength(50)]
        public string? Gemeente { get; set; }

        [Required(ErrorMessage = "Land is verplicht!")]
        public int Land { get; set; } = 1;

        public string? LandNaam { get; set; }

        public IEnumerable<SelectListItem>? Landen { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Het wachtwoord moet minstens 6 tekens bevatten.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$", ErrorMessage = "Het wachtwoord moet minstens één hoofdletter, één kleine letter, één cijfer en één speciaal teken bevatten.")]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Tweede wachtwoord is verplicht in te vullen.")]
        [Compare("Password", ErrorMessage = "De wachtwoorden komen niet overeen.")]
        [DataType(DataType.Password)]
        [Display(Name = "Herhaal wachtwoord")]
        public string ConfirmPassword { get; set; } = "";

        [Required(ErrorMessage = "Rollen zijn verplicht!")]
        public IEnumerable<string> Rollen { get; set; } = new List<string> { "Klant" };

        public IEnumerable<SelectListItem> AllRollen { get; set; } = new List<SelectListItem>();
    }
}
