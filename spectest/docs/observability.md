# Observability — Museum API

This short guide shows how to view traces locally using the console exporter and how to point traces to a local OTLP collector (and view them in Jaeger). It also includes a note on enabling EF Core instrumentation (prerelease).

## Packages

Install the common OpenTelemetry packages (if not already present):

```bash
dotnet add MuseumApi package OpenTelemetry.Extensions.Hosting
dotnet add MuseumApi package OpenTelemetry.Exporter.Otlp
dotnet add MuseumApi package OpenTelemetry.Instrumentation.AspNetCore
dotnet add MuseumApi package OpenTelemetry.Instrumentation.Http
# Console exporter (optional, for development)
dotnet add MuseumApi package OpenTelemetry.Exporter.Console
```

EF Core instrumentation may be published as a prerelease. To install the prerelease package:

```bash
dotnet add MuseumApi package OpenTelemetry.Instrumentation.EntityFrameworkCore --prerelease
```

Notes:
- Use `--prerelease` only when you need the EF Core instrumentation and there is no stable release.

## Minimal Program.cs snippets

Console exporter example (development):

```csharp
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

builder.Services.AddOpenTelemetryTracing(b =>
{
    b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MuseumApi"));
    b.AddAspNetCoreInstrumentation();
    b.AddHttpClientInstrumentation();
    b.AddConsoleExporter(); // prints spans to the app console
});
```

OTLP exporter example (send to local collector at http://localhost:4317):

```csharp
builder.Services.AddOpenTelemetryTracing(b =>
{
    b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MuseumApi"));
    b.AddAspNetCoreInstrumentation();
    b.AddHttpClientInstrumentation();
    b.AddOtlpExporter(o =>
    {
        o.Endpoint = new Uri("http://localhost:4317");
        // o.Protocol = OtlpExportProtocol.Grpc; // default
    });
});
```

EF Core instrumentation snippet:

```csharp
// requires OpenTelemetry.Instrumentation.EntityFrameworkCore (prerelease maybe)
tracing.AddEntityFrameworkCoreInstrumentation(options =>
{
    options.SetDbStatementForText = true; // optional, shows SQL
});
```

## Run a local collector and Jaeger (examples)

Option A — OpenTelemetry Collector (simple):

```bash
# Pull and run a collector that listens on 4317 (OTLP gRPC)
docker run --rm -p 4317:4317 -p 55681:55681 otel/opentelemetry-collector:latest
```

Option B — Jaeger all-in-one (good for quick UI checks):

```bash
docker run --rm -p 6831:6831/udp -p 16686:16686 jaegertracing/all-in-one:1.41
```

A minimal collector configuration (collector.yaml) that receives OTLP and exports to Jaeger could look like:

```yaml
receivers:
  otlp:
    protocols:
      grpc: {}

exporters:
  jaeger:
    endpoint: "http://host.docker.internal:14268/api/traces" # adjust for your environment

service:
  pipelines:
    traces:
      receivers: [otlp]
      exporters: [jaeger]
```

Run the collector with the config:

```bash
docker run --rm -p 4317:4317 -v $(pwd)/collector.yaml:/etc/otelcol/config.yaml otel/opentelemetry-collector:latest --config /etc/otelcol/config.yaml
```

## Environment variables (runtime override)

You can set the OTLP endpoint and resource attributes with environment variables instead of editing code:

```bash
export OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
export OTEL_RESOURCE_ATTRIBUTES=service.name=MuseumApi,service.version=1.0
dotnet run --project MuseumApi
```

## Viewing traces

- Console: enable `AddConsoleExporter()` and run the app. Spans will be printed as they are created.
- Jaeger: open `http://localhost:16686` after running the collector + Jaeger. Search for the `MuseumApi` service and inspect traces.

## Troubleshooting tips

- If you don't see traces in Jaeger, ensure your collector is receiving OTLP (check collector logs) and that the collector exports to Jaeger correctly.
- If EF Core spans are missing, ensure the EF Core instrumentation package is installed and the `AddEntityFrameworkCoreInstrumentation` call is present in your tracing pipeline.
- In production, avoid printing full SQL statements to logs / traces unless you understand the security and cost implications.

## Quick commands summary

```bash
# Install prerelease EF Core instrumentation (if needed)
dotnet add MuseumApi package OpenTelemetry.Instrumentation.EntityFrameworkCore --prerelease

# Run Jaeger UI
docker run --rm -p 6831:6831/udp -p 16686:16686 jaegertracing/all-in-one:1.41

# Run a basic OpenTelemetry Collector
docker run --rm -p 4317:4317 -p 55681:55681 otel/opentelemetry-collector:latest

# Start the app pointing to local collector
export OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
dotnet run --project MuseumApi
```

---

If you want, I can also add a tiny collector config file under `specs/001-museumapi-is-a/` and a sample `docker-compose.yml` to spin up the collector + Jaeger for local testing.
