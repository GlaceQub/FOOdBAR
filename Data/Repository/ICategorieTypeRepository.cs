public interface ICategorieTypeRepository
{
    Task<IEnumerable<CategorieType>> GetAllWithCategoriesAndProductsAsync();

    Task<CategorieType?> GetByCategorieIdAsync(int categorieId);
}