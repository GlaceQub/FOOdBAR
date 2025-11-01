
namespace Restaurant.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantContext _context;
        public ILandRepository Landen { get; }
        public IReservatieRepository Reservaties { get; }
        public RestaurantContext RestaurantContext => _context;

        public UnitOfWork(RestaurantContext context)
        {
            _context = context;
            Landen = new LandRepository(_context);
            Reservaties = new ReservatieRepository(_context);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
