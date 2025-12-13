namespace Restaurant.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync();

        // Account related
        ILandRepository Landen { get; }

        // Bestelling related
        IBestellingRepository Bestellingen { get; }

        // TafelLijst related
        ITafelLijstRepository TafelLijsten { get; }

        // Categorie related
        ICategorieRepository Categorieen { get; }
        ICategorieTypeRepository CategorieTypen { get; }

        // Product related
        IProductRepository Producten { get; }

        // PrijsProduct related
        IPrijsProductRepository PrijsProducten { get; }

        // Status related
        IStatusRepository Statussen { get; }

        // Reservatie related
        IReservatieRepository Reservaties { get; }
        
        RestaurantContext RestaurantContext { get; }

        int Save();
    }
}
