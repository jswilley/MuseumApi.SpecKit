namespace MuseumApi.DTOs;

public record MuseumHoursDto(
    DateOnly Date,
    TimeOnly TimeOpen,
    TimeOnly TimeClosed
);
