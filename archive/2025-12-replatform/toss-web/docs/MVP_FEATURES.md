# MVP Feature Matrix

This document tracks the current Minimum Viable Product (MVP) feature set across modules. Keep it up to date as work progresses. Status values: Planned · In Progress · Baseline Implemented · Needs QA · Done.

## Legend

- Unit tests / E2E tests: none · partial · good
- Owner: Web (Nuxt) · Platform (Backend/API) · Shared

## Feature Overview

| Module | Scope (Key Features) | Status | Owner | Unit tests | E2E tests | Key Dependencies | Current Risks / Notes |
|---|---|---|---|---|---|---|---|
| Authentication & Registration | Email/phone login, JWT session, roles (Admin, Vendor, Driver), self-registration flows (vendor/driver) | Baseline Implemented | Shared | partial | partial | nuxt-auth flow, jwt-decode, server API routes (`/server/api/auth/*`) | Validate API base config across environments; expand negative-path tests |
| POS Sales | Cart, add/remove items, discounts, payment selection, hold/resume sale, receipt export | In Progress | Web | partial | partial | Pinia, Vue components, xlsx, jspdf/autotable, html2canvas | Edge cases: refunds/voids; browser print and PDF sizes; cash rounding |
| Inventory & Products | Product list, categories, stock levels, imports | Planned | Shared | none | none | Pinia, date-fns, Chart.js (dashboards) | Data model alignment with backend; seed/migration plan |
| Orders & Queue | Create order, queue-based orchestration of tasks, order status | Baseline Implemented | Shared | partial | partial | Store queue module, server routes | Concurrency, idempotency, retry/backoff strategy validation |
| Suppliers & Purchasing | Vendor list, purchase orders, intake | Planned | Platform | none | none | Backend API, database | Requires finalized schema; permissions mapping |
| Stores & Branches | Multi-store config, staff roles, device registration | Planned | Platform | none | none | Backend API, auth | Device-bound sessions; role scoping |
| Payments | Payment methods, tender capture, change calculation | Baseline Implemented | Web | partial | partial | Enum mapping, POS UI | Integration to gateways deferred; rounding rules per locale |
| Reporting & Exports | Daily sales, item performance, CSV/XLSX, PDF export | In Progress | Web | partial | none | xlsx, jspdf, html2canvas | Large dataset performance; memory usage in browsers |
| PWA & Offline | Installable app, caching strategy, offline queues/sync | In Progress | Web | partial | none | @vite-pwa/nuxt, workbox | Conflict resolution during sync; quota management |
| Observability | Client/server error reporting, tracing, replays | Baseline Implemented | Web | n/a | n/a | @sentry/nuxt, sentry client/server configs | Add source maps upload; doc release process |
| AI Copilot & Tasking | Component fetcher (shadcn MCP), Taskmaster tasks | Baseline Implemented | Web | n/a | n/a | .vscode/mcp.json | Establish governance on AI actions; API keys via env |
| Internationalization | i18n strings, currency/number formatting | Baseline Implemented | Web | partial | none | vue-i18n, custom `useMoney` | Locale-specific formatting differences covered by tests |
| UI/Design System | Tailwind, Headless UI, Heroicons | Baseline Implemented | Web | n/a | n/a | tailwindcss, @headlessui/vue, @heroicons/vue | Establish component patterns and accessibility checks |
| Build & Deploy | Nuxt build/preview, CI/CD, hosting (Vercel FE, Azure BE) | Planned | Shared | n/a | n/a | nuxt, Node 22 | Define environments, secrets, and pipelines |

## Exit Criteria for MVP

- Core flows work online and in intermittent connectivity (offline-first where applicable)
- Auth/RBAC enforced for Admin, Vendor, Driver
- POS happy path tested (unit + E2E) including hold/resume and receipt
- Error tracking active with Sentry across client/server
- Export workflows (CSV/XLSX/PDF) verified for typical data sizes

## Testing Checklist (per module)

- Unit tests cover: happy path, one edge case, one failure case
- E2E tests: login, POS sale, hold/resume, order creation
- Offline tests: cached assets, queued actions, sync on reconnect

## How to Update

1. Edit the row for the module you updated and adjust Status/Owner/tests.
2. If adding a new module, append a new row with dependencies and risks.
3. Link to relevant issues or docs inline where helpful.

