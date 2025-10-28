**Purpose**: Project initialization and basic structure

- [ ] T001 Create project structure per implementation plan
- [ ] T002 Initialize [language] project with [framework] dependencies
- [ ] T003 [P] Configure linting and formatting tools

***

## 🏛️ Speckit Task: FR-000 Infrastructure setup

| Attribute | Value |
| :--- | :--- |
| **Feature ID** | FR-000 |
| **Feature Name** | Project Setup |
| **Priority** | High |


***

### 🎯 Goal (User Story)

The goals is to setup the project in preparation for the more specific tasks to come.

***

### 🛠️ Technical Specification/Foundational Tasks


#### **Visual Studio Project Creation**
Create a Visual Studio 2022 project for ASP.Net Core minimal API using HTTPS.  The Project name will be MuseumApi.csproj

##### **MuseumApi.csproj structure** 
MuseumApi.csproj/
├── models/
├── services/
├── endpoints/
└── data/
└── context/
└── security/

Create another Visual Studio 2022 xunit project called MuseumApi.Test.csproj.  

MuseumApi.Tests.csproj/
├── integration/
└── unit/

#### **Visual Nuget Provisioning**
For the MuseumApi.csproj:
Download a copy of SQL Lite and store it in a subdirectory called /Data.
Run terminal command: dotnet tool install --global dotnet-ef
Run terminal command: export PATH="$PATH:/home/mcjsw/.dotnet/tools"
Add nuget packages for entityframework core sqlite.
Add nuget packages for Serilog AspNet.Core and File sinks.
Add Asp.Versioning.Http and Microsoft.AspNetCore.OpenApi to create api versioning support in program.cs file
Add OpenApi.Extensions Add code similar to below in program.cs
   // Program.cs

using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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
            Title = $"My Minimal API {description.ApiVersion}",
            Version = description.ApiVersion.ToString(),
            Description = description.IsDeprecated 
                ? $"**DEPRECATED**: This version is no longer recommended." 
                : "The current stable API version."
        });
    }

    // Set up the operation filter to ensure each endpoint is associated with its document
    options.OperationFilter<SwaggerDefaultValues>();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
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


// 4. Define Minimal API Endpoints with Versioning 🌐
// Version 1.0 endpoint
app.MapGet("/api/v{version:apiVersion}/weatherforecast", (HttpContext context) =>
{
    return new[] { new { Date = DateTime.Now, TemperatureC = 25, Summary = "Mild v1" } };
})
.WithApiVersionSet(app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .Build())
.MapToApiVersion(new ApiVersion(1, 0))
.WithName("GetWeatherForecastV1")
.WithOpenApi();

// Version 2.0 endpoint
app.MapGet("/api/v{version:apiVersion}/weatherforecast", (HttpContext context) =>
{
    return new[] { new { Date = DateTime.Now, TemperatureC = 30, Summary = "Hot v2", WindSpeed = 15 } };
})
.WithApiVersionSet(app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(2, 0))
    .Build())
.MapToApiVersion(new ApiVersion(2, 0))
.WithName("GetWeatherForecastV2")
.WithOpenApi();


app.Run();

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


Add IConfiguration to the program.cs file and pull in the appsettings.json as well as environmental variables.
Code similar to below can be used:
 IConfiguration Configuration  = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
    .Build();


Add Logging with Serilog AspNet Core and File sinks. 

Add ReferenceHandler.Preserve on JsonSerializerOptions to support cycles


For MuseumApi.Test.csproj
It will have a project reference to MuseumApi.csproj.
Add Nuget package Moq for unit testing.

#### **sqlite table creation**
In the MuseumApi project the SQLite download will have the following tables.

Table name: SpecialEvent
| Name | Type | Required | Default | Description | Validation |
| :--- | :--- | :--- | :--- | :--- |
| `EventID` | **UUID** | **Yes** |  | Unique Identifier for Event. | |
| `EventName` | **string** | **Yes** |  | Name of Event. | Maximum length is 150.|
| `EventDescription` | **string** | **Yes** |  | Description of Event. | Maximum length is 750.|
| 'Price' | **decimal** | **Yes**  | | The ticket price. | must be a positive number |


