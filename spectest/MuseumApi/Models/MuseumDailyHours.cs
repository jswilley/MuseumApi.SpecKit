using System.ComponentModel.DataAnnotations;

namespace MuseumApi.Models;

public class MuseumDailyHours
{
    [Key]
    public DateOnly Date { get; set; }
    
    [Required]
    public TimeOnly TimeOpen { get; set; }
    
    [Required]
    public TimeOnly TimeClosed { get; set; }
}
