# Tasks Regeneration Summary

**Date**: October 22, 2025  
**Reason**: plan.md and quickstart.md were updated with new architectural requirements

## Key Architectural Changes

### 1. **Service Layer Architecture** (NEW)
- **Old**: Endpoints directly accessed DbContext
- **New**: Service layer with interfaces wraps EF Core operations
  - `IMuseumHoursService` / `MuseumHoursService`
  - `ISpecialEventsService` / `SpecialEventsService`
  - `ITicketPurchaseService` / `TicketPurchaseService`
- **Benefit**: Better testability, separation of concerns, business logic isolation

### 2. **Security & Authorization** (NEW)
- **New Components**:
  - `SecurityPolicies.cs` - Policy definitions (IsAdmin, IsUser)
  - `SecurityRoleMap.cs` - Active Directory group mappings
  - `SecurityPolicyBuilder.cs` - Policy construction with conjunctions
- **Authorization**: All endpoints require authentication, admin endpoints require IsAdmin policy

### 3. **API Versioning** (NEW)
- **Package**: Asp.Versioning.Http
- **Strategy**: URL-based versioning (e.g., `/v1/museumhours`)
- **Configuration**: Default v1.0, Swagger shows all versions
- **SwaggerDefaultValues** filter for proper documentation

### 4. **Enhanced Logging** (NEW)
- **Serilog Configuration**:
  - Console + File sinks
  - CompactJsonFormatter for structured logs
  - LogContext enrichment
- **Structured Logging**: Entry/exit/error logging in all service methods

### 5. **Database Changes**
- **Old**: In-memory SQLite for tests
- **New**: File-based SQLite for both development and tests
- **Reason**: More realistic test environment, persistent data during testing

### 6. **.NET Version**
- **Old**: .NET 8.0
- **New**: .NET 9.0 (per plan.md)

## New Package Dependencies

### Added to API Project:
- `Asp.Versioning.Http` - API versioning
- `Microsoft.Extensions.Configuration` - Configuration management
- `Microsoft.Extensions.DependencyInjection` - DI container
- `Serilog.Formatting.Compact` - Structured log formatting
- Additional Serilog packages (Settings.Configuration, Sinks.Debug)

### Test Project:
- Same as before (no changes to test packages)

## Task Breakdown Comparison

### Old Task Structure (421 lines, ~80 tasks):
```
Phase 1: Setup (T001-T005)
Phase 2: Foundational (T006-T012)
Phase 3: User Story 1 (T013-T022)
Phase 4: User Story 2 (T023-T036)
Phase 5: User Story 3 (T037-T049)
Phase 6: User Story 4 (T050-T056)
Phase 7: User Story 5 (T057-T068)
Phase 8: Polish (T069-T080)
```

### New Task Structure (213 tasks):
```
Phase 0: Initial Setup & Infrastructure (T001-T009) - 9 tasks
Phase 1: Foundational Architecture (T010-T033) - 24 tasks
Phase 2: User Story 1 - Museum Hours (T034-T063) - 30 tasks
Phase 3: User Story 2 - Special Events (T064-T099) - 36 tasks
Phase 4: User Story 3 - General Tickets (T100-T130) - 31 tasks
Phase 5: User Story 4 - Event Tickets (T131-T148) - 18 tasks
Phase 6: User Story 5 - Admin (T149-T176) - 28 tasks
Phase 7: Cross-Cutting & Polish (T177-T213) - 37 tasks
```

**Total Tasks**: 213 (increased from ~80)  
**Reason**: More granular breakdown, service layer adds significant tasks, security infrastructure

## MVP Definition Comparison

### Old MVP (Phases 1-4, ~50 tasks):
- Setup + Foundational
- User Story 1: Museum Hours (endpoints only)
- User Story 2: Special Events (endpoints only)
- **Result**: Basic API with direct DbContext access

