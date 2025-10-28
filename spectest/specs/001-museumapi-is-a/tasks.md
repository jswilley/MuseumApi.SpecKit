# Implementation Tasks: Museum API

**Branch**: `001-museumapi-is-a` | **Generated**: October 22, 2025  
**Architecture**: ASP.NET Core Minimal APIs with Service Layer, .NET 9.0, SQLite, API Versioning, Security Policies

## Task Breakdown Strategy

This implementation follows a **test-driven development (TDD)** approach with **layered architecture**:
1. **Services layer** wraps Entity Framework Core context calls with interfaces
2. **Security layer** implements policy-based authorization
3. **API Versioning** for future-proof endpoint management
4. **File-based SQLite** for both development and testing (not in-memory)
5. Each phase delivers **independently testable user value**

**Constitution Compliance**: All tasks align with architectural principles - test-first, REST, maintainability, DI

---

## Phase 0: Initial Setup & Infrastructure

**Goal**: Establish solution structure, dependencies, and foundational architecture

### Project & Package Setup

- [ ] T001 [SETUP] Create solution: `dotnet new sln -n MuseumApi`
- [ ] T002 [SETUP] Create API project: `dotnet new webapi -n MuseumApi --use-minimal-apis` and add to solution
- [ ] T003 [SETUP] Create test project: `dotnet new xunit -n MuseumApi.Tests`, add to solution, add project reference
- [ ] T004 [SETUP] Install API packages: Entity Framework Core (Sqlite, Design), Swashbuckle, OpenAPI, Serilog (AspNetCore, Console, File, Formatting.Compact), Asp.Versioning.Http, Microsoft.Extensions (Configuration, DependencyInjection)
- [ ] T005 [SETUP] Install test packages: Microsoft.AspNetCore.Mvc.Testing, Microsoft.EntityFrameworkCore.Sqlite, Moq, xUnit (all packages)

### Directory Structure

- [ ] T006 [SETUP] Create directory structure: Context/, Models/, DTOs/, Endpoints/, Services/, Security/, Data/
- [ ] T007 [SETUP] Create test directory structure: IntegrationTests/, Unit/

### Configuration Files

- [ ] T008 [SETUP] Configure appsettings.json with connection string, Serilog configuration (Console + File sinks with CompactJsonFormatter), GeneralAdmissionPrice, SecurityEnvironment
- [ ] T009 [SETUP] Create appsettings.Development.json with development-specific settings

**Checkpoint**: Solution structure complete, all packages installed, configuration ready

---

## Phase 1: Foundational Architecture

**Goal**: Establish database context, security policies, versioning, logging, and test infrastructure

### Database Context & Models

- [ ] T010 [FOUNDATION] Create MuseumContext.cs in Context/ with DbSet properties for all entities
- [ ] T011 [FOUNDATION] Configure entity relationships in MuseumContext.OnModelCreating (composite keys, decimal precision, cascade deletes, unique constraints)
- [ ] T012 [FOUNDATION] Create DbInitializer.cs in Context/ for database seeding logic
- [ ] T013 [FOUNDATION] Create MuseumDailyHour model in Models/ with Date (PK), TimeOpen, TimeClosed, validation attributes
- [ ] T014 [FOUNDATION] Create SpecialEvent model in Models/ with EventId (Guid PK), EventName, EventDescription, Price, validation attributes
- [ ] T015 [FOUNDATION] Create SpecialEventDate model in Models/ with composite PK (EventId, Date), navigation properties
- [ ] T016 [FOUNDATION] Create TicketPurchase model in Models/ with PurchaseId, VisitDate, Quantity, TotalCost, EventId (nullable), PurchaseDate

### Security Infrastructure

- [ ] T017 [FOUNDATION] Create SecurityRoleMap.cs in Security/ with RoleGroup constants (Administrators, Users)
- [ ] T018 [FOUNDATION] Create SecurityPolicyBuilder.cs in Security/ with GroupMembershipPolicyBuilder class for policy conjunctions (InAnyRoleGroup, MemberOf, Or, Except, Build)
- [ ] T019 [FOUNDATION] Create SecurityPolicies.cs in Security/ with static policy definitions (IsAdmin, IsUser, AdminPolicy, UserPolicy)

### Program.cs Configuration

