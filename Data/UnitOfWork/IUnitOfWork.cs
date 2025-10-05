namespace Restaurant.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        ILandRepository Landen { get; }
        Task<int> CompleteAsync();
    }
}
