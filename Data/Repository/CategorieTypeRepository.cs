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

    public async Task<CategorieType?> GetByCategorieIdAsync(int categorieId)
    {
        // Find the Categorie with the given Id
        var categorie = await _context.Categorien
            .FirstOrDefaultAsync(c => c.Id == categorieId);

        if (categorie == null)
            return null;

        // Find the CategorieType with the TypeId from the Categorie
        return await _context.Types
            .FirstOrDefaultAsync(t => t.Id == categorie.TypeId);
    }
}