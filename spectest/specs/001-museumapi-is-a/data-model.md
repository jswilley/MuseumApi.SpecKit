# Data Model: Museum API

**Feature**: Museum API  
**Date**: October 21, 2025  
**Status**: Complete

## Overview

This document defines the data model for the Museum API, including entities, relationships, constraints, and validation rules derived from functional requirements.

## Entity Relationship Diagram

```
┌─────────────────────┐
│  MuseumDailyHours   │
├─────────────────────┤
│ Date (PK)           │
│ TimeOpen            │
│ TimeClosed          │
└─────────────────────┘

┌─────────────────────┐         ┌──────────────────────┐
│   SpecialEvent      │────┐    │  SpecialEventDate    │
├─────────────────────┤    │    ├──────────────────────┤
│ EventId (PK)        │    └───<│ EventId (FK)         │
│ EventName           │         │ Date                 │
│ EventDescription    │         │ (Composite PK)       │
│ Price               │         └──────────────────────┘
└─────────────────────┘

┌─────────────────────┐
│  TicketPurchase     │
├─────────────────────┤
│ PurchaseId (PK)     │
│ VisitDate           │
│ Quantity            │
│ TotalCost           │
│ EventId (FK, null)  │──┐
│ PurchaseDate        │  │
└─────────────────────┘  │
                         │
                         └──> (Optional reference to SpecialEvent)
```

## Entities

### MuseumDailyHours

Represents the museum's operating schedule for a specific date.

**Purpose**: Satisfies FR-001, FR-002, FR-003 (provide and query museum operating hours)

**Fields**:

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Date | DateTime | PK, NOT NULL, UNIQUE | The calendar date (date-only, no time component) |
| TimeOpen | TimeSpan | NOT NULL | Museum opening time (e.g., 09:00:00) |
| TimeClosed | TimeSpan | NOT NULL | Museum closing time (e.g., 17:00:00) |

**Validation Rules**:
- `Date` must be unique (FR-022) - enforced by primary key
- `TimeOpen` < `TimeClosed` (business rule)
- Both times must be valid time-of-day values (00:00:00 to 23:59:59)

**Relationships**:
- None (standalone entity)

**Indexes**:
- Primary key on `Date` enables fast date-based lookups

**Notes**:
- Closed days represented by absence of entry (simpler than explicit "IsClosed" flag)
- Alternative: Could add `IsClosed` boolean if explicit closed status is needed
- Date stored as date-only (no time component) for query simplicity

---

### SpecialEvent

Represents a museum exhibition, workshop, or special program.

**Purpose**: Satisfies FR-004, FR-006, FR-007 (maintain special events catalog with name, description, pricing)

**Fields**:

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| EventId | Guid | PK, NOT NULL | Unique identifier for the event |
| EventName | string | NOT NULL, MaxLength(200) | Display name of the event |
| EventDescription | string | NOT NULL, MaxLength(2000) | Detailed description of the event |
| Price | decimal(10,2) | NOT NULL, CHECK (Price > 0) | Ticket price for the event (FR-020) |

**Validation Rules**:
- `EventId` must be unique - enforced by primary key
- `EventName` required, max 200 characters
- `EventDescription` required, max 2000 characters
- `Price` must be positive (FR-020) - enforced by CHECK constraint or validation attribute
- All string fields should be trimmed and non-empty

**Relationships**:
- One-to-many with `SpecialEventDate` (one event can occur on multiple dates)
- One-to-many with `TicketPurchase` (one event can have multiple purchases)

**Indexes**:
- Primary key on `EventId`
- Consider index on `EventName` for search functionality

**Notes**:
- Using Guid instead of int identity for distributed system compatibility
- Price stored as decimal to avoid floating-point precision issues
- Description length allows rich detail without exceeding reasonable limits

---

### SpecialEventDate

Associates a special event with specific dates when it occurs.

**Purpose**: Satisfies FR-005, FR-008 (associate events with dates, support date-based filtering)

**Fields**:

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| EventId | Guid | PK, FK to SpecialEvent, NOT NULL | The event being scheduled |
| Date | DateTime | PK, NOT NULL | The date the event occurs (date-only, no time) |

**Validation Rules**:
- Composite primary key on `(EventId, Date)` ensures uniqueness (FR-023)
- `Date` must be a valid calendar date (FR-021)
- `EventId` must reference existing `SpecialEvent` (referential integrity)

**Relationships**:
- Many-to-one with `SpecialEvent` (foreign key on `EventId`)

**Indexes**:
- Composite primary key on `(EventId, Date)`
- Consider index on `Date` alone for date-range queries

