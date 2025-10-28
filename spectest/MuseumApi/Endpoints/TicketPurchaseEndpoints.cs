using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using MuseumApi.DTOs;
using MuseumApi.Services;

namespace MuseumApi.Endpoints;

public static class TicketPurchaseEndpoints
{
    public static void MapTicketPurchaseEndpoints(this WebApplication app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();

        var group = app.MapGroup("/v{version:apiVersion}/tickets")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Ticket Purchase")
            .WithOpenApi();

        // POST /v1/tickets/purchase - Purchase general admission or event ticket
        group.MapPost("/purchase", async (
            [FromServices] ITicketPurchaseService service,
            [FromBody] TicketPurchaseRequest request) =>
        {
            // Basic validation
            if (request.Quantity <= 0)
            {
                return Results.BadRequest(new { message = "Quantity must be greater than zero" });
            }

            try
            {
                TicketPurchaseResponse response;

                // Handle general admission (no EventId) vs event ticket purchase
                if (!request.EventId.HasValue)
                {
                    // General admission purchase
                    var isOpen = await service.ValidateMuseumOpenAsync(request.VisitDate);
                    if (!isOpen)
                    {
                        return Results.BadRequest(new 
                        { 
                            message = $"Museum is closed on {request.VisitDate:yyyy-MM-dd}" 
                        });
                    }

                    response = await service.PurchaseGeneralAdmissionAsync(
                        request.VisitDate,
                        request.Quantity);
                }
                else
                {
                    // Event ticket purchase
                    var isEventScheduled = await service.ValidateEventDateAsync(
                        request.EventId.Value,
                        request.VisitDate);
                    
                    if (!isEventScheduled)
                    {
                        return Results.BadRequest(new 
                        { 
                            message = $"Event is not scheduled on {request.VisitDate:yyyy-MM-dd}" 
                        });
                    }

                    response = await service.PurchaseEventTicketAsync(
                        request.EventId.Value,
                        request.VisitDate,
                        request.Quantity);
                }

                return Results.Created($"/v1/tickets/purchase/{response.PurchaseId}", response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: 500,
                    title: "An error occurred while processing the ticket purchase");
            }
        })
        .WithName("PurchaseTicket")
        .WithSummary("Purchase museum tickets")
        .WithDescription("Purchase general admission tickets or special event tickets. " +
                        "For general admission, omit EventId. For event tickets, include EventId.")
        .Produces<TicketPurchaseResponse>(StatusCodes.Status201Created)
        .Produces<object>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
        .MapToApiVersion(1, 0);
    }
}
