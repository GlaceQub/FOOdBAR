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
        public async Task<IActionResult> Create(int reservatieId)
        {
            var hasAssignedTable = await _unitOfWork.TafelLijsten.HasAssignedTableAsync(reservatieId);

            var types = await _unitOfWork.CategorieTypen.GetAllWithCategoriesAndProductsAsync();

            var menuTypes = types
                .OrderBy(type => type.Id)
                .Select(type => new CategorieTypeViewModel
                {
                    Naam = type.Naam,
                    Categorieen = type.Categorieen
                        .OrderBy(c => c.Id)
                        .Select(c => new CategorieViewModel
                        {
                            Naam = c.Naam,
                            Producten = c.Producten
                                .OrderBy(p => p.Id)
                                .Select(p => new ProductViewModel
                                {
                                    Id = p.Id,
                                    Naam = p.Naam,
                                    Beschrijving = p.Beschrijving,
                                    Prijs = p.PrijsProducten
                                        .OrderByDescending(pp => pp.DatumVanaf)
                                        .FirstOrDefault()?.Prijs ?? 0
                                }).ToList()
                        }).ToList()
                }).ToList();

            var model = new BestellingCreateViewModel
            {
                ReservatieId = reservatieId,
                HasAssignedTable = hasAssignedTable,
                MenuTypes = menuTypes,
                CartItemsWithProduct = new List<CartItemWithProductViewModel>(),
                TotaalBedrag = 0
            };
            ViewBag.CartItemsJson = "[]";
            return View(model);
        }

        [Authorize(Roles = "Eigenaar, zaalverantwoordelijke, ober, kok, klant")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(BestellingCreateViewModel model, string CartItemsJson)
        {
            // Restore cart from hidden field
            var cartItems = new List<CartItemWithProductViewModel>();
            if (!string.IsNullOrEmpty(CartItemsJson))
            {
                // Deserialize to a simple DTO (ProductId, Aantal)
                var simpleCart = System.Text.Json.JsonSerializer.Deserialize<List<CartItemWithProductViewModel>>(CartItemsJson)
                    ?? new List<CartItemWithProductViewModel>();

                // Rebuild CartItemsWithProduct for display and processing
                foreach (var item in simpleCart)
                {
                    var product = await _unitOfWork.Producten.GetByIdWithPriceAsync(item.ProductId);
                    if (product != null)
                    {
                        cartItems.Add(new CartItemWithProductViewModel
                        {
                            ProductId = item.ProductId,
                            Aantal = item.Aantal,
                            Naam = product.Naam,
                            Prijs = product.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).FirstOrDefault()?.Prijs ?? 0
                        });
                    }
                }
            }
            model.CartItemsWithProduct = cartItems;
            model.TotaalBedrag = cartItems.Sum(ci => ci.Aantal * ci.Prijs);

            // re-check table assignment using the model's ReservatieId
            model.HasAssignedTable = await _unitOfWork.TafelLijsten.HasAssignedTableAsync(model.ReservatieId);

            if (!model.HasAssignedTable)
            {
                // Re-populate menu for redisplay
                var types = await _unitOfWork.CategorieTypen.GetAllWithCategoriesAndProductsAsync();
                model.MenuTypes = types
                    .OrderBy(type => type.Id)
                    .Select(type => new CategorieTypeViewModel
                    {
                        Naam = type.Naam,
                        Categorieen = type.Categorieen
                            .OrderBy(c => c.Id)
                            .Select(c => new CategorieViewModel
                            {
                                Naam = c.Naam,
                                Producten = c.Producten
                                    .OrderBy(p => p.Id)
                                    .Select(p => new ProductViewModel
                                    {
                                        Id = p.Id,
                                        Naam = p.Naam,
                                        Beschrijving = p.Beschrijving,
                                        Prijs = p.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).FirstOrDefault()?.Prijs ?? 0
                                    }).ToList()
                            }).ToList()
                    }).ToList();

                ViewBag.CartItemsJson = CartItemsJson ?? "[]";
                return View(model);
            }

            // Process the order
            foreach (var item in cartItems)
            {
                var bestelling = new Bestelling
                {
                    ReservatieId = model.ReservatieId,
                    ProductId = item.ProductId,
                    Aantal = item.Aantal,
                    TijdstipBestelling = DateTime.Now,
                    StatusId = 5, // Status "Toegevoegd"
                };

                await _unitOfWork.Bestellingen.AddAsync(bestelling);
            }

            await _unitOfWork.CompleteAsync();

            // TODO: Bevestigingsmail sturen en notificaties verwerken

            return RedirectToAction("Bevestiging");
        }

        [Authorize(Roles = "Eigenaar, zaalverantwoordelijke, ober, kok, klant")]
        public async Task<IActionResult> Bevestiging()
        {
            return View();
        }
    }
}
