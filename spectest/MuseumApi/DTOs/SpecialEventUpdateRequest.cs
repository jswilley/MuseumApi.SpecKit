namespace MuseumApi.DTOs;

/// <summary>
/// Request payload for updating a special event (admin operation)
/// Fields are optional - only provided values are updated.
/// </summary>
/// <param name="EventName">New event name (optional)</param>
/// <param name="EventDescription">New description (optional)</param>
/// <param name="Price">New ticket price (optional, must be >= 0 if provided)</param>
/// <param name="ReplaceDates">Optional full replacement set of dates; if provided replaces all existing dates</param>
public record SpecialEventUpdateRequest(
    string? EventName = null,
    string? EventDescription = null,
    decimal? Price = null,
    DateOnly[]? ReplaceDates = null
);
