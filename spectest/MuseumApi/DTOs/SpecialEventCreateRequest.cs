namespace MuseumApi.DTOs;

/// <summary>
/// Request payload for creating a special event (admin operation)
/// </summary>
/// <param name="EventName">Display name of the event</param>
/// <param name="EventDescription">Detailed description</param>
/// <param name="Price">Ticket price (must be >= 0)</param>
/// <param name="InitialDates">Optional initial scheduled dates</param>
public record SpecialEventCreateRequest(
    string EventName,
    string EventDescription,
    decimal Price,
    DateOnly[]? InitialDates = null
);
