using Restaurant.ViewModels.Rekening;

namespace Restaurant.Data.Repository
{
    public class ReservatieRepository : IReservatieRepository
    {
        private readonly RestaurantContext _context;
        public ReservatieRepository(RestaurantContext context)
        {
            _context = context;
        }

        public async Task<RekeningInfoReservatieViewModel?> GetReservatieWithKlantByIdAsync(int reservatieId)
        {
            var reservatie = await _context.Reservaties
                .Include(r => r.CustomUser)
                .Include(r => r.Tafellijsten)
                    .ThenInclude(tl => tl.Tafel)
                .FirstOrDefaultAsync(r => r.Id == reservatieId);

            if (reservatie == null)
                return null;

            var klantNaam = reservatie.CustomUser?.Achternaam ?? "";
            var klantVoornaam = reservatie.CustomUser?.Voornaam ?? "";
            var tafelNummer = reservatie.Tafellijsten.FirstOrDefault()?.Tafel?.TafelNummer ?? "";

            return new RekeningInfoReservatieViewModel {
                ReservatieId = reservatieId,
                KlantNaam = klantNaam,
                KlantVoornaam = klantVoornaam,
                TafelNummer = tafelNummer
            };
        }

        public async Task<bool> BehandelBetaling(int reservatieId)
        {
            var reservatie = await _context.Reservaties.FindAsync(reservatieId);
            if (reservatie == null)
                return false;
            reservatie.Bestaald = true;
            _context.Reservaties.Update(reservatie);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
