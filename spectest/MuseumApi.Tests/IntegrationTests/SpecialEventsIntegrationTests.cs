using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MuseumApi.Data;
using MuseumApi.DTOs;
using MuseumApi.Models;
using Xunit;

namespace MuseumApi.Tests.IntegrationTests;

public class SpecialEventsIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public SpecialEventsIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSpecialEvents_ReturnsAllEvents()
    {
        // Arrange
        await SeedTestEvents();

        // Act
        var response = await _client.GetAsync("/v1/specialevents");

        // Assert
        response.EnsureSuccessStatusCode();
        var events = await response.Content.ReadFromJsonAsync<List<SpecialEventDto>>();
        Assert.NotNull(events);
        Assert.NotEmpty(events);
    }

    [Fact]
    public async Task GetSpecialEventById_ReturnsEvent()
    {
        // Arrange
        var eventId = await SeedTestEvents();

        // Act
        var response = await _client.GetAsync($"/v1/specialevents/{eventId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var eventDto = await response.Content.ReadFromJsonAsync<SpecialEventDto>();
        Assert.NotNull(eventDto);
        Assert.Equal(eventId, eventDto.EventId);
        Assert.NotEmpty(eventDto.EventDates);
    }

    [Fact]
    public async Task GetSpecialEventById_NotFound_Returns404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/v1/specialevents/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSpecialEvents_FilterByDate_ReturnsMatchingEvents()
    {
        // Arrange
        var targetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
        await SeedTestEvents(targetDate);

        // Act
        var response = await _client.GetAsync($"/v1/specialevents?date={targetDate:yyyy-MM-dd}");

        // Assert
        response.EnsureSuccessStatusCode();
        var events = await response.Content.ReadFromJsonAsync<List<SpecialEventDto>>();
        Assert.NotNull(events);
        Assert.All(events, e => Assert.Contains(e.EventDates, d => d == targetDate));
    }

    [Fact]
    public async Task GetSpecialEvents_FilterByDateRange_ReturnsMatchingEvents()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(7);
        await SeedTestEvents(startDate);

        // Act
        var response = await _client.GetAsync($"/v1/specialevents?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

        // Assert
        response.EnsureSuccessStatusCode();
        var events = await response.Content.ReadFromJsonAsync<List<SpecialEventDto>>();
        Assert.NotNull(events);
        Assert.NotEmpty(events);
    }

    private async Task<Guid> SeedTestEvents(DateOnly? specificDate = null)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MuseumContext>();
        
        var eventId = Guid.NewGuid();
        var testDate = specificDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(10));

        // Check if event already exists
        if (!context.SpecialEvents.Any(e => e.EventId == eventId))
        {
            var specialEvent = new SpecialEvent
            {
                EventId = eventId,
                EventName = "Test Special Event",
                EventDescription = "A test event for integration testing",
                Price = 25.00m
            };

            context.SpecialEvents.Add(specialEvent);
            
            // Add event dates
            context.SpecialEventDates.Add(new SpecialEventDate
            {
                EventId = eventId,
                Date = testDate
            });
            
            context.SpecialEventDates.Add(new SpecialEventDate
            {
                EventId = eventId,
                Date = testDate.AddDays(1)
            });

            await context.SaveChangesAsync();
        }

        return eventId;
    }
}
