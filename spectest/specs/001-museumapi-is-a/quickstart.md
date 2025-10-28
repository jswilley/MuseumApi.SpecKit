# Quick Start Guide: Museum API

**Feature**: Museum API  
**Date**: October 21, 2025  
**Audience**: Developers setting up local development environment

## Prerequisites

- .NET 8.0 SDK or later ([Download](https://dotnet.microsoft.com/download))
- IDE: Visual Studio 2022, VS Code, or JetBrains Rider
- SQLite (included with .NET, no separate install needed)
- Git (for source control)
- Postman, curl, or similar tool for API testing (optional)

## Getting Started

### 1. Create the Project

```bash
# Create solution and API project
dotnet new sln -n MuseumApi
dotnet new webapi -n MuseumApi --use-minimal-apis
dotnet sln add MuseumApi/MuseumApi.csproj

# Create test project
dotnet new xunit -n MuseumApi.Tests
dotnet sln add MuseumApi.Tests/MuseumApi.Tests.csproj
dotnet add MuseumApi.Tests reference MuseumApi

cd MuseumApi
```

### 2. Install Dependencies

```bash
# Entity Framework Core with SQLite provider
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design

# OpenAPI/Swagger documentation
dotnet add package Swashbuckle.AspNetCore
dotnet add package microsoft.aspnetcore.openapi
dotnet add package microsoft.openapi

# Microsoft Standard Extensions
dotnet add package microsoft.extensions.configuration
dotnet add pacakge microsoft.extensions.dependencyinjection

# Serilog
dotnet add package Serilog.Aspnetcore
dotnet add package Serilog.Extensions.logging
dotnet add package Serilog.Formatting.compact
dotnet add package Serilog.Settings.configuration
dotnet add package Serilog.Sinks.console
dotnet add package Serilog.Sinks.debug
dotnet add package Serilog.Sinks.file

#opentelemetry
• OpenTelemetry.Extensions.Hosting (Core setup and configuration)
• OpenTelemetry.Exporter.Otlp (For exporting data to the collector)
• OpenTelemetry.Instrumentation.AspNetCore (Auto-instrumentation for HTTP requests)
• OpenTelemetry.Instrumentation.Runtime (Metrics for CPU, memory, garbage collection)
• OpenTelemetry.Instrumentation.Process (Metrics for process-level details)
• OpenTelemetry.Instrumentation.EntityFrameworkCore (Auto-instrumentation for database queries)
• Serilog.AspNetCore (For structured logging integration)
• Serilog.Sinks.OpenTelemetry (To export logs via OTLP and include trace context)

cd ../MuseumApi.Tests

# Testing dependencies
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Moq
dotnet add package Xunit
dotnet add package Xunit.Core
dotnet add package Xunit.Assert
dotnet add package Xunit.Extensibility.Core

```

### 3. Project Structure

Create the following directory structure:

```
MuseumApi/
├── Program.cs
├── MuseumApi.csproj
├── Context/
│   ├── MuseumContext.cs
│   └── DbInitializer.cs
├── Models/
│   ├── MuseumDailyHour.cs
│   ├── SpecialEvent.cs
│   ├── SpecialEventDate.cs
│   └── TicketPurchase.cs
├── DTOs/
│   ├── MuseumHoursDto.cs
│   ├── SpecialEventDto.cs
│   ├── TicketPurchaseRequest.cs
│   └── TicketPurchaseResponse.cs
├── Endpoints/
│   ├── MuseumHoursEndpoints.cs
│   ├── SpecialEventsEndpoints.cs
│   └── TicketPurchaseEndpoints.cs
├── Services/
│   ├── MuseumHoursService.cs
│   ├── IMuseumHoursService.cs
│   ├── SpecialEventsService.cs
│   ├── ISpecialEventsService.cs
│   ├── TicketPurchaseService.cs
│   └── ITicketPurchaseService.cs
├── Security/
│   ├── SecurityPolicies.cs
│   ├── SecurityRoleMap.cs
│   └── SecurityPolicyBuilder.cs
└── Data/
   └── Museum.db

MuseumApi.Tests/
├── IntegrationTests/
│   ├── TestWebApplicationFactory.cs
│   ├── MuseumHoursIntegrationTests.cs
│   ├── SpecialEventsIntegrationTests.cs
│   └── TicketPurchaseIntegrationTests.cs
└── Unit/
    ├── MuseumContextTests.cs
    └── ValidationTests.cs
```

### 4. Configure Database

Add connection string to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=museum.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  ,
 "Serilog": {
   "Using": [
     "Serilog.Sinks.Console",
     "Serilog.Sinks.File"
   ],
   "MinimumLevel": "Information",
   "WriteTo": [
     {
       "Name": "Console"
     },
     {
       "Name": "File",
       "Args": {
         "path": "logs\\MuseumApi.txt",
         "rollingInterval": "Day",
         "shared": true
         /*"outputTemplate": "{Timestamp:o} [{Level:u3}] ({Application}/{MachineName}/{ThreadId}) {Message}{NewLine}{Exception}",*/
         ,
         "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
       }
     }
   ],
   "Enrich": [
     "FromLogContext",
     "WithMachineName",
     "WithThreadId",
     "WithExceptionDetails"
   ],
   "Properties": {
     "Application": "MuseumApi"
   }
 },
  "AllowedHosts": "*"
}
```

### 5. Configure Program.cs

Update `Program.cs` with minimal boilerplate:

```csharp
using Microsoft.EntityFrameworkCore;
using MuseumApi.Data;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;
using Serilog.Settings.Configuration;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;


IConfiguration configuration = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
     .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
     .Build();


var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<MuseumContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

ConfigureTelemetry(builder);
ConfigureLogging(builder); //logging
ConfigureVersion(builder); //versioning
ConfigurePolicies(builder, configuration); //security policies
ConfigureServices(builder)


builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

d 

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    // 3. Configure Swagger UI to show all versions
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        
        // Build a swagger endpoint for each discovered API version
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();

// Map endpoints (will be added in separate files)
// MuseumHoursEndpoints.MapEndpoints(app);
// SpecialEventsEndpoints.MapEndpoints(app);
// TicketPurchaseEndpoints.MapEndpoints(app);

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MuseumContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

app.Run();

static void ConfigureTelemetry(WebApplicationBuilder builder)
{
    
// 1. Configure OpenTelemetry Tracing and Metrics
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true);
        tracing.AddSource("MuseumApi"); // Custom Source for manual spans
        tracing.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]);
        });
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddRuntimeInstrumentation();
        metrics.AddProcessInstrumentation();
        metrics.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri(builder.Configuration["Otlp:Endpoint"]);
        });
    })
    .ConfigureResource(resource =>
        resource.AddService(
            serviceName: builder.Configuration["Otlp:ServiceName"],
            serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown"
        ));
}

