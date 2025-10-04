namespace Restaurant.Dto.Account
{
    public class AccountLoginDto
    {
        [Required(ErrorMessage = "E-mail is verplicht!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is verplicht!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
