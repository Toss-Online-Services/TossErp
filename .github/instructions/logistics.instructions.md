---
description: Rules and best practices for Logistics & Supply Chain Management in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# Logistics Instructions
- Integrate shipping, carrier management, and real-time tracking.
- Automate route planning, delivery scheduling, and mobile confirmations.
- Support advanced warehousing (wave picking, cross-docking, automation).
- Provide logistics analytics and EDI for partner collaboration.
- Enforce multi-company inventory ownership and strict controls.

## Security & Compliance
- OAuth2 scopes: logistics:read, logistics:write, shipment:write; enforce carrier/warehouse scoping.
- Audit logs for shipment creation, status updates, proof-of-delivery, and EDI exchanges.
- Ensure data sharing agreements and compliance with customs/export controls where applicable.

## Acceptance Criteria
- API: RFC7807, cursor pagination, idempotency on shipment creation/updates; rate limits on tracking endpoints.
- Conflict handling: 409 for double scans or invalid status transitions; webhooks for status events.
- Cross-links: [Model](mdc:docs/models/logistics.model.md) · [API](mdc:docs/api-specs/logistics.openapi.md) · [Workflow](mdc:docs/architecture/logistics.workflow.md)
- Validate route planning constraints, delivery confirmations, and advanced warehousing flows.
