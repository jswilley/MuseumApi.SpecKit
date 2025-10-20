using System.ComponentModel.DataAnnotations;

namespace MuseumApi.Models;

public class MuseumDailyHours
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan TimeOpen { get; set; }

    [Required]
    public TimeSpan TimeClosed { get; set; }
}