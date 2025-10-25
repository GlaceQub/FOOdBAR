public class ProductRepository : IProductRepository
{
    private readonly RestaurantContext _context;
    public ProductRepository(RestaurantContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Producten.ToListAsync();
    }

    public async Task<Product?> GetByIdWithPriceAsync(int id)
    {
        return await _context.Producten
            .Include(p => p.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).Take(1))
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}