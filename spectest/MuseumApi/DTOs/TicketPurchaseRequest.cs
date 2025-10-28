namespace MuseumApi.DTOs;

/// <summary>
/// Request DTO for ticket purchase
/// </summary>
/// <param name="VisitDate">Date of museum visit</param>
/// <param name="Quantity">Number of tickets to purchase</param>
/// <param name="EventId">Optional special event ID (null for general admission)</param>
public record TicketPurchaseRequest(
    DateOnly VisitDate,
    int Quantity,
    Guid? EventId = null
);