**Notes**:
- Junction table enabling many-to-many relationship between events and dates
- Composite key prevents duplicate (event, date) combinations
- Date stored as date-only for consistency with MuseumDailyHours

---

### TicketPurchase

Represents a ticket transaction for museum admission or special event attendance.

**Purpose**: Satisfies FR-009, FR-010, FR-013, FR-014 (process purchases, calculate costs, provide confirmation)

**Fields**:

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| PurchaseId | Guid | PK, NOT NULL | Unique transaction identifier |
| VisitDate | DateTime | NOT NULL | Date of intended visit (date-only) |
| Quantity | int | NOT NULL, CHECK (Quantity > 0) | Number of tickets purchased |
| TotalCost | decimal(10,2) | NOT NULL, CHECK (TotalCost >= 0) | Total purchase amount |
| EventId | Guid | FK to SpecialEvent, NULL | Associated event (null for general admission) |
| PurchaseDate | DateTime | NOT NULL, DEFAULT(GETDATE()) | Transaction timestamp |

**Validation Rules**:
- `PurchaseId` must be unique - enforced by primary key
- `Quantity` must be positive integer
- `TotalCost` must be non-negative decimal
- `VisitDate` must be a future date or today (business rule validation)
- If `EventId` is not null, it must reference existing `SpecialEvent`
- `PurchaseDate` defaults to current timestamp

**Relationships**:
- Many-to-one with `SpecialEvent` (optional foreign key on `EventId`)
  - Null `EventId` indicates general admission purchase
  - Non-null `EventId` indicates special event purchase

**Indexes**:
- Primary key on `PurchaseId`
- Foreign key index on `EventId` for join performance
- Consider index on `PurchaseDate` for reporting queries
- Consider index on `VisitDate` for capacity analysis

**Notes**:
- General admission: `EventId` is NULL, `TotalCost` = general admission price * `Quantity`
- Special event: `EventId` references event, `TotalCost` = event price * `Quantity`
- `TotalCost` denormalized for simplicity (avoids recalculation)
- Consider adding `PurchaserEmail` or `PurchaserName` fields for confirmation

---

## Data Constraints Summary

### Primary Keys
- `MuseumDailyHours`: Date
- `SpecialEvent`: EventId (Guid)
- `SpecialEventDate`: (EventId, Date) composite
- `TicketPurchase`: PurchaseId (Guid)

### Foreign Keys
- `SpecialEventDate.EventId` → `SpecialEvent.EventId` (CASCADE DELETE recommended)
- `TicketPurchase.EventId` → `SpecialEvent.EventId` (SET NULL or RESTRICT on DELETE)

### Unique Constraints
- `MuseumDailyHours.Date` (FR-022)
- `SpecialEventDate.(EventId, Date)` (FR-023)

### Check Constraints
- `SpecialEvent.Price > 0` (FR-020)
- `TicketPurchase.Quantity > 0`
- `TicketPurchase.TotalCost >= 0`
- `MuseumDailyHours.TimeOpen < TimeClosed`

### Nullable Fields
- `TicketPurchase.EventId` (nullable to support general admission)

## Entity Framework Core Mappings

### Model Configuration

```csharp
// MuseumDailyHours entity
public class MuseumDailyHours
{
    [Key]
    public DateTime Date { get; set; }  // Date-only, no time component
    
    [Required]
    public TimeSpan TimeOpen { get; set; }
    
    [Required]
    public TimeSpan TimeClosed { get; set; }
}

// SpecialEvent entity
public class SpecialEvent
{
    [Key]
    public Guid EventId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string EventName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2000)]
    public string EventDescription { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    // Navigation property
    public ICollection<SpecialEventDate> EventDates { get; set; } = new List<SpecialEventDate>();
}

// SpecialEventDate entity
public class SpecialEventDate
{
    [Required]
    public Guid EventId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }  // Date-only, no time component
    
    // Navigation property
    public SpecialEvent? SpecialEvent { get; set; }
}

// TicketPurchase entity
public class TicketPurchase
{
    [Key]
    public Guid PurchaseId { get; set; }
    
    [Required]
    public DateTime VisitDate { get; set; }  // Date-only, no time component
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalCost { get; set; }
    
    public Guid? EventId { get; set; }  // Nullable for general admission
    
    [Required]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public SpecialEvent? SpecialEvent { get; set; }
}
```

