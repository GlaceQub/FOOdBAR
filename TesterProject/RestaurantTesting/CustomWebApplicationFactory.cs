using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant;
using Restaurant.Data;

namespace RestaurantTesting
{
    internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private DbConnection? _connection;

        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Force environment to "Test" so your Program.cs Test-branch is used
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration (SQL Server)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<RestaurantContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Create a single in-memory SQLite connection for the life of the factory
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                services.AddDbContext<RestaurantContext>(options =>
                {
                    options.UseSqlite(_connection);
                });
            });

            var host = base.CreateHost(builder);

            // Apply latest migrations to the in-memory DB
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RestaurantContext>();

                // Optional: drop to start clean each test run
                //context.Database.EnsureDeleted();

                context.Database.Migrate();
            }

            return host;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection?.Dispose();
            _connection = null;
        }
    }
}