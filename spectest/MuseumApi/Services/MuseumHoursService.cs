using Microsoft.EntityFrameworkCore;
using MuseumApi.Data;
using MuseumApi.DTOs;

namespace MuseumApi.Services;

/// <summary>
/// Service implementation for museum hours operations
/// </summary>
public class MuseumHoursService : IMuseumHoursService
{
    private readonly MuseumContext _context;
    private readonly ILogger<MuseumHoursService> _logger;

    public MuseumHoursService(MuseumContext context, ILogger<MuseumHoursService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MuseumHoursDto?> GetHoursByDateAsync(DateOnly date)
    {
        _logger.LogInformation("Getting museum hours for date: {Date}", date);

        try
        {
            var hours = await _context.MuseumDailyHours
                .Where(h => h.Date == date)
                .Select(h => new MuseumHoursDto(h.Date, h.TimeOpen, h.TimeClosed))
                .FirstOrDefaultAsync();

            if (hours == null)
            {
                _logger.LogInformation("No hours found for date: {Date} - museum is closed", date);
            }

            return hours;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting museum hours for date: {Date}", date);
            throw;
        }
    }

    public async Task<IEnumerable<MuseumHoursDto>> GetHoursByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        _logger.LogInformation("Getting museum hours for date range: {StartDate} to {EndDate}", startDate, endDate);

        try
        {
            var hours = await _context.MuseumDailyHours
                .Where(h => h.Date >= startDate && h.Date <= endDate)
                .OrderBy(h => h.Date)
                .Select(h => new MuseumHoursDto(h.Date, h.TimeOpen, h.TimeClosed))
                .ToListAsync();

            _logger.LogInformation("Found {Count} days with museum hours in range", hours.Count);
            return hours;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting museum hours for date range: {StartDate} to {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<IEnumerable<MuseumHoursDto>> GetAllHoursAsync()
    {
        _logger.LogInformation("Getting all museum hours");

        try
        {
            var hours = await _context.MuseumDailyHours
                .OrderBy(h => h.Date)
                .Select(h => new MuseumHoursDto(h.Date, h.TimeOpen, h.TimeClosed))
                .ToListAsync();

            _logger.LogInformation("Found {Count} days with museum hours", hours.Count);
            return hours;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all museum hours");
            throw;
        }
    }
}