- [ ] T020 [FOUNDATION] Configure Serilog in Program.cs with ConfigureLogging method (console + file sinks, CompactJsonFormatter)
- [ ] T021 [FOUNDATION] Configure API versioning in Program.cs with ConfigureVersion method (default v1.0, URL-based versioning, SwaggerDefaultValues filter)
- [ ] T022 [FOUNDATION] Configure security policies in Program.cs with ConfigurePolicies method (AddAuthorization, CORS policy)
- [ ] T023 [FOUNDATION] Configure services in Program.cs with ConfigureServices method (register service interfaces with scoped lifetime)
- [ ] T024 [FOUNDATION] Configure DbContext in Program.cs with SQLite connection string
- [ ] T025 [FOUNDATION] Configure Swagger in Program.cs to support multiple API versions with proper documentation
- [ ] T026 [FOUNDATION] Add database initialization to Program.cs (EnsureCreated, DbInitializer.Initialize in scope)
- [ ] T027 [FOUNDATION] Add SwaggerDefaultValues operation filter class to handle versioning metadata
- [ ] T028 [FOUNDATION] Add partial Program class declaration for test accessibility

### Test Infrastructure

- [ ] T029 [FOUNDATION] Create TestWebApplicationFactory.cs in IntegrationTests/ with file-based SQLite configuration (not in-memory)
- [ ] T030 [FOUNDATION] Configure TestWebApplicationFactory to use separate test database file
- [ ] T031 [FOUNDATION] Implement IDisposable in TestWebApplicationFactory for proper cleanup

### Initial Migration

- [ ] T032 [FOUNDATION] Create initial EF migration: `dotnet ef migrations add InitialCreate --project MuseumApi`
- [ ] T033 [FOUNDATION] Review generated migration for correctness (composite keys, constraints, indexes)

**Checkpoint**: Database schema created, security policies defined, versioning configured, logging operational, test infrastructure ready

---

## Phase 2: User Story 1 - Query Museum Hours (P1 - MVP)

**Goal**: Visitors can query museum operating hours

**Test-First Strategy**: Write integration tests for endpoints, unit tests for service logic, then implement

### Tests for User Story 1

- [ ] T034 [P] [US1] Create MuseumContextTests.cs in Unit/ - test unique date constraint on MuseumDailyHour (FR-022)
- [ ] T035 [P] [US1] Add test for storing/retrieving museum hours from database
- [ ] T036 [P] [US1] Create MuseumHoursIntegrationTests.cs in IntegrationTests/ - test GET /v1/museumhours endpoint (all scenarios)
- [ ] T037 [P] [US1] Add integration test: query hours for today, verify correct times returned
- [ ] T038 [P] [US1] Add integration test: query hours for specific date, verify correct times
- [ ] T039 [P] [US1] Add integration test: query hours for date range, verify multiple days returned
- [ ] T040 [P] [US1] Add integration test: query hours for closed day, verify empty/no result (FR-002)
- [ ] T041 [P] [US1] Create MuseumHoursServiceTests.cs in Unit/ - test service methods with mocked DbContext

### Service Layer for Museum Hours

- [ ] T042 [US1] Create IMuseumHoursService.cs in Services/ with method signatures (GetHoursByDateAsync, GetHoursByDateRangeAsync, GetAllHoursAsync)
- [ ] T043 [US1] Create MuseumHoursService.cs in Services/ implementing IMuseumHoursService
- [ ] T044 [US1] Implement GetHoursByDateAsync in MuseumHoursService (query by specific date, return DTO or null)
- [ ] T045 [US1] Implement GetHoursByDateRangeAsync in MuseumHoursService (query date range, return IEnumerable<DTO>)
- [ ] T046 [US1] Implement GetAllHoursAsync in MuseumHoursService (return all hours ordered by date)
- [ ] T047 [US1] Add proper async/await and error handling to all service methods

### DTOs & Endpoints

