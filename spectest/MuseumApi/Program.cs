using Microsoft.EntityFrameworkCore;
using MuseumApi.Data;
using Serilog;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddDbContext<MuseumContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Museum API",
        Version = "v1",
        Description = "API for querying museum hours, special events, and purchasing tickets"
    });
});

// Application services
builder.Services.AddScoped<MuseumApi.Services.IMuseumHoursService, MuseumApi.Services.MuseumHoursService>();
builder.Services.AddScoped<MuseumApi.Services.ISpecialEventsService, MuseumApi.Services.SpecialEventsService>();
builder.Services.AddScoped<MuseumApi.Services.ITicketPurchaseService, MuseumApi.Services.TicketPurchaseService>();

// API Versioning (Minimal APIs)
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = false; // keep minimal until needed
})
// Add versioned API explorer for Swagger grouping if needed later
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Simple authentication/authorization for admin endpoints (dev/test only)
builder.Services.AddAuthentication("TestScheme")
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, MuseumApi.Auth.TestAuthHandler>("TestScheme", options => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.AddAuthenticationSchemes("TestScheme");
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("role", "admin");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map API endpoints
MuseumApi.Endpoints.MuseumHoursEndpoints.MapMuseumHoursEndpoints(app);
MuseumApi.Endpoints.SpecialEventsEndpoints.MapSpecialEventsEndpoints(app);
MuseumApi.Endpoints.TicketPurchaseEndpoints.MapTicketPurchaseEndpoints(app);
MuseumApi.Endpoints.AdminSpecialEventsEndpoints.MapAdminSpecialEventsEndpoints(app);

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MuseumContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