static void ConfigureLogging(WebApplicationBuilder builder)
{
    LoggerProviderCollection Providers = new();
    //how do I use ConfigurationReaderOptions with Serilog?
    
    var options = new ConfigurationReaderOptions() { SectionName = "Serilog" };

    
    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration, options)
                             .Enrich.FromLogContext()
                            .WriteTo.Providers(Providers)
                             .WriteTo.Console(new RenderedCompactJsonFormatter())
                             .CreateLogger();
    builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration, options)
                            .Enrich.FromLogContext()
                            .WriteTo.Providers(Providers)
                            .WriteTo.Console(new RenderedCompactJsonFormatter()
                           .WriteTo.OpenTelemetry(opts =>
                            {
                                opts.Endpoint = builder.Configuration["Otlp:Endpoint"];
                                opts.Protocol = OtlpProtocol.Grpc;
                            })
     ));
}

static void ConfigureVersion(WebApplicationBuilder builder)
{

// 1. Configure API Versioning 🎯
builder.Services.AddApiVersioning(options =>
{
    // Report the supported API versions in the response headers
    options.ReportApiVersions = true;
    
    // Set a default version if the client doesn't specify one
    options.DefaultApiVersion = new ApiVersion(1, 0); 
    options.AssumeDefaultVersionWhenUnspecified = true;
})
.AddMvc() // Use Mvc's support for versioning attributes
.AddApiExplorer(options =>
{
    // Format the version as 'vMajor.Minor' (e.g., v1.0, v2.1)
    options.GroupNameFormat = "'v'VVV"; 
    
    // Note: The Minimal API approach typically infers versions from routing, 
    // but AddApiExplorer is essential for Swagger integration.
    options.SubstituteApiVersionInUrl = true;
});


// 2. Configure OpenAPI/Swagger Generation 📜
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Resolve IApiVersionDescriptionProvider from DI
    var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

    // Add a Swagger document for each discovered API version
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, new OpenApiInfo()
        {
            Title = $"Museum API {description.ApiVersion}",
            Version = description.ApiVersion.ToString(),
            Description = description.IsDeprecated 
                ? $"**DEPRECATED**: This version is no longer recommended." 
                : "The current stable API version."
        });
    }

    // Set up the operation filter to ensure each endpoint is associated with its document
    options.OperationFilter<SwaggerDefaultValues>();
});
}

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IMuseumHoursService, MuseumHoursService>();
    builder.Services.AddScoped<ISpecialEventsService, SpecialEventsService>();
    builder.Services.AddScoped<ITicketPurchaseService, TicketPurchaseService>();
}

