using Microsoft.AspNetCore.Authentication.JwtBearer;
using Restaurant.Dto.Account;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Restaurant.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/account")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly ILogger<AccountApiController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountApiController(ILogger<AccountApiController> logger,
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(AccountLoginDto model)
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
