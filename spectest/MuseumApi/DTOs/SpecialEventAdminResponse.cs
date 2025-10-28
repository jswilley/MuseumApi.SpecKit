namespace MuseumApi.DTOs;

/// <summary>
/// Unified admin response DTO for special events (includes dates)
/// </summary>
/// <param name="EventId">Unique identifier</param>
/// <param name="EventName">Event name</param>
/// <param name="EventDescription">Event description</param>
/// <param name="Price">Current ticket price</param>
/// <param name="Dates">Scheduled dates (sorted ascending)</param>
public record SpecialEventAdminResponse(
    Guid EventId,
    string EventName,
    string EventDescription,
    decimal Price,
    DateOnly[] Dates
);