Table name: SpecialEventDate
| Name | Type | Required | Default | Description | Validation |
| `EventId` | **UUID** | **Yes** |  | Unique Identifier for Event. | Taken from SpecialEvent table row \
| `Date` | **date** (ISO 8601) | **Yes** |  |The specific date for the Event. | The date must be unique for a particular event id |

Relationships
SpecialEvent → SpecialEventDate: One-to-Many relationship.


Table name: MuseumDailyHour
|  Name | Type | Required | Default | Description | Validation |
| `ID` | **integer** | **Yes** | | unique id for row generated by table. | primary key. |
| `Date` | **date** (ISO 8601) | **Yes** | | The specific date for the operating hours. | must be a unique date. |
| `TimeOpen` | **string** (Time) | The opening time of the museum for that date. | `09:00:00` |
| `TimeClosed` | **string** (Time) | The closing time of the museum for that date. | `17:30:00` |

Table name: Ticket
|  Name | Type | Required | Default | Description | Validation |
| `TicketID` | **UUID** | **Yes** | | unique id for row generated by table. | primary key. |
| `TicketDate` | **date** (ISO 8601) | **Yes** | | The specific date for the ticket. | must be a correspond to a MuseumDailyHour Date. |
| `TicketType` | **string** | **Yes** |  | Type of Ticket. | can only be 'General' or 'SpecialEvent'.|
| `EventID` | **UUID** | **No** |  | Unique Identifier from SpecialEvent table. |  If the type is specialevent it must correspond to an eventid in the specialevent table. |


#### **Migration Framework**
EntityFramework Core will be used to setup the data structures.
EntityFramework Core will be used to dataseed the data structures.
Run terminal command: cd /home/mcjsw/spectest/MuseumApi && dotnet ef migrations add InitialCreate
Run terminal command: cd /home/mcjsw/spectest/MuseumApi && dotnet ef database update
In the program.cs create a dataconnection between the project and the sqlite db stored in the /Data directory.


#### **sqlite table data seeding**
##### **Data for MuseumDailyHour**
db.MuseumDailyHour.AddRange(
  new MuseumDailyHour() {
Date = "2023-09-11",
TimeOpen = "09:00",
TimeClose = "18:00"
  },
  new MuseumDailyHour() {  
Date = "2023-09-12",
TimeOpen = "09:00",
TimeClose = "18:00"

  },
new MuseumDailyHour() { 
    Date = "2023-09-13",
    TimeOpen = "09:00",
    TimeClose = "18:00"
 },
 new MuseumDailyHour() { Date = "2023-09-14",
TimeOpen = "09:00",
TimeClose = "18:00"
 },
new MuseumDailyHour() { Date = "2023-09-15",
TimeOpen = "10:00",
TimeClose = "16:00" },
new MuseumDailyHour() { 
  Date = "2023-09-18",
TimeOpen = "09:00",
 },
 new MuseumDailyHour() { Date = "2023-09-19",
TimeOpen = "09:00",
TimeClose = "18:00"  },
TimeClose = "18:00"  },

 new MuseumDailyHour() {  ate = "2023-09-20",
TimeOpen = "09:00",
TimeClose = "18:00"},
 new MuseumDailyHour() { Date = "2023-09-21",
TimeOpen = "09:00",
TimeClose = "18:00"
 },
 new MuseumDailyHour() { Date = "2023-09-22",
TimeOpen = "09:00",
TimeClose = "18:00"  }

);



