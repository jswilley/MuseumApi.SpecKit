## Project Constitution: Museum API

| Field | Value |
| :--- | :--- |
| **Project Name** | Museum API |
| **Project Type** | RESTful** |
---

### 1. Governance and Roles

| Role/Status | Details |
| :--- | :--- |
| **Role Structure** | Solo Developer (Primary Role) |
| **Developer Responsibilities** | Responsible for ensuring project alignment with principles. Excels at breaking down complex requirements into detailed, sequential **Product Requirements Documents (PRDs)** using **Agile methodologies**. Primary task is to generate multiple, granular PRDs for a phased project rollout based on user requests. Defines `/specify`, approves `/plan`, executes `/implement`, and ensures constitutional compliance. |
| **Developer Strategy** | **Prioritize Incremental Changes:** Focus on identifying minimal, non-disruptive changes to the existing application foundation. **Avoid rebuilding core functionality.** |
| **Contributor Status** | None (No external contributors will be accepted or processed by the AI agent.) |

---

### 2. Core Principles


1.  **Architectural Consistency:** All technical decisions must align with the defined **Opinionated Stack** and constraints.
2.  **Test-First Development:** Every feature requires clear **Acceptance Criteria** and mandatory **Contract Tests**.
3.  **Generated Code:** Follow **RESTful** design principles.  Where ever possible adhere to **SOLID** principles.
4.  **Maintainability:** Code coverage above 70%.
5. **Observability** All projects must be instrumented using OpenTelemetry standards. Telemetry data is a first-class requirement for all features.  Semantic Conventions: All resources, spans, and attributes must adhere strictly to the OpenTelemetry Semantic Conventions (e.g., db.statement, http.method, net.host.name). Custom attributes must use the project-specific prefix (spec.kit.*).

---

### 3. Architectural Constraints

#### 3.1. API Governance

| Constraint | Details |
| :--- | :--- |
| **Compliance** | **Mandatory Contract Tests** (e.g., Specmatic) must run pre-merge to verify implementation against the OpenAPI contract. |
| **HTTP Standards** | All endpoints must consistently return **standard HTTP status codes** (200, 201, 400, 404, etc.) to ensure clarity.  Use the correct HTTP verbs. Use MapGet for retrieving data, MapPost for creating, MapPut/MapPatch for updating, and MapDelete for deleting resources. |
| **Variables** |  Keep application settings in appsettings.json and environment-specific overrides in appsettings.Development.json, appsettings.Production.json. |
| **Error Handling** | Centralized Exception Handling: Implement global exception handling middleware to return consistent error responses (e.g., Problem Details RFC 7807).  Use Serilog for implementation. Meaningful Error Messages: Provide clear and concise error messages without exposing sensitive information. Appropriate HTTP Status Codes: Use correct HTTP status codes to indicate the nature of the error (e.g., 400 Bad Request, 404 Not Found, 500 Internal Server Error).. |
| **Naming Convention** | CamelCase for Parameters: Use camelCase for route parameters and query string parameters.
PascalCase for Types and Methods: Follow standard C# naming conventions for classes, interfaces, and methods. |
| **Input Validation** | Input Validation: Implement validation for incoming request data using attributes (e.g., [Required], [StringLength]) or a custom validation pipeline.
Avoid Trusting Client Input: Always validate data received from clients before processing it. |
| **Authentication and Authorization** | Middleware for Global Auth: Configure app.UseAuthentication() and app.UseAuthorization() in Program.cs.
Endpoint-Level Authorization: Use [Authorize] attributes or policies to secure specific endpoints. |
| **Observability** | The solution must collect the Three Pillars of Observability:
    ◦ Traces: Mandatory for all cross-service and database calls.
    ◦ Metrics: Mandatory for all business-critical operations (e.g., plan creation count, kit inventory level).
    ◦ Logs: Mandatory structured logging with trace context correlation. Structured Logging: Use a logging framework (Serilog) for structured and informative logging. Appropriate Log Levels: Use different log levels (Debug, Information, Warning, Error) to categorize logs effectively.  The serilog configuration should be kept in the appsettings.json file.
    ◦ If Blazor Wasm is involved, Every API request originating from the Blazor WASM client must propagate its Trace Context to the Minimal API backend to enable end-to-end trace correlation.  Context Propagation: The W3C Trace Context protocol (traceparent and tracestate headers) must be used for all inter-service communication (Blazor WASM → Minimal API).
    ◦ Semantic Conventions: All resources, spans, and attributes must adhere strictly to the **OpenTelemetry Semantic Conventions** (e.g., db.statement, http.method, net.host.name). Custom attributes must use the project-specific prefix (spec.kit.*).
