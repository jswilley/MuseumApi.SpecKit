using MuseumApi.DTOs;

namespace MuseumApi.Services;

/// <summary>
/// Service interface for museum hours operations
/// Provides business logic layer between endpoints and data access
/// </summary>
public interface IMuseumHoursService
{
    /// <summary>
    /// Get museum hours for a specific date
    /// </summary>
    /// <param name="date">The date to query</param>
    /// <returns>Museum hours DTO or null if closed/not found</returns>
    Task<MuseumHoursDto?> GetHoursByDateAsync(DateOnly date);

    /// <summary>
    /// Get museum hours for a date range
    /// </summary>
    /// <param name="startDate">Start of date range</param>
    /// <param name="endDate">End of date range</param>
    /// <returns>Collection of museum hours DTOs</returns>
    Task<IEnumerable<MuseumHoursDto>> GetHoursByDateRangeAsync(DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Get all museum hours ordered by date
    /// </summary>
    /// <returns>Collection of all museum hours DTOs</returns>
    Task<IEnumerable<MuseumHoursDto>> GetAllHoursAsync();
}
