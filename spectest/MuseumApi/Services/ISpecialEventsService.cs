using MuseumApi.DTOs;

namespace MuseumApi.Services;

/// <summary>
/// Service interface for special events operations
/// Provides business logic layer between endpoints and data access
/// </summary>
public interface ISpecialEventsService
{
    /// <summary>
    /// Get all special events with their scheduled dates
    /// </summary>
    /// <returns>Collection of all special event DTOs</returns>
    Task<IEnumerable<SpecialEventDto>> GetAllEventsAsync();

    /// <summary>
    /// Get a specific special event by ID
    /// </summary>
    /// <param name="eventId">The event's unique identifier</param>
    /// <returns>Special event DTO or null if not found</returns>
    Task<SpecialEventDto?> GetEventByIdAsync(Guid eventId);

    /// <summary>
    /// Get special events occurring on a specific date
    /// </summary>
    /// <param name="date">The date to filter by</param>
    /// <returns>Collection of special event DTOs</returns>
    Task<IEnumerable<SpecialEventDto>> GetEventsByDateAsync(DateOnly date);

    /// <summary>
    /// Get special events occurring within a date range
    /// </summary>
    /// <param name="startDate">Start of date range</param>
    /// <param name="endDate">End of date range</param>
    /// <returns>Collection of special event DTOs</returns>
    Task<IEnumerable<SpecialEventDto>> GetEventsByDateRangeAsync(DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Create a new special event (admin operation)
    /// </summary>
    /// <param name="eventName">Event name</param>
    /// <param name="eventDescription">Event description</param>
    /// <param name="price">Ticket price</param>
    /// <param name="eventDates">Scheduled dates</param>
    /// <returns>Created special event DTO</returns>
    Task<SpecialEventDto> CreateEventAsync(string eventName, string eventDescription, decimal price, DateOnly[] eventDates);

    /// <summary>
    /// Update an existing special event (admin operation)
    /// </summary>
    /// <param name="eventId">Event ID to update</param>
    /// <param name="eventName">Optional new event name</param>
    /// <param name="eventDescription">Optional new event description</param>
    /// <param name="price">Optional new price</param>
    /// <param name="eventDates">Optional new scheduled dates</param>
    /// <returns>Updated special event DTO or null if not found</returns>
    Task<SpecialEventDto?> UpdateEventAsync(Guid eventId, string? eventName = null, string? eventDescription = null, decimal? price = null, DateOnly[]? eventDates = null);

    /// <summary>
    /// Delete a special event (admin operation)
    /// </summary>
    /// <param name="eventId">Event ID to delete</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteEventAsync(Guid eventId);

    /// <summary>
    /// Add a single date to an existing special event
    /// </summary>
    /// <param name="eventId">Event ID</param>
    /// <param name="date">Date to add</param>
    /// <returns>Updated special event DTO or null if not found</returns>
    Task<SpecialEventDto?> AddEventDateAsync(Guid eventId, DateOnly date);

    /// <summary>
    /// Remove a single date from an existing special event
    /// </summary>
    /// <param name="eventId">Event ID</param>
    /// <param name="date">Date to remove</param>
    /// <returns>Updated special event DTO or null if not found</returns>
    Task<SpecialEventDto?> RemoveEventDateAsync(Guid eventId, DateOnly date);
}
