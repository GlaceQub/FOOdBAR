namespace Restaurant.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantContext _context;
        public ILandRepository Landen { get; }
        public IBestellingRepository Bestellingen { get; }

        public UnitOfWork(RestaurantContext context)
        {
            _context = context;
            Landen = new LandRepository(_context);
            Bestellingen = new BestellingRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
