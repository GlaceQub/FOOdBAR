
namespace Restaurant.Data.Repository
{
    public class ReservatieRepository : IReservatieRepository
    {
        private readonly RestaurantContext _context;

        public ReservatieRepository(RestaurantContext context)
        {
            _context = context;
        }

        // Haal alle reservaties op (inclusief tafels en tijdslot)
        public IEnumerable<Reservatie> GetAll()
        {
            return _context.Reservaties
                .Include(r => r.Tafellijsten)
                    .ThenInclude(tl => tl.Tafel)
                .Include(r => r.Tijdslot)
                .ToList();
        }

        // Haal een reservatie op via id
        public Reservatie? GetById(int id)
        {
            return _context.Reservaties
                .Include(r => r.Tafellijsten)
                    .ThenInclude(tl => tl.Tafel)
                .Include(r => r.Tijdslot)
                .FirstOrDefault(r => r.Id == id);
        }

        // Voeg een reservatie toe
        public void Add(Reservatie reservatie)
        {
            _context.Reservaties.Add(reservatie);
            _context.SaveChanges();
        }

        // Update een reservatie
        public void Update(Reservatie reservatie)
        {
            _context.Reservaties.Update(reservatie);
            _context.SaveChanges();
        }

        // Verwijder een reservatie
        public void Delete(int id)
        {
            var reservatie = _context.Reservaties.Find(id);
            if (reservatie != null)
            {
                _context.Reservaties.Remove(reservatie);
                _context.SaveChanges();
            }
        }

        // Haal reservaties zonder toegewezen tafels op
        public IEnumerable<Reservatie> GetReservatiesZonderTafel()
        {
            return _context.Reservaties
                .Include(r => r.CustomUser)
                .Include(r => r.Tijdslot)
                .Include(r => r.Tafellijsten)
                .Where(r => !r.Tafellijsten.Any())
                .ToList();
        }

        // Haal beschikbare tafels op voor een datum en tijdslot
        public IEnumerable<Tafel> GetBeschikbareTafels(DateTime datum, int tijdslotId, int aantalPersonen)
        {
            // Tafels die al gekoppeld zijn aan een reservatie op deze datum en tijdslot
            var bezetteTafelIds = _context.TafelLijsten
                .Include(tl => tl.Reservatie)
                .Where(tl => tl.Reservatie.Datum == datum && tl.Reservatie.TijdSlotId == tijdslotId)
                .Select(tl => tl.TafelId)
                .ToList();

            // Enkel actieve tafels die geschikt zijn voor het aantal personen en niet bezet zijn
            return _context.Tafels
                .Where(t => t.Actief
                    && t.MinAantalPersonen <= aantalPersonen
                    && t.AantalPersonen >= aantalPersonen
                    && !bezetteTafelIds.Contains(t.Id))
                .ToList();
        }

        // Haal een tafel op via id
        public Tafel? GetTafelById(int tafelId)
        {
            return _context.Tafels.FirstOrDefault(t => t.Id == tafelId);
        }

        // Update een tafel
        public void UpdateTafel(Tafel tafel)
        {
            _context.Tafels.Update(tafel);
            _context.SaveChanges();
        }
    }
}