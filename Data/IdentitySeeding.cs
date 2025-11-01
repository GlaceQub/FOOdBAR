using System.Data.Common;
using System.Linq;

namespace Restaurant.Data
{
    public class IdentitySeeding
    {
        public async Task IdentitySeedingAsync(UserManager<CustomUser> userManager)
        {

            try
            {
                // Gebruiker aanmaken
                // Admin bestaat nog niet?
                if (await userManager.FindByNameAsync("admin@foodbar.be") == null)
                {
                    // Gebruiker voorzien
                    var defaultUser = new CustomUser
                    {
                        UserName = "admin@foodbar.be",
                        Email = "admin@foodbar.be",
                        EmailConfirmed = true,
                        LandId = 1,
                        Actief = true,
                    };

                    // Gebruiker aanmaken
                    var result = await userManager.CreateAsync(defaultUser, "F00d.B4r");
                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
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
