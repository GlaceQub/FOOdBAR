public interface IProductRepository
{
    Task Update(Product product);
    Task Add(Product product);
    Task Delete(Product product);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetAllWithCategorieAsync();
    Task<IEnumerable<Product>> GetAllWithCategorieAndPricesAsync();
    Task<Product?> GetByIdWithPriceAsync(int id);
}