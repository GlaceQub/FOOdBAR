public interface ICategorieRepository
{
    Task<IEnumerable<Categorie>> GetAllAsync();

    Task<IEnumerable<Categorie>> GetAllWithProductsAndPricesAsync();
}