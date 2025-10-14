namespace Restaurant.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        ILandRepository Landen { get; }
        IBestellingRepository Bestellingen { get; }
        Task<int> CompleteAsync();
    }
}
