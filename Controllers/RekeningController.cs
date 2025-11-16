using Restaurant.ViewModels.Rekening;

namespace Restaurant.Controllers
{
    [Authorize]
    public class RekeningController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public RekeningController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Afrekenen rekening van klant
        [Authorize(Roles = "Zaalverantwoordelijke, Eigenaar")]
        [HttpGet("Rekening/Afrekenen/{reservatieId}")]
        public async Task<IActionResult> Afrekenen(int reservatieId)
        {
            // Populate the viewmodel with all needed data
            var bestellingenInfo = await _unitOfWork.Bestellingen.GetBestellingInfoRekeningByReservatieIdAsync(reservatieId);
            if (bestellingenInfo == null || !bestellingenInfo.Any())
            {
                return RedirectToAction("GeenGeserveerdeBestellingen", "Rekening", new { reservatieId });
            }

            var totaalPrijs = await _unitOfWork.Bestellingen.GetTotaalBedragByReservatieIdAsync(reservatieId);
            var reservatieInfo = await _unitOfWork.Reservaties.GetReservatieWithKlantByIdAsync(reservatieId);
            if (reservatieInfo == null)
            {
                return NotFound("Geen reservatie gevonden");
            }

            var model = new AfrekenenViewModel
            {
                ReservatieId = reservatieId,
                BestellingenInfoRekening = bestellingenInfo,
                TotaalPrijs = totaalPrijs,
                TafelNummer = reservatieInfo.TafelNummer,
                KlantNaam = reservatieInfo.KlantNaam,
                KlantVoornaam = reservatieInfo.KlantVoornaam
            };
            return View(model);
        }

        [Authorize(Roles = "Zaalverantwoordelijke, Eigenaar")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Afrekenen(AfrekenenViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var reservatieId = model.ReservatieId;
            var betaalMethode = model.BetaalMethode;

            // Check if reservatie is already betaald BEFORE processing payment
            var reservatie = await _unitOfWork.Reservaties.GetReservatieByIdAsync(reservatieId);
            if (reservatie == null || reservatie.Bestaald)
            {
                // Already paid or not found, redirect to Mislukt
                return RedirectToAction("Mislukt", new { reservatieId, betaalMethode, errorType = "reedsBetaald" });
            }

            if (await BehandelBetaling(reservatieId, betaalMethode))
            {
                return RedirectToAction("Bevestiging", new { reservatieId });
                // TODO send email with receipt and enquete link
            }
            return RedirectToAction("Mislukt", new { reservatieId, betaalMethode });
        }
        #endregion

        #region Geen geserveerde bestellingen gevonden scherm
        [Authorize(Roles = "Zaalverantwoordelijke, Eigenaar")]
        [HttpGet("Rekening/Afrekenen/GeenGeserveerdeBestellingen/{reservatieId}")]
        public IActionResult GeenGeserveerdeBestellingen(int reservatieId)
        {
            ViewBag.ReservatieId = reservatieId;
            return View();
        }
        #endregion

        #region Afrekenen bevestiging schermen
        [Authorize(Roles = "Zaalverantwoordelijke, Eigenaar")]
        [HttpGet("Rekening/Afrekenen/Bevestiging/{reservatieId}")]
        public IActionResult Bevestiging(int reservatieId)
        {
            ViewBag.ReservatieId = reservatieId;
            return View();
        }

        [Authorize(Roles = "Zaalverantwoordelijke, Eigenaar")]
        [HttpGet("Rekening/Afrekenen/Mislukt/{reservatieId}")]
        public IActionResult Mislukt(int reservatieId, string betaalMethode, string errorType = "default")
        {
            ViewBag.ReservatieId = reservatieId;
            ViewBag.BetaalMethode = betaalMethode;
            ViewBag.ErrorType = errorType;
            return View();
        }
        #endregion

        #region Betaling methods
        private async Task<bool> BehandelBetaling(int reservatieId, string betaalMethode)
        {
            switch (betaalMethode)
            {   case "Cash":
                    return await _unitOfWork.Reservaties.BehandelBetaling(reservatieId);
                case "Payconic":
                    // Logica voor Payconic betaling
                    break;
            }
            return false;
        }

        #endregion
    }
}
