using MuseumApi.DTOs;

namespace MuseumApi.Services;

/// <summary>
/// Service interface for ticket purchase operations
/// Provides business logic layer between endpoints and data access
/// </summary>
public interface ITicketPurchaseService
{
    /// <summary>
    /// Purchase general admission tickets
    /// </summary>
    /// <param name="visitDate">Date of visit</param>
    /// <param name="quantity">Number of tickets</param>
    /// <returns>Purchase confirmation DTO</returns>
    Task<TicketPurchaseResponse> PurchaseGeneralAdmissionAsync(DateOnly visitDate, int quantity);

    /// <summary>
    /// Purchase special event tickets
    /// </summary>
    /// <param name="eventId">Event ID</param>
    /// <param name="visitDate">Date of visit</param>
    /// <param name="quantity">Number of tickets</param>
    /// <returns>Purchase confirmation DTO</returns>
    Task<TicketPurchaseResponse> PurchaseEventTicketAsync(Guid eventId, DateOnly visitDate, int quantity);

    /// <summary>
    /// Validate that the museum is open on a specific date
    /// </summary>
    /// <param name="date">Date to validate</param>
    /// <returns>True if museum is open</returns>
    Task<bool> ValidateMuseumOpenAsync(DateOnly date);

    /// <summary>
    /// Validate that a special event is scheduled on a specific date
    /// </summary>
    /// <param name="eventId">Event ID</param>
    /// <param name="date">Date to validate</param>
    /// <returns>True if event is scheduled on that date</returns>
    Task<bool> ValidateEventDateAsync(Guid eventId, DateOnly date);
}
