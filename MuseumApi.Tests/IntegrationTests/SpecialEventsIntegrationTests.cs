using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
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
    public async Task GetSpecialEvents_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/events");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetSpecialEvent_WithValidId_ReturnsEvent()
    {
        // Arrange
        var newEvent = new SpecialEvent
        {
            EventId = Guid.NewGuid(),
            EventName = "Test Event",
            EventDescription = "Test Description",
            Price = 10.00m
        };

        var postResponse = await _client.PostAsJsonAsync("/api/events", newEvent);
        postResponse.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync($"/api/events/{newEvent.EventId}");
        var content = await response.Content.ReadFromJsonAsync<SpecialEvent>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(newEvent.EventName, content.EventName);
    }

    [Fact]
    public async Task GetSpecialEvent_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/events/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateSpecialEvent_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newEvent = new SpecialEvent
        {
            EventId = Guid.NewGuid(),
            EventName = "Test Event",
            EventDescription = "Test Description",
            Price = 10.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/events", newEvent);
        var content = await response.Content.ReadFromJsonAsync<SpecialEvent>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(newEvent.EventName, content.EventName);
    }
}