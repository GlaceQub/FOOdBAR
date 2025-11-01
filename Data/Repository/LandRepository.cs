public class LandRepository : ILandRepository
{
    private readonly RestaurantContext _context;
    public LandRepository(RestaurantContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Land>> GetActieveLandenAsync()
    {
        return await _context.Landen.Where(l => l.Actief).ToListAsync();
    }
}