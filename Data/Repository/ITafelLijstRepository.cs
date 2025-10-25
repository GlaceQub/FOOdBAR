namespace Restaurant.Data.Repository
{
    public interface ITafelLijstRepository
    {
        Task<bool> HasAssignedTableAsync(int reservatieId);
    }
}
