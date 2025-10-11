using Restaurant.ViewModels.Reservation;
using System.Text.RegularExpressions;

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
            var tijdsloten = _context.Tijdslots
                .Where(t => t.Actief)
                .Select(t => new TijdslotDto
                {
                    Id = t.Id,
                    Naam = Regex.Match(t.Naam, @"\d{1,2}u\d{0,2}\s*tot\s*\d{1,2}u\d{0,2}").Value
                })
                .ToList();

            var model = new ReservationViewModel
            {
                LunchTijdsloten = tijdsloten.Where(t => t.Naam != null && t.Naam != "" && t.Naam.Contains("11u") || t.Naam.Contains("12u")).ToList(),
                DinerTijdsloten = tijdsloten.Where(t => t.Naam != null && t.Naam != "" && t.Naam.Contains("17u") || t.Naam.Contains("19u")).ToList()
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
