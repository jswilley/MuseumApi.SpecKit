namespace MuseumApi.DTOs;

/// <summary>
/// Response DTO for ticket purchase confirmation
/// </summary>
/// <param name="PurchaseId">Unique purchase identifier</param>
/// <param name="VisitDate">Date of museum visit</param>
/// <param name="Quantity">Number of tickets purchased</param>
/// <param name="TotalCost">Total cost of purchase</param>
/// <param name="EventId">Event ID if this is an event ticket</param>
/// <param name="EventName">Event name if this is an event ticket</param>
/// <param name="PurchaseDate">Date/time of purchase</param>
public record TicketPurchaseResponse(
    Guid PurchaseId,
    DateOnly VisitDate,
    int Quantity,
    decimal TotalCost,
    Guid? EventId,
    string? EventName,
    DateTime PurchaseDate
);
