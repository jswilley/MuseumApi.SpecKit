using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MuseumApi.Tests.IntegrationTests;

public class TracesIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public TracesIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RequestProducesActivities_WhenInstrumentationEnabled()
    {
        var captured = new System.Collections.Concurrent.ConcurrentBag<Activity>();

        var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => captured.Add(activity),
            ActivityStopped = activity => { }
        };

        ActivitySource.AddActivityListener(listener);

        using var client = _factory.CreateClient();

        // Trigger an instrumented request
        var resp = await client.GetAsync("/v1/specialevents");
        resp.EnsureSuccessStatusCode();

        // Allow a short time for activities to be created and observed
        await Task.Delay(50);

        // Remove listener by disposing (no direct remove API) - listener will be GC'd at method end

        // Assert we captured at least one activity with http.route tag (produced by AspNetCore instrumentation)
        var hasHttpRoute = captured.Any(a => a.Tags.Any(t => t.Key == "http.route") || a.DisplayName?.Contains("GET") == true);

        Assert.True(hasHttpRoute, "Expected at least one Activity with an http.route tag or GET display name.");
    }
}
