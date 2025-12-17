using Restaurant.ViewModels.Rekening;

namespace Restaurant.Data.Repository
{
    public interface IReservatieRepository
    {
        Task<RekeningInfoReservatieViewModel> GetReservatieWithKlantByIdAsync(int reservatieId);

        Task<bool> BehandelBetaling(int reservatieId);

        Task<Reservatie> GetReservatieByIdAsync(int reservatieId);

        IEnumerable<Reservatie> GetAll();

        IEnumerable<Reservatie> GetByKlantId(string klantId);

        Reservatie? GetById(int id);
        void Add(Reservatie reservatie);
        void Update(Reservatie reservatie);
        void Delete(int id);
        IEnumerable<Reservatie> GetReservatiesZonderTafel();
        IEnumerable<Tafel> GetBeschikbareTafels(DateTime datum, int tijdslotId, int aantalPersonen);
        Tafel? GetTafelById(int tafelId);
        void UpdateTafel(Tafel tafel);
    }
}