- [ ] T048 [P] [US1] Create MuseumHoursDto.cs in DTOs/ as record type (Date, TimeOpen, TimeClosed)
- [ ] T049 [P] [US1] Create MuseumHoursEndpoints.cs in Endpoints/ with MapMuseumHoursEndpoints extension method
- [ ] T050 [P] [US1] Implement GET /v1/museumhours endpoint with versioning, inject IMuseumHoursService
- [ ] T051 [P] [US1] Add query parameters support: date (DateOnly?), startDate (DateOnly?), endDate (DateOnly?) per FR-003
- [ ] T052 [US1] Add filtering logic: specific date OR date range (startDate/endDate) OR all hours
- [ ] T053 [US1] Add OpenAPI annotations: WithName, WithSummary, WithDescription, WithTags
- [ ] T054 [US1] Add authorization policy: RequireAuthorization(Policies.IsUser) for museum hours endpoint
- [ ] T055 [US1] Register IMuseumHoursService in Program.cs ConfigureServices method (AddScoped)
- [ ] T056 [US1] Map MuseumHoursEndpoints in Program.cs after middleware configuration

### Seed Data

- [ ] T057 [US1] Implement museum hours seeding in DbInitializer (30 days of sample data following quickstart.md examples)
- [ ] T058 [US1] Verify seed data includes variety: weekdays, weekends, closed days (per quickstart Monday closures)

### Validation

- [ ] T059 [US1] Run museum hours tests: `dotnet test --filter "FullyQualifiedName~MuseumHours"`
- [ ] T060 [US1] Verify all integration tests pass (GET endpoint scenarios)
- [ ] T061 [US1] Verify all unit tests pass (service layer, database constraints)
- [ ] T062 [US1] Test via Swagger UI at https://localhost:5001/swagger - verify v1.0 endpoints visible
- [ ] T063 [US1] Manual curl test: `curl "http://localhost:5000/v1/museumhours?date=2025-10-22"`

**Checkpoint**: ✅ User Story 1 complete - Visitors can query museum hours via versioned REST API with service layer

---

## Phase 3: User Story 2 - Browse Special Events (P1 - MVP)

**Goal**: Visitors can discover special events

### Tests for User Story 2

- [ ] T064 [P] [US2] Add SpecialEventDate unique constraint test to MuseumContextTests.cs (composite PK, FR-023)
- [ ] T065 [P] [US2] Create SpecialEventTests.cs in Unit/ - test price validation (FR-020), date validation (FR-021)
- [ ] T066 [P] [US2] Add test for storing special events with multiple dates, verify navigation properties work
- [ ] T067 [P] [US2] Create SpecialEventsIntegrationTests.cs in IntegrationTests/ - test GET /v1/specialevents endpoints
- [ ] T068 [P] [US2] Add integration test: get all special events, verify complete list with dates
- [ ] T069 [P] [US2] Add integration test: get specific event by ID, verify details including EventDates array
- [ ] T070 [P] [US2] Add integration test: 404 handling for non-existent event ID
- [ ] T071 [P] [US2] Add integration test: filter events by specific date (FR-008)
- [ ] T072 [P] [US2] Add integration test: filter events by date range (startDate/endDate)
- [ ] T073 [P] [US2] Create SpecialEventsServiceTests.cs in Unit/ - test service methods with mocked DbContext

### Service Layer for Special Events

- [ ] T074 [US2] Create ISpecialEventsService.cs in Services/ with method signatures (GetAllEventsAsync, GetEventByIdAsync, GetEventsByDateAsync, GetEventsByDateRangeAsync, CreateEventAsync, UpdateEventAsync, DeleteEventAsync)
- [ ] T075 [US2] Create SpecialEventsService.cs in Services/ implementing ISpecialEventsService
- [ ] T076 [US2] Implement GetAllEventsAsync in SpecialEventsService (include EventDates with .Include(), return DTOs)
- [ ] T077 [US2] Implement GetEventByIdAsync in SpecialEventsService (find by ID, include EventDates, return DTO or null)
- [ ] T078 [US2] Implement GetEventsByDateAsync in SpecialEventsService (filter events occurring on specific date)
- [ ] T079 [US2] Implement GetEventsByDateRangeAsync in SpecialEventsService (filter events in date range)
- [ ] T080 [US2] Implement CreateEventAsync, UpdateEventAsync, DeleteEventAsync (for Phase 5 admin features, basic implementation)
- [ ] T081 [US2] Add proper async/await, error handling, and validation to all service methods

### DTOs & Endpoints

