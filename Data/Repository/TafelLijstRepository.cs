namespace Restaurant.Data.Repository
{
    public class TafelLijstRepository : ITafelLijstRepository
    {
        private readonly RestaurantContext _context;

        public TafelLijstRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<bool> HasAssignedTableAsync(int reservatieId)
        {
            return await _context.TafelLijsten.AnyAsync(tl => tl.ReservatieId == reservatieId);
        }
    }
}
