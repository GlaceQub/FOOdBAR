namespace Restaurant.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantContext _context;
        public ILandRepository Landen { get; }
        public IBestellingRepository Bestellingen { get; }
        public ITafelLijstRepository TafelLijsten { get; }
        public ICategorieRepository Categorieen { get; }
        public IProductRepository Producten { get; }
        public IPrijsProductRepository PrijsProducten { get; }
        public ICategorieTypeRepository CategorieTypen { get; }
        public IStatusRepository Statussen { get; }
        public IReservatieRepository Reservaties { get; }
        public RestaurantContext RestaurantContext => _context;

        public UnitOfWork(RestaurantContext context)
        {
            _context = context;
            Landen = new LandRepository(_context);
            Bestellingen = new BestellingRepository(_context);
            TafelLijsten = new TafelLijstRepository(_context);
            Categorieen = new CategorieRepository(_context);
            Producten = new ProductRepository(_context);
            PrijsProducten = new PrijsProductRepository(_context);
            CategorieTypen = new CategorieTypeRepository(_context);
            Statussen = new StatusRepository(_context);
            Reservaties = new ReservatieRepository(_context);
            Reservaties = new ReservatieRepository(_context);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
    }
}