static void ConfigurePolicies(WebApplicationBuilder builder, IConfiguration configuration)
{
    HttpClientPolicyConfiguration policyConfigs = new();
        Policies.InitPolicies(configuration);

        builder.Services.AddAuthorization(config =>
        {
            config.AddPolicy(Policies.IsAdmin, Policies.AdminPolicy());
            config.DefaultPolicy = Policies.UserPolicy();

            // When no policy is specified.
            config.FallbackPolicy = Policies.UserPolicy();
        });
    

builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy", builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


}

// 5. Helper Class (often in a separate file, but included here for completeness)
/// <summary>
/// This is a required OperationFilter to ensure the 'api-version' parameter 
/// and other versioning metadata is correctly reflected in the Swagger UI.
/// </summary>
public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated = apiDescription.IsDeprecated();
        
        // NOTE: With URL-based versioning, the 'api-version' parameter 
        // is typically inferred from the routing template {version:apiVersion}.
        // This ensures the documentation is correct for the chosen version.
        
        var apiVersion = apiDescription.GetApiVersion();
        
        // Remove the standard, non-versioned route parameter, which 
        // conflicts with the versioned route parameter defined in the MapGet.
        var versionParameter = operation.Parameters.FirstOrDefault(p => p.Name == "api-version" && p.In == ParameterLocation.Query);
        if (versionParameter != null)
        {
            operation.Parameters.Remove(versionParameter);
        }
    }
}

// Make Program class accessible for integration tests
public partial class Program { }
```

#### 5.1 Configuration SecurityPolicyBuilder.cs
Update `SecurityPolicyBuilder.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;

namespace MuseumApi.Security;

    public class GroupMembershipPolicyBuilder
    {
        private List<string> groups;
        private string _securityEnvironment { get; }
        public GroupMembershipPolicyBuilder(IConfiguration configuration)
        {
            groups = new List<string>();
            _securityEnvironment = configuration.GetValue<string>("SecurityEnvironment");
        }

        public GroupMembershipPolicyBuilder InAnyRoleGroup()
        {
            groups = new List<string>
            {
                RoleGroup.Administrators + _securityEnvironment,
                RoleGroup.Users + _securityEnvironment,
            };
            return this;
        }
        public GroupMembershipPolicyBuilder MemberOf(string group)
        {
            groups = new List<string> { group + _securityEnvironment };
            return this;
        }
        public GroupMembershipPolicyBuilder Or(string group)
        {
            groups.Add(group + _securityEnvironment);
            return this;
        }
        public GroupMembershipPolicyBuilder Except(string group)
        {
            groups.Remove(group + _securityEnvironment);
            return this;
        }
        public AuthorizationPolicy Build()
        {
            return new AuthorizationPolicyBuilder().RequireRole(groups.OrderBy(a => a).Distinct()).RequireAuthenticatedUser().Build();
        }
    }

