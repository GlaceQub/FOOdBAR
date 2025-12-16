using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Data.Repository;
using Restaurant.ViewModels.Quiz;

namespace Restaurant.Controllers
{
    public class QuizController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuizController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /Quiz/Gerecht
        [HttpGet]
        public IActionResult Gerecht(int vraag = 0)
        {
            var vm = new QuizGerechtViewModel();
            vm.CurrentQuestion = vraag;
            return View(vm);
        }

        // POST: /Quiz/Gerecht
        [HttpPost]
        public async Task<IActionResult> Gerecht(QuizGerechtViewModel model)
        {
            // Welke knop stuurde de gebruiker?
            var action = (Request.Form["submit"].FirstOrDefault() ?? "next").ToLowerInvariant();

            // Als gebruiker op "Terug" klikt, ga één vraag terug zonder validatie
            if (action == "back")
            {
                model.CurrentQuestion = Math.Max(0, model.CurrentQuestion - 1);
                ModelState.Clear(); // zodat de view de waarden uit het model gebruikt
                return View(model);
            }

            // "next" knop: valideer het antwoord van de huidige vraag
            if (!ModelState.IsValid)
            {
                // ModelState bevat validatiefouten (bv. required radio niet gekozen)
                return View(model);
            }

            // Verwerk/acceptatie van antwoord habituely gebeurt via model-binding (hidden inputs behouden eerdere antwoorden)
            model.CurrentQuestion++;

            ModelState.Clear(); // belangrijk zodat helpers de nieuwe modelwaarden tonen

            // Als er nog vragen zijn: toon volgende vraag
            if (model.CurrentQuestion < QuizGerechtViewModel.Vragen.Count)
            {
                return View(model);
            }

            // Alle vragen beantwoord: bereken beste match
            var quizEigenschappen = await _unitOfWork.QuizEigenschappen.GetQuizEigenschappenVoorGerechtenAsync();

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
            // Zet CurrentQuestion op Count zodat view resultaat kan tonen
            model.CurrentQuestion = QuizGerechtViewModel.Vragen.Count;
            return View(model);
        }

        // GET: /Quiz/Drank
        [HttpGet]
        public IActionResult Drank(int vraag = 0)
        {
            var vm = new QuizDrankViewModel();
            vm.CurrentQuestion = vraag;
            return View(vm);
        }

        // POST: /Quiz/Drank
        [HttpPost]
        public async Task<IActionResult> Drank(QuizDrankViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verhoog het vraagnummer in het model
            model.CurrentQuestion++;

            // Belangrijk: clear ModelState zodat Razor de bijgewerkte modelwaarden gebruikt
            ModelState.Clear();

            // Toon volgende vraag als we nog niet klaar zijn
            if (model.CurrentQuestion < QuizDrankViewModel.Vragen.Count)
            {
                return View(model);
            }

            // Alle vragen beantwoord: bereken de beste match
            var quizEigenschappen = await _unitOfWork.QuizEigenschappen.GetQuizEigenschappenVoorDrankenAsync();

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

            // model.CurrentQuestion is nu >= Count -> view kan resultaat tonen
            return View(model);
        }
    }
}