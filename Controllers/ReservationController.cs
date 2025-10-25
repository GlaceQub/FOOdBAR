using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Repository;
using Restaurant.Models;
using Restaurant.ViewModels.Reservation;
using Restaurant.ViewModels.Tafel;
using System.Security.Claims;

namespace Restaurant.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ReservatieRepository _reservatieRepository;
        private readonly RestaurantContext _context; // Voor tijdsloten

        public ReservationController(ReservatieRepository reservatieRepository, RestaurantContext context)
        {
            _reservatieRepository = reservatieRepository;
            _context = context;
        }

        // GET: /Reservation/Create
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ReservationViewModel
            {
                Datum = DateTime.Today,
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

            var beschikbareTafels = _reservatieRepository.GetBeschikbareTafels(
                model.Datum, model.TijdSlotId, model.AantalPersonen
            ).ToList();

            var vrijeTafel = beschikbareTafels.FirstOrDefault();
            if (vrijeTafel == null)
            {
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

            _reservatieRepository.Add(reservatie);
            _reservatieRepository.KoppelTafelAanReservatie(reservatie.Id, vrijeTafel.Id);

            return RedirectToAction("Confirmation", new { id = reservatie.Id });
        }

        // GET: /Reservation/Confirmation/{id}
        [HttpGet]
        public IActionResult Confirmation(int id)
        {
            var reservatie = _reservatieRepository.GetById(id);
            if (reservatie == null)
                return NotFound();

            return View(reservatie);
        }

        // Extra: Overzicht van reservaties (optioneel)
        [Authorize(Roles = "Eigenaar,Zaalverantwoordelijke")]
        [HttpGet]
        public IActionResult Index()
        {
            var reservaties = _reservatieRepository.GetAll();
            return View(reservaties);
        }

        // Extra: Verwijder een reservatie (optioneel)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _reservatieRepository.Delete(id);
            return RedirectToAction("Index");
        }

        // Extra: Bewerk een reservatie (optioneel)
        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var reservatie = _reservatieRepository.GetById(id);
            if (reservatie == null)
                return NotFound();

            var model = new ReservationViewModel
            {
                Datum = reservatie.Datum ?? DateTime.Today,
                TijdSlotId = reservatie.TijdSlotId,
                AantalPersonen = reservatie.AantalPersonen,
                Opmerking = reservatie.Opmerking,
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.LunchTijdsloten = _context.Tijdslots
                    .Where(t => t.Actief && t.Naam.ToLower().Contains("lunch"))
                    .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                    .ToList();
                model.DinerTijdsloten = _context.Tijdslots
                    .Where(t => t.Actief && t.Naam.ToLower().Contains("diner"))
                    .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                    .ToList();
                return View(model);
            }

            var reservatie = _reservatieRepository.GetById(id);
            if (reservatie == null)
                return NotFound();

            reservatie.Datum = model.Datum;
            reservatie.TijdSlotId = model.TijdSlotId;
            reservatie.AantalPersonen = model.AantalPersonen;
            reservatie.Opmerking = model.Opmerking;

            _reservatieRepository.Update(reservatie);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Zaalverantwoordelijke, Eigenaar")]
        [HttpGet]
        public IActionResult Toewijzen()
        {
            var reservaties = _reservatieRepository.GetReservatiesZonderTafel()
                .OrderBy(r => r.Tafellijsten.FirstOrDefault()?.Tafel.TafelNummer ?? ""); // Sorteer op tafelnummer indien mogelijk

            var viewModels = reservaties.Select(r => new TafelToewijzenViewModel
            {
                Reservatie = r,
                BeschikbareTafels = _reservatieRepository.GetBeschikbareTafels(
                    r.Datum ?? DateTime.Today, r.TijdSlotId, r.AantalPersonen)
            }).ToList();

            return View(viewModels);
        }

        [Authorize(Roles = "Zaalverantwoordelijke, Eigenaar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToewijsTafel(int reservatieId, int tafelId)
        {
            _reservatieRepository.KoppelTafelAanReservatie(reservatieId, tafelId);

            var tafel = _reservatieRepository.GetTafelById(tafelId);
            if (tafel != null)
            {
                _reservatieRepository.UpdateTafel(tafel);
            }

            TempData["Message"] = "Tafel succesvol toegewezen!";
            return RedirectToAction("Toewijzen");
        }
    }
}