#### 5.2 Configuration SecurityPolicies.cs
Update 'SecurityPolicies.cs':
```csharp
namespace MuseumApi.Security;
public static class Policies
 {
     private static IConfiguration configuration;
     public static void InitPolicies(IConfiguration _configuration)
     {
         configuration = _configuration;
     }

     public const string IsAdmin = "IsAdmin";
     public const string IsUser = "IsUser";

     public static AuthorizationPolicy AdminPolicy()
     {
         return new GroupMembershipPolicyBuilder(configuration)
             .MemberOf(RoleGroup.Administrators)                
             .Build();
     }

     public static AuthorizationPolicy UserPolicy()
     {
         return new GroupMembershipPolicyBuilder(configuration)
             .InAnyRoleGroup()
             .Build();
     }
 }

#### 5.2 Configuration SecurityPolicies.cs
Update 'SecurityPolicies.cs':
```csharp
namespace MuseumApi.Security;
public static class Policies
 {
     private static IConfiguration configuration;
     public static void InitPolicies(IConfiguration _configuration)
     {
         configuration = _configuration;
     }

     public const string IsAdmin = "IsAdmin";
     public const string IsUser = "IsUser";

     public static AuthorizationPolicy AdminPolicy()
     {
         return new GroupMembershipPolicyBuilder(configuration)
             .MemberOf(RoleGroup.Administrators)                
             .Build();
     }

     public static AuthorizationPolicy UserPolicy()
     {
         return new GroupMembershipPolicyBuilder(configuration)
             .InAnyRoleGroup()
             .Build();
     }
 }

#### 5.3 Configure SecurityRoleMap.cs
Update 'SecurityRoleMap.cs':
```csharp
namespace MuseumApi.Security;

 public static class RoleGroup
  {
      public const string Administrators = "MuseumAdmin";
      public const string Users = "MuseumUser";
      
  }



### 6. Create Initial Migration

```bash
# From MuseumApi directory
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 7. Run the Application

```bash
# From solution root
dotnet run --project MuseumApi

# Or use watch mode for auto-reload during development
dotnet watch run --project MuseumApi
```

The API will start on `http://localhost:5000` and `https://localhost:5001`.

### 8. Access Swagger UI

Open browser to `https://localhost:5001/swagger` to:
- View API documentation
- Test endpoints interactively
- See request/response schemas

## Quick Test

### Test Museum Hours Endpoint

```bash
# Get museum hours for today
curl http://localhost:5000/museumhours

# Get hours for specific date
curl "http://localhost:5000/museumhours?date=2025-10-21"
```

Expected response:
```json
[
  {
    "date": "2025-10-21",
    "timeOpen": "09:00:00",
    "timeClosed": "17:00:00"
  }
]
```

### Test Special Events Endpoint

```bash
# List all special events
curl http://localhost:5000/specialevents

# Get specific event
curl http://localhost:5000/specialevents/{eventId}
```

### Test Ticket Purchase

```bash
# Purchase general admission tickets
curl -X POST http://localhost:5000/tickets/purchase \
  -H "Content-Type: application/json" \
  -d '{
    "visitDate": "2025-10-21",
    "quantity": 2,
    "eventId": null
  }'
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test project
dotnet test MuseumApi.Tests

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverageReporter=html
```

## Development Workflow

### 1. Write Tests First (TDD)

