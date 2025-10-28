using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using MuseumApi.Services;

namespace MuseumApi.Endpoints;

public static class SpecialEventsEndpoints
{
    public static void MapSpecialEventsEndpoints(this WebApplication app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();

        var group = app.MapGroup("/v{version:apiVersion}/specialevents")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Special Events")
            .WithOpenApi();

        // GET /v1/specialevents - Get all special events or filter by date/date range
        group.MapGet("/", async (
            [FromServices] ISpecialEventsService service,
            [FromQuery] DateOnly? date,
            [FromQuery] DateOnly? startDate,
            [FromQuery] DateOnly? endDate) =>
        {
            // Single date filter
            if (date.HasValue)
            {
                var dateEvents = await service.GetEventsByDateAsync(date.Value);
                return Results.Ok(dateEvents);
            }

            // Date range filter
            if (startDate.HasValue || endDate.HasValue)
            {
                var start = startDate ?? DateOnly.MinValue;
                var end = endDate ?? DateOnly.MaxValue;
                var rangeEvents = await service.GetEventsByDateRangeAsync(start, end);
                return Results.Ok(rangeEvents);
            }

            // All events
            var allEvents = await service.GetAllEventsAsync();
            return Results.Ok(allEvents);
        })
        .WithName("GetSpecialEvents")
        .WithSummary("Get all special events")
        .WithDescription("Retrieve special events, optionally filtered by date or date range")
        .MapToApiVersion(1, 0);

        // GET /v1/specialevents/{id} - Get a specific special event by ID
        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] ISpecialEventsService service) =>
        {
            var specialEvent = await service.GetEventByIdAsync(id);

            if (specialEvent == null)
            {
                return Results.NotFound(new { message = $"Special event with ID {id} not found" });
            }

            return Results.Ok(specialEvent);
        })
        .WithName("GetSpecialEventById")
        .WithSummary("Get a special event by ID")
        .WithDescription("Retrieve details of a specific special event including all scheduled dates")
        .MapToApiVersion(1, 0);
    }
}

