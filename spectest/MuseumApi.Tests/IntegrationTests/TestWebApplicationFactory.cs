using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MuseumApi.Data;

namespace MuseumApi.Tests.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            services.RemoveAll(typeof(DbContextOptions<MuseumContext>));
            services.RemoveAll(typeof(MuseumContext));

            // Create and open a connection for the in-memory database
            // This connection must be kept open for the lifetime of the test
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Add DbContext using the in-memory database
            services.AddDbContext<MuseumContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<MuseumContext>();

            // Ensure the database is created
            db.Database.EnsureCreated();

            // Seed test data
            try
            {
                DbInitializer.Initialize(db);
            }
            catch
            {
                // Ignore seeding errors in tests - tests will seed their own data
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
        base.Dispose(disposing);
    }
}
