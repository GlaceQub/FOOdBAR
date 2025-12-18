using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Restaurant;
using Restaurant.Models;
using Xunit;
using RestaurantTesting;


namespace RestaurantE2ETesting
{
    public class BestellingE2ETests : IAsyncLifetime
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private readonly CustomWebApplicationFactory _factory;
        private string _baseAddress;

        public BestellingE2ETests()
        {
            _factory = new CustomWebApplicationFactory();
            _baseAddress = string.Empty;
        }

        public async Task InitializeAsync()
        {
            // Trigger host creation and get base address
            var client = _factory.CreateClient();
            _baseAddress = client.BaseAddress!.ToString().TrimEnd('/');

            // Seed users/roles inside the test host
            using (var scope = _factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<CustomUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await IdentitySeeder.SeedDefaultUsersAndRolesAsync(userManager, roleManager);
            }

            // Initialize Playwright and launch browser
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        }

        public async Task DisposeAsync()
        {
            await _browser.DisposeAsync();
            _playwright.Dispose();
            _factory.Dispose();
        }

        [Fact, Trait("Category", "E2E")]
        public async Task Complete_Bestelling_Workflow_Works()
        {
            var context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                BaseURL = _baseAddress
            });

            var page = await context.NewPageAsync();

            await LoginAsync(page, "admin@foodbar.be", "Test123!");
            int reservatieId = await MakeReservationAsync(page);

            await LogoutAsync(page);
            await LoginAsync(page, "zaal@foodbar.be", "Test123!");
            await AssignTableAsync(page, reservatieId);

            await LogoutAsync(page);
            await LoginAsync(page, "klant@foodbar.be", "Test123!");
            await PlaceOrderAsync(page, reservatieId);

            await page.WaitForURLAsync("**/Bestelling/Bevestiging/*");
            var content = await page.ContentAsync();
            Assert.Contains("Bedankt voor uw bestelling", content);
        }

        private async Task LoginAsync(IPage page, string email, string password)
        {
            await page.GotoAsync("/Account/Login");
            await page.FillAsync("input[name='Email']", email);
            await page.FillAsync("input[name='Password']", password);
            await page.ClickAsync("button[type='submit']");
            await page.WaitForSelectorAsync("nav, .navbar, .dashboard, .btn-logout", new() { Timeout = 5000 });
        }

        private async Task LogoutAsync(IPage page)
        {
            var logoutButton = await page.QuerySelectorAsync("form[action='/Account/Logout'] button[type='submit'], a[href='/Account/Logout']");
            if (logoutButton != null)
            {
                await logoutButton.ClickAsync();
                await page.WaitForSelectorAsync("input[name='Email']", new() { Timeout = 5000 });
            }
        }

        private async Task<int> MakeReservationAsync(IPage page)
        {
            await page.GotoAsync("/Reservation/Create");
            await page.FillAsync("input[name='AantalPersonen']", "2");
            await page.FillAsync("input[name='Datum']", System.DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            await page.SelectOptionAsync("select[name='TijdSlotId']", new[] { "1" });
            await page.ClickAsync("button[type='submit']");

            var confirmation = await page.WaitForSelectorAsync(".reservation-confirmation, .alert-success, .card", new() { Timeout = 5000 });
            var content = await confirmation.InnerTextAsync();

            var reservatieId = 1;
            var match = System.Text.RegularExpressions.Regex.Match(content, @"nummer (\d+)");
            if (match.Success)
                reservatieId = int.Parse(match.Groups[1].Value);
            else
            {
                var url = page.Url;
                var urlMatch = System.Text.RegularExpressions.Regex.Match(url, @"Reservation/Details/(\d+)");
                if (urlMatch.Success)
                    reservatieId = int.Parse(urlMatch.Groups[1].Value);
            }
            return reservatieId;
        }

        private async Task AssignTableAsync(IPage page, int reservatieId)
        {
            await page.GotoAsync("/Reservation/Toewijzen");
            var cardSelector = $"form input[name='reservatieId'][value='{reservatieId}']";
            await page.WaitForSelectorAsync(cardSelector, new() { Timeout = 5000 });

            var selectSelector = $"form input[name='reservatieId'][value='{reservatieId}'] ~ select[name='tafelId']";
            await page.SelectOptionAsync(selectSelector, new[] { "" });

            var formSelector = $"form input[name='reservatieId'][value='{reservatieId}']";
            var form = await page.QuerySelectorAsync(formSelector);
            if (form != null)
            {
                var submitBtn = await form.EvaluateHandleAsync("form => form.querySelector('button[type=submit]')");
                await submitBtn.AsElement().ClickAsync();
                await page.WaitForSelectorAsync(".alert-success", new() { Timeout = 5000 });
            }
        }

        private async Task PlaceOrderAsync(IPage page, int reservatieId)
        {
            await page.GotoAsync("/Account/Dashboard");
            var bestelSelector = $"a[href='/Bestelling/Create/{reservatieId}'], a[data-reservatie-id='{reservatieId}']";
            await page.ClickAsync(bestelSelector);

            await page.WaitForSelectorAsync("form[action*='Bestelling/Create']", new() { Timeout = 5000 });

            await page.ClickAsync("button.btn-success.btn-sm", new() { Timeout = 5000 });
            await page.WaitForSelectorAsync("#addToCartModal.show");
            await page.FillAsync("#modalAantal", "1");
            await page.ClickAsync("#modalAddBtn");
            await page.WaitForSelectorAsync("#addToCartModal", new() { State = WaitForSelectorState.Hidden });
            await page.WaitForSelectorAsync("#cartTableBody tr:not(:first-child)", new() { Timeout = 5000 });

            await page.ClickAsync("button.btn-custom-primary[type='submit']");
        }
    }

    public static class IdentitySeeder
    {
        public static async Task SeedDefaultUsersAndRolesAsync(UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Define roles and users
            var roles = new[] { "zaalverantwoordelijke", "klant" };
            var users = new[]
            {
                new { Email = "zaal@foodbar.be", Role = "zaalverantwoordelijke" },
                new { Email = "klant@foodbar.be", Role = "klant" }
            };
            const string defaultPassword = "Test123!";

            // Ensure roles exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Ensure users exist and are assigned to roles
            foreach (var userInfo in users)
            {
                var user = await userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    user = new CustomUser { UserName = userInfo.Email, Email = userInfo.Email, EmailConfirmed = true };
                    await userManager.CreateAsync(user, defaultPassword);
                }
                if (!await userManager.IsInRoleAsync(user, userInfo.Role))
                {
                    await userManager.AddToRoleAsync(user, userInfo.Role);
                }
            }
        }
    }
}