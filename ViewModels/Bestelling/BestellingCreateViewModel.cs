using System.ComponentModel.DataAnnotations;

public class BestellingCreateViewModel
{
    public bool HasAssignedTable { get; set; }

    [Required]
    public int ReservatieId { get; set; }

    public IEnumerable<CategorieViewModel> MenuOverzicht { get; set; } = new List<CategorieViewModel>();
    public Dictionary<int, int> SelectedItems { get; set; } = new();
    public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
    public decimal TotaalBedrag { get; set; }
    public bool NetworkError { get; set; }
}