### New MVP (Phases 0-3, 99 tasks):
- Setup + Infrastructure
- Foundational Architecture (security, versioning, services)
- User Story 1: Museum Hours (service layer + endpoints)
- User Story 2: Special Events (service layer + endpoints)
- **Result**: Production-ready API with proper architecture

**MVP Scope Increase**: ~50 tasks → 99 tasks (98% increase)  
**Reason**: Adding service layer, security, versioning to MVP ensures architectural foundation is solid before building remaining features

## Current Implementation Status

### What Exists (from old implementation):
- ✅ Solution and projects created
- ✅ Basic packages installed (.NET 8.0, EF Core, Swashbuckle)
- ✅ Models created (MuseumDailyHours, SpecialEvent, SpecialEventDate, TicketPurchase)
- ✅ DbContext with entity configurations
- ✅ Basic Program.cs with Serilog
- ✅ TestWebApplicationFactory (in-memory SQLite)
- ✅ DTOs (MuseumHoursDto, SpecialEventDto)
- ✅ Endpoints (MuseumHoursEndpoints, SpecialEventsEndpoints) - directly accessing DbContext
- ✅ DbInitializer with seed data
- ✅ Integration tests (7 for museum hours, 10 for special events)
- ✅ Unit tests (database constraints)
- ✅ All 17 tests passing

### What Needs to Change:
1. **Upgrade to .NET 9.0** (if not already)
2. **Add new packages** (Asp.Versioning.Http, additional Serilog packages)
3. **Create service layer** with interfaces
4. **Refactor endpoints** to use services instead of DbContext
5. **Add security infrastructure** (policies, role maps, authorization)
6. **Add API versioning** to all endpoints
7. **Switch to file-based SQLite** for tests
8. **Enhance Program.cs** with new configuration methods
9. **Add service tests** (new unit tests for business logic)
10. **Update integration tests** for versioned endpoints

## Migration Strategy

### Option 1: Refactor Existing (Recommended)
**Approach**: Gradually add new architecture to existing codebase
1. Phase 0: Add new packages, update to .NET 9.0
2. Phase 1: Add security, versioning, service interfaces
3. Phase 2-3: Refactor existing endpoints to use services, add versioning
4. Continue with Phases 4-7 as planned

**Pros**: Preserves existing tests, incremental changes  
**Cons**: Requires careful refactoring to maintain passing tests

### Option 2: Fresh Start
**Approach**: Start new implementation following new tasks.md from T001
1. Keep old implementation as reference
2. Build new project structure from scratch
3. Copy over models, DbContext configuration
4. Implement with proper architecture from day 1

**Pros**: Clean architecture from start, no legacy code  
**Cons**: Lose existing tests, more work upfront

### Option 3: Hybrid
**Approach**: Keep models/DbContext, rebuild everything else
1. Extract models, DbContext, DTOs to separate files
2. Create new solution with new architecture
3. Import existing data layer
4. Build new services, endpoints, tests with proper architecture

**Pros**: Reuse data layer work, fresh service/endpoint layer  
**Cons**: Some duplication during transition

## Recommendation

**Recommended Approach**: **Option 1 - Refactor Existing**

**Reasoning**:
1. Most work is in data models and DbContext - already done correctly
2. Endpoints are simple - easy to refactor to use services
3. Tests are valuable - they define expected behavior
4. Incremental changes allow continuous validation

**Next Steps**:
1. Review new tasks.md (Phase 0-1) to understand new architecture
2. Decide: refactor existing or start fresh
3. If refactoring: Start with Phase 0 tasks (add packages, update config)
4. If fresh start: Begin with T001 following new tasks.md

## Files Updated
- ✅ `/specs/001-museumapi-is-a/tasks.md` - Completely regenerated (213 tasks)
- ✅ `/specs/001-museumapi-is-a/tasks.md.old` - Backup of original tasks

## Notes
- Original tasks.md backed up to tasks.md.old for reference
- New architecture is significantly more robust but requires more upfront work
- MVP scope increased but delivers production-ready architecture
- Constitution compliance improved with service layer, security, versioning
