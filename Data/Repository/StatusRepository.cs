public class StatusRepository : IStatusRepository
{
    private readonly RestaurantContext _context;
    public StatusRepository(RestaurantContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Status>> GetAllAsync()
    {
        return await _context.Statussen
            .OrderBy(s => s.Id)
            .ToListAsync();
    }

    public async Task<Status?> GetByIdAsync(int id)
    {
        return await _context.Statussen.FindAsync(id);
    }
}