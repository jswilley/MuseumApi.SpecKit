using System.ComponentModel.DataAnnotations;

namespace MuseumApi.Models;

public class SpecialEventDate
{
    [Required]
    public Guid EventId { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }
    
    // Navigation property
    public SpecialEvent SpecialEvent { get; set; } = null!;
}