| **Code Readability** | Concise Lambdas: Leverage the conciseness of lambda expressions in Minimal APIs but ensure they remain readable.
Comments (when necessary): Add comments to explain complex logic or non-obvious design choices. Consistent Formatting: Adhere to a consistent code formatting style (e.g., using dotnet format or an IDE's formatting tools). |
| **Documention** | Use OpenAPI/Swagger. Always use the built-in Swagger integration to generate clear, discoverable documentation. This makes it easier for other developers to use your API. |
| **Request and Response Standardization** | Use records for data transfer objects (DTOs). Define specific record types for your requests and responses. This ensures clear, immutable contracts and avoids exposing internal domain models. Enforce validation. Use data annotations ([Required], [Range], etc.) on your record types. The Minimal API model binder will automatically handle validation. Return typed results. Instead of returning raw objects, use Results<T, TError> or other IResult factory methods (e.g., Results.Ok(), Results.NotFound()) to ensure consistent HTTP responses. |


#### 3.2. Opinionated Stack

| Component | Technology / Rule |
| :--- | :--- |
| **Language/Framework** | C# Minimal API Project .net 9 or above. |
| **Database** | Sqlite |
| **Dependency Management** | All business logic must be encapsulated in **injectable services** (**Dependency Injection is Mandatory**). |
| **Code Standards** | **No inline business logic** is permitted directly in API route definitions or `app.Map*` calls. Separate Concerns: While Minimal APIs can be concise, avoid cramming all logic into Program.cs. For larger applications, separate endpoint definitions and their implementations into distinct files or classes.  Endpoint Groups: Utilize MapGroup to organize related endpoints under a common prefix, improving clarity and reducing repetition.  Leverage extension methods. To keep Program.cs clean as the API grows, create extension methods (e.g., builder.Services.AddMyService()) to register complex service collections.|
| **Coding Style** | Favor implicit typing (var). Use var for local variables to reduce noise, especially when the type is obvious from the right-hand side. Align and organize chains. When chaining multiple calls on an endpoint (e.g., .WithName().WithSummary()), format each call on a new line for improved readability. Use a consistent naming convention for variables, parameters, and methods. Follow the standard C# naming conventions. Add comments where necessary. Use XML comments on methods to document what they do, their parameters, and what they return. This improves the generated OpenAPI documentation.|
| **Api Versioning** | use Asp.Versioning.Http to create api versioning support in program.cs file.  Use URI path versioning.  Use semantic versioning: Label breaking changes as a major version bump (e.g., v1 to v2). Deprecate gracefully: When retiring an old version, announce a deprecation timeline and notify clients. Document all versions: Provide clear and separate documentation for every version of your API. Use MapGroup and extension methods: For larger applications, organize your versioned endpoints into separate files and use MapGroup to apply the version set, as shown in the primary example.|Please see apiversion.md for specific details on version implementation.


#### 3.3. Performance Requirements

| Requirement | Rule |
| :--- | :--- |
| **I/O Operations** | All I/O-bound operations must use **asynchronous programming** (`async`/`await` or equivalent). |

---

### 4. Development Standards

#### 4.1. Quality Gates

| Standard | Rule |
| :--- | :--- |
| **Testing Priority** | **Integration tests** using in-memory runners (e.g., `WebApplicationFactory` for C#) are the preferred testing method. |
| **Coverage Minimum** | Maintain a minimum automated test coverage percentage of **70%**. |
| **Documentation Status** | Generated artifacts (`spec.md`, `plan.md`, `constitution.md`) are considered **living documents** that must be synchronized with the codebase. |

#### 4.2 Version Control and Git Standards

These standards are designed to maintain a clean, stable `main` branch and ensure a consistent, auditable development process.

-----

##### 4.2.1 Branch Naming Conventions 🏷️

All new branches must adhere to a specific naming format to indicate their purpose and originator clearly.

| Branch Type | Prefix | Format | Example | Purpose |
| :--- | :--- | :--- | :--- | :--- |
| **Feature** | `feature/` | `feat/<short-description>` | `feat/user-auth` | New functionality or major addition. |
| **Bugfix** | `bugfix/` | `fix/<short-description>` | `fix/login-crash` | A fix for a recognized issue/bug. |
| **Hotfix** | `hotfix/` | `hotfix/<issue-id-or-description>` | `hotfix/critical-api-bug` | Urgent fix for a production issue. |
| **Chore/Refactor** | `chore/` or `refactor/` | `chore/<description>` | `chore/update-deps` | Routine maintenance, build updates, or code restructuring without changing external behavior. |
| **Release** | `release/` | `release/<description>` | `release/api-usage` | Release branch. |

  * **Separators:** Use **hyphens** (`-`) to separate words within the `<short-description>`. **Do not** use underscores (`_`) or spaces.
  * **Case:** All branch names **must** be in **lowercase**.

-----

##### 4.2.2 Branch Management and Checkout 🔄

###### **4.2.2.1. Main Development Branch**

  * The **primary development branch** is designated as **`main`**.
  * All new feature work, bug fixes, and other changes **must** start by branching off the latest state of **`main`**.

###### **4.2.2.2. Creating a New Branch (Checkout Method)**

1.  **Sync `main`:** Ensure your local `main` branch is up-to-date:
    ```bash
    git checkout main
    git pull origin main
    ```
2.  **Create and Checkout:** Create your new branch (using the naming standard) and switch to it:
    ```bash
    git checkout -b <branch-type>/<description>
    ```

###### **4.2.2.3. Keeping Branches Updated**

  * Developers are encouraged to **rebase** their feature branch onto `main` frequently to ensure smooth integration and to minimize merge conflicts.

    ```bash
    git checkout <your-branch>
    git fetch origin
    git rebase origin/main
    ```

-----

##### **4.2.3.. Merging and Pull Request (PR) Requirements 🔒

**Direct merges into the `main` branch are strictly prohibited.** All code changes must follow the Pull Request (PR) workflow to ensure review, testing, and approval.

###### **4.2.3.1. Pull Request Workflow**

1.  **Open a PR:** Once work is complete and tested locally, push the branch to the remote repository and open a Pull Request targeting **`main`**.
2.  **Clear Title and Description:** The PR must have a clear, descriptive title (often following the commit message convention) and a detailed description explaining the changes, **why** they were made, and any relevant issue numbers.
<! -- 3.  **Required Reviews:** Every PR **must** receive approval from at least **one (1)** designated code reviewer before it can be merged. -->
4.  **Continuous Integration (CI):** All automated tests and CI checks **must** pass successfully before a PR can be merged.

###### **4.2.3.2. Merge Method**

  * PRs **must** be merged using the **Squash and Merge** or **Rebase and Merge** strategy (depending on repository settings) to keep the `main` branch history clean and linear. **Do not** use a standard merge commit that preserves all feature branch commits.

  * Once merged, the source branch **must** be deleted from the remote repository.

---

### 5. Decision Making

| Category | Details |
| :--- | :--- |
| **Process** | All decisions are made by the **Developer** and documented in the relevant `plan.md` artifact, ensuring decisions are grounded by this constitution. |
| **Tools** | Spec Kit Commands (`/specify`, `/plan`, `/tasks`), Git/Pull Requests. |
| **LLM Constraint** | Must avoid proposing a complete rebuild of the foundation. Instead: **Highlight specific areas** for adjustment (e.g., "Extend the User model to include Field Z") and provide **backward-compatible solutions** (e.g., "Add optional parameters to existing APIs"). |

### 6. Open Telemetry Coding Principles
#### 6.1 Manual Instrumentation: Manual instrumentation (creating custom spans/activities) is mandatory for any complex business operation that spans multiple layers (e.g., inside the repository or handler logic).
#### 6.2 Attribute Standard: All custom spans must include the following attributes:
    ◦ spec.kit.feature: The high-level feature being executed (e.g., PlanCreation).
    ◦ spec.kit.user_id: The authenticated user's ID (Server-side only).
#### 6.3 No Sensitive Data: No telemetry payload (attributes or log messages) must contain Personally Identifiable Information (PII) or secrets (e.g., passwords, connection strings, API keys).

### 7. Configuration and Export 
####  7.1 Export Protocol: Data must be exported using the OTLP (OpenTelemetry Protocol) via gRPC or HTTP/protobuf. HTTP is the preferred transport for the Blazor WASM client due to browser constraints.
####  7.2 Environment Variables: The configuration for the OTLP Exporter endpoint must be driven by standard OpenTelemetry environment variables:
    ◦ OTEL_EXPORTER_OTLP_ENDPOINT
    ◦ OTEL_RESOURCE_ATTRIBUTES
    ◦ OTEL_SERVICE_NAME
#### 7.3 Service Naming: The application components must be named consistently using the convention {Environment}.{Component}.{Service}:
    ◦ Client: Prod.SpecKit.BlazorWASM
    ◦ Server: Prod.SpecKit.MinimalAPI
####  7.4 Logging Correlation: Serilog (or standard .NET logging) must be used on the server, configured to include the TraceId and SpanId in every log event to enable correlation with traces.