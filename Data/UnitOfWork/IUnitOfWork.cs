namespace Restaurant.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        ILandRepository Landen { get; }
        IReservatieRepository Reservaties { get; }
        RestaurantContext RestaurantContext { get; }
        Task<int> CompleteAsync();
        int Save();
    }
}
