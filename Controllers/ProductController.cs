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

        // GET: /Product/Gerechten
        public async Task<IActionResult> Gerechten()
        {
            var producten = await _unitOfWork.Producten.GetAllWithCategorieAndPricesAsync();
            var gerechten = producten.Where(p => p.Categorie != null && (p.Categorie.TypeId == 2 || p.Categorie.TypeId == 3 || p.Categorie.TypeId == 4)).ToList();
            return View("Gerechten/Index", gerechten);
        }

        // GET: /Product/Gerechten/Edit/id
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
    }
}