- [ ] T082 [P] [US2] Create SpecialEventDto.cs in DTOs/ as record type (EventId, EventName, EventDescription, Price, DateOnly[] EventDates)
- [ ] T083 [P] [US2] Create SpecialEventsEndpoints.cs in Endpoints/ with MapSpecialEventsEndpoints extension method
- [ ] T084 [P] [US2] Implement GET /v1/specialevents endpoint (list all) - inject ISpecialEventsService, return all events ordered by name
- [ ] T085 [P] [US2] Implement GET /v1/specialevents/{id:guid} endpoint - return specific event with EventDates, 404 if not found
- [ ] T086 [US2] Add query parameters to list endpoint: date (DateOnly?), startDate (DateOnly?), endDate (DateOnly?) per FR-008
- [ ] T087 [US2] Add filtering logic in endpoint: delegate to appropriate service method based on parameters
- [ ] T088 [US2] Add OpenAPI annotations to all endpoints (WithName, WithSummary, WithDescription, WithTags)
- [ ] T089 [US2] Add authorization policy: RequireAuthorization(Policies.IsUser) for public special events endpoints
- [ ] T090 [US2] Register ISpecialEventsService in Program.cs ConfigureServices method (AddScoped)
- [ ] T091 [US2] Map SpecialEventsEndpoints in Program.cs

### Seed Data

- [ ] T092 [US2] Implement special events seeding in DbInitializer (5 events from quickstart.md: Sasquatch Ballet, Solar Telescope, Cook Like Caveman, Mermaid Treasure, Time Traveler Tea)
- [ ] T093 [US2] Implement SpecialEventDate seeding in DbInitializer (map events to dates per quickstart examples)
- [ ] T094 [US2] Verify seed data includes variety of date patterns (single day events, multi-day events)

### Validation

- [ ] T095 [US2] Run special events tests: `dotnet test --filter "FullyQualifiedName~SpecialEvent"`
- [ ] T096 [US2] Verify all integration tests pass (list, get by ID, filtering, 404)
- [ ] T097 [US2] Verify all unit tests pass (service layer, price validation, constraints)
- [ ] T098 [US2] Test via Swagger UI - verify special events v1.0 endpoints work
- [ ] T099 [US2] Manual curl test: `curl "http://localhost:5000/v1/specialevents?startDate=2025-09-11&endDate=2025-09-20"`

**Checkpoint**: ✅ User Story 2 complete - Visitors can browse special events with filtering. **MVP ACHIEVED** (P1 stories complete)

---

## Phase 4: User Story 3 - Purchase Museum Tickets (P2)

**Goal**: Visitors can purchase general admission tickets

### Tests for User Story 3

- [ ] T100 [P] [US3] Create TicketPurchaseIntegrationTests.cs in IntegrationTests/ - test POST /v1/tickets/purchase endpoint
- [ ] T101 [P] [US3] Add integration test: purchase general admission tickets, verify successful response with confirmation
- [ ] T102 [P] [US3] Add integration test: purchase for closed date, verify 400 error (FR-011)
- [ ] T103 [P] [US3] Add integration test: purchase with negative/zero quantity, verify validation error
- [ ] T104 [P] [US3] Add integration test: verify total cost calculation (quantity * general admission price)
- [ ] T105 [P] [US3] Create TicketPurchaseServiceTests.cs in Unit/ - test service validation logic

### Service Layer for Ticket Purchase

- [ ] T106 [US3] Create ITicketPurchaseService.cs in Services/ with method signatures (PurchaseGeneralAdmissionAsync, PurchaseEventTicketAsync, ValidateMuseumOpenAsync, ValidateEventDateAsync, CalculateTotalCostAsync)
- [ ] T107 [US3] Create TicketPurchaseService.cs in Services/ implementing ITicketPurchaseService
- [ ] T108 [US3] Implement ValidateMuseumOpenAsync (check MuseumDailyHours for date, return bool per FR-011)
- [ ] T109 [US3] Implement CalculateTotalCostAsync for general admission (quantity * config price)
- [ ] T110 [US3] Implement PurchaseGeneralAdmissionAsync (validate date, calculate cost, create TicketPurchase, save, return DTO)
- [ ] T111 [US3] Add validation: positive quantity, valid visit date, museum open on date
- [ ] T112 [US3] Add error handling: return appropriate error messages for validation failures

### DTOs & Endpoints

