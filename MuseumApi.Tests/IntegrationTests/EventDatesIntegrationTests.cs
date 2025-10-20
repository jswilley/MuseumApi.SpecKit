using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MuseumApi.Models;
using Xunit;

namespace MuseumApi.Tests.IntegrationTests;

public class EventDatesIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public EventDatesIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetEventDates_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/eventdates");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetEventDate_WithValidId_ReturnsEventDate()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var specialEvent = new SpecialEvent
        {
            EventId = eventId,
            EventName = "Test Event",
            EventDescription = "Test Description",
            Price = 10.00m
        };

        await _client.PostAsJsonAsync("/api/events", specialEvent);

        var eventDate = new SpecialEventDate
        {
            EventId = eventId,
            Date = DateTime.Today.AddDays(1)
        };

        var postResponse = await _client.PostAsJsonAsync("/api/eventdates", eventDate);
        var createdEventDate = await postResponse.Content.ReadFromJsonAsync<SpecialEventDate>();

        // Act
        var response = await _client.GetAsync($"/api/eventdates/{createdEventDate!.Id}");
        var content = await response.Content.ReadFromJsonAsync<SpecialEventDate>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(eventDate.Date.Date, content.Date.Date);
    }

    [Fact]
    public async Task GetEventDate_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/eventdates/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateEventDate_WithValidData_ReturnsCreated()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var specialEvent = new SpecialEvent
        {
            EventId = eventId,
            EventName = "Test Event",
            EventDescription = "Test Description",
            Price = 10.00m
        };

        await _client.PostAsJsonAsync("/api/events", specialEvent);

        var eventDate = new SpecialEventDate
        {
            EventId = eventId,
            Date = DateTime.Today.AddDays(1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/eventdates", eventDate);
        var content = await response.Content.ReadFromJsonAsync<SpecialEventDate>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(eventDate.Date.Date, content.Date.Date);
    }
}