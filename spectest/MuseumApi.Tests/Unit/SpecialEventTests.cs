using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MuseumApi.Data;
using MuseumApi.Models;
using Xunit;

namespace MuseumApi.Tests.Unit;

public class SpecialEventTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public SpecialEventTests()
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
    public async Task SpecialEvent_PriceValidation_RejectsNegativePrice()
    {
        // Arrange
        using var context = CreateContext();
        var specialEvent = new SpecialEvent
        {
            EventId = Guid.NewGuid(),
            EventName = "Test Event",
            EventDescription = "Test Description",
            Price = -10.00m // Negative price
        };

        // Act & Assert
        context.SpecialEvents.Add(specialEvent);
        
        // EF Core validation will catch this on SaveChanges
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(specialEvent);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            specialEvent, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(SpecialEvent.Price)));
    }

    [Fact]
    public async Task SpecialEvent_PriceValidation_AcceptsPositivePrice()
    {
        // Arrange
        using var context = CreateContext();
        var specialEvent = new SpecialEvent
        {
            EventId = Guid.NewGuid(),
            EventName = "Test Event",
            EventDescription = "Test Description",
            Price = 25.00m
        };

        // Act
        context.SpecialEvents.Add(specialEvent);
        await context.SaveChangesAsync();

        // Assert
        var retrieved = await context.SpecialEvents
            .FirstOrDefaultAsync(e => e.EventId == specialEvent.EventId);
        
        Assert.NotNull(retrieved);
        Assert.Equal(25.00m, retrieved.Price);
    }

    [Fact]
    public async Task SpecialEventDate_CanStoreAndRetrieve()
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
            Price = 20.00m
        };

        var eventDate = new SpecialEventDate
        {
            EventId = eventId,
            Date = date
        };

        // Act
        context.SpecialEvents.Add(specialEvent);
        context.SpecialEventDates.Add(eventDate);
        await context.SaveChangesAsync();

        // Assert
        var retrieved = await context.SpecialEventDates
            .Include(ed => ed.SpecialEvent)
            .FirstOrDefaultAsync(ed => ed.EventId == eventId && ed.Date == date);
        
        Assert.NotNull(retrieved);
        Assert.Equal(eventId, retrieved.EventId);
        Assert.Equal(date, retrieved.Date);
        Assert.NotNull(retrieved.SpecialEvent);
        Assert.Equal("Test Event", retrieved.SpecialEvent.EventName);
    }

    [Fact]
    public async Task SpecialEventDate_UniqueDateConstraint_PreventsDuplicates()
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
            Price = 20.00m
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

        // Detach to avoid tracking conflicts
        context.Entry(eventDate1).State = EntityState.Detached;

        var eventDate2 = new SpecialEventDate
        {
            EventId = eventId,
            Date = date // Same event and date
        };

        context.SpecialEventDates.Add(eventDate2);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await context.SaveChangesAsync());
    }
}
