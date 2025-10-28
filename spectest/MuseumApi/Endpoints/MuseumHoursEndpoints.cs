using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using MuseumApi.Services;

namespace MuseumApi.Endpoints;

public static class MuseumHoursEndpoints
{
    public static void MapMuseumHoursEndpoints(this WebApplication app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();

        var group = app.MapGroup("/v{version:apiVersion}/museumhours")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Museum Hours")
            .WithOpenApi();

        // GET /v1/museumhours - Get all museum hours or filter by date/date range
        group.MapGet("/", async (
            [FromServices] IMuseumHoursService service,
            [FromQuery] DateOnly? date,
            [FromQuery] DateOnly? startDate,
            [FromQuery] DateOnly? endDate) =>
        {
            // Single date filter
            if (date.HasValue)
            {
                var singleHours = await service.GetHoursByDateAsync(date.Value);
                return singleHours != null
                    ? Results.Ok(new[] { singleHours })
                    : Results.Ok(Array.Empty<object>());
            }

            // Date range filter
            if (startDate.HasValue || endDate.HasValue)
            {
                var start = startDate ?? DateOnly.MinValue;
                var end = endDate ?? DateOnly.MaxValue;
                var rangeHours = await service.GetHoursByDateRangeAsync(start, end);
                return Results.Ok(rangeHours);
            }

            // All hours
            var allHours = await service.GetAllHoursAsync();
            return Results.Ok(allHours);
        })
        .WithName("GetMuseumHours")
        .WithSummary("Get museum operating hours")
        .WithDescription("Retrieve museum hours for a specific date, date range, or all available dates")
        .MapToApiVersion(1, 0);
    }
}