```bash
# Create new test file
touch MuseumApi.Tests/IntegrationTests/NewFeatureTests.cs

# Run tests (they should fail - RED)
dotnet test

# Implement feature
# Edit MuseumApi/... files

# Run tests again (they should pass - GREEN)
dotnet test

# Refactor if needed
```

### 2. Add New Endpoint

1. Define DTO models in `DTOs/`
2. Create endpoint file in `Endpoints/`
3. Map endpoints in `Program.cs`
4. Write integration tests in `MuseumApi.Tests/IntegrationTests/`
5. Update OpenAPI contract in `contracts/openapi.yaml`

### 3. Database Changes

```bash
# Add new entity to Models/
# Update MuseumContext.cs

# Create migration
dotnet ef migrations add DescriptiveMigrationName --project MuseumApi

# Review generated migration in Migrations/

# Apply migration
dotnet ef database update --project MuseumApi

# To rollback
dotnet ef database update PreviousMigrationName --project MuseumApi
```

### 4. Seed Data

Edit `Data/DbInitializer.cs` to add sample data:

```csharp
public static class DbInitializer
{
    public static void Initialize(MuseumContext context)
    {
        if (context.MuseumDailyHours.Any())
            return; // Already seeded

        // Add museum hours
       context.MuseumDailyHour.AddRange
    (
        new MuseumDailyHour() {
        Date = "2025-10-11",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() {  
        Date = "2025-10-12",
        TimeOpen = "09:00",
        TimeClose = "18:00"

        },
        new MuseumDailyHour() { 
            Date = "2025-09-13",
            TimeOpen = "09:00",
            TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-09-14",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-09-15",
        TimeOpen = "10:00",
        TimeClose = "16:00" },
        new MuseumDailyHour() { 
        Date = "2025-09-18",
        TimeOpen = "09:00",
        },
        new MuseumDailyHour() { Date = "2025-09-19",
        TimeOpen = "09:00",
        TimeClose = "18:00"  },

        new MuseumDailyHour() {  ate = "2025-09-20",
        TimeOpen = "09:00",
        TimeClose = "18:00"},
        new MuseumDailyHour() { Date = "2025-09-21",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-09-22",
        TimeOpen = "09:00",
        TimeClose = "18:00"  },
        new MuseumDailyHour() {
        Date = "2025-09-23",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() {  
        Date = "2025-09-24",
        TimeOpen = "09:00",
        TimeClose = "18:00"

        },
        new MuseumDailyHour() { 
            Date = "2025-09-25",
            TimeOpen = "09:00",
            TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-09-26",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-09-27",
        TimeOpen = "10:00",
        TimeClose = "16:00" },
        new MuseumDailyHour() { 
        Date = "2025-09-28",
        TimeOpen = "09:00",
         TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-09-29",
        TimeOpen = "09:00",
        TimeClose = "18:00"  },
        new MuseumDailyHour() {  ate = "2025-09-30",
        TimeOpen = "09:00",
        TimeClose = "18:00"},
        new MuseumDailyHour() { Date = "2025-10-01",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-10-02",
        TimeOpen = "09:00",
        TimeClose = "18:00"  },
        new MuseumDailyHour() {  
        Date = "2025-09-12",
        TimeOpen = "09:00",
        TimeClose = "18:00"

        },
        new MuseumDailyHour() { 
            Date = "2025-010-03",
            TimeOpen = "09:00",
            TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-10-04",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-10-05",
        TimeOpen = "10:00",
        TimeClose = "16:00" },
        new MuseumDailyHour() { 
        Date = "2025-10-18",
        TimeOpen = "09:00",
        TimeClose = "16:00" },
        new MuseumDailyHour() { Date = "2025-10-09",
        TimeOpen = "09:00",
        TimeClose = "18:00"  },
            new MuseumDailyHour() {  ate = "2025-10-10",
        TimeOpen = "09:00",
        TimeClose = "18:00"},
        new MuseumDailyHour() { Date = "2025-10-11",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-10-12",
        TimeOpen = "09:00",
        TimeClose = "18:00"  },
        new MuseumDailyHour() {
        Date = "2025-10-13",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() {  
        Date = "2025-10-14",
        TimeOpen = "09:00",
        TimeClose = "18:00"

        },
        new MuseumDailyHour() { 
            Date = "2025-10-15",
            TimeOpen = "09:00",
            TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-10-16",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-10-17",
        TimeOpen = "10:00",
        TimeClose = "16:00" },
        new MuseumDailyHour() { 
        Date = "2025-10-18",
        TimeOpen = "09:00",
        TimeClose = "16:00" 
        },
        new MuseumDailyHour() { Date = "2025-10-19",
        TimeOpen = "09:00",
        TimeClose = "18:00"  },
        new MuseumDailyHour() {  ate = "2025-10-30",
        TimeOpen = "09:00",
        TimeClose = "18:00"},
        new MuseumDailyHour() { Date = "2025-10-31",
        TimeOpen = "09:00",
        TimeClose = "18:00"
        },
        new MuseumDailyHour() { Date = "2025-11-02",
        TimeOpen = "09:00",
        TimeClose = "18:00"  }
    );

context.SpecialEvents.AddRange(
	new SpecialEvent {
		EventID = Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97"),
		EventName = "Sasquatch Ballet",
		EventDescription = "They're big, they're hairy, but they're also graceful. Come learn how the biggest feet can have the lightest touch.",
		Price = 15.00
	},
	new SpecialEvent {
		EventID = Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98"),
		EventName = "Solar Telescope Demo",
		EventDescription = "Look at the sun without going blind!",
		Price = 10.00
	},
	new SpecialEvent {
		EventID = Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e77"),
		EventName = "Cook Like a Caveman",
		EventDescription = "Learn how to cook on open flame.",
		Price = 20.00
	},
	new SpecialEvent {
		EventID = Guid.Parse("dad4bce8-f5cb-4078-a211-995864315e39"),
		EventName = "Mermaid Treasure Identification and Analysis",
		EventDescription = "Join us as we review and classify a rare collection of 20 thingamabobs, gadgets, gizmos, whoosits, and whatsits — kindly donated by Ariel.",
		Price = 10.00
	},
	new SpecialEvent {
		EventID = Guid.Parse("6744a0da-4121-49cd-8479-f8cc20526495"),
		EventName = "Time Traveler Tea Party",
		EventDescription = "Sip tea with important historical figures.",
		Price = 30.00
	}
);
context.SpecialEventDates.AddRange(
    new SpecialEventDate()
    {EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97"),
        Date= "09/11/2025"},
    new SpecialEventDate() {
        EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97"),
        Date= "09/12/2025"
    }, 
    new SpecialEventDate() { EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97"),
        Date= "09/13/2025"},
    new SpecialEventDate() { EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98"),
        Date= "09/11/2025" },	
    new SpecialEventDate() { EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98"),
        Date= "09/14/2025"},	
    new SpecialEventDate() { 	EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98"),
        Date= "09/15/2025"},	
    new SpecialEventDate() { EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e77"),	
        Date= "09/16/2025"},	
    new SpecialEventDate() { 	EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e77"),	
        Date= "09/17/2025" },
    new SpecialEventDate() {EventID= Guid.Parse("dad4bce8-f5cb-4078-a211-995864315e39"),
        Date= "09/17/2025"},
    new SpecialEventDate() {EventID= Guid.Parse("dad4bce8-f5cb-4078-a211-995864315e39"),
        Date= "09/18/2025" },
    new SpecialEventDate() { EventID= Guid.Parse("6744a0da-4121-49cd-8479-f8cc20526495"),	
        Date= "09/19/2025" }	
);


        context.SaveChanges();
    }
}
```

