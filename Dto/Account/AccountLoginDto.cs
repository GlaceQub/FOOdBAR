namespace Restaurant.Dto.Account
{
    public class AccountLoginDto
    {
        [Required(ErrorMessage = "E-mailadres is verplicht!")]
        [EmailAddress(ErrorMessage = "Ongeldig e-mailadres!")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mailadres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht!")]
        [Display(Name = "Wachtwoord")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
