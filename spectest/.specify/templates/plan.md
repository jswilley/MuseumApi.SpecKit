The Plan Phase (`/plan`) uses the project requirements defined in `specify.md` and the governance principles established in `constitution.md` to establish the technical foundation for generating the C\# Minimal API.

Here is the detailed project plan structured as `plan.md`:

---

## Project Plan: C\# Minimal API for Museum Catalog (`plan.md`) 🏗️

### 1. Technology Stack & Target Platform

The technical architecture is constrained by the requirement to build a C\# Minimal API that adheres to the established opinionated stack.

| Technical Component | Decision | Rationale (Constitution Alignment) |
| :--- | :--- | :--- |
| **Target Framework** | **.NET 9** | Standard, modern framework for high-performance Minimal APIs. |
| **Project Type** | **Minimal API** | Aligns with the project goal of generating a lightweight API. |
| **Architectural Style** | **Feature Group / Extension Methods** | Ensures the main `Program.cs` file remains minimal and endpoint definitions are externalized to an endpoint class, adhering to the Single Responsibility Principle. |
| **Persistence** | Sqlite Repository | Business logic and persistence must be encapsulated in injectable services (DI is mandatory). |
| **Logging & Error Handling** | **Serilog and Global Middleware** | Required for proper error handling and observability, and mandates using Serilog for logging. |
| **Configuration** | **`appsettings.json`** | Configuration must be managed through `appsettings.json` variables. |

### 2. Project Structure
MuseumApi.csproj/
├── models/
├── services/
├── endpoints/
└── data/
└── context/
    

MuseumApi.Tests.csproj/
├── integration/
└── unit/


#### Phase 2.1: Code Generation and Typing

1.  **Generate Data Transfer Objects (DTOs):** Use the **OpenAPI Generator CLI** to generate all required C\# data models, including `SpecialEvent`, `MuseumHours`, `Ticket`, and `MuseumTicketConfirmation`, directly from the OpenAPI specification.
2.  **DTO Type Enforcement:** All generated models **MUST** use **C\# `record` types** for concise, immutable, and thread-safe data transfer, aligning with the "minimal" philosophy and constitutional constraints.
3.  **Generate Service Interfaces (Stub):** Generate interfaces or abstract classes that define the necessary signatures for implementing the business logic, ensuring strict adherence to the defined contract paths and parameters (e.g., `getMuseumHours`, `createSpecialEvent`).

#### Phase 2.2: Endpoint Implementation

1.  **Business Logic Separation (DI Mandatory):** All endpoint handlers **MUST** resolve and delegate I/O-bound logic to an injected service (e.g., `IMuseumService`). **No inline business logic** should exist directly in the `app.Map*` calls.
2.  **Asynchronous Operations:** All I/O-bound endpoint handlers (such as CRUD operations and ticket purchases) must be declared **`async`** and use `await` for I/O operations, adhering to the performance principle.
3.  **Standard Results:** All endpoints must return standard `Microsoft.AspNetCore.Http.IResult` types (e.g., `Results.Ok()`, `Results.Created()`, `Results.NotFound()`, `Results.BadRequest()`) to maintain consistent and testable HTTP responses, as mandated by the constitution.

### 3. Security, Validation, and Authorization

The plan must address security for administrative actions and enforce data integrity requirements defined in the specification.

| Requirement | Implementation Detail | Source Reference |
| :--- | :--- | :--- |
| **Admin Authorization** | Endpoints categorized under "manage special events" (`POST`, `PUT`, `DELETE /SpecialEvent`) must be secured using **token-based authentication** (Bearer) and authorization middleware (`RequireAuthorization()`). | |
| **Data Validation** | Input validation must be implemented for DTOs. Specifically, implementation for `createSpecialEvent` and `updateSpecialEvent` must validate that the **`EventPrice` is a positive decimal value**. | (FR-003) |
| **Validation Response** | If validation fails (e.g., non-positive price), the API must return a standard **`HTTP 400 Bad Request`** response, consistent with constitutional requirements for standard HTTP Results. | (FR-003 AC 1) |

### 4. Testing & Validation Strategy

Testing is mandatory.

| Test Phase | Tool / Methodology | Objective | Source Reference |
| :--- | :--- | :--- | :--- |
| **Integration Testing** | **`WebApplicationFactory` / `HttpClient`** | This is the preferred testing method. Tests will instantiate the Minimal API in-memory, mocking external dependencies via DI to achieve fast, isolated checks on endpoint behavior. | |
| **Coverage Gate** | CI/CD Pipeline Check | Must maintain a **minimum automated test coverage percentage of 60%** to meet constitutional quality gates. | |

See Testing.md for more information.

### 5. Key Entity Data Models

The implementation will require generated C\# `record` types corresponding to the following key entities identified in `specify.md`:

1.  `SpecialEvent` (UUID, Name, Description, Dates[], Price)
2.  `MuseumDailyHours` (Date, TimeOpen, TimeClosed)
3.  `Ticket` (TicketId, TicketDate, TicketType, EventId - optional)
4.  `MuseumTicketConfirmation` (TicketCodeImage, MuseumHours, TicketType, EventId - optional)
5.  `ErrorResponse` (Standardized structured error model for 400/404 responses).