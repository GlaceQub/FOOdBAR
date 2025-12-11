public interface IProductRepository
{
    public void Update(Product product);
    Task Add(Product product);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetAllWithCategorieAsync();
    Task<IEnumerable<Product>> GetAllWithCategorieAndPricesAsync();
    Task<Product?> GetByIdWithPriceAsync(int id);
}