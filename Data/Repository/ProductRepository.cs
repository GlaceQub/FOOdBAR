public class ProductRepository : IProductRepository
{
    private readonly RestaurantContext _context;
    public ProductRepository(RestaurantContext context)
    {
        _context = context;
    }

    public void Update(Product product)
    {
        _context.Producten.Update(product);
        _context.SaveChanges();
    }

    public async Task Add(Product product)
    {
        await _context.Producten.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Producten.ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithCategorieAsync()
    {
        return await _context.Producten
            .Include(p => p.Categorie)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithCategorieAndPricesAsync()
    {
        return await _context.Producten
            .Include(p => p.Categorie)
            .Include(p => p.PrijsProducten)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdWithPriceAsync(int id)
    {
        return await _context.Producten
            .Include(p => p.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).Take(1))
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}