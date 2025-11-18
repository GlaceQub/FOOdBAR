using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Restaurant.ViewModels.Account;
using Restaurant.Data;

namespace Restaurant.Controllers
{
    [Authorize]
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(SignInManager<CustomUser> signInManager, UserManager<CustomUser> userManager, IUnitOfWork unitOfWork)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Klant, Eigenaar, Zaalverantwoordelijke, Kok, Ober")]
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet("Registratie")]
        public async Task<IActionResult> Registratie()
        {
            var model = new RegistratieViewModel
            {
                Landen = await GetLandenSelectListAsync()
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) 
            {
                ModelState.AddModelError("Email", "Account niet gevonden");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard", "Account");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Account is gedactiveerd");
                return View(model);
            }

            ModelState.AddModelError("Password", "Onjuist wachtwoord");
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost("Registratie")]
        public async Task<IActionResult> Registratie(RegistratieViewModel model, string? terug)
        {
            model.Landen = await GetLandenSelectListAsync();

            // Debugging: if ModelState is invalid, log all errors and show a summary on the page
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value.Errors.Count > 0)
                    .Select(kvp => new
                    {
                        Field = kvp.Key,
                        Errors = kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage).ToArray()
                    })
                    .ToList();

                System.Diagnostics.Debug.WriteLine("Registratie ModelState is invalid:");
                foreach (var e in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"{e.Field}: {string.Join(", ", e.Errors)}");
                }

                // Add a top-level message so the view shows a summary (helps when client doesn't show field errors)
                ModelState.AddModelError(string.Empty, "Validatie mislukt — controleer de velden hieronder.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(terug))
            {
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Dit e-mailadres is al in gebruik");
                    return View(model);
                }

                // Lookup country name and set it
                var land = (await _unitOfWork.Landen.GetActieveLandenAsync())
                    .FirstOrDefault(l => l.Id == model.Land);
                model.LandNaam = land?.Naam;

                return View("RegistratieBevestigen", model);
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost("RegistratieBevestigen")]
        public async Task<IActionResult> RegistratieBevestigen(RegistratieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Landen = await GetLandenSelectListAsync();
                return View("Registratie", model);
            }

            var user = new CustomUser
            {
                UserName = model.Email,
                Email = model.Email,
                Voornaam = model.Voornaam,
                Achternaam = model.Naam,
                Adres = model.Adres,
                Huisnummer = model.Huisnummer,
                Postcode = model.Postcode,
                Gemeente = model.Gemeente,
                LandId = model.Land,
                Actief = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Assign "Klant" role to new user
                await _userManager.AddToRoleAsync(user, "Klant");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Dashboard", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            model.Landen = await GetLandenSelectListAsync();
            return View("Registratie", model);
        }

        [Authorize]
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task<IEnumerable<SelectListItem>> GetLandenSelectListAsync()
        {
            var landen = await _unitOfWork.Landen.GetActieveLandenAsync();
            return landen.Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Naam });
        }
    }
}
