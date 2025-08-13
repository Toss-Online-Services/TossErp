# Architecture Overview

## Bounded Contexts

- Inventory (existing `src/Services/Stock`)
- Sales, Buying, Accounting, CRM, Collaboration, Notifications, Identity, AI (stubs)

## Gateway (YARP + BFF)

- Reverse proxy to services under `/services/{context}/...`
- BFF endpoints for mobile/web dashboards and mobile sync batch APIs

## Eventing

- RabbitMQ via MassTransit (planned)
- Shared contracts in `services/_shared/Contracts`

## Data

- Postgres per service (schemas or DBs)
- Redis cache (future)

## Observability

- Serilog to console, OpenTelemetry (planned), health endpoints



