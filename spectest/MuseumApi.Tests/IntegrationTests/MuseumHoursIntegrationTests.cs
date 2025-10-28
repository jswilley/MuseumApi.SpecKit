using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MuseumApi.Data;
using MuseumApi.DTOs;
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
    public async Task GetMuseumHours_ForToday_ReturnsSuccess()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.Today);
        await SeedTestData(today);

        // Act
        var response = await _client.GetAsync($"/v1/museumhours?date={today:yyyy-MM-dd}");

        // Assert
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<List<MuseumHoursDto>>();
        Assert.NotNull(hours);
        Assert.Single(hours);
        Assert.Equal(today, hours[0].Date);
    }

    [Fact]
    public async Task GetMuseumHours_ForDateRange_ReturnsMultiple()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddDays(3);
        await SeedTestData(startDate, endDate);

        // Act
        var response = await _client.GetAsync($"/v1/museumhours?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

        // Assert
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<List<MuseumHoursDto>>();
        Assert.NotNull(hours);
        Assert.Equal(4, hours.Count); // 4 days inclusive
    }

    [Fact]
    public async Task GetMuseumHours_ForClosedDate_ReturnsEmpty()
    {
        // Arrange
        var closedDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

        // Act
        var response = await _client.GetAsync($"/v1/museumhours?date={closedDate:yyyy-MM-dd}");

        // Assert
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<List<MuseumHoursDto>>();
        Assert.NotNull(hours);
        Assert.Empty(hours); // Museum is closed (no entry for this date)
    }

    [Fact]
    public async Task GetMuseumHours_AllHours_ReturnsSuccess()
    {
        // Arrange
        await SeedTestData(DateOnly.FromDateTime(DateTime.Today));

        // Act
        var response = await _client.GetAsync("/v1/museumhours");

        // Assert
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<List<MuseumHoursDto>>();
        Assert.NotNull(hours);
        Assert.NotEmpty(hours);
    }

    private async Task SeedTestData(DateOnly startDate, DateOnly? endDate = null)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MuseumContext>();
        
        var datesToSeed = new List<DateOnly> { startDate };
        if (endDate.HasValue)
        {
            for (var date = startDate.AddDays(1); date <= endDate.Value; date = date.AddDays(1))
            {
                datesToSeed.Add(date);
            }
        }

        foreach (var date in datesToSeed)
        {
            if (!context.MuseumDailyHours.Any(h => h.Date == date))
            {
                context.MuseumDailyHours.Add(new MuseumDailyHours
                {
                    Date = date,
                    TimeOpen = new TimeOnly(9, 0),
                    TimeClosed = new TimeOnly(17, 0)
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
