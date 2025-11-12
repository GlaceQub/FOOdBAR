using Microsoft.AspNetCore.SignalR;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    [Authorize]
    public class BestellingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<BestellingNotificationHub> _hubContext;

        public BestellingController(IUnitOfWork unitOfWork, IHubContext<BestellingNotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        #region Index (overzicht bestellingen)
        [Authorize(Roles = "Eigenaar, Kok, Ober")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bestellingen = await _unitOfWork.Bestellingen.GetAllAsync();

            // Bepaal filter voor Ober of Kok
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var filteredBestellingen = bestellingen;

            switch (userRole)
            {
                case "Ober":
                    filteredBestellingen = bestellingen.Where(b => _unitOfWork.CategorieTypen.GetByCategorieIdAsync(b.Product.CategorieId).Result?.Naam == "Dranken").ToList(); // Dranken
                    break;
                case "Kok":
                    filteredBestellingen = bestellingen.Where(b => _unitOfWork.CategorieTypen.GetByCategorieIdAsync(b.Product.CategorieId).Result?.Naam != "Dranken").ToList(); // Gerechten
                    break;
            }
            filteredBestellingen = filteredBestellingen
                .OrderBy(b => b.TijdstipBestelling)
                .ToList();

            var statussen = await _unitOfWork.Statussen.GetAllAsync();
            var statusColors = new Dictionary<int, string>
            {
                { 1, "primary" },   // In behandeling
                { 2, "success" },   // Klaar
                { 3, "info" },      // Gereserveerd
                { 4, "danger" },    // Geannuleerd
                { 5, "warning" }       // Toegevoegd
            };
            ViewBag.StatusList = statussen;
            ViewBag.StatusColors = statusColors;

            return View(filteredBestellingen);
        }

        [Authorize(Roles = "Eigenaar, Kok, Ober")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int StatusId)
        {
            var bestelling = await _unitOfWork.Bestellingen.GetByIdAsync(id);
            if (bestelling == null)
                return NotFound();

            bestelling.StatusId = StatusId;
            await _unitOfWork.CompleteAsync();

            return Ok();
        }

        #endregion

        #region Create (menu)
        [Authorize(Roles = "Klant, Eigenaar")]
        [HttpGet]
        public async Task<IActionResult> Create(int reservatieId)
        {
            var hasAssignedTable = await _unitOfWork.TafelLijsten.HasAssignedTableAsync(reservatieId);

            var menuTypes = await GetMenuTypesAsync();

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

        [Authorize(Roles = "Klant, Eigenaar")]
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
                            CategorieId = product.CategorieId,
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
                model.MenuTypes = await GetMenuTypesAsync();

                ViewBag.CartItemsJson = CartItemsJson ?? "[]";
                return View(model);
            }

            // Process the order
            int drinkCount = 0;
            int foodCount = 0;

            foreach (var item in cartItems)
            {
                var bestelling = new Bestelling
                {
                    ReservatieId = model.ReservatieId,
                    ProductId = item.ProductId,
                    Aantal = item.Aantal,
                    TijdstipBestelling = DateTime.Now,
                    StatusId = 5, // Status "Toegevoegd"
                    Opmerking = item.Opmerking
                };

                var categorieType = await _unitOfWork.CategorieTypen.GetByCategorieIdAsync(item.CategorieId);
                if (categorieType?.Naam == "Dranken") drinkCount++; else foodCount++;

                await _unitOfWork.Bestellingen.AddAsync(bestelling);
            }

            int result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                // Notify relevant staff based on order contents
                if (drinkCount > 0)
                {
                    await _hubContext.Clients.Group("Ober").SendAsync("NieuweBestelling", $"{drinkCount} Nieuwe drankbestelling{(drinkCount > 1 ? "en" : "")}");
                }
                if (foodCount > 0)
                {
                    await _hubContext.Clients.Group("Kok").SendAsync("NieuweBestelling", $"{foodCount} Nieuwe gerechtbestelling{(foodCount > 1 ? "en" : "")}");
                }
                // TODO: Bevestigingsmail sturen en notificaties verwerken

                return RedirectToAction("Bevestiging");
            }
            else
            {
                // Handle failure (e.g., show error, redisplay form, etc.)
                ModelState.AddModelError("", "Er is een fout opgetreden bij het verwerken van de bestelling.");
                // Re-populate menu for redisplay
                model.MenuTypes = await GetMenuTypesAsync();

                ViewBag.CartItemsJson = CartItemsJson ?? "[]";
                return View(model);
            }
        }

        [Authorize(Roles = "Eigenaar, Klant")]
        public async Task<IActionResult> Bevestiging()
        {
            return View();
        }

        private async Task<List<CategorieTypeViewModel>> GetMenuTypesAsync()
        {
            var types = await _unitOfWork.CategorieTypen.GetAllWithCategoriesAndProductsAsync();
            return types
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
        }
        #endregion
    }
}
