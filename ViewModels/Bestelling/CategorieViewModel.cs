public class CategorieViewModel
{
    public string Naam { get; set; } = string.Empty;
    public List<ProductViewModel> Producten { get; set; } = new List<ProductViewModel>();
}