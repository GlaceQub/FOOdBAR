namespace Restaurant.ViewModels.Reservation
{
    public class ReservationListViewModel
    {
        public DateTime GeselecteerdeDatum { get; set; }
        public IEnumerable<Reservatie> Reserveringen { get; set; } = new List<Reservatie>();
    }
}
