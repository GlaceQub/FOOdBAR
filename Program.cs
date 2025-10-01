using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Get current machine and use appropriate connectionString
string machineName = Environment.MachineName;
string connectionString;

switch (machineName)
{
    case "DESKTOP-DAVID":
        connectionString = builder.Configuration.GetConnectionString("DavidHomeDBConnection");
        break;
    default:
        connectionString = builder.Configuration.GetConnectionString("LocalDBConnection");
        break;
}
builder.Services.AddDbContext<RestaurantContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<CustomUser, IdentityRole>()
    .AddEntityFrameworkStores<RestaurantContext>();


// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login"; // Pad naar de inlogpagina
    options.AccessDeniedPath = "/Home/Index"; // Pad naar de homepagina
});

// Adding authentication with cookie scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Voorkom redirects naar een loginpagina wanneer de gebruiker niet is geauthenticeerd
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401; // Unauthorized
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Niet geauthenticeerd. Log in om toegang te krijgen.\"}");
        };

        // Voorkom redirects naar een access denied pagina wanneer de gebruiker geen toegang heeft
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403; // Forbidden
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Toegang geweigerd. U heeft niet de juiste rechten.\"}");
        };
    });

builder.Services.AddTransient<IdentitySeeding>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Seed initial user
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeding>();
    UserManager<CustomUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<CustomUser>>();
    await seeder.IdentitySeedingAsync(userManager);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
