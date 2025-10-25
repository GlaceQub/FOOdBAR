public interface ICategorieTypeRepository
{
    Task<IEnumerable<CategorieType>> GetAllWithCategoriesAndProductsAsync();
}