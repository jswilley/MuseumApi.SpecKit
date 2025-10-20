using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuseumApi.Models;

public class SpecialEventDate
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid EventId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [ForeignKey(nameof(EventId))]
    public SpecialEvent? Event { get; set; }
}