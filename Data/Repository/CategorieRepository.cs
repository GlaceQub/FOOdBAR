public class CategorieRepository : ICategorieRepository
{
    private readonly RestaurantContext _context;
    public CategorieRepository(RestaurantContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Categorie>> GetAllAsync()
    {
        return await _context.Categorien.Include(c => c.Producten).ToListAsync();
    }

    public async Task<IEnumerable<Categorie>> GetAllWithProductsAndPricesAsync()
    {
        return await _context.Categorien
            .Include(c => c.Producten)
                .ThenInclude(p => p.PrijsProducten)
            .ToListAsync();
    }
}