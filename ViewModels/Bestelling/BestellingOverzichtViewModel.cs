
public class BestellingOverzichtViewModel
{
    [Required]
    public IEnumerable<Bestelling> Bestellingen { get; set; } = new List<Bestelling>();
    [Required]
    public IEnumerable<Status> StatusList { get; set; } = new List<Status>();
    [Required]
    public IDictionary<int, string> StatusColors { get; set; } = new Dictionary<int, string>();
}
