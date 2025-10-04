namespace Restaurant.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail is verplicht!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is verplicht!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
