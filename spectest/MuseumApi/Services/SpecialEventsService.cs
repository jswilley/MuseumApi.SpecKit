using Microsoft.EntityFrameworkCore;
using MuseumApi.Data;
using MuseumApi.DTOs;
using MuseumApi.Models;

namespace MuseumApi.Services;

/// <summary>
/// Service implementation for special events operations
/// </summary>
public class SpecialEventsService : ISpecialEventsService
{
    private readonly MuseumContext _context;
    private readonly ILogger<SpecialEventsService> _logger;

    public SpecialEventsService(MuseumContext context, ILogger<SpecialEventsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<SpecialEventDto>> GetAllEventsAsync()
    {
        _logger.LogInformation("Getting all special events");

        try
        {
            var events = await _context.SpecialEvents
                .Include(e => e.EventDates)
                .OrderBy(e => e.EventName)
                .Select(e => new SpecialEventDto(
                    e.EventId,
                    e.EventName,
                    e.EventDescription,
                    e.Price,
                    e.EventDates.Select(ed => ed.Date).OrderBy(d => d).ToArray()
                ))
                .ToListAsync();

            _logger.LogInformation("Found {Count} special events", events.Count);
            return events;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all special events");
            throw;
        }
    }

    public async Task<SpecialEventDto?> GetEventByIdAsync(Guid eventId)
    {
        _logger.LogInformation("Getting special event by ID: {EventId}", eventId);

        try
        {
            var specialEvent = await _context.SpecialEvents
                .Include(e => e.EventDates)
                .Where(e => e.EventId == eventId)
                .Select(e => new SpecialEventDto(
                    e.EventId,
                    e.EventName,
                    e.EventDescription,
                    e.Price,
                    e.EventDates.Select(ed => ed.Date).OrderBy(d => d).ToArray()
                ))
                .FirstOrDefaultAsync();

            if (specialEvent == null)
            {
                _logger.LogInformation("Special event not found: {EventId}", eventId);
            }

            return specialEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting special event by ID: {EventId}", eventId);
            throw;
        }
    }

    public async Task<IEnumerable<SpecialEventDto>> GetEventsByDateAsync(DateOnly date)
    {
        _logger.LogInformation("Getting special events for date: {Date}", date);

        try
        {
            var events = await _context.SpecialEvents
                .Include(e => e.EventDates)
                .Where(e => e.EventDates.Any(ed => ed.Date == date))
                .OrderBy(e => e.EventName)
                .Select(e => new SpecialEventDto(
                    e.EventId,
                    e.EventName,
                    e.EventDescription,
                    e.Price,
                    e.EventDates.Select(ed => ed.Date).OrderBy(d => d).ToArray()
                ))
                .ToListAsync();

            _logger.LogInformation("Found {Count} special events for date: {Date}", events.Count, date);
            return events;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting special events for date: {Date}", date);
            throw;
        }
    }

    public async Task<IEnumerable<SpecialEventDto>> GetEventsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        _logger.LogInformation("Getting special events for date range: {StartDate} to {EndDate}", startDate, endDate);

        try
        {
            var events = await _context.SpecialEvents
                .Include(e => e.EventDates)
                .Where(e => e.EventDates.Any(ed => ed.Date >= startDate && ed.Date <= endDate))
                .OrderBy(e => e.EventName)
                .Select(e => new SpecialEventDto(
                    e.EventId,
                    e.EventName,
                    e.EventDescription,
                    e.Price,
                    e.EventDates.Select(ed => ed.Date).OrderBy(d => d).ToArray()
                ))
                .ToListAsync();

            _logger.LogInformation("Found {Count} special events in date range", events.Count);
            return events;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting special events for date range: {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<SpecialEventDto> CreateEventAsync(string eventName, string eventDescription, decimal price, DateOnly[] eventDates)
    {
        _logger.LogInformation("Creating special event: {EventName} with {DateCount} dates", eventName, eventDates.Length);

        try
        {
            var specialEvent = new SpecialEvent
            {
                EventId = Guid.NewGuid(),
                EventName = eventName,
                EventDescription = eventDescription,
                Price = price
            };

            _context.SpecialEvents.Add(specialEvent);
            await _context.SaveChangesAsync();

            // Add event dates
            foreach (var date in eventDates)
            {
                _context.SpecialEventDates.Add(new SpecialEventDate
                {
                    EventId = specialEvent.EventId,
                    Date = date
                });
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Created special event: {EventId}", specialEvent.EventId);

            return new SpecialEventDto(
                specialEvent.EventId,
                specialEvent.EventName,
                specialEvent.EventDescription,
                specialEvent.Price,
                eventDates.OrderBy(d => d).ToArray()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating special event: {EventName}", eventName);
            throw;
        }
    }

    public async Task<SpecialEventDto?> UpdateEventAsync(Guid eventId, string? eventName = null, string? eventDescription = null, decimal? price = null, DateOnly[]? eventDates = null)
    {
        _logger.LogInformation("Updating special event: {EventId}", eventId);

        try
        {
            var specialEvent = await _context.SpecialEvents
                .Include(e => e.EventDates)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (specialEvent == null)
            {
                _logger.LogInformation("Special event not found for update: {EventId}", eventId);
                return null;
            }

            // Update fields if provided
            if (eventName != null) specialEvent.EventName = eventName;
            if (eventDescription != null) specialEvent.EventDescription = eventDescription;
            if (price.HasValue) specialEvent.Price = price.Value;

            // Update event dates if provided
            if (eventDates != null)
            {
                // Remove old dates
                _context.SpecialEventDates.RemoveRange(specialEvent.EventDates);

                // Add new dates
                foreach (var date in eventDates)
                {
                    _context.SpecialEventDates.Add(new SpecialEventDate
                    {
                        EventId = eventId,
                        Date = date
                    });
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated special event: {EventId}", eventId);

            // Reload to get updated dates
            var updatedEvent = await GetEventByIdAsync(eventId);
            return updatedEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating special event: {EventId}", eventId);
            throw;
        }
    }

    public async Task<bool> DeleteEventAsync(Guid eventId)
    {
        _logger.LogInformation("Deleting special event: {EventId}", eventId);

        try
        {
            var specialEvent = await _context.SpecialEvents
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (specialEvent == null)
            {
                _logger.LogInformation("Special event not found for deletion: {EventId}", eventId);
                return false;
            }

            _context.SpecialEvents.Remove(specialEvent);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted special event: {EventId}", eventId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting special event: {EventId}", eventId);
            throw;
        }
    }

    /// <summary>
    /// Add a single date to an existing special event
    /// </summary>
    /// <param name="eventId">Event identifier</param>
    /// <param name="date">Date to add</param>
    /// <returns>Updated DTO or null if event not found</returns>
    public async Task<SpecialEventDto?> AddEventDateAsync(Guid eventId, DateOnly date)
    {
        _logger.LogInformation("Adding date {Date} to special event {EventId}", date, eventId);

        try
        {
            var specialEvent = await _context.SpecialEvents
                .Include(e => e.EventDates)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (specialEvent == null)
            {
                _logger.LogInformation("Special event not found for add date: {EventId}", eventId);
                return null;
            }

            // Validation: prevent duplicate date
            if (specialEvent.EventDates.Any(ed => ed.Date == date))
            {
                throw new InvalidOperationException($"Date {date:yyyy-MM-dd} already exists for event {eventId}");
            }

            _context.SpecialEventDates.Add(new SpecialEventDate
            {
                EventId = eventId,
                Date = date
            });

            await _context.SaveChangesAsync();

            // Return updated projection
            return await GetEventByIdAsync(eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding date {Date} to special event {EventId}", date, eventId);
            throw;
        }
    }

    /// <summary>
    /// Remove a single date from an existing special event
    /// </summary>
    /// <param name="eventId">Event identifier</param>
    /// <param name="date">Date to remove</param>
    /// <returns>Updated DTO or null if event not found. Throws if date not associated.</returns>
    public async Task<SpecialEventDto?> RemoveEventDateAsync(Guid eventId, DateOnly date)
    {
        _logger.LogInformation("Removing date {Date} from special event {EventId}", date, eventId);

        try
        {
            var specialEvent = await _context.SpecialEvents
                .Include(e => e.EventDates)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (specialEvent == null)
            {
                _logger.LogInformation("Special event not found for remove date: {EventId}", eventId);
                return null;
            }

            var existingDate = specialEvent.EventDates.FirstOrDefault(ed => ed.Date == date);
            if (existingDate == null)
            {
                throw new InvalidOperationException($"Date {date:yyyy-MM-dd} does not exist for event {eventId}");
            }

            _context.SpecialEventDates.Remove(existingDate);
            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing date {Date} from special event {EventId}", date, eventId);
            throw;
        }
    }
}