### DbContext Configuration

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // MuseumDailyHours
    modelBuilder.Entity<MuseumDailyHours>()
        .HasKey(h => h.Date);
    
    // SpecialEvent
    modelBuilder.Entity<SpecialEvent>()
        .HasKey(e => e.EventId);
    
    modelBuilder.Entity<SpecialEvent>()
        .Property(e => e.Price)
        .HasPrecision(10, 2);
    
    // SpecialEventDate - composite key
    modelBuilder.Entity<SpecialEventDate>()
        .HasKey(ed => new { ed.EventId, ed.Date });
    
    modelBuilder.Entity<SpecialEventDate>()
        .HasOne(ed => ed.SpecialEvent)
        .WithMany(e => e.EventDates)
        .HasForeignKey(ed => ed.EventId)
        .OnDelete(DeleteBehavior.Cascade);
    
    // TicketPurchase
    modelBuilder.Entity<TicketPurchase>()
        .HasKey(t => t.PurchaseId);
    
    modelBuilder.Entity<TicketPurchase>()
        .Property(t => t.TotalCost)
        .HasPrecision(10, 2);
    
    modelBuilder.Entity<TicketPurchase>()
        .HasOne(t => t.SpecialEvent)
        .WithMany()
        .HasForeignKey(t => t.EventId)
        .OnDelete(DeleteBehavior.SetNull);
}
```

## Migration Strategy

### Initial Migration
1. Create all four tables with primary keys
2. Add foreign key constraints
3. Add check constraints (if supported by database)
4. Seed sample data for development

### Versioning
- Use EF Core migrations for all schema changes
- Name migrations descriptively (e.g., `AddEventDescriptionLength`)
- Never modify existing migrations after deployment
- Test migrations against production-like data volumes

## Data Seeding

Sample seed data for development and testing:

```csharp
// Museum hours - 7 days
modelBuilder.Entity<MuseumDailyHours>().HasData(
    new MuseumDailyHours { Date = new DateTime(2025, 10, 21), TimeOpen = new TimeSpan(9, 0, 0), TimeClosed = new TimeSpan(17, 0, 0) },
    // ... additional days
);

// Sample special events
var eventId1 = Guid.NewGuid();
modelBuilder.Entity<SpecialEvent>().HasData(
    new SpecialEvent { EventId = eventId1, EventName = "Ancient Egypt Exhibit", EventDescription = "...", Price = 15.00m }
);

// Event dates
modelBuilder.Entity<SpecialEventDate>().HasData(
    new SpecialEventDate { EventId = eventId1, Date = new DateTime(2025, 10, 21) },
    new SpecialEventDate { EventId = eventId1, Date = new DateTime(2025, 10, 22) }
);
```

## Validation Rules Implementation

### Application-Level Validation
- Data annotations (Required, Range, MaxLength)
- Custom validators for business rules (e.g., visit date in future)
- Model state validation in API endpoints

### Database-Level Validation
- Primary key constraints (uniqueness)
- Foreign key constraints (referential integrity)
- Check constraints (positive prices, positive quantities)
- Unique indexes (prevent duplicates)

### Edge Case Handling

**Missing museum hours** (FR-011 validation):
- Query returns null for unscheduled dates
- API returns 404 or empty result
- Validation prevents ticket purchases for unscheduled dates

**Event deletion with existing purchases** (FR-019):
- Use `DeleteBehavior.SetNull` to preserve purchase records
- Admin API warns before deleting events with sales
- Consider soft-delete pattern (IsDeleted flag) for audit trail

**Concurrent purchases**:
- Use database transactions with appropriate isolation level
- Consider optimistic concurrency with RowVersion/Timestamp fields if capacity limits are added

## Future Enhancements

### Potential Schema Additions
- `TicketPurchase.PurchaserEmail` - for confirmation emails
- `TicketPurchase.PaymentTransactionId` - link to payment provider
- `SpecialEvent.Capacity` - maximum attendees per date
- `SpecialEvent.IsActive` - soft delete for historical events
- `MuseumConfig` - global settings (general admission price, tax rate)

### Audit Trail
- Consider adding CreatedDate, ModifiedDate, CreatedBy, ModifiedBy to all entities
- Implement change tracking for compliance and debugging

### Performance Optimization
- Add database indexes based on query patterns
- Consider read replicas for query scaling
- Implement caching for frequently accessed data (hours, event list)

## Conclusion

This data model provides:
- ✅ Simple, normalized schema with clear relationships
- ✅ All functional requirements mapped to entity constraints
- ✅ Strong data integrity through constraints and validation
- ✅ Flexibility for future enhancements
- ✅ EF Core-friendly design with clean navigation properties
- ✅ Performance considerations built-in (appropriate indexing)

The model is production-ready and can be implemented directly in Entity Framework Core with minimal adjustments.
