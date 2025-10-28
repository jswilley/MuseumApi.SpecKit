namespace MuseumApi.DTOs;

public record SpecialEventDto(
    Guid EventId,
    string EventName,
    string EventDescription,
    decimal Price,
    DateOnly[] EventDates
);