### OpenTelemetry Manual Instrumentation and Naming
Component
Action
Code Requirement
Minimal API Handlers
Add manual spans for critical business logic paths (e.g., inside CreatePlanHandler).
Use ActivitySource.StartActivity() and include the mandatory spec.kit.feature attribute.
Health Check
Must be excluded from general logging/tracing overhead.
Use app.UseOpenTelemetryPrometheusScrapingEndpoint(context => context.Request.Path == "/metrics") for metrics, but ensure the health endpoint (/health) is suppressed from AspNetCoreInstrumentation.
Configuration
Define OTLP endpoint and service name in appsettings.json.
Must include a section for Otlp with Endpoint and ServiceName keys, matching the names used in Program.cs.
--------------------------------------------------------------------------------
### OpenTelemetry Feature-Specific Metrics
Implement a custom Meter in the application logic to capture business metrics.
Metric Name
Type
Unit
Description
spec.kit.plan.created_total
Counter
{plans}
Incremented upon successful plan creation.
spec.kit.kit.inventory_level
Gauge
{kits}
Reports the current stock level of a specific kit type.
Requirement: The agent must create a static Meter instance and use it to record these specific metrics within the corresponding handler logic.


## Troubleshooting

### Database Issues

**Problem**: `SqliteException: no such table`
```bash
# Solution: Ensure database is created and migrations applied
dotnet ef database drop --project MuseumApi  # Warning: deletes all data
dotnet ef database update --project MuseumApi
```

