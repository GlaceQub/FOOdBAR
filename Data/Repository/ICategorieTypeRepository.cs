public interface ICategorieTypeRepository
{
    public IEnumerable<CategorieType> GetAll();
    Task<IEnumerable<CategorieType>> GetAllWithCategoriesAndProductsAsync();
    Task<CategorieType?> GetByCategorieIdAsync(int categorieId);
}