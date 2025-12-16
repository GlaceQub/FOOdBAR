namespace Restaurant.ViewModels.Account
{
    public class GebruikerViewModel
    {
        public CustomUser User { get; set; } = default!;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
