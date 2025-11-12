public interface IStatusRepository
{
    Task<IEnumerable<Status>> GetAllAsync();
}
