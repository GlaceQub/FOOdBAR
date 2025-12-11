namespace Restaurant.Data.Repository
{
    public class PrijsProductRepository : IPrijsProductRepository
    {
        private readonly RestaurantContext _context;
        public PrijsProductRepository(RestaurantContext context)
        {
            _context = context;
        }
        public void Add(PrijsProduct prijsProduct)
        {
            _context.PrijsProducten.Add(prijsProduct);
            _context.SaveChanges();
        }
    }
}
