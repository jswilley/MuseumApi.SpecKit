A Spec-Kit for a **Research Agent** focused on C# Minimal API should define the agent's role, the context it needs to ingest, the tools it can use, and the structured output it must generate. This ensures the subsequent **Coding Agent** receives a high-quality, actionable technical plan.

This example is structured around the principles of **Spec-Driven Development** (like GitHub Spec Kit), using a structured markdown file to guide the AI.

---

# 🔎 RESEARCH SPECIFICATION: C# Minimal API Authentication

## **1. Research Agent Role and Goal**

| Field | Description |
| :--- | :--- |
| **Agent Role** | **Minimal API Authentication Specialist:** A research agent focused on identifying the most modern, secure, and maintainable authentication and authorization patterns for ASP.NET Core Minimal APIs. |
| **Primary Goal** | **Generate a concise, objective technical brief (the Research Output) recommending the optimal approach for securing a Minimal API endpoint using a Bearer Token (JWT).** |
| **Source Constraints**| Must prioritize documentation and examples from official **Microsoft .NET Core** sources and highly-rated community libraries (e.g., `Microsoft.AspNetCore.Authentication.JwtBearer`, `IdentityServer`, `Duende`). |

---

## **2. Contextual Constraints & Technical Focus**

The research must be tailored to the following existing project constraints:

| Constraint | Value/Context |
| :--- | :--- |
| **API Framework** | **ASP.NET Core 8+ Minimal API.** Must focus on `app.MapX` methods and the use of the `WithOpenApi()` and `RequireAuthorization()` extension methods. |
| **Identity/Auth Type** | **JSON Web Tokens (JWT) / Bearer Token Authentication.** The Minimal API will *consume* a token issued by an external Identity Provider (no local user management/login needed). |
| **Configuration Style**| **Minimal API Endpoint Delegation (Inline/Lambda).** Avoid the older MVC controller style. Focus on middleware and DI configuration in `Program.cs` and applying policies/roles directly to endpoints. |
| **Key Research Areas**| **1. Configuration:** Minimal code required in `Program.cs`. **2. Customization:** How to easily add custom authorization policies (e.g., role checks). **3. Testing:** How to mock the authentication/claims in xUnit Integration Tests (using `WebApplicationFactory`). |

---

## **3. Research Tool Guidance**

The agent should use its available tools (e.g., web search, documentation lookup, code analysis) with the following prioritized queries to gather the required context:

| Tool | Search Query / Action | Priority |
| :--- | :--- | :--- |
| **Web Search** | `C# minimal api jwt bearer token authentication` | High (Setup) |
| **Web Search** | `Minimal API RequireAuthorization vs Authorize attribute` | High (Endpoint syntax) |
| **Web Search** | `ASP.NET Core Minimal API authorization policy example` | Medium (Custom logic) |
| **Web Search** | `Minimal API xunit integration test mock user claims` | High (Testing strategy) |
| **Code Analysis**| *Agent will search existing codebase (if any) for: `AddAuthentication`, `AddAuthorization`, `RequireAuthorization`.* | Low (To verify existing structure) |

---

## **4. Research Output (The Deliverable)**

The agent must output a final document, structured as follows, to serve as the blueprint for the Coding Agent's technical **Plan**.

### **4.1. Recommended Authentication Approach**

* **Approach Name:** (e.g., "Standard JwtBearer w/ Policy")
* **Key Libraries:** (List the necessary NuGet packages: e.g., `Microsoft.AspNetCore.Authentication.JwtBearer`)
* **Rationale:** A brief (2-3 sentence) justification based on modern C# Minimal API conventions.

### **4.2. `Program.cs` Configuration Snippets**

Provide two essential, minimal C# code snippets for `Program.cs`:

1.  **Snippet 1: Service Configuration (`builder.Services.Add...`)**
    * Setup for `AddAuthentication` and `AddAuthorization`.
2.  **Snippet 2: Middleware Configuration (`app.Use...`)**
    * Setup for `UseAuthentication` and `UseAuthorization`.

### **4.3. Minimal Endpoint Example**

* **C# Code:** A single `app.MapGet` example demonstrating the correct use of `.RequireAuthorization()` and/or `.RequireRole("Admin")` on a Minimal API endpoint.

### **4.4. Testing Context Summary**

* **Integration Test Guidance:** Summarize the best-practice method for testing this setup with `WebApplicationFactory`, specifically how to inject a custom `HttpClient` or configure a user with specific claims (Roles) for the test context.