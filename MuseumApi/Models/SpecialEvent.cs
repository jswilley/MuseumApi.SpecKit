using System.ComponentModel.DataAnnotations;

namespace MuseumApi.Models;

public class SpecialEvent
{
    [Key]
    public Guid EventId { get; set; }

    [Required]
    [MaxLength(150)]
    public string EventName { get; set; } = string.Empty;

    [Required]
    [MaxLength(750)]
    public string EventDescription { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public ICollection<SpecialEventDate> EventDates { get; set; } = new List<SpecialEventDate>();
}