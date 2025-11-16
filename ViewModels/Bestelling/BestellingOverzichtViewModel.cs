
public class BestellingOverzichtViewModel
{
    public IEnumerable<Bestelling> Bestellingen { get; set; } = new List<Bestelling>();
    public IEnumerable<Status> StatusList { get; set; } = new List<Status>();
    public IDictionary<int, string> StatusColors { get; set; } = new Dictionary<int, string>();
}
