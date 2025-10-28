using System.ComponentModel.DataAnnotations;

namespace MuseumApi.Models;

public class SpecialEvent
{
    [Key]
    public Guid EventId { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public string EventName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string EventDescription { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    // Navigation property
    public ICollection<SpecialEventDate> EventDates { get; set; } = new List<SpecialEventDate>();
}
