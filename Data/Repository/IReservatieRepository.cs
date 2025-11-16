using Restaurant.ViewModels.Rekening;

namespace Restaurant.Data.Repository
{
    public interface IReservatieRepository
    {
        Task<RekeningInfoReservatieViewModel> GetReservatieWithKlantByIdAsync(int reservatieId);

        Task<bool> BehandelBetaling(int reservatieId);

        Task<Reservatie> GetReservatieByIdAsync(int reservatieId);
    }
}