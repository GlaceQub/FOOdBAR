public interface ICategorieRepository
{
    public IEnumerable<Categorie> GetAll();
    public Categorie? GetById(int id);

    Task<IEnumerable<Categorie>> GetAllAsync();

    Task<IEnumerable<Categorie>> GetAllWithProductsAndPricesAsync();
    void Update(Categorie categorie);
    public void Add(Categorie categorie);
    public void Delete(Categorie categorie);
}