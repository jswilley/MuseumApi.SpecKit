using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MuseumApi.Models;
using Xunit;

namespace MuseumApi.Tests.IntegrationTests;

public class MuseumHoursIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public MuseumHoursIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMuseumHours_ReturnsSuccessAndCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/hours");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetMuseumHours_WithValidDate_ReturnsHours()
    {
        // Arrange
        var hours = new MuseumDailyHours
        {
            Date = DateTime.Today,
            TimeOpen = new TimeSpan(9, 0, 0),
            TimeClosed = new TimeSpan(17, 0, 0)
        };

        await _client.PostAsJsonAsync("/api/hours", hours);

        // Act
        var response = await _client.GetAsync($"/api/hours/{DateTime.Today:yyyy-MM-dd}");
        var content = await response.Content.ReadFromJsonAsync<MuseumDailyHours>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(hours.Date.Date, content.Date.Date);
        Assert.Equal(hours.TimeOpen, content.TimeOpen);
        Assert.Equal(hours.TimeClosed, content.TimeClosed);
    }

    [Fact]
    public async Task GetMuseumHours_WithInvalidDate_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/hours/{DateTime.Today.AddYears(1):yyyy-MM-dd}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateMuseumHours_WithValidData_ReturnsCreated()
    {
        // Arrange
        var hours = new MuseumDailyHours
        {
            Date = DateTime.Today.AddDays(1),
            TimeOpen = new TimeSpan(9, 0, 0),
            TimeClosed = new TimeSpan(17, 0, 0)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/hours", hours);
        var content = await response.Content.ReadFromJsonAsync<MuseumDailyHours>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(hours.Date.Date, content.Date.Date);
        Assert.Equal(hours.TimeOpen, content.TimeOpen);
        Assert.Equal(hours.TimeClosed, content.TimeClosed);
    }
}