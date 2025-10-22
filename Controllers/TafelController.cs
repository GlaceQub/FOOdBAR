
namespace Restaurant.Controllers
{
    [Authorize]
    public class TafelController : Controller
    {
        private readonly RestaurantContext _context;

        public TafelController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: /Tafel
        public IActionResult Index()
        {
            var tafels = _context.Tafels.ToList();
            return View(tafels);
        }

        // GET: /Tafel/Create
        public IActionResult Create()
        {
            var tafel = new Tafel
            {
                Actief = true
            };
            return View(tafel);
        }

        // POST: /Tafel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tafel tafel)
        {

            if (tafel.MinAantalPersonen > tafel.AantalPersonen)
            {
                ModelState.AddModelError(nameof(tafel.MinAantalPersonen), "Minimum aantal personen mag niet hoger zijn dan het aantal personen.");
            }

            if (ModelState.IsValid)
            {
                _context.Tafels.Add(tafel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(tafel);
        }

        // GET: /Tafel/Edit/5
        public IActionResult Edit(int id)
        {
            var tafel = _context.Tafels.Find(id);
            if (tafel == null)
                return NotFound();
            return View(tafel);
        }

        // POST: /Tafel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Tafel tafel)
        {
            if (id != tafel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(tafel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(tafel);
        }

        // GET: /Tafel/Delete/5
        public IActionResult Delete(int id)
        {
            var tafel = _context.Tafels.Find(id);
            if (tafel == null)
                return NotFound();
            return View(tafel);
        }

        // POST: /Tafel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tafel = _context.Tafels.Find(id);
            if (tafel != null)
            {
                _context.Tafels.Remove(tafel);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}