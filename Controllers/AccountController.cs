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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(SignInManager<CustomUser> signInManager, UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Klant, Eigenaar, Zaalverantwoordelijke, Kok, Ober")]
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }

        #region Inloggen en registreren
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

        #endregion

        #region Gebruikersbeheer
        [Authorize(Roles = "Eigenaar")]
        [HttpGet("Gebruikers")]
        public async Task<IActionResult> Gebruikers()
        {
            var users = await _userManager.Users.ToListAsync();
            // Filter out deleted users
            users = users.Where(u => u.Email != "Verwijderd").ToList();

            var viewModel = new List<GebruikerViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                viewModel.Add(new GebruikerViewModel { User = user, Roles = roles.ToList() });
            }
            return View(viewModel);
        }

        [Authorize(Roles = "Eigenaar")]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var gebruiker = await _userManager.FindByIdAsync(id);
            if (gebruiker == null)
                return NotFound();

            var AllRoles = await GetRollenSelectListAsync();

            var landenSelectList = await GetLandenSelectListAsync();

            var viewModel = new GebruikerEditViewModel
            {
                Id = id,
                Voornaam = gebruiker.Voornaam,
                Achternaam = gebruiker.Achternaam,
                Email = gebruiker.Email,
                Adres = gebruiker.Adres,
                Huisnummer = gebruiker.Huisnummer,
                Postcode = gebruiker.Postcode,
                Gemeente = gebruiker.Gemeente,
                LandId = gebruiker.LandId,
                Actief = gebruiker.Actief,
                Rollen = await _userManager.GetRolesAsync(gebruiker),
                AllRollen = AllRoles,
                Landen = landenSelectList
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Eigenaar")]
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, GebruikerEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns for the view
                vm.AllRollen = await GetRollenSelectListAsync();
                vm.Landen = await GetLandenSelectListAsync();
                return View(vm);
            }

            var gebruiker = await _userManager.FindByIdAsync(id);
            if (gebruiker == null)
                return NotFound();

            // Check if email is being changed to one that already exists
            if (gebruiker.Email != vm.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(vm.Email);
                if (existingUser != null && existingUser.Id != id)
                {
                    ModelState.AddModelError("Email", "Dit e-mailadres is al in gebruik");
                    // Repopulate dropdowns for the view
                    vm.AllRollen = await GetRollenSelectListAsync();
                    vm.Landen = await GetLandenSelectListAsync();
                    return View(vm);
                }
            }

            // Update user properties
            gebruiker.Voornaam = vm.Voornaam;
            gebruiker.Achternaam = vm.Achternaam;
            gebruiker.Email = vm.Email;
            gebruiker.UserName = vm.Email; // If you want username to match email
            gebruiker.Adres = vm.Adres;
            gebruiker.Huisnummer = vm.Huisnummer;
            gebruiker.Postcode = vm.Postcode;
            gebruiker.Gemeente = vm.Gemeente;
            gebruiker.LandId = vm.LandId;
            gebruiker.Actief = vm.Actief;

            // Update roles
            var currentRoles = await _userManager.GetRolesAsync(gebruiker);
            var rolesToAdd = vm.Rollen.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(vm.Rollen);

            if (rolesToRemove.Any())
                await _userManager.RemoveFromRolesAsync(gebruiker, rolesToRemove);

            if (rolesToAdd.Any())
                await _userManager.AddToRolesAsync(gebruiker, rolesToAdd);

            // Update user in database
            var result = await _userManager.UpdateAsync(gebruiker);

            if (result.Succeeded)
            {
                return RedirectToAction("Gebruikers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // Repopulate dropdowns for the view
            vm.AllRollen = await GetRollenSelectListAsync();
            vm.Landen = await GetLandenSelectListAsync();
            return View(vm);
        }

        [Authorize(Roles = "Eigenaar")]
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var gebruiker = await _userManager.FindByIdAsync(id);
            if (gebruiker == null)
                return NotFound();

            // Prevent deleting the last "Eigenaar"
            if (await _userManager.IsInRoleAsync(gebruiker, "Eigenaar"))
            {
                var eigenaars = await _userManager.GetUsersInRoleAsync("Eigenaar");
                if (eigenaars.Count == 1 && eigenaars[0].Id == gebruiker.Id)
                {
                    TempData["Error"] = "De enige eigenaar kan niet worden verwijderd.";
                    return RedirectToAction("Gebruikers");
                }
            }

            // Remove all roles
            var roles = await _userManager.GetRolesAsync(gebruiker);
            if (roles.Any())
                await _userManager.RemoveFromRolesAsync(gebruiker, roles);

            // Set required fields to "<<Verwijderd>>", others to null or empty
            gebruiker.Email = "Verwijderd";
            gebruiker.UserName = "Verwijderd";
            gebruiker.Voornaam = "Verwijderd";
            gebruiker.Achternaam = "Verwijderd";
            gebruiker.Adres = null;
            gebruiker.Huisnummer = null;
            gebruiker.Postcode = null;
            gebruiker.Gemeente = null;
            gebruiker.LandId = 1;
            gebruiker.Actief = false;

            var result = await _userManager.UpdateAsync(gebruiker);
            if (result.Succeeded)
                return RedirectToAction("Gebruikers");

            foreach (var error in result.Errors)
            {
                TempData["Error"] = error.Description;
            }

            return RedirectToAction("Gebruikers");
        }
        #endregion

        [Authorize(Roles = "Eigenaar")]
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var vm = new RegistratieViewModel()
            {
                AllRollen = await GetRollenSelectListAsync(),
                Landen = await GetLandenSelectListAsync()
            };
            return View(vm);
        }

        [Authorize(Roles = "Eigenaar")]
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistratieViewModel model)
        {
            model.Landen = await GetLandenSelectListAsync();
            model.AllRollen = await GetRollenSelectListAsync();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Dit e-mailadres is al in gebruik");
                return View(model);
            }
            var user = new CustomUser
            {
                UserName = model.Email,
                Email = model.Email,
                Voornaam = model.Voornaam,
                Achternaam = model.Naam,
                LandId = model.Land,
                Actief = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Assign selected roles to new user
                if (model.Rollen != null && model.Rollen.Any())
                {
                    await _userManager.AddToRolesAsync(user, model.Rollen);
                }
                return RedirectToAction("Gebruikers", "Account");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        #region Helpers
        private async Task<IEnumerable<SelectListItem>> GetLandenSelectListAsync()
        {
            var landen = await _unitOfWork.Landen.GetActieveLandenAsync();
            return landen.Select(l => new SelectListItem { Value = l.Id.ToString(), Text = l.Naam });
        }

        private async Task<IEnumerable<SelectListItem>> GetRollenSelectListAsync()
        {
            var rollen = await _roleManager.Roles.ToListAsync();
            return rollen.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
        }
        #endregion
    }
}
