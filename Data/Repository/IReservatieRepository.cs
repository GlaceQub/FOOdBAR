namespace Restaurant.Data.Repository
{
    public interface IReservatieRepository
    {
        IEnumerable<Reservatie> GetAll();
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
