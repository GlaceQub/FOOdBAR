using Restaurant.Models;
using Restaurant.ViewModels;

namespace Restaurant.Controllers
{
    public class CategorieController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategorieController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /Categorie/Index
        public IActionResult Index()
        {
            var categories = _unitOfWork.Categorieen.GetAll();
            return View(categories);
        }

        // GET: /Categorie/Edit/id
        public IActionResult Edit(int id)
        {
            var category = _unitOfWork.Categorieen.GetById(id);
            if (category == null)
                return NotFound();

            var viewModel = new CategorieEditViewModel
            {
                Id = category.Id,
                Naam = category.Naam,
                TypeId = category.TypeId,
                Actief = category.Actief,
                TypeList = _unitOfWork.CategorieTypen.GetAll()
            };

            return View(viewModel);
        }

        // POST: /Categorie/Edit/id
        [HttpPost]
        public IActionResult Edit(int id, CategorieEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                model.TypeList = _unitOfWork.CategorieTypen.GetAll();
                return View(model);
            }

            var categorie = _unitOfWork.Categorieen.GetById(id);
            if (categorie == null)
                return NotFound();

            categorie.Naam = model.Naam;
            categorie.TypeId = model.TypeId;
            categorie.Actief = model.Actief;

            _unitOfWork.Categorieen.Update(categorie);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Categorie/Create
        public IActionResult Create()
        {
            var viewModel = new CategorieCreateViewModel
            {
                Actief = true
            };

            ViewBag.TypeList = new SelectList(_unitOfWork.CategorieTypen.GetAll(), "Id", "Naam");
            return View(viewModel);
        }

        // POST: /Categorie/Create
        [HttpPost]
        public IActionResult Create(CategorieCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var categorie = new Categorie
                {
                    Naam = model.Naam,
                    TypeId = model.TypeId,
                    Actief = model.Actief,
                };
                _unitOfWork.Categorieen.Add(categorie);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TypeList = new SelectList(_unitOfWork.CategorieTypen.GetAll(), "Id", "Naam");
            return View(model);
        }

        // POST: /Categorie/Delete/id
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var categorie = _unitOfWork.Categorieen.GetById(id);
            if (categorie == null)
                return NotFound();
            _unitOfWork.Categorieen.Delete(categorie);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}

