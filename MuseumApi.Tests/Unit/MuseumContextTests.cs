using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MuseumApi.Data;
using MuseumApi.Models;
using Xunit;

namespace MuseumApi.Tests.Unit;

public class MuseumContextTests : IDisposable
{
    private readonly DbContextOptions<MuseumContext> _options;
    private readonly MuseumContext _context;
    private readonly SqliteConnection _connection;

    public MuseumContextTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<MuseumContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new MuseumContext(_options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public void SpecialEventDate_RequiresUniqueEventIdAndDate()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var date = DateTime.Today;

        var specialEvent = new SpecialEvent
        {
            EventId = eventId,
            EventName = "Test Event",
            EventDescription = "Test",
            Price = 10.00m
        };
        _context.SpecialEvents.Add(specialEvent);

        _context.SpecialEventDates.Add(new SpecialEventDate
        {
            EventId = eventId,
            Date = date
        });
        _context.SaveChanges();

        // Act & Assert
        var exception = Assert.Throws<DbUpdateException>(() =>
        {
            _context.SpecialEventDates.Add(new SpecialEventDate
            {
                EventId = eventId,
                Date = date
            });
            _context.SaveChanges();
        });

        Assert.Contains("UNIQUE", exception.InnerException?.Message ?? string.Empty);
    }

    [Fact]
    public void MuseumDailyHours_RequiresUniqueDate()
    {
        // Arrange
        var date = DateTime.Today;

        _context.MuseumDailyHours.Add(new MuseumDailyHours
        {
            Date = date,
            TimeOpen = new TimeSpan(9, 0, 0),
            TimeClosed = new TimeSpan(17, 0, 0)
        });
        _context.SaveChanges();

        // Act & Assert
        var exception = Assert.Throws<DbUpdateException>(() =>
        {
            _context.MuseumDailyHours.Add(new MuseumDailyHours
            {
                Date = date,
                TimeOpen = new TimeSpan(10, 0, 0),
                TimeClosed = new TimeSpan(18, 0, 0)
            });
            _context.SaveChanges();
        });

        Assert.Contains("UNIQUE", exception.InnerException?.Message ?? string.Empty);
    }

    [Fact]
    public void SpecialEvent_CanHaveMultipleEventDates()
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

        _context.SpecialEvents.Add(specialEvent);

        _context.SpecialEventDates.AddRange(
            new SpecialEventDate { EventId = eventId, Date = DateTime.Today },
            new SpecialEventDate { EventId = eventId, Date = DateTime.Today.AddDays(1) }
        );

        // Act
        _context.SaveChanges();

        // Assert
        var eventWithDates = _context.SpecialEvents
            .Include(e => e.EventDates)
            .First(e => e.EventId == eventId);

        Assert.Equal(2, eventWithDates.EventDates.Count);
    }
}