- [ ] T113 [P] [US3] Create TicketPurchaseRequest.cs in DTOs/ as record type (VisitDate, Quantity, EventId nullable)
- [ ] T114 [P] [US3] Create TicketPurchaseResponse.cs in DTOs/ as record type (PurchaseId, VisitDate, Quantity, TotalCost, EventId, EventName, PurchaseDate)
- [ ] T115 [P] [US3] Create TicketPurchaseEndpoints.cs in Endpoints/ with MapTicketPurchaseEndpoints extension method
- [ ] T116 [P] [US3] Implement POST /v1/tickets/purchase endpoint - inject ITicketPurchaseService
- [ ] T117 [US3] Add request validation in endpoint: check required fields, call service validation methods
- [ ] T118 [US3] Handle general admission purchase: when EventId is null, call PurchaseGeneralAdmissionAsync
- [ ] T119 [US3] Add proper HTTP status codes: 201 Created for success, 400 Bad Request for validation errors
- [ ] T120 [US3] Add OpenAPI annotations with request/response examples
- [ ] T121 [US3] Add authorization policy: RequireAuthorization(Policies.IsUser)
- [ ] T122 [US3] Register ITicketPurchaseService in Program.cs ConfigureServices method (AddScoped)
- [ ] T123 [US3] Map TicketPurchaseEndpoints in Program.cs

### Configuration

- [ ] T124 [US3] Add GeneralAdmissionPrice to appsettings.json (e.g., 10.00)
- [ ] T125 [US3] Inject IConfiguration into TicketPurchaseService for price retrieval

### Validation

- [ ] T126 [US3] Run ticket purchase tests: `dotnet test --filter "FullyQualifiedName~TicketPurchase"`
- [ ] T127 [US3] Verify integration tests pass (successful purchase, validation errors)
- [ ] T128 [US3] Verify unit tests pass (service validation logic)
- [ ] T129 [US3] Test via Swagger UI - attempt general admission purchase
- [ ] T130 [US3] Manual curl test with valid and invalid data

**Checkpoint**: ✅ User Story 3 complete - Visitors can purchase general admission tickets with validation

---

## Phase 5: User Story 4 - Purchase Special Event Tickets (P2)

**Goal**: Visitors can purchase special event tickets

### Tests for User Story 4

- [ ] T131 [P] [US4] Add event ticket purchase tests to TicketPurchaseIntegrationTests.cs
- [ ] T132 [P] [US4] Add integration test: purchase event ticket, verify event price used (not general admission price)
- [ ] T133 [P] [US4] Add integration test: purchase for date when event not scheduled, verify 400 error (FR-012)
- [ ] T134 [P] [US4] Add integration test: purchase for invalid event ID, verify 404 error
- [ ] T135 [P] [US4] Add integration test: verify total cost calculation (quantity * event price)

### Service Layer Extension

- [ ] T136 [US4] Implement ValidateEventDateAsync in TicketPurchaseService (check SpecialEventDates for event + date combo per FR-012)
- [ ] T137 [US4] Implement CalculateTotalCostAsync overload for event tickets (quantity * event.Price)
- [ ] T138 [US4] Implement PurchaseEventTicketAsync (validate event exists, validate event on date, calculate cost, create purchase with EventId, save)
- [ ] T139 [US4] Add validation: event exists, event scheduled on date, positive quantity
- [ ] T140 [US4] Update service to fetch event name for response DTO

### Endpoint Extension

- [ ] T141 [US4] Update POST /v1/tickets/purchase endpoint to handle EventId when provided
- [ ] T142 [US4] Add logic: if EventId is present, call PurchaseEventTicketAsync instead of general admission
- [ ] T143 [US4] Include event name in TicketPurchaseResponse for event tickets
- [ ] T144 [US4] Update OpenAPI documentation to show optional EventId parameter

### Validation

- [ ] T145 [US4] Run all ticket purchase tests: `dotnet test --filter "FullyQualifiedName~TicketPurchase"`
- [ ] T146 [US4] Verify event ticket tests pass (purchase, validation, pricing)
- [ ] T147 [US4] Test via Swagger UI - purchase both general admission and event tickets
- [ ] T148 [US4] Manual curl test with event ID

**Checkpoint**: ✅ User Story 4 complete - Visitors can purchase both general admission and event tickets

---

