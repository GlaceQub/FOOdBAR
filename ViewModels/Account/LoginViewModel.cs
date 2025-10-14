using System.ComponentModel.DataAnnotations;

namespace Restaurant.ViewModels.Account
{
    public class LoginViewModel
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
