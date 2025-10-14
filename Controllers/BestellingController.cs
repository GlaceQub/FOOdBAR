namespace Restaurant.Controllers
{
    [Authorize(Roles = "Eigenaar")]
    public class BestellingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BestellingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Klant, Kok, Ober, Zaalverantwoordelijke, Eigenaar")]
        public async Task<IActionResult> Index()
        {
            var bestellingen = await _unitOfWork.Bestellingen.GetAllAsync();
            return View(bestellingen);
        }

        [Authorize(Roles = "Klant, Eigenaar")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new BestellingCreateViewModel
            {
                
            };
            return View(model);
        }

        [Authorize(Roles = "Klant, Eigenaar")]
        [HttpPost]
        public async Task<IActionResult> Create(BestellingCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Controle is er een tafel toegewezen?
            if (!model.HasAssignedTable)
            {
                ModelState.AddModelError("", "U heeft nog geen tafel toegewezen.");
                return View(model);
            }

            // Voeg de bestelling toe voor elk geselecteerd product
            foreach (var kvp in model.SelectedItems.Where(x => x.Value > 0))
            {
                var bestelling = new Bestelling
                {
                    ReservatieId = model.ReservatieId,
                    ProductId = kvp.Key,
                    Aantal = kvp.Value,
                    TijdstipBestelling = DateTime.Now,
                    StatusId = 5, // Status "Toegevoegd"
                };

                await _unitOfWork.Bestellingen.AddAsync(bestelling);
            }

            await _unitOfWork.CompleteAsync();

            // TODO: Bevestigingsmail sturen en notificaties verwerken

            return RedirectToAction("Index");
        }
    }
}
