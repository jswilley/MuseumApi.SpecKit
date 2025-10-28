using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MuseumApi.Data;
using MuseumApi.Models;
using Xunit;

namespace MuseumApi.Tests.Unit;

public class MuseumContextTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public MuseumContextTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    private MuseumContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MuseumContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new MuseumContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task MuseumDailyHours_UniqueDateConstraint_PreventsDuplicates()
    {
        // Arrange
        using var context = CreateContext();
        var date = DateOnly.FromDateTime(DateTime.Today);

        var hours1 = new MuseumDailyHours
        {
            Date = date,
            TimeOpen = new TimeOnly(9, 0),
            TimeClosed = new TimeOnly(17, 0)
        };

        // Act
        context.MuseumDailyHours.Add(hours1);
        await context.SaveChangesAsync();

        // Detach the entity to avoid tracking conflicts
        context.Entry(hours1).State = EntityState.Detached;

        var hours2 = new MuseumDailyHours
        {
            Date = date, // Same date
            TimeOpen = new TimeOnly(10, 0),
            TimeClosed = new TimeOnly(18, 0)
        };

        context.MuseumDailyHours.Add(hours2);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await context.SaveChangesAsync());
    }

    [Fact]
    public async Task MuseumDailyHours_CanStoreAndRetrieve()
    {
        // Arrange
        using var context = CreateContext();
        var date = DateOnly.FromDateTime(DateTime.Today);
        var hours = new MuseumDailyHours
        {
            Date = date,
            TimeOpen = new TimeOnly(9, 0),
            TimeClosed = new TimeOnly(17, 0)
        };

        // Act
        context.MuseumDailyHours.Add(hours);
        await context.SaveChangesAsync();

        var retrieved = await context.MuseumDailyHours
            .FirstOrDefaultAsync(h => h.Date == date);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(date, retrieved.Date);
        Assert.Equal(new TimeOnly(9, 0), retrieved.TimeOpen);
        Assert.Equal(new TimeOnly(17, 0), retrieved.TimeClosed);
    }

    [Fact]
    public async Task SpecialEventDate_CompositeKeyConstraint_PreventsDuplicates()
    {
        // Arrange
        using var context = CreateContext();
        var eventId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today);
        
        var specialEvent = new SpecialEvent
        {
            EventId = eventId,
            EventName = "Test Event",
            EventDescription = "Test Description",
            Price = 15.00m
        };

        var eventDate1 = new SpecialEventDate
        {
            EventId = eventId,
            Date = date
        };

        // Act
        context.SpecialEvents.Add(specialEvent);
        context.SpecialEventDates.Add(eventDate1);
        await context.SaveChangesAsync();

        // Detach to avoid tracking
        context.Entry(eventDate1).State = EntityState.Detached;

        var eventDate2 = new SpecialEventDate
        {
            EventId = eventId,
            Date = date // Duplicate composite key
        };

        context.SpecialEventDates.Add(eventDate2);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await context.SaveChangesAsync());
    }
}
