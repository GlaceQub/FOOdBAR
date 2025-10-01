using Microsoft.AspNetCore.Mvc;
using Restaurant.Dto.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Restaurant.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ILogger<UserController> logger,
                                IMapper mapper,
                                UserManager<CustomUser> userManager,
                                SignInManager<CustomUser> signInManager,
                                RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("message", "Account niet gevonden");
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("message", "Het emailadres is nog niet bevestigd.");
                return BadRequest(ModelState);
            }

            if (await _userManager.CheckPasswordAsync(user, model.Password) == false)
            {
                ModelState.AddModelError("message", "Verkeerde logincombinatie!");
                return BadRequest(ModelState);
            }
            
            // Email is hetzelfde als de username
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("message", "Account geblokkeerd!!");
                return BadRequest(ModelState);
            }


            if (result.Succeeded)
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles != null)
                {
                    foreach (var userRole in userRoles)
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = Token.GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            ModelState.AddModelError("message", "Ongeldige loginpoging");
            return Unauthorized(ModelState);
        }
    }
}
