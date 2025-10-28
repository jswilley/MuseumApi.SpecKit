using System.ComponentModel.DataAnnotations;

namespace MuseumApi.Models;

public class TicketPurchase
{
    [Key]
    public Guid PurchaseId { get; set; } = Guid.NewGuid();
    
    [Required]
    public DateOnly VisitDate { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalCost { get; set; }
    
    public Guid? EventId { get; set; }
    
    [Required]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public SpecialEvent? SpecialEvent { get; set; }
}
