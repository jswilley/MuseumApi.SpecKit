using Microsoft.EntityFrameworkCore;
using MuseumApi.Data;
using MuseumApi.DTOs;
using MuseumApi.Models;

namespace MuseumApi.Services;

/// <summary>
/// Service implementation for ticket purchase operations
/// </summary>
public class TicketPurchaseService : ITicketPurchaseService
{
    private readonly MuseumContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TicketPurchaseService> _logger;

    public TicketPurchaseService(MuseumContext context, IConfiguration configuration, ILogger<TicketPurchaseService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TicketPurchaseResponse> PurchaseGeneralAdmissionAsync(DateOnly visitDate, int quantity)
    {
        _logger.LogInformation("Processing general admission purchase for {VisitDate}, quantity: {Quantity}", visitDate, quantity);

        try
        {
            // Validate quantity
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
            }

            // Validate museum is open
            if (!await ValidateMuseumOpenAsync(visitDate))
            {
                throw new InvalidOperationException($"Museum is closed on {visitDate}");
            }

            // Calculate cost
            var generalAdmissionPrice = _configuration.GetValue<decimal>("GeneralAdmissionPrice", 10.00m);
            var totalCost = generalAdmissionPrice * quantity;

            // Create purchase record
            var purchase = new TicketPurchase
            {
                PurchaseId = Guid.NewGuid(),
                VisitDate = visitDate,
                Quantity = quantity,
                TotalCost = totalCost,
                EventId = null,
                PurchaseDate = DateTime.UtcNow
            };

            _context.TicketPurchases.Add(purchase);
            await _context.SaveChangesAsync();

            _logger.LogInformation("General admission purchase completed: {PurchaseId}", purchase.PurchaseId);

            return new TicketPurchaseResponse(
                purchase.PurchaseId,
                purchase.VisitDate,
                purchase.Quantity,
                purchase.TotalCost,
                null,
                null,
                purchase.PurchaseDate
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing general admission purchase for {VisitDate}", visitDate);
            throw;
        }
    }

    public async Task<TicketPurchaseResponse> PurchaseEventTicketAsync(Guid eventId, DateOnly visitDate, int quantity)
    {
        _logger.LogInformation("Processing event ticket purchase for event {EventId}, date {VisitDate}, quantity: {Quantity}", eventId, visitDate, quantity);

        try
        {
            // Validate quantity
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be positive", nameof(quantity));
            }

            // Get event details
            var specialEvent = await _context.SpecialEvents
                .Include(e => e.EventDates)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (specialEvent == null)
            {
                throw new InvalidOperationException($"Event not found: {eventId}");
            }

            // Validate event is scheduled on this date
            if (!await ValidateEventDateAsync(eventId, visitDate))
            {
                throw new InvalidOperationException($"Event {specialEvent.EventName} is not scheduled on {visitDate}");
            }

            // Calculate cost
            var totalCost = specialEvent.Price * quantity;

            // Create purchase record
            var purchase = new TicketPurchase
            {
                PurchaseId = Guid.NewGuid(),
                VisitDate = visitDate,
                Quantity = quantity,
                TotalCost = totalCost,
                EventId = eventId,
                PurchaseDate = DateTime.UtcNow
            };

            _context.TicketPurchases.Add(purchase);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Event ticket purchase completed: {PurchaseId}", purchase.PurchaseId);

            return new TicketPurchaseResponse(
                purchase.PurchaseId,
                purchase.VisitDate,
                purchase.Quantity,
                purchase.TotalCost,
                eventId,
                specialEvent.EventName,
                purchase.PurchaseDate
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event ticket purchase for event {EventId}, date {VisitDate}", eventId, visitDate);
            throw;
        }
    }

    public async Task<bool> ValidateMuseumOpenAsync(DateOnly date)
    {
        _logger.LogDebug("Validating museum is open on {Date}", date);

        try
        {
            return await _context.MuseumDailyHours.AnyAsync(h => h.Date == date);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating museum hours for {Date}", date);
            throw;
        }
    }

    public async Task<bool> ValidateEventDateAsync(Guid eventId, DateOnly date)
    {
        _logger.LogDebug("Validating event {EventId} is scheduled on {Date}", eventId, date);

        try
        {
            return await _context.SpecialEventDates
                .AnyAsync(ed => ed.EventId == eventId && ed.Date == date);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating event date for event {EventId}, date {Date}", eventId, date);
            throw;
        }
    }
}
