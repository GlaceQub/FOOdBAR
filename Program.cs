using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Token overpompen in MySettings
Token.Settings = new Settings
{
    Secret = (builder.Configuration["Settings:Secret"] ?? "d4f.5E6a7-8b9c-0d1e-WfGl1m-4h5i6j7k8l9m").ToCharArray(),
    ValidIssuer = builder.Configuration["Settings:ValidIssuer"] ?? "https://localhost:7055",
    ValidAudience = builder.Configuration["Settings:ValidAudience"] ?? "https://localhost:7055"
};

builder.Configuration.GetRequiredSection(nameof(Settings)).Bind(Token.Settings);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ReservatieRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Get current machine and use appropriate connectionString
string machineName = Environment.MachineName;
string connectionString;

switch (machineName)
{
    case "DESKTOP-DAVID":
        connectionString = builder.Configuration.GetConnectionString("DavidHomeDBConnection");
        break;
    case "DESKTOP-K5018T2":
        connectionString = builder.Configuration.GetConnectionString("LanderLaptopDBConnection");
        break;
    default:
        connectionString = builder.Configuration.GetConnectionString("LocalDBConnection");
        break;
}
builder.Services.AddDbContext<RestaurantContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<CustomUser, IdentityRole>()
    .AddEntityFrameworkStores<RestaurantContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Pad naar de inlogpagina
    options.AccessDeniedPath = "/Home/Index"; // Pad naar de homepagina
});

// Adding authentication with cookie scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/Index";
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Niet geauthenticeerd. Log in om toegang te krijgen.\"}");
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Toegang geweigerd. U heeft niet de juiste rechten.\"}");
        };
    })
    .AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateLifetime = false,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Token.Settings.Secret))
        };
    });

builder.Services.AddTransient<IdentitySeeding>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(swagger => {
    //This is to generate the Default UI of Swagger Documentation    
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "AuthTest Web API",
        Description = "Authentication and Authorization in ASP.NET with JWT and Swagger"
    });
    // To Enable authorization using Swagger (JWT)    
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        //Type = SecuritySchemeType.ApiKey,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter ZONDER Bearer. Dus gewoon je TOKEN: \r\n\r\nExample: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Add swagger when in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


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
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/UnderConstruction");
    }
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed initial user
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeding>();
    UserManager<CustomUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<CustomUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();


    await seeder.IdentitySeedingAsync(userManager);
}

app.Run();