**Problem**: Migration errors
```bash
# Solution: Remove bad migration and start fresh
dotnet ef migrations remove --project MuseumApi
dotnet ef migrations add NewMigrationName --project MuseumApi
```

### Port Conflicts

**Problem**: Port 5000/5001 already in use
```bash
# Solution: Change port in Properties/launchSettings.json
# Or kill the process using the port
lsof -ti:5000 | xargs kill -9  # macOS/Linux
```

### Test Failures

**Problem**: Integration tests fail with database errors
- Ensure `TestWebApplicationFactory` uses separate test database  do not use an in memory sqlite db.  file-based SQLite database for the tests
- Check that tests are properly isolated (each test gets fresh context)

## Production Deployment

### Prerequisites
- SQL Server, PostgreSQL, or MySQL for production database
- Web server (IIS, Nginx, or cloud platform)
- Environment variables configured

### Steps

1. **Update connection string** for production database
2. **Disable Swagger** in production (remove from Program.cs or use environment check)
3. **Enable HTTPS** with valid certificates
4. **Configure authentication** (JWT, OAuth, etc.)
5. **Set up monitoring** (Application Insights, Seq, etc.)
6. **Apply migrations** to production database

```bash
# Publish application
dotnet publish -c Release -o ./publish

# Run database migrations
dotnet ef database update --project MuseumApi --connection "ProductionConnectionString"
```

### Docker Deployment

Create `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MuseumApi/MuseumApi.csproj", "MuseumApi/"]
RUN dotnet restore "MuseumApi/MuseumApi.csproj"
COPY . .
WORKDIR "/src/MuseumApi"
RUN dotnet build "MuseumApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MuseumApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MuseumApi.dll"]
```

Build and run:

```bash
docker build -t museumapi .
docker run -d -p 8080:80 -e ConnectionStrings__DefaultConnection="..." museumapi
```

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [Minimal APIs Overview](https://docs.microsoft.com/aspnet/core/fundamentals/minimal-apis)
- [xUnit Documentation](https://xunit.net)
- OpenAPI contract: `specs/001-museumapi-is-a/contracts/openapi.yaml`
- Data model: `specs/001-museumapi-is-a/data-model.md`
- Technical decisions: `specs/001-museumapi-is-a/research.md`

## Next Steps

1. Review the [data model documentation](./data-model.md)
2. Review the [API contracts](./contracts/openapi.yaml)
3. Run `/speckit.tasks` to generate implementation tasks
4. Begin test-driven development workflow

## Support

For questions or issues:
- Review specification: `specs/001-museumapi-is-a/spec.md`
- Check research decisions: `specs/001-museumapi-is-a/research.md`
- Consult API contracts: `specs/001-museumapi-is-a/contracts/openapi.yaml`
