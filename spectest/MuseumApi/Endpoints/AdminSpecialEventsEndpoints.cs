using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using MuseumApi.DTOs;
using MuseumApi.Services;

namespace MuseumApi.Endpoints;

/// <summary>
/// Admin endpoints for managing special events and their scheduled dates.
/// </summary>
public static class AdminSpecialEventsEndpoints
{
    public static void MapAdminSpecialEventsEndpoints(this WebApplication app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();

        var group = app.MapGroup("/v{version:apiVersion}/admin/specialevents")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Admin Special Events")
            .WithOpenApi()
            .RequireAuthorization("AdminPolicy");

        // CREATE EVENT
        group.MapPost("/", async (
            [FromServices] ISpecialEventsService service,
            [FromBody] SpecialEventCreateRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.EventName))
            {
                return Results.BadRequest(new { message = "EventName is required" });
            }
            if (request.Price < 0)
            {
                return Results.BadRequest(new { message = "Price must be non-negative" });
            }

            try
            {
                var dates = request.InitialDates ?? Array.Empty<DateOnly>();
                var created = await service.CreateEventAsync(
                    request.EventName.Trim(),
                    request.EventDescription.Trim(),
                    request.Price,
                    dates);

                var response = new SpecialEventAdminResponse(
                    created.EventId,
                    created.EventName,
                    created.EventDescription,
                    created.Price,
                    created.EventDates.OrderBy(d => d).ToArray());

                return Results.Created($"/v1/admin/specialevents/{response.EventId}", response);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        })
        .WithName("AdminCreateSpecialEvent")
        .WithSummary("Create a special event")
        .Produces<SpecialEventAdminResponse>(StatusCodes.Status201Created)
        .Produces<object>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
        .MapToApiVersion(1, 0);

        // UPDATE EVENT (partial, optional date replacement)
        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromServices] ISpecialEventsService service,
            [FromBody] SpecialEventUpdateRequest request) =>
        {
            if (request.Price.HasValue && request.Price.Value < 0)
            {
                return Results.BadRequest(new { message = "Price must be non-negative" });
            }

            try
            {
                var updated = await service.UpdateEventAsync(
                    id,
                    request.EventName?.Trim(),
                    request.EventDescription?.Trim(),
                    request.Price,
                    request.ReplaceDates);

                if (updated == null)
                {
                    return Results.NotFound(new { message = $"Special event with ID {id} not found" });
                }

                var response = new SpecialEventAdminResponse(
                    updated.EventId,
                    updated.EventName,
                    updated.EventDescription,
                    updated.Price,
                    updated.EventDates.OrderBy(d => d).ToArray());

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        })
        .WithName("AdminUpdateSpecialEvent")
        .WithSummary("Update a special event")
        .Produces<SpecialEventAdminResponse>(StatusCodes.Status200OK)
        .Produces<object>(StatusCodes.Status400BadRequest)
        .Produces<object>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
        .MapToApiVersion(1, 0);

        // DELETE EVENT
        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] ISpecialEventsService service) =>
        {
            try
            {
                var deleted = await service.DeleteEventAsync(id);
                return deleted
                    ? Results.NoContent()
                    : Results.NotFound(new { message = $"Special event with ID {id} not found" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        })
        .WithName("AdminDeleteSpecialEvent")
        .WithSummary("Delete a special event")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<object>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
        .MapToApiVersion(1, 0);

        // ADD DATE
        group.MapPost("/{id:guid}/dates", async (
            Guid id,
            [FromServices] ISpecialEventsService service,
            [FromBody] SpecialEventDateAddRequest request) =>
        {
            try
            {
                var updated = await service.AddEventDateAsync(id, request.Date);
                if (updated == null)
                {
                    return Results.NotFound(new { message = $"Special event with ID {id} not found" });
                }

                var response = new SpecialEventAdminResponse(
                    updated.EventId,
                    updated.EventName,
                    updated.EventDescription,
                    updated.Price,
                    updated.EventDates.OrderBy(d => d).ToArray());
                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        })
        .WithName("AdminAddSpecialEventDate")
        .WithSummary("Add a date to a special event")
        .Produces<SpecialEventAdminResponse>(StatusCodes.Status200OK)
        .Produces<object>(StatusCodes.Status400BadRequest)
        .Produces<object>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
        .MapToApiVersion(1, 0);

        // REMOVE DATE
        group.MapDelete("/{id:guid}/dates/{date}", async (
            Guid id,
            DateOnly date,
            [FromServices] ISpecialEventsService service) =>
        {
            try
            {
                var updated = await service.RemoveEventDateAsync(id, date);
                if (updated == null)
                {
                    return Results.NotFound(new { message = $"Special event with ID {id} not found" });
                }

                var response = new SpecialEventAdminResponse(
                    updated.EventId,
                    updated.EventName,
                    updated.EventDescription,
                    updated.Price,
                    updated.EventDates.OrderBy(d => d).ToArray());
                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        })
        .WithName("AdminRemoveSpecialEventDate")
        .WithSummary("Remove a date from a special event")
        .Produces<SpecialEventAdminResponse>(StatusCodes.Status200OK)
        .Produces<object>(StatusCodes.Status400BadRequest)
        .Produces<object>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
        .MapToApiVersion(1, 0);
    }
}
