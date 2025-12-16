using System.Data.Common;
using System.Linq;

namespace Restaurant.Data
{
    public class IdentitySeeding
    {
        public async Task IdentitySeedingAsync(UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            try
            {
                // Define all roles and ensure they exist
                var roles = new[] { "Eigenaar", "Klant", "Zaalverantwoordelijke", "Ober", "Kok" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                // Define all seed users with their roles
                var seedUsers = new[]
                {
                    new { Email = "admin@foodbar.be", Role = "Eigenaar" },
                    new { Email = "zv@foodbar.be", Role = "Zaalverantwoordelijke" },
                    new { Email = "ober@foodbar.be", Role = "Ober" },
                    new { Email = "kok@foodbar.be", Role = "Kok" },
                    new { Email = "klant@foodbar.be", Role = "Klant" },
                };

                // Create or update each seed user and assign their role
                foreach (var su in seedUsers)
                {
                    var user = await userManager.FindByEmailAsync(su.Email);
                    if (user == null)
                    {
                        user = new CustomUser
                        {
                            UserName = su.Email,
                            Email = su.Email,
                            EmailConfirmed = true,
                            LandId = 1,
                            Actief = true,
                        };
                        var result = await userManager.CreateAsync(user, "F00d.B4r");
                        if (!result.Succeeded)
                            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                    if (!await userManager.IsInRoleAsync(user, su.Role))
                        await userManager.AddToRoleAsync(user, su.Role);
                }

                // Assign "Klant" role to all users who have no roles
                var allUsers = userManager.Users.ToList();
                foreach (var user in allUsers)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    if (userRoles == null || userRoles.Count == 0)
                    {
                        await userManager.AddToRoleAsync(user, "Klant");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Seeding Failed", ex);
            }
        }
    }
}
