using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MuseumApi.Data;
using MuseumApi.DTOs;
using MuseumApi.Models;
using Xunit;

namespace MuseumApi.Tests.IntegrationTests;

public class TicketPurchaseIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public TicketPurchaseIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PurchaseGeneralAdmission_ValidDate_Returns201Created()
    {
        // Arrange
        var visitDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        await SeedMuseumHours(visitDate);

        var request = new TicketPurchaseRequest(
            VisitDate: visitDate,
            Quantity: 2,
            EventId: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/tickets/purchase", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var purchaseResponse = await response.Content.ReadFromJsonAsync<TicketPurchaseResponse>();
        
        Assert.NotNull(purchaseResponse);
        Assert.NotEqual(Guid.Empty, purchaseResponse.PurchaseId);
        Assert.Equal(visitDate, purchaseResponse.VisitDate);
        Assert.Equal(2, purchaseResponse.Quantity);
        Assert.Equal(20.00m, purchaseResponse.TotalCost); // 2 * $10 general admission
        Assert.Null(purchaseResponse.EventId);
        Assert.Null(purchaseResponse.EventName);
    }

    [Fact]
    public async Task PurchaseGeneralAdmission_MuseumClosed_Returns400BadRequest()
    {
        // Arrange
        var closedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(100)); // Use a date far in the future to avoid conflicts
        // Don't seed museum hours for this date - museum is closed

        var request = new TicketPurchaseRequest(
            VisitDate: closedDate,
            Quantity: 1,
            EventId: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/tickets/purchase", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PurchaseEventTicket_ValidEvent_Returns201Created()
    {
        // Arrange
        var eventDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
        var eventId = await SeedSpecialEvent(eventDate, "Concert Night", 35.00m);

        var request = new TicketPurchaseRequest(
            VisitDate: eventDate,
            Quantity: 3,
            EventId: eventId
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/tickets/purchase", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var purchaseResponse = await response.Content.ReadFromJsonAsync<TicketPurchaseResponse>();
        
        Assert.NotNull(purchaseResponse);
        Assert.NotEqual(Guid.Empty, purchaseResponse.PurchaseId);
        Assert.Equal(eventDate, purchaseResponse.VisitDate);
        Assert.Equal(3, purchaseResponse.Quantity);
        Assert.Equal(105.00m, purchaseResponse.TotalCost); // 3 * $35 event price
        Assert.Equal(eventId, purchaseResponse.EventId);
        Assert.Equal("Concert Night", purchaseResponse.EventName);
    }

    [Fact]
    public async Task PurchaseEventTicket_EventNotScheduledOnDate_Returns400BadRequest()
    {
        // Arrange
        var scheduledDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
        var wrongDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
        var eventId = await SeedSpecialEvent(scheduledDate, "Concert Night", 35.00m);

        var request = new TicketPurchaseRequest(
            VisitDate: wrongDate, // Event not scheduled on this date
            Quantity: 1,
            EventId: eventId
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/tickets/purchase", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PurchaseTicket_InvalidQuantity_Returns400BadRequest()
    {
        // Arrange
        var visitDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        await SeedMuseumHours(visitDate);

        var request = new TicketPurchaseRequest(
            VisitDate: visitDate,
            Quantity: 0, // Invalid quantity
            EventId: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/tickets/purchase", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PurchaseEventTicket_NonExistentEvent_Returns400BadRequest()
    {
        // Arrange
        var visitDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var nonExistentEventId = Guid.NewGuid();

        var request = new TicketPurchaseRequest(
            VisitDate: visitDate,
            Quantity: 1,
            EventId: nonExistentEventId
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/tickets/purchase", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task SeedMuseumHours(DateOnly date)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MuseumContext>();
        
        // Check if hours already exist for this date
        if (!context.MuseumDailyHours.Any(h => h.Date == date))
        {
            var hours = new MuseumDailyHours
            {
                Date = date,
                TimeOpen = new TimeOnly(9, 0),
                TimeClosed = new TimeOnly(17, 0)
            };

            context.MuseumDailyHours.Add(hours);
            await context.SaveChangesAsync();
        }
    }

    private async Task<Guid> SeedSpecialEvent(DateOnly eventDate, string eventName, decimal price)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MuseumContext>();
        
        var eventId = Guid.NewGuid();

        // Check if event already exists
        if (!context.SpecialEvents.Any(e => e.EventId == eventId))
        {
            var specialEvent = new SpecialEvent
            {
                EventId = eventId,
                EventName = eventName,
                EventDescription = $"Test event: {eventName}",
                Price = price
            };

            context.SpecialEvents.Add(specialEvent);
            
            // Add event date
            context.SpecialEventDates.Add(new SpecialEventDate
            {
                EventId = eventId,
                Date = eventDate
            });

            await context.SaveChangesAsync();
        }

        return eventId;
    }
}
