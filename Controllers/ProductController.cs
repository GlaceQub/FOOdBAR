using Restaurant.ViewModels.Product.Gerechten;

namespace Restaurant.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ------------------------------------------//
        // ----------------GERECHTEN-----------------//
        // ------------------------------------------//

        // GET: /Product/Gerechten
        [Authorize(Roles = "Kok, Eigenaar")]
        public async Task<IActionResult> Gerechten()
        {
            var producten = await _unitOfWork.Producten.GetAllWithCategorieAndPricesAsync();
            var gerechten = producten.Where(p => p.Categorie != null && (p.Categorie.TypeId == 2 || p.Categorie.TypeId == 3 || p.Categorie.TypeId == 4)).ToList();
            return View("Gerechten/Index", gerechten);
        }

        // GET: /Product/Gerechten/Edit/id
        [Authorize(Roles = "Kok, Eigenaar")]
        public async Task<IActionResult> EditGerecht(int id)
        {
            var product = await _unitOfWork.Producten.GetByIdWithPriceAsync(id);
            if (product == null)
                return NotFound();

            var viewmodel = new GerechtenEditViewModel
            {
                Id = product.Id,
                Naam = product.Naam,
                Beschrijving = product.Beschrijving,
                AllergenenInfo = product.AllergenenInfo,
                CategorieId = product.CategorieId,
                Actief = product.Actief,
                Prijs = product.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).FirstOrDefault()?.Prijs ?? 0,
                CategorieList = _unitOfWork.Categorieen.GetAll().Where(c => c.TypeId == 2 || c.TypeId == 3 || c.TypeId == 4).ToList()
            };

            return View("Gerechten/Edit", viewmodel);
        }

        // POST: /Product/Gerechten/Edit/id
        [HttpPost]
        [Authorize(Roles = "Kok, Eigenaar")]
        public async Task<IActionResult> EditGerecht(int id, GerechtenEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();
            if (!ModelState.IsValid)
            {
                model.CategorieList = _unitOfWork.Categorieen.GetAll().Where(c => c.TypeId == 2 || c.TypeId == 3 || c.TypeId == 4).ToList();
                return View("Gerechten/Edit", model);
            }
            var product = await _unitOfWork.Producten.GetByIdWithPriceAsync(id);
            if (product == null)
                return NotFound();
            product.Naam = model.Naam;
            product.Beschrijving = model.Beschrijving;
            product.AllergenenInfo = model.AllergenenInfo;
            product.CategorieId = model.CategorieId;
            product.Actief = model.Actief;
            var huidigePrijsProduct = product.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).FirstOrDefault();
            if (huidigePrijsProduct == null || huidigePrijsProduct.Prijs != model.Prijs)
            {
                var nieuwePrijsProduct = new PrijsProduct
                {
                    ProductId = product.Id,
                    Prijs = model.Prijs,
                    DatumVanaf = DateTime.Now
                };
                _unitOfWork.PrijsProducten.Add(nieuwePrijsProduct);
            }
            _unitOfWork.Producten.Update(product);
            _unitOfWork.Save();
            return RedirectToAction("Gerechten");
        }

        // GET: /Product/Gerechten/Create
        [Authorize(Roles = "Kok, Eigenaar")]
        public async Task<ActionResult> CreateGerecht()
        {
            var viewmodel = new GerechtenCreateViewModel
            {
                CategorieList = _unitOfWork.Categorieen.GetAll().Where(c => c.TypeId == 2 || c.TypeId == 3 || c.TypeId == 4).ToList()
            };
            return View("Gerechten/Create", viewmodel);
        }


        // POST: /Product/Gerechten/Create
        [HttpPost]
        [Authorize(Roles = "Kok, Eigenaar")]
        public async Task<ActionResult> CreateGerecht(GerechtenCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CategorieList = _unitOfWork.Categorieen.GetAll().Where(c => c.TypeId == 2 || c.TypeId == 3 || c.TypeId == 4).ToList();
                return View("Gerechten/Create", model);
            }


            var newProduct = new Product
            {
                Naam = model.Naam,
                Beschrijving = model.Beschrijving,
                AllergenenInfo = model.AllergenenInfo,
                CategorieId = model.CategorieId,
                Actief = model.Actief,

            };

            _unitOfWork.Producten.Add(newProduct);
            _unitOfWork.Save();

            var newPrijsProduct = new PrijsProduct
            {
                ProductId = newProduct.Id,
                Prijs = model.Prijs,
                DatumVanaf = DateTime.Now
            };
            _unitOfWork.PrijsProducten.Add(newPrijsProduct);
            _unitOfWork.Save();

            return RedirectToAction("Gerechten");
        }

        // POST: /Product/Gerechten/Delete/id
        [HttpPost]
        [Authorize(Roles = "Kok, Eigenaar")]
        public async Task<IActionResult> DeleteGerecht(int id)
        {
            var product = await _unitOfWork.Producten.GetByIdWithPriceAsync(id);
            if (product == null)
                return NotFound();

            // Niet-verplichte velden op null zetten
            product.Beschrijving = null;
            product.AllergenenInfo = null;
            product.IsSuggestie = false;

            // Verplichte velden op "VERWIJDERD" zetten
            product.Naam = "VERWIJDERD";
            product.CategorieId = 0; 
            product.Actief = false;

            _unitOfWork.Producten.Update(product);
            _unitOfWork.Save();
            return RedirectToAction("Gerechten");
        }

        // ------------------------------------------//
        // ----------------DRANKEN-------------------//
        // ------------------------------------------//

        // GET: /Product/Dranken
        [Authorize(Roles = "Ober, Eigenaar")]

        public async Task<IActionResult> Dranken(string filter = "active")
        {
            var producten = await _unitOfWork.Producten.GetAllWithCategorieAndPricesAsync();

            var query = producten.Where(p => p.Categorie != null && p.Categorie.TypeId == 1);

            if (string.Equals(filter, "active", StringComparison.OrdinalIgnoreCase))
                query = query.Where(p => p.Actief);
            else if (string.Equals(filter, "inactive", StringComparison.OrdinalIgnoreCase))
                query = query.Where(p => !p.Actief);

            var dranken = query.ToList();
            ViewBag.SelectedFilter = filter;
            return View("Dranken/Index", dranken);
        }

        // GET: /Product/Dranken/Edit/id
        [Authorize(Roles = "Ober, Eigenaar")]

        public async Task<IActionResult> EditDrank(int id)
        {
            var product = await _unitOfWork.Producten.GetByIdWithPriceAsync(id);
            if (product == null)
                return NotFound();

            var viewmodel = new GerechtenEditViewModel
            {
                Id = product.Id,
                Naam = product.Naam,
                Beschrijving = product.Beschrijving,
                AllergenenInfo = product.AllergenenInfo,
                CategorieId = product.CategorieId,
                Actief = product.Actief,
                Prijs = product.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).FirstOrDefault()?.Prijs ?? 0,
                CategorieList = _unitOfWork.Categorieen.GetAll().Where(c => c.TypeId == 1 || c.TypeId == 2 || c.TypeId == 3 || c.TypeId == 4).ToList()
            };

            return View("Dranken/Edit", viewmodel);
        }

        // POST: /Product/Dranken/Edit/id
        [HttpPost]
        [Authorize(Roles = "Ober, Eigenaar")]

        public async Task<IActionResult> EditDrank(int id, GerechtenEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();
            if (!ModelState.IsValid)
            {
                model.CategorieList = _unitOfWork.Categorieen.GetAll().Where(c => c.TypeId == 1 || c.TypeId == 2 || c.TypeId == 3 || c.TypeId == 4).ToList();
                return View("Dranken/Edit", model);
            }
            var product = await _unitOfWork.Producten.GetByIdWithPriceAsync(id);
            if (product == null)
                return NotFound();

            product.Naam = model.Naam;
            product.Beschrijving = model.Beschrijving;
            product.AllergenenInfo = model.AllergenenInfo;
            product.CategorieId = model.CategorieId;
            product.Actief = model.Actief;
            product.IsSuggestie = model.IsSuggestie;

            var huidigePrijsProduct = product.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).FirstOrDefault();

            if (huidigePrijsProduct == null || huidigePrijsProduct.Prijs != model.Prijs)
            {
                var nieuwePrijsProduct = new PrijsProduct
                {
                    ProductId = product.Id,
                    Prijs = model.Prijs,
                    DatumVanaf = DateTime.Now
                };
            await _unitOfWork.PrijsProducten.Add(nieuwePrijsProduct);
            }
            await _unitOfWork.Producten.Update(product);
            return RedirectToAction("Dranken");
        }

        // GET: /Product/Dranken/Create
        [Authorize(Roles = "Ober, Eigenaar")]

        public async Task<ActionResult> CreateDrank()
        {
            var viewmodel = new GerechtenCreateViewModel
            {
                CategorieList = _unitOfWork.Categorieen.GetAll().Where(c => c.TypeId == 1 || c.TypeId == 2 || c.TypeId == 3 || c.TypeId == 4).ToList()
            };
            return View("Dranken/Create", viewmodel);
        }

        // POST: /Product/Dranken/Create
        [HttpPost]
        [Authorize(Roles = "Ober, Eigenaar")]

        public async Task<ActionResult> CreateDrank(GerechtenCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CategorieList = _unitOfWork.Categorieen.GetAll().Where(c => c.TypeId == 1 || c.TypeId == 2 || c.TypeId == 3 || c.TypeId == 4).ToList();
                return View("Dranken/Create", model);
            }

            var newProduct = new Product
            {
                Naam = model.Naam,
                Beschrijving = model.Beschrijving,
                AllergenenInfo = model.AllergenenInfo,
                CategorieId = model.CategorieId,
                Actief = model.Actief,
            };

            await _unitOfWork.Producten.Add(newProduct);
          

            var newPrijsProduct = new PrijsProduct
            {
                ProductId = newProduct.Id,
                Prijs = model.Prijs,
                DatumVanaf = DateTime.Now
            };

           await _unitOfWork.PrijsProducten.Add(newPrijsProduct);

            return RedirectToAction("Dranken");
        }

        // POST: /Product/Dranken/Delete/id
        [HttpPost]
        [Authorize(Roles = "Ober, Eigenaar")]

        public async Task<IActionResult> DeleteDrank(int id)
        {
            var product = await _unitOfWork.Producten.GetByIdWithPriceAsync(id);
            if (product == null)
                return NotFound();

            product.Actief = false;
            product.Naam = !string.IsNullOrWhiteSpace(product.Naam) ? product.Naam + " (verwijderd)" : "VERWIJDERD";
            product.Beschrijving = null;
            product.AllergenenInfo = null;
            product.IsSuggestie = false;

            var huidigePrijsProduct = product.PrijsProducten?.OrderByDescending(pp => pp.DatumVanaf).FirstOrDefault();
            if (huidigePrijsProduct == null || huidigePrijsProduct.Prijs != 0m)
            {
                var nieuwePrijsProduct = new PrijsProduct
                {
                    ProductId = product.Id,
                    Prijs = 0m,
                    DatumVanaf = DateTime.Now
                };
                await _unitOfWork.PrijsProducten.Add(nieuwePrijsProduct);
            }

            await _unitOfWork.Producten.Update(product);

            return RedirectToAction("Dranken");
        }
    }
}
