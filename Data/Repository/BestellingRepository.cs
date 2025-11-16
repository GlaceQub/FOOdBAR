using Microsoft.EntityFrameworkCore;
using Restaurant.Models;
using Restaurant.ViewModels.Rekening;

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

        public async Task<IEnumerable<BestellingInfoRekeningViewModel>> GetBestellingInfoRekeningByReservatieIdAsync(int reservatieId)
        {
            var geserveerdStatusId = await _context.Statussen
                .Where(s => s.Naam == "Geserveerd")
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            var reservatie = await _context.Reservaties
                .FirstOrDefaultAsync(r => r.Id == reservatieId);

            if (reservatie == null || reservatie.Bestaald)
            {
                return Enumerable.Empty<BestellingInfoRekeningViewModel>();
            }

            return await _context.Bestellingen
                .Include(b => b.Product)
                .Include(b => b.Reservatie)
                .Where(b => b.ReservatieId == reservatieId && b.StatusId == geserveerdStatusId)
                .Select(b => new BestellingInfoRekeningViewModel
                {
                    ProductNaam = b.Product.Naam,
                    PrijsPerEenheid = b.Product.PrijsProducten
                        .OrderByDescending(pp => pp.DatumVanaf)
                        .FirstOrDefault().Prijs,
                    Aantal = b.Aantal,
                    TijdstipBestelling = b.TijdstipBestelling
                })
                .OrderBy(b => b.TijdstipBestelling)
                .ToListAsync();
        }

        public async Task<decimal> GetTotaalBedragByReservatieIdAsync(int reservatieId)
        {
            var geleverdStatusId = await _context.Statussen
                .Where(s => s.Naam == "Geserveerd")
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            // Query bestellingen with status "Geleverd" for the reservatie, including Product and PrijsProducten
            var bestellingen = await _context.Bestellingen
                .Include(b => b.Product)
                    .ThenInclude(p => p.PrijsProducten)
                .Where(b => b.ReservatieId == reservatieId && b.StatusId == geleverdStatusId)
                .ToListAsync();

            // Calculate total
            decimal totaal = 0;
            foreach (var bestelling in bestellingen)
            {
                // Get the latest price for the product
                var prijs = bestelling.Product?.PrijsProducten
                    .OrderByDescending(pp => pp.DatumVanaf)
                    .FirstOrDefault()?.Prijs ?? 0;

                totaal += bestelling.Aantal * prijs;
            }

            return totaal;
        }
    }
}
