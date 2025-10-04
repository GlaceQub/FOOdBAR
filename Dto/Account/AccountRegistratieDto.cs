namespace Restaurant.Dto.Account
{
    public class AccountRegistratieDto
    {
        [StringLength(50)]
        public string Voornaam { get; set; } = "";

        [StringLength(50)]
        public string Naam { get; set; } = "";

        [EmailAddress(ErrorMessage = "Ongeldig emailadres")]
        [Required(ErrorMessage = "Email is verplicht!")]
        public string Email { get; set; } = "";

        [StringLength(100)]
        public string Adres { get; set; } = "";

        [StringLength(10)]
        public string Huisnummer { get; set; } = "";

        [StringLength(10)]
        public string Postcode { get; set; } = "";

        [StringLength(50)]
        public string Gemeente { get; set; } = "";

        [Required(ErrorMessage = "Land is verplicht!")]
        public int LandId { get; set; }

        [Required(ErrorMessage = "Password is verplicht!")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Tweede wachtwoord is verplicht in te vullen.")]
        [Compare("Password", ErrorMessage = "De wachtwoorden komen niet overeen.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
