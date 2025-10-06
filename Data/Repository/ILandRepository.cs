public interface ILandRepository
{
    Task<IEnumerable<Land>> GetActieveLandenAsync();
}