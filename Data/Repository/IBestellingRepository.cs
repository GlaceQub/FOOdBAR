namespace Restaurant.Data.Repository
{
    public interface IBestellingRepository
    {
        Task<IEnumerable<Bestelling>> GetAllAsync();
        Task<Bestelling?> GetByIdAsync(int id);
        Task AddAsync(Bestelling bestelling);
        void Update(Bestelling bestelling);
        void Remove(Bestelling bestelling);

        Task<IEnumerable<Bestelling>> GetByKlantIdAsync(string klantId);
        Task<IEnumerable<Bestelling>> GetByReservatieIdAsync(int reservatieId);
    }
}
