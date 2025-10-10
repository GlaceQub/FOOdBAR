using Restaurant.ViewModels.Reservation;

namespace Restaurant.Controllers
{
    public class ReservationController : Controller
    {
        private readonly RestaurantContext _context;

        public ReservationController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: /Reservation/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ReservationViewModel
            {
                BeschikbareTijdsloten = _context.Tijdslots
                    .Where(t => t.Actief)
                    .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                    .ToList()
            };
            return View(model);
        }

        // POST: /Reservation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReservationViewModel model)
        {
            // Herlaad tijdsloten bij fout
            model.BeschikbareTijdsloten = _context.Tijdslots
                .Where(t => t.Actief)
                .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                .ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Maak een nieuwe Reservatie aan
            var reservatie = new Reservatie
            {
                Datum = model.Datum,
                TijdSlotId = model.TijdSlotId,
                AantalPersonen = model.AantalPersonen,
                Opmerking = model.Opmerking,
                // KlantId moet hier gezet worden als de gebruiker is ingelogd
                // Bijvoorbeeld: KlantId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _context.Reservaties.Add(reservatie);
            _context.SaveChanges();

            return RedirectToAction("Confirmation");
        }

        // GET: /Reservation/Confirmation
        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
