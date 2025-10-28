## this describes how the project will implement endpoint versioning.

OpenApi.Extensions Add code similar to below in program.cs
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