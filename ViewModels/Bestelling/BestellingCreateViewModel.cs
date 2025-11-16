public class BestellingCreateViewModel
{
    public bool HasAssignedTable { get; set; }

    [Required]
    public int ReservatieId { get; set; }

    public IEnumerable<CategorieViewModel> MenuOverzicht { get; set; } = new List<CategorieViewModel>();
    public Dictionary<int, int> SelectedItems { get; set; } = new();
    public List<CartItemWithProductViewModel> CartItemsWithProduct { get; set; } = new();
    public decimal TotaalBedrag { get; set; }
    public bool NetworkError { get; set; }
    public List<CategorieTypeViewModel> MenuTypes { get; set; } = new();
}
