using Restaurant.ViewModels.Quiz;

namespace Restaurant.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizRepository _quizRepository;

        public QuizController(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        // GET: /Quiz/Gerecht
        [HttpGet]
        public IActionResult Gerecht()
        {
            var vm = new QuizGerechtViewModel();
            return View(vm);
        }

        // POST: /Quiz/Gerecht
        [HttpPost]
        public async Task<IActionResult> Gerecht(QuizGerechtViewModel model)
        {
            var quizEigenschappen = await _quizRepository.GetQuizEigenschappenVoorGerechtenAsync();

            // Bereken de beste match
            var bestMatch = quizEigenschappen
                .Select(q => new
                {
                    Product = q.Product,
                    Score =
                        Math.Abs(q.IsZoet - model.IsZoet) +
                        Math.Abs(q.IsZout - model.IsZout) +
                        Math.Abs(q.IsBitter - model.IsBitter) +
                        Math.Abs(q.IsFris - model.IsFris) +
                        Math.Abs(q.IsPikant - model.IsPikant) +
                        Math.Abs(q.IsAlcoholisch - model.IsAlcoholisch) +
                        Math.Abs(q.IsWarm - model.IsWarm) +
                        Math.Abs(q.IsKoud - model.IsKoud) +
                        Math.Abs(q.IsLicht - model.IsLicht) +
                        Math.Abs(q.IsZwaar - model.IsZwaar) +
                        Math.Abs(q.IsRomig - model.IsRomig) +
                        Math.Abs(q.IsFruitig - model.IsFruitig) +
                        Math.Abs(q.IsKruidig - model.IsKruidig) +
                        Math.Abs(q.IsExotisch - model.IsExotisch)
                })
                .OrderBy(x => x.Score)
                .FirstOrDefault();

            ViewBag.BestMatch = bestMatch?.Product?.Naam ?? "Geen match gevonden";
            return View(model);
        }

        // GET: /Quiz/Drank
        [HttpGet]
        public IActionResult Drank()
        {
            var vm = new QuizDrankViewModel();
            return View(vm);
        }

        // POST: /Quiz/Drank
        [HttpPost]
        public async Task<IActionResult> Drank(QuizDrankViewModel model)
        {
            var quizEigenschappen = await _quizRepository.GetQuizEigenschappenVoorDrankenAsync();

            // Bereken de beste match
            var bestMatch = quizEigenschappen
                .Select(q => new
                {
                    Product = q.Product,
                    Score =
                        Math.Abs(q.IsZoet - model.IsZoet) +
                        Math.Abs(q.IsBitter - model.IsBitter) +
                        Math.Abs(q.IsFris - model.IsFris) +
                        Math.Abs(q.IsAlcoholisch - model.IsAlcoholisch) +
                        Math.Abs(q.IsWarm - model.IsWarm) +
                        Math.Abs(q.IsKoud - model.IsKoud) +
                        Math.Abs(q.IsFruitig - model.IsFruitig) +
                        Math.Abs(q.IsKruidig - model.IsKruidig) +
                        Math.Abs(q.IsExotisch - model.IsExotisch)
                })
                .OrderBy(x => x.Score)
                .FirstOrDefault();

            ViewBag.BestMatch = bestMatch?.Product?.Naam ?? "Geen match gevonden";
            return View(model);
        }
    }
}
