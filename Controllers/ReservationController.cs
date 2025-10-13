using Restaurant.ViewModels.Reservation;
using System.Security.Claims;

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
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ReservationViewModel
            {
                LunchTijdsloten = _context.Tijdslots
                    .Where(t => t.Actief && t.Naam.ToLower().Contains("lunch"))
                    .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                    .ToList(),
                DinerTijdsloten = _context.Tijdslots
                    .Where(t => t.Actief && t.Naam.ToLower().Contains("diner"))
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
            model.LunchTijdsloten = _context.Tijdslots
                .Where(t => t.Actief && t.Naam.ToLower().Contains("lunch"))
                .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                .ToList();
            model.DinerTijdsloten = _context.Tijdslots
                .Where(t => t.Actief && t.Naam.ToLower().Contains("diner"))
                .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                .ToList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var klantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(klantId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Zoek alle tafels die actief zijn en geschikt zijn voor het aantal personen
            var beschikbareTafels = _context.Tafels
                .Where(t => t.Actief && t.MinAantalPersonen <= model.AantalPersonen && t.AantalPersonen >= model.AantalPersonen)
                .ToList();

            // Zoek alle reeds gereserveerde tafels voor deze datum en tijdslot
            var reedsGereserveerdeTafelIds = _context.TafelLijsten
                .Where(tl => tl.Reservatie.Datum == model.Datum && tl.Reservatie.TijdSlotId == model.TijdSlotId)
                .Select(tl => tl.TafelId)
                .ToList();

            // Zoek een vrije tafel
            var vrijeTafel = beschikbareTafels.FirstOrDefault(t => !reedsGereserveerdeTafelIds.Contains(t.Id));

            if (vrijeTafel == null)
            {
                // Geen tafel beschikbaar: toon foutmelding of alternatieven
                ModelState.AddModelError("", "Er is geen tafel beschikbaar voor het gekozen tijdslot en aantal personen. Kies een ander tijdslot.");
                return View(model);
            }

            var reservatie = new Reservatie
            {
                Datum = model.Datum,
                TijdSlotId = model.TijdSlotId,
                AantalPersonen = model.AantalPersonen,
                Opmerking = model.Opmerking,
                KlantId = klantId
            };

            _context.Reservaties.Add(reservatie);
            _context.SaveChanges();

            // Koppel de tafel aan de reservatie
            var tafelLijst = new TafelLijst
            {
                ReservatieId = reservatie.Id,
                TafelId = vrijeTafel.Id
            };
            _context.TafelLijsten.Add(tafelLijst);
            _context.SaveChanges();

            return RedirectToAction("Confirmation", new { id = reservatie.Id });
        }

        // GET: /Reservation/Confirmation/{id}
        [HttpGet]
        public IActionResult Confirmation(int id)
        {
            var reservatie = _context.Reservaties
                .Include(r => r.Tijdslot)
                .FirstOrDefault(r => r.Id == id);

            if (reservatie == null)
                return NotFound();

            return View(reservatie);
        }
    }
}