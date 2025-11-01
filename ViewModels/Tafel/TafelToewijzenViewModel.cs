using Restaurant.Models;

namespace Restaurant.ViewModels.Tafel
{
    public class TafelToewijzenViewModel
    {
        public Reservatie Reservatie { get; set; }
        public IEnumerable<Models.Tafel> BeschikbareTafels { get; set; }
        public int? GeselecteerdeTafelId { get; set; }
    }
}
