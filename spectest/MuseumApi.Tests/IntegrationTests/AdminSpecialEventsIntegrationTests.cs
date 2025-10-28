using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MuseumApi.Data;
using MuseumApi.DTOs;
using MuseumApi.Models;
using Xunit;

namespace MuseumApi.Tests.IntegrationTests;

public class AdminSpecialEventsIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public AdminSpecialEventsIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        // Add test admin header to authenticate admin endpoints via the TestAuthHandler
        _client.DefaultRequestHeaders.Add("X-Test-Admin", "true");
    }

    [Fact]
    public async Task CreateEvent_WithInitialDates_Returns201AndEvent()
    {
        // Arrange
        var dates = new[]
        {
            DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            DateOnly.FromDateTime(DateTime.Today.AddDays(8))
        };
        var request = new SpecialEventCreateRequest(
            EventName: "Admin Created Event",
            EventDescription: "Admin create test",
            Price: 50.0m,
            InitialDates: dates
        );

        // Act
        var response = await _client.PostAsJsonAsync("/v1/admin/specialevents", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<SpecialEventAdminResponse>();
        Assert.NotNull(created);
        Assert.Equal(request.EventName, created.EventName);
        Assert.Equal(2, created.Dates.Length);
    }

    [Fact]
    public async Task AddDate_Duplicate_Returns400()
    {
        // Arrange - create event with a date
        var initialDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
        var createReq = new SpecialEventCreateRequest(
            EventName: "Duplicate Date Event",
            EventDescription: "Testing duplicate date add",
            Price: 25m,
            InitialDates: new[] { initialDate }
        );
        var createResp = await _client.PostAsJsonAsync("/v1/admin/specialevents", createReq);
        var created = await createResp.Content.ReadFromJsonAsync<SpecialEventAdminResponse>();
        Assert.NotNull(created);

        var addDateReq = new SpecialEventDateAddRequest(initialDate);

        // Act - attempt duplicate date add
        var duplicateResp = await _client.PostAsJsonAsync($"/v1/admin/specialevents/{created.EventId}/dates", addDateReq);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResp.StatusCode);
    }

    [Fact]
    public async Task UpdateEvent_PriceChange_ReturnsUpdatedEvent()
    {
        // Arrange - create event
        var createReq = new SpecialEventCreateRequest(
            EventName: "Price Change Event",
            EventDescription: "Initial description",
            Price: 30m,
            InitialDates: null
        );
        var createResp = await _client.PostAsJsonAsync("/v1/admin/specialevents", createReq);
        var created = await createResp.Content.ReadFromJsonAsync<SpecialEventAdminResponse>();
        Assert.NotNull(created);

        var updateReq = new SpecialEventUpdateRequest(
            EventName: null,
            EventDescription: null,
            Price: 45m,
            ReplaceDates: null
        );

        // Act
        var updateResp = await _client.PutAsJsonAsync($"/v1/admin/specialevents/{created.EventId}", updateReq);

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResp.StatusCode);
        var updated = await updateResp.Content.ReadFromJsonAsync<SpecialEventAdminResponse>();
        Assert.NotNull(updated);
        Assert.Equal(45m, updated.Price);
    }

    [Fact]
    public async Task DeleteEvent_RemovesEventAndDates()
    {
        // Arrange - create event with dates
        var dates = new[] { DateOnly.FromDateTime(DateTime.Today.AddDays(3)) };
        var createReq = new SpecialEventCreateRequest(
            EventName: "Delete Event Test",
            EventDescription: "To be deleted",
            Price: 15m,
            InitialDates: dates
        );
        var createResp = await _client.PostAsJsonAsync("/v1/admin/specialevents", createReq);
        var created = await createResp.Content.ReadFromJsonAsync<SpecialEventAdminResponse>();
        Assert.NotNull(created);

        // Act - delete
        var deleteResp = await _client.DeleteAsync($"/v1/admin/specialevents/{created.EventId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);

        // Verify not found afterward
        var getResp = await _client.GetAsync($"/v1/specialevents/{created.EventId}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }

    [Fact]
    public async Task AddAndRemoveDate_Succeeds()
    {
        // Arrange - create event without dates
        var createReq = new SpecialEventCreateRequest(
            EventName: "AddRemove Date Event",
            EventDescription: "Testing add/remove",
            Price: 40m,
            InitialDates: null
        );
        var createResp = await _client.PostAsJsonAsync("/v1/admin/specialevents", createReq);
        var created = await createResp.Content.ReadFromJsonAsync<SpecialEventAdminResponse>();
        Assert.NotNull(created);
        Assert.Empty(created.Dates);

        var newDate = DateOnly.FromDateTime(DateTime.Today.AddDays(11));
        var addReq = new SpecialEventDateAddRequest(newDate);
        var addResp = await _client.PostAsJsonAsync($"/v1/admin/specialevents/{created.EventId}/dates", addReq);
        Assert.Equal(HttpStatusCode.OK, addResp.StatusCode);
        var afterAdd = await addResp.Content.ReadFromJsonAsync<SpecialEventAdminResponse>();
        Assert.Contains(newDate, afterAdd!.Dates);

        // Remove
        var removeResp = await _client.DeleteAsync($"/v1/admin/specialevents/{created.EventId}/dates/{newDate:yyyy-MM-dd}");
        Assert.Equal(HttpStatusCode.OK, removeResp.StatusCode);
        var afterRemove = await removeResp.Content.ReadFromJsonAsync<SpecialEventAdminResponse>();
        Assert.DoesNotContain(newDate, afterRemove!.Dates);
    }

    [Fact]
    public async Task NonExistentEvent_Returns404OnUpdateAndDateOps()
    {
        // Arrange
        var missingId = Guid.NewGuid();
        var updateReq = new SpecialEventUpdateRequest(Price: 99m);

        // Act
        var updateResp = await _client.PutAsJsonAsync($"/v1/admin/specialevents/{missingId}", updateReq);
        var addDateResp = await _client.PostAsJsonAsync($"/v1/admin/specialevents/{missingId}/dates", new SpecialEventDateAddRequest(DateOnly.FromDateTime(DateTime.Today)));        
        var removeDateResp = await _client.DeleteAsync($"/v1/admin/specialevents/{missingId}/dates/{DateTime.Today:yyyy-MM-dd}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, updateResp.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, addDateResp.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, removeDateResp.StatusCode);
    }
}
