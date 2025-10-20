using Microsoft.EntityFrameworkCore;
using MuseumApi.Models;

namespace MuseumApi.Data;

public class MuseumContext : DbContext
{
    public MuseumContext(DbContextOptions<MuseumContext> options)
        : base(options)
    {
    }

    public DbSet<SpecialEvent> SpecialEvents { get; set; }
    public DbSet<SpecialEventDate> SpecialEventDates { get; set; }
    public DbSet<MuseumDailyHours> MuseumDailyHours { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure SpecialEventDate unique constraint
        modelBuilder.Entity<SpecialEventDate>()
            .HasIndex(e => new { e.EventId, e.Date })
            .IsUnique();

        // Configure MuseumDailyHours unique date
        modelBuilder.Entity<MuseumDailyHours>()
            .HasIndex(m => m.Date)
            .IsUnique();

        // Configure Ticket types
        modelBuilder.Entity<Ticket>()
            .Property(t => t.TicketType)
            .HasConversion<string>();

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var event1Id = new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35");
        var event2Id = new Guid("da2fd609-d754-4feb-8acd-c4f9ff13ba96");

        // Seed SpecialEvents
        modelBuilder.Entity<SpecialEvent>().HasData(
            new SpecialEvent { 
                EventId = event1Id,
                EventName = "Dinosaur Exhibition",
                EventDescription = "Experience the prehistoric era",
                Price = 25.00m
            },
            new SpecialEvent { 
                EventId = event2Id,
                EventName = "Space Adventure",
                EventDescription = "Journey through the cosmos",
                Price = 30.00m
            }
        );

        // Seed SpecialEventDates
        modelBuilder.Entity<SpecialEventDate>().HasData(
            new SpecialEventDate { Id = 1, EventId = event1Id, Date = new DateTime(2025, 10, 20) },
            new SpecialEventDate { Id = 2, EventId = event1Id, Date = new DateTime(2025, 10, 21) },
            new SpecialEventDate { Id = 3, EventId = event2Id, Date = new DateTime(2025, 10, 25) }
        );

        // Seed MuseumDailyHours
        modelBuilder.Entity<MuseumDailyHours>().HasData(
            new MuseumDailyHours { 
                Id = 1, 
                Date = new DateTime(2025, 10, 20),
                TimeOpen = new TimeSpan(9, 0, 0),
                TimeClosed = new TimeSpan(17, 0, 0)
            },
            new MuseumDailyHours {
                Id = 2,
                Date = new DateTime(2025, 10, 21),
                TimeOpen = new TimeSpan(9, 0, 0),
                TimeClosed = new TimeSpan(17, 0, 0)
            },
            new MuseumDailyHours {
                Id = 3,
                Date = new DateTime(2025, 10, 25),
                TimeOpen = new TimeSpan(9, 0, 0),
                TimeClosed = new TimeSpan(20, 0, 0)
            }
        );
    }
}