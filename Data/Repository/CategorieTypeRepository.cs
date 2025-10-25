public class CategorieTypeRepository : ICategorieTypeRepository
{
    private readonly RestaurantContext _context;
    public CategorieTypeRepository(RestaurantContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategorieType>> GetAllWithCategoriesAndProductsAsync()
    {
        return await _context.Types
            .Include(t => t.Categorieen)
                .ThenInclude(c => c.Producten)
                    .ThenInclude(p => p.PrijsProducten)
            .ToListAsync();
    }
}