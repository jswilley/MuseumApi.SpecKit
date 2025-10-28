using Microsoft.EntityFrameworkCore;
using MuseumApi.Models;

namespace MuseumApi.Data;

public class MuseumContext : DbContext
{
    public MuseumContext(DbContextOptions<MuseumContext> options)
        : base(options)
    {
    }

    // DbSet placeholders - will be populated as models are created
    public DbSet<MuseumDailyHours> MuseumDailyHours { get; set; } = null!;
    public DbSet<SpecialEvent> SpecialEvents { get; set; } = null!;
    public DbSet<SpecialEventDate> SpecialEventDates { get; set; } = null!;
    public DbSet<TicketPurchase> TicketPurchases { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // MuseumDailyHours configuration
        modelBuilder.Entity<MuseumDailyHours>(entity =>
        {
            entity.HasKey(e => e.Date);
            entity.HasIndex(e => e.Date).IsUnique();
        });

        // SpecialEvent configuration
        modelBuilder.Entity<SpecialEvent>(entity =>
        {
            entity.HasKey(e => e.EventId);
            entity.Property(e => e.Price).HasPrecision(10, 2);
        });

        // SpecialEventDate configuration (composite key)
        modelBuilder.Entity<SpecialEventDate>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.Date });
            
            entity.HasOne(e => e.SpecialEvent)
                .WithMany(s => s.EventDates)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TicketPurchase configuration
        modelBuilder.Entity<TicketPurchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId);
            entity.Property(e => e.TotalCost).HasPrecision(10, 2);
            
            entity.HasOne(e => e.SpecialEvent)
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
