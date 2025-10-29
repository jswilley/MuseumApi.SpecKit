# Spec-Kit: Architecture & Contracts

This document captures the specification elements (Spec-Kit) for the `spectest` solution targeting .NET 8 / C# 12. It defines structural layout, API contracts, test strategy, and extension points.

## Solution Layout

## Technology Stack
- .NET 8 / C# 12
- ASP.NET Core Minimal APIs
- Entity Framework Core
- Swashbuckle (OpenAPI)
- API Versioning (URL segment style)

## Domain: Special Events (Admin)
Represents scheduled special events with one or more active dates.

### DTO Contracts
| DTO | Purpose | Key Fields |
|-----|---------|------------|
| `SpecialEventCreateRequest` | Create event | Name, Description, Initial Dates |
| `SpecialEventUpdateRequest` | Update mutable fields | Name, Description |
| `SpecialEventDateAddRequest` | Add a single date | DateOnly Date |
| `SpecialEventAdminResponse` | Returned to admin clients | Id, Name, Description, Dates[], CreatedUtc |

### Service Interfaces
- `ISpecialEventsService.AddEventDateAsync(Guid eventId, DateOnly date)`
- `ISpecialEventsService.RemoveEventDateAsync(Guid eventId, DateOnly date)`

### Admin Endpoint Pattern
Base route (versioned): `/v{version:apiVersion}/admin/specialevents`

| Method | Route | Body | Description |
|--------|-------|------|-------------|
| POST | `/` | `SpecialEventCreateRequest` | Create new event |
| GET | `/` | — | List events |
| GET | `/{id:guid}` | — | Get by Id |
| PUT | `/{id:guid}` | `SpecialEventUpdateRequest` | Update event (non-date fields) |
| POST | `/{id:guid}/dates` | `SpecialEventDateAddRequest` | Add a date |
| DELETE | `/{id:guid}/dates/{date}` | — | Remove a date |
| DELETE | `/{id:guid}` | — | Delete entire event |

Date format: `yyyy-MM-dd` for route segments and JSON.

### Authentication (Local Dev)
Test admin handler requires header:

Grants role `admin`. Replace with production auth before deployment.

## API Versioning
Configured via API Versioning + versioned API explorer. URL segment style: `/v1/...`. Add new versions incrementally; keep existing stable contracts.

## Minimal API Conventions
- Use explicit binding attributes: `[FromBody]`, `[FromQuery]`, `[FromServices]`.
- Return standardized error payloads (recommended shape):

## Testing Strategy
Location: `tests/`
- Integration tests exercise full request pipeline against in-memory SQLite.
- `AdminSpecialEventsIntegrationTests` cover create/update/delete and date operations.
- Avoid state bleed: use unique dates or isolated DB connections.
- Run:

## Build & Run
From solution root or `backend/`:

Navigate to generated Swagger/OpenAPI UI (e.g., `/swagger`).

## Extension Points
| Concern | How to Extend |
|---------|---------------|
| Validation | FluentValidation or custom endpoint guards before service calls |
| Auth | Replace test handler with real JWT / OIDC middleware |
| Observability | Add logging scopes + OpenTelemetry tracing |
| Versioning | Introduce `/v2` with additive changes; keep `/v1` intact |
| Persistence | Swap in real relational DB (configure EF Core provider) |

## Design Principles
- Clear, versioned contracts.
- Explicit parameter binding to avoid ambiguity.
- Separation of concerns: Endpoint layer -> Service -> Data.
- Deterministic tests (no external dependencies).

## Quick Checklist (Adding New Admin Endpoint)
1. Define DTO(s).
2. Implement service method.
3. Add Minimal API endpoint with explicit bindings.
4. Update OpenAPI (ensure it appears in explorer).
5. Add integration test.
6. Respect existing version or introduce new version folder/registration.

## Notes
- Keep date handling culture-invariant.
- Ensure idempotency where feasible (e.g., adding an existing date returns suitable error).

---

For deeper details, inspect service and test implementations in `backend/` and `tests/`.