## Phase 6: User Story 5 - Manage Special Events (Admin) (P3)

**Goal**: Administrators can manage special events catalog

### Tests for User Story 5

- [ ] T149 [P] [US5] Add admin endpoint tests to SpecialEventsIntegrationTests.cs
- [ ] T150 [P] [US5] Add integration test: POST /v1/admin/specialevents (create event), verify 201 Created
- [ ] T151 [P] [US5] Add integration test: PUT /v1/admin/specialevents/{id} (update event), verify 200 OK
- [ ] T152 [P] [US5] Add integration test: DELETE /v1/admin/specialevents/{id} (delete event), verify 204 No Content
- [ ] T153 [P] [US5] Add integration test: verify authorization - 401/403 for non-admin users (FR-015)
- [ ] T154 [P] [US5] Add test: delete event with ticket purchases, verify referential integrity handling (FR-019)

### Service Layer Completion

- [ ] T155 [US5] Complete CreateEventAsync implementation in SpecialEventsService (create event + event dates in transaction)
- [ ] T156 [US5] Complete UpdateEventAsync implementation (update event fields, handle date changes - add new, remove old)
- [ ] T157 [US5] Complete DeleteEventAsync implementation (delete event with cascade to event dates, handle tickets with SetNull per FR-019)
- [ ] T158 [US5] Add validation: event name required, price positive (FR-020), dates valid (FR-021)
- [ ] T159 [US5] Add transaction handling for multi-step operations (event + dates)

### DTOs for Admin Operations

- [ ] T160 [P] [US5] Create CreateSpecialEventRequest.cs in DTOs/ (EventName, EventDescription, Price, DateOnly[] EventDates)
- [ ] T161 [P] [US5] Create UpdateSpecialEventRequest.cs in DTOs/ (optional fields: EventName?, EventDescription?, Price?, DateOnly[]? EventDates)
- [ ] T162 [US5] Add validation attributes to request DTOs (Required, Range for price)

### Admin Endpoints

- [ ] T163 [P] [US5] Add MapPost for POST /v1/admin/specialevents to SpecialEventsEndpoints
- [ ] T164 [US5] Implement create endpoint: validate request, call CreateEventAsync, return 201 with Location header
- [ ] T165 [US5] Add MapPut for PUT /v1/admin/specialevents/{id:guid}
- [ ] T166 [US5] Implement update endpoint: validate request, call UpdateEventAsync, return 200 or 404
- [ ] T167 [US5] Add MapDelete for DELETE /v1/admin/specialevents/{id:guid}
- [ ] T168 [US5] Implement delete endpoint: call DeleteEventAsync, return 204 or 404
- [ ] T169 [US5] Add authorization: RequireAuthorization(Policies.IsAdmin) to all admin endpoints (per FR-015)
- [ ] T170 [US5] Add OpenAPI annotations with security requirements
- [ ] T171 [US5] Add endpoint tags: "Special Events - Admin" to group in Swagger

### Validation

- [ ] T172 [US5] Run special events tests: `dotnet test --filter "FullyQualifiedName~SpecialEvent"`
- [ ] T173 [US5] Verify admin endpoint tests pass (create, update, delete)
- [ ] T174 [US5] Verify authorization tests pass (401/403 for non-admin)
- [ ] T175 [US5] Test via Swagger UI - verify admin endpoints require authorization
- [ ] T176 [US5] Manual test: create event, update it, delete it via API

**Checkpoint**: ✅ User Story 5 complete - Administrators can manage special events catalog

---

## Phase 7: Cross-Cutting Concerns & Polish

**Goal**: Error handling, logging, testing coverage, documentation

### Error Handling

- [ ] T177 [POLISH] Add global exception handler middleware to Program.cs
- [ ] T178 [POLISH] Create standardized error response format (ProblemDetails)
- [ ] T179 [POLISH] Add validation error handling with descriptive messages
- [ ] T180 [POLISH] Add specific exception handling for database errors, not found errors
- [ ] T181 [POLISH] Ensure all service methods return proper error information

### Logging

- [ ] T182 [POLISH] Add structured logging to all service methods (entry, exit, errors)
- [ ] T183 [POLISH] Add logging to endpoints for request/response tracking
- [ ] T184 [POLISH] Verify Serilog configuration writes to both console and file
- [ ] T185 [POLISH] Add correlation IDs for request tracking
- [ ] T186 [POLISH] Test log output with various scenarios (success, errors, validation failures)

