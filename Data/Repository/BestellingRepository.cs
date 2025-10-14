using Microsoft.EntityFrameworkCore;
using Restaurant.Models;

namespace Restaurant.Data.Repository
{
    public class BestellingRepository : IBestellingRepository
    {
        private readonly RestaurantContext _context;

        public BestellingRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bestelling>> GetAllAsync()
        {
            return await _context.Bestellingen
                .Include(b => b.Product)
                .Include(b => b.Status)
                .Include(b => b.Reservatie)
                .ToListAsync();
        }

        public async Task<Bestelling?> GetByIdAsync(int id)
        {
            return await _context.Bestellingen
                .Include(b => b.Product)
                .Include(b => b.Status)
                .Include(b => b.Reservatie)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Bestelling bestelling)
        {
            await _context.Bestellingen.AddAsync(bestelling);
        }

        public void Update(Bestelling bestelling)
        {
            _context.Bestellingen.Update(bestelling);
        }

        public void Remove(Bestelling bestelling)
        {
            _context.Bestellingen.Remove(bestelling);
        }

        public async Task<IEnumerable<Bestelling>> GetByKlantIdAsync(string klantId)
        {
            return await _context.Bestellingen
                .Include(b => b.Product)
                .Include(b => b.Status)
                .Include(b => b.Reservatie)
                .Where(b => b.Reservatie.KlantId == klantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bestelling>> GetByReservatieIdAsync(int reservatieId)
        {
            return await _context.Bestellingen
                .Include(b => b.Product)
                .Include(b => b.Status)
                .Include(b => b.Reservatie)
                .Where(b => b.ReservatieId == reservatieId)
                .ToListAsync();
        }
    }
}
