namespace Restaurant.Data.Repository
{
    public class PrijsProductRepository : IPrijsProductRepository
    {
        private readonly RestaurantContext _context;
        public PrijsProductRepository(RestaurantContext context)
        {
            _context = context;
        }
        public async Task Add(PrijsProduct prijsProduct)
        {
            await _context.PrijsProducten.AddAsync(prijsProduct);
            await _context.SaveChangesAsync();
        }
    }
}