##### **Data for SpecialEvent table**
db.SpecialEvents.AddRange(
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
	
##### **Data for SpecialEventDate table**
db.SpecialEventDates.AddRange(
    new SpecialEventDate()
    {EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97"),
        Date= "09/11/2023"},
    new SpecialEventDate() {
        EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97"),
        Date= "09/12/2023"
    }, 
    new SpecialEventDate() { EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e97"),
        Date= "09/13/2023"},
    new SpecialEventDate() { EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98"),
        Date= "09/11/2023" },	
    new SpecialEventDate() { EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98"),
        Date= "09/14/2023"},	
    new SpecialEventDate() { 	EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e98"),
        Date= "09/15/2023"},	
    new SpecialEventDate() { EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e77"),	
        Date= "09/16/2023"},	
    new SpecialEventDate() { 	EventID= Guid.Parse("f3e0e76e-e4a8-466e-ab9c-ae36c15b8e77"),	
        Date= "09/17/2023" },
    new SpecialEventDate() {EventID= Guid.Parse("dad4bce8-f5cb-4078-a211-995864315e39"),
        Date= "09/17/2023"},
    new SpecialEventDate() {EventID= Guid.Parse("dad4bce8-f5cb-4078-a211-995864315e39"),
        Date= "09/18/2023" },
    new SpecialEventDate() { EventID= Guid.Parse("6744a0da-4121-49cd-8479-f8cc20526495"),	
        Date= "09/19/2023" }	
);
	


#### **Classes**
Class should be created in a subdirectory called Models
 Class Name | Description | Attributes |
| :--- | :--- | :--- |
| **SpecialEvent** | The core entity representing a museum SpecialEvent. | `EventId`: **GUID**,`EventName`: string,`EventDescription`: string,`EventDates`: *Items:* **SpecialEventDates**,`Price`: **decimal** |
| **SpecialEventCollection** | List of upcoming events. | `array` | *Items:* **SpecialEvent** |
| **MuseumDailyHour** | The hours of operations for a day. |`Date`: **date**,`TimeOpen`: **time**,`TimeClosed`: **time** |
| **MuseumHours** | The hours of operations for a date range. | *Items:* **MuseumDailyHours** ,`SearchStartDate`: **date**,`SearchEndDate`: **date** |
| **Ticket** | Ticket for museum entry (can be general or special event). |`TicketId`: **GUID**,`TicketDate`: **date**, `TicketType`: **string**,`EventId`: **EventId** |
| **MuseumTicketConfirmation** | Details for a museum ticket after a successful purchase. |`TicketCodeImage`: **string**,`MuseumHours`: **MuseumHours**,`TicketType`: **string**,`EventId`: **GUID**, EventName **string** |
| **BuyMuseumTickets** | Data to purchase a ticket. | `TicketMessage`: **string**, `TicketId`: **GUID**, `TicketConfirmation`: **string** |
| **Error** | Standard error object. |`Type`: **string**`Title`: **string**, 'Details': **string** |


#### **Map Table structure to Classes**
Create current content of our `MuseumContext.cs` in Context subdirectory.
Create a subdirectory called context.  In the directory map the table structure to classes using Entityframework Core.  

Run terminal command: cd /home/mcjsw/spectest/MuseumApi && dotnet ef migrations add SeedData
Run terminal command: cd /home/mcjsw/spectest/MuseumApi && dotnet ef database update

#### **Endpoints**
Create a subdirectory called endpoints where the static enpoints can be put.  These endpoints will not directly call the entityframework
connection but will call the Services found in the next section.

- Task001.md getMuseumHours.  This relates to FR-001 found in specify.md
- Task002.md createSpecialEvent This relates to FR-002 found in specify.md
- Task004.md getSpecialEvent This relates to FR-004 found in specify.md
- Task005.md updateSpecialEvent This relates to FR-005 found in specify.md
- Task006.md deleteSpecialEvent This relates to FR-006 found in specify.md
- Task007.md buyMuseumTickets This relates to FR-007 found in specify.md



#### **Services in MuseumApi**
The specific classes are defined in the tasks above.

in the MuseumApi Services directory Create a class called BuyTicketService   
it will take the endpoints data and reponds with pulls or pushes the the database. Inject the databasee context
Method 1: name: BuyTicket, parameter: Ticket, return MuseumTicketConfirmation.  This ties to Task007.md

### **AppSetting.json creation**
initial appsettings.json configuration. Include SQLite connection string 

#### **Logging**
please see Logging.md for details on serilog implementation.

#### **Error Handling**

#### **Authentication and Authorization**
please see security.md for details on policy creation and initialization.

#### **Test Projects** 
If not already created, Create an xunit a Museum Api test project that references MuseumApi.csproj.  it will be named MuseumApi.Test.csproj.
Please see test.md for more details on setup.

