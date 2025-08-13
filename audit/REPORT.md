# TOSS Monorepo Audit Report

Date: {{TODAY}}

## Repository Overview

Detected key components:

- Web (Nuxt 3): `TossErp.Web/`
- .NET services (Stock bounded context): `src/Services/Stock/*` with layers:
  - `Stock.API` (Minimal API)
  - `Stock.Application`, `Stock.Domain`, `Stock.Infrastructure`
  - `Stock.Processor` (Worker)
- Shared libraries:
  - `src/ServiceDefaults` (health, telemetry, resilience)
  - `src/EventBus` (abstractions and extensions)
- Solutions: `src/eShop.sln`, `src/Services/Stock/Stock.sln`
- Mobile: `TossErp.Mobile/` (README only, no Flutter app yet)

Missing or not detected:

- API Gateway (YARP/Ocelot)
- Docker Compose infra
- Additional microservices (Identity, Sales, Buying, Accounting, CRM, Collaboration, Notifications, AI)
- Shared event contracts package
- CI/CD workflows in `.github/workflows/`
- K8s manifests

## Toolchain Versions (detected)

- .NET SDK: 9.0.200 (from `src/global.json`)
- Node/NPM: Not explicitly pinned; Nuxt `^4.0.1` (Node 18+ recommended)
- Flutter/Dart: Not detected

## Configuration/Infra

- Postgres/Redis/RabbitMQ: Not present (no `infra/docker` yet)
- Central props: `src/Directory.Build.props` (TreatWarningsAsErrors=true, ImplicitUsings enabled)

## Decisions per Component

| Component | Path | Action |
|---|---|---|
| Web (Nuxt 3) | `TossErp.Web/` | keep (enhance pages/stores as needed) |
| Mobile (Flutter) | `apps/toss_mobile` | create (initial skeleton) |
| Inventory (Stock) | `src/Services/Stock/*` | keep (reuse as Inventory service) |
| Sales | `services/sales` | create (stub) |
| Buying | `services/buying` | create (stub) |
| Accounting | `services/accounting` | create (stub) |
| CRM | `services/crm` | create (stub) |
| Collaboration | `services/collaboration` | create (stub) |
| Notifications | `services/notifications` | create (stub) |
| Identity | `services/identity` | create (stub) |
| AI | `services/ai` | create (stub) |
| API Gateway | `gateway/` | create (YARP + BFF endpoints) |
| Shared Contracts | `services/_shared/Contracts` | create |
| Docker Compose | `infra/docker/docker-compose.yml` | create |
| K8s Manifests | `infra/k8s/` | create (skeleton) |
| CI Workflows | `.github/workflows/` | create |

Notes:

- Existing inventory bounded context under `src/Services/Stock` will be referenced as the Inventory service to avoid duplication. A shim README will be placed under `services/inventory/` pointing to the existing implementation.
- Proceeding with .NET 9 SDK already used by the repo (superset of .NET 8 requirement). Target frameworks can be adjusted later if needed.

## Next Steps (Phase 1)

1. Add infra docker compose with Postgres, Redis, RabbitMQ, gateway, stock api, and placeholders for other services.
2. Scaffold API Gateway with YARP and BFF endpoints.
3. Create shared contracts project and reference from services incrementally.
4. Add CI workflow to build .NET services, web, and containers.
5. Provide local dev docs and bootstrap script.



