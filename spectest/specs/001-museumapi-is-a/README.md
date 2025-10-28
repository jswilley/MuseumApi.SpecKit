# Local Observability Setup

## Quick Start

1. Start OpenTelemetry Collector and Jaeger:

```bash
cd specs/001-museumapi-is-a
# Use default config (exports to Jaeger)
docker-compose up
# Or use override config for extra endpoints
docker-compose -f docker-compose.yml -f docker-compose.override.yml up
# Or use collector-stdout.yaml for Jaeger + stdout
OTELCOL_CONFIG=collector-stdout.yaml docker-compose up otel-collector
```

2. Run the API and point traces to the collector:

```bash
export OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
export OTEL_RESOURCE_ATTRIBUTES=service.name=MuseumApi
cd ../../MuseumApi
# If EF Core instrumentation is enabled, ensure the prerelease package is installed:
dotnet add package OpenTelemetry.Instrumentation.EntityFrameworkCore --prerelease
# Run the API
DOTNET_ENVIRONMENT=Development dotnet run
```

3. View traces:
- Jaeger UI: http://localhost:16686
- Collector logs: docker logs <container_id> (if using logging exporter)

## Makefile targets

A sample Makefile is provided for convenience:

- `make up` — starts collector + Jaeger
- `make run` — runs the API
- `make all` — starts everything
- `make down` — stops all containers

## Troubleshooting
- If you want to see traces in stdout, use `collector-stdout.yaml`.
- If EF Core spans are missing, ensure the prerelease package is installed and enabled in Program.cs.
