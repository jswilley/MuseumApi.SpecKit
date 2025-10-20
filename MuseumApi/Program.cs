using Microsoft.EntityFrameworkCore;
using Serilog;
using MuseumApi.Data;
using MuseumApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) => 
    config.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Add SQLite database
builder.Services.AddDbContext<MuseumContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Special Events endpoints
app.MapGet("/api/events", async (MuseumContext db) =>
    await db.SpecialEvents.Include(e => e.EventDates)
        .ToListAsync())
.WithName("GetSpecialEvents")
.WithOpenApi();

app.MapGet("/api/events/{id}", async (Guid id, MuseumContext db) =>
    await db.SpecialEvents.Include(e => e.EventDates)
        .FirstOrDefaultAsync(e => e.EventId == id)
        is SpecialEvent @event
        ? Results.Ok(@event)
        : Results.NotFound())
.WithName("GetSpecialEventById")
.WithOpenApi();

// Event Dates endpoints
app.MapGet("/api/eventdates", async (MuseumContext db) =>
    await db.SpecialEventDates
        .Include(d => d.Event)
        .ToListAsync())
.WithName("GetEventDates")
.WithOpenApi();

app.MapGet("/api/eventdates/{id}", async (int id, MuseumContext db) =>
    await db.SpecialEventDates
        .Include(d => d.Event)
        .FirstOrDefaultAsync(d => d.Id == id)
        is SpecialEventDate date
        ? Results.Ok(date)
        : Results.NotFound())
.WithName("GetEventDateById")
.WithOpenApi();

// Museum Hours endpoints
app.MapGet("/api/hours", async (MuseumContext db) =>
    await db.MuseumDailyHours.ToListAsync())
.WithName("GetMuseumHours")
.WithOpenApi();

app.MapGet("/api/hours/{date}", async (DateTime date, MuseumContext db) =>
    await db.MuseumDailyHours
        .FirstOrDefaultAsync(h => h.Date.Date == date.Date)
        is MuseumDailyHours hours
        ? Results.Ok(hours)
        : Results.NotFound())
.WithName("GetMuseumHoursByDate")
.WithOpenApi();

// Add POST endpoints for creating new records
app.MapPost("/api/events", async (SpecialEvent @event, MuseumContext db) =>
{
    db.SpecialEvents.Add(@event);
    await db.SaveChangesAsync();
    return Results.Created($"/api/events/{@event.EventId}", @event);
})
.WithName("CreateSpecialEvent")
.WithOpenApi();

app.MapPost("/api/eventdates", async (SpecialEventDate date, MuseumContext db) =>
{
    db.SpecialEventDates.Add(date);
    await db.SaveChangesAsync();
    return Results.Created($"/api/eventdates/{date.Id}", date);
})
.WithName("CreateEventDate")
.WithOpenApi();

app.MapPost("/api/hours", async (MuseumDailyHours hours, MuseumContext db) =>
{
    db.MuseumDailyHours.Add(hours);
    await db.SaveChangesAsync();
    return Results.Created($"/api/hours/{hours.Id}", hours);
})
.WithName("CreateMuseumHours")
.WithOpenApi();

// Add some basic error handling
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "An error occurred while processing your request" });
        Log.Error(ex, "An unhandled exception occurred while processing the request");
    }
});

app.Run();

public partial class Program { }
