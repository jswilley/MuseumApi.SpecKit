namespace MuseumApi.DTOs;

/// <summary>
/// Request payload for adding a single date to an existing special event
/// </summary>
/// <param name="Date">Date to add</param>
public record SpecialEventDateAddRequest(DateOnly Date);
