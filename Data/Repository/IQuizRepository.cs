namespace Restaurant.Data.Repository
{
    public interface IQuizRepository 
    {
        Task<List<Quiz>> GetQuizEigenschappenVoorGerechtenAsync();
        Task<List<Quiz>> GetQuizEigenschappenVoorDrankenAsync();
    }
}
