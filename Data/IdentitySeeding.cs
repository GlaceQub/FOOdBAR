using System.Data.Common;

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
                if (userManager.FindByNameAsync("Admin").Result == null)
                {
                    // Gebruiker voorzien
                    var defaultUser = new CustomUser
                    {
                        UserName = "admin@foodbar.be",
                        Email = "admin@foodbar.be",
                        EmailConfirmed = true
                    };

                    // Gebruiker aanmaken
                    await userManager.CreateAsync(defaultUser, "F00dB4r");

                }
            }
            catch (DbException ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
