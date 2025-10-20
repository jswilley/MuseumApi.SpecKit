using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MuseumApi.Data;

namespace MuseumApi.Tests.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the SQLite database context
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MuseumContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Remove other database-related services
            var dbContextDescriptor = services
                .Where(d => d.ServiceType.Namespace?.Contains("EntityFrameworkCore") ?? false)
                .ToList();

            foreach (var d in dbContextDescriptor)
            {
                services.Remove(d);
            }

            services.AddDbContext<MuseumContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MuseumContext>();
                db.Database.EnsureCreated();
            }
        });
    }
}