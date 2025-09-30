namespace Restaurant.Models
{
    public class Settings
    {
        public string? Name { get; set; }
        public char[]? Secret { get; set; }
        public string? ValidIssuer { get; set; }
        public string? ValidAudience { get; set; }
    }
}
