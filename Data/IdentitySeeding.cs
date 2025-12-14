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
                // Roles aanmaken als deze nog niet bestaan
                if (!await roleManager.RoleExistsAsync("Eigenaar"))
                    await roleManager.CreateAsync(new IdentityRole("Eigenaar"));
                if (!await roleManager.RoleExistsAsync("Klant"))
                    await roleManager.CreateAsync(new IdentityRole("Klant"));

                // Admin gebruiker aanmaken als deze nog niet bestaat
                var adminEmail = "admin@foodbar.be";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var defaultUser = new CustomUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        LandId = 1,
                        Actief = true,
                    };
                    var result = await userManager.CreateAsync(defaultUser, "F00d.B4r");
                    if (!result.Succeeded)
                        throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
                    adminUser = defaultUser;
                }
                if (!await userManager.IsInRoleAsync(adminUser, "Eigenaar"))
                    await userManager.AddToRoleAsync(adminUser, "Eigenaar");

                // Ken de rol "Klant" toe aan alle andere gebruikers zonder rol
                var allUsers = userManager.Users.ToList();
                foreach (var user in allUsers)
                {
                    if (user.Email == adminEmail)
                        continue; // Overgeslagen admin, al afgehandeld

                    var roles = await userManager.GetRolesAsync(user);
                    if (roles == null || roles.Count == 0)
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
