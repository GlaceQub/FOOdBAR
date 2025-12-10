public class CategorieRepository : ICategorieRepository
{
    private readonly RestaurantContext _context;
    public CategorieRepository(RestaurantContext context)
    {
        _context = context;
    }

    public IEnumerable<Categorie> GetAll()
    {
        return _context.Categorien
            .Include(c => c.Type)
            .ToList();
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

    public Categorie? GetById(int id)
    {
        return _context.Categorien
            .Include(c => c.Type)
            .FirstOrDefault(c => c.Id == id);
    }

    public void Update(Categorie categorie)
    {
        _context.Categorien.Update(categorie);
        _context.SaveChanges();
    }

    public void Add(Categorie categorie)
    {
        _context.Categorien.Add(categorie);
        _context.SaveChanges();
    }
    public void Delete(Categorie categorie)
    {
        _context.Categorien.Remove(categorie);
        _context.SaveChanges();
    }
}