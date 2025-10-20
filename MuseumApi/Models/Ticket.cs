using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuseumApi.Models;

public class Ticket
{
    [Key]
    public Guid TicketId { get; set; }

    [Required]
    public DateTime TicketDate { get; set; }

    [Required]
    public string TicketType { get; set; } = string.Empty; // "General" or "SpecialEvent"

    public Guid? EventId { get; set; }

    [ForeignKey(nameof(EventId))]
    public SpecialEvent? Event { get; set; }
}