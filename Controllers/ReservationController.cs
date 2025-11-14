using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data.Repository;
using Restaurant.Data.UnitOfWork;
using Restaurant.Models;
using Restaurant.ViewModels.Reservation;
using Restaurant.ViewModels.Tafel;
using System.Security.Claims;

namespace Restaurant.Controllers
{
    [Authorize(Roles = "Eigenaar")]
    public class ReservationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /Reservation/Create
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ReservationViewModel
            {
                Datum = DateTime.Today,
                LunchTijdsloten = _unitOfWork.RestaurantContext.Tijdslots
                    .Where(t => t.Actief && t.Naam.ToLower().Contains("lunch"))
                    .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                    .ToList(),
                DinerTijdsloten = _unitOfWork.RestaurantContext.Tijdslots
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
            model.LunchTijdsloten = _unitOfWork.RestaurantContext.Tijdslots
                .Where(t => t.Actief && t.Naam.ToLower().Contains("lunch"))
                .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                .ToList();
            model.DinerTijdsloten = _unitOfWork.RestaurantContext.Tijdslots
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

            var beschikbareTafels = _unitOfWork.Reservaties.GetBeschikbareTafels(
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

            _unitOfWork.Reservaties.Add(reservatie);
            _unitOfWork.Save();

            return RedirectToAction("Confirmation", new { id = reservatie.Id });
        }

        // GET: /Reservation/Confirmation/{id}
        [HttpGet]
        public IActionResult Confirmation(int id)
        {
            var reservatie = _unitOfWork.Reservaties.GetById(id);
            if (reservatie == null)
                return NotFound();

            return View(reservatie);
        }

        [Authorize(Roles = "Eigenaar,Zaalverantwoordelijke")]
        [HttpGet]
        public IActionResult Index(DateTime? datum)
        {
            var reservaties = _unitOfWork.Reservaties.GetAll();

            DateTime filterDatum = datum ?? DateTime.Today;
            reservaties = reservaties.Where(r => r.Datum.HasValue && r.Datum.Value.Date == filterDatum.Date);
            ViewBag.GeselecteerdeDatum = filterDatum;

            return View(reservaties);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _unitOfWork.Reservaties.Delete(id);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var reservatie = _unitOfWork.Reservaties.GetById(id);
            if (reservatie == null)
                return NotFound();

            var model = new ReservationViewModel
            {
                Id = reservatie.Id,
                Datum = reservatie.Datum ?? DateTime.Today,
                TijdSlotId = reservatie.TijdSlotId,
                AantalPersonen = reservatie.AantalPersonen,
                Opmerking = reservatie.Opmerking,
                LunchTijdsloten = _unitOfWork.RestaurantContext.Tijdslots
                    .Where(t => t.Actief && t.Naam.ToLower().Contains("lunch"))
                    .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                    .ToList(),
                DinerTijdsloten = _unitOfWork.RestaurantContext.Tijdslots
                    .Where(t => t.Actief && t.Naam.ToLower().Contains("diner"))
                    .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                    .ToList()
            };

            ViewBag.TijdslotId = new SelectList(_unitOfWork.RestaurantContext.Tijdslots.Where(t => t.Actief), "Id", "Naam", model.TijdSlotId);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.LunchTijdsloten = _unitOfWork.RestaurantContext.Tijdslots
                    .Where(t => t.Actief && t.Naam.ToLower().Contains("lunch"))
                    .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                    .ToList();
                model.DinerTijdsloten = _unitOfWork.RestaurantContext.Tijdslots
                    .Where(t => t.Actief && t.Naam.ToLower().Contains("diner"))
                    .Select(t => new TijdslotDto { Id = t.Id, Naam = t.Naam })
                    .ToList();

                ViewBag.TijdslotId = new SelectList(_unitOfWork.RestaurantContext.Tijdslots.Where(t => t.Actief), "Id", "Naam", model.TijdSlotId);

                return View(model);
            }

            var reservatie = _unitOfWork.Reservaties.GetById(id);
            if (reservatie == null)
                return NotFound();

            reservatie.Datum = model.Datum;
            reservatie.TijdSlotId = model.TijdSlotId;
            reservatie.AantalPersonen = model.AantalPersonen;
            reservatie.Opmerking = model.Opmerking;

            _unitOfWork.Reservaties.Update(reservatie);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Zaalverantwoordelijke, Eigenaar")]
        [HttpGet]
        public IActionResult Toewijzen()
        {
            var reservaties = _unitOfWork.Reservaties.GetReservatiesZonderTafel()
                .OrderBy(r => r.Tafellijsten.FirstOrDefault()?.Tafel.TafelNummer ?? "");

            var viewModels = reservaties.Select(r => new TafelToewijzenViewModel
            {
                Reservatie = r,
                BeschikbareTafels = _unitOfWork.Reservaties.GetBeschikbareTafels(
                    r.Datum ?? DateTime.Today, r.TijdSlotId, r.AantalPersonen)
            }).ToList();

            return View(viewModels);
        }

        [Authorize(Roles = "Zaalverantwoordelijke, Eigenaar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToewijsTafel(int reservatieId, int tafelId)
        {
            _unitOfWork.Reservaties.KoppelTafelAanReservatie(reservatieId, tafelId);

            var tafel = _unitOfWork.Reservaties.GetTafelById(tafelId);
            if (tafel != null)
            {
                _unitOfWork.Reservaties.UpdateTafel(tafel);
            }

            _unitOfWork.Save();
            TempData["Message"] = "Tafel succesvol toegewezen!";
            return RedirectToAction("Toewijzen");
        }
    }
}