### Testing Coverage

- [ ] T187 [POLISH] Run code coverage: `dotnet test /p:CollectCoverage=true /p:CoverageReporter=html`
- [ ] T188 [POLISH] Verify minimum 70% code coverage as per constitution
- [ ] T189 [POLISH] Add missing unit tests for uncovered service methods
- [ ] T190 [POLISH] Add missing integration tests for edge cases
- [ ] T191 [POLISH] Add performance tests for query endpoints (verify <200ms p95 per constitution)

### API Documentation

- [ ] T192 [POLISH] Verify Swagger UI shows all API versions correctly
- [ ] T193 [POLISH] Add comprehensive XML documentation comments to all public methods
- [ ] T194 [POLISH] Add example requests/responses to OpenAPI annotations
- [ ] T195 [POLISH] Verify authorization requirements visible in Swagger UI
- [ ] T196 [POLISH] Export OpenAPI spec to contracts/openapi.yaml
- [ ] T197 [POLISH] Update contracts/openapi.yaml with complete documentation

### Database

- [ ] T198 [POLISH] Review all EF migrations for correctness
- [ ] T199 [POLISH] Add database indexes for query performance (Date columns, EventId foreign keys)
- [ ] T200 [POLISH] Verify seed data is comprehensive (30 days hours, 5+ events with varied dates)
- [ ] T201 [POLISH] Test database initialization on fresh environment

### Security

- [ ] T202 [POLISH] Verify all admin endpoints require IsAdmin policy
- [ ] T203 [POLISH] Verify all public endpoints require IsUser policy (authenticated)
- [ ] T204 [POLISH] Test CORS policy configuration
- [ ] T205 [POLISH] Add security headers (HTTPS redirection confirmed)

### Final Validation

- [ ] T206 [POLISH] Run full test suite: `dotnet test`
- [ ] T207 [POLISH] Verify all tests pass (unit + integration)
- [ ] T208 [POLISH] Run application: `dotnet run --project MuseumApi`
- [ ] T209 [POLISH] Test all endpoints via Swagger UI (museum hours, events, tickets, admin)
- [ ] T210 [POLISH] Perform manual end-to-end test: query hours, browse events, purchase tickets, admin operations
- [ ] T211 [POLISH] Verify logging output in console and log files
- [ ] T212 [POLISH] Check performance: query response times under load
- [ ] T213 [POLISH] Verify API versioning working (v1.0 in URLs, Swagger shows versions)

**Checkpoint**: ✅ All cross-cutting concerns implemented, documentation complete, production-ready

---

## Summary Statistics

**Total Tasks**: 213
- **Phase 0 (Setup)**: 9 tasks
- **Phase 1 (Foundation)**: 24 tasks  
- **Phase 2 (User Story 1 - Museum Hours)**: 30 tasks
- **Phase 3 (User Story 2 - Special Events)**: 36 tasks
- **Phase 4 (User Story 3 - General Admission Tickets)**: 31 tasks
- **Phase 5 (User Story 4 - Event Tickets)**: 18 tasks
- **Phase 6 (User Story 5 - Admin Features)**: 28 tasks
- **Phase 7 (Polish)**: 37 tasks

**MVP Definition** (Phases 0-3): User Stories 1 & 2 complete = 99 tasks for minimum viable product

**Priority Breakdown**:
- [P] tasks (test-first, critical path): ~65 tasks
- [US1] Museum Hours: 30 tasks
- [US2] Special Events: 36 tasks
- [US3] General Tickets: 31 tasks
- [US4] Event Tickets: 18 tasks
- [US5] Admin: 28 tasks
- [SETUP/FOUNDATION/POLISH]: 70 tasks

**Testing Strategy**:
- Integration tests for all endpoints (user-facing behavior)
- Unit tests for services (business logic isolation)
- Unit tests for database constraints (data integrity)
- File-based SQLite for realistic test environment
- Minimum 70% code coverage target

**Architecture Notes**:
- Services layer provides testable abstraction over EF Core
- Security policies enable fine-grained authorization
- API versioning enables future evolution without breaking changes
- Structured logging with Serilog provides production observability
- All async operations use async/await per constitution
