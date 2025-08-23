---
description: Rules and best practices for Shared Asset & Tool Sharing Network in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# Asset Sharing Instructions
- Maintain a registry of shareable assets with schedules and usage terms.
- Provide integrated booking and reservation system with notifications.
- Track costs, maintenance, and usage for each asset.
- Automate cost sharing, invoicing, and accountability workflows.
- Enforce permissions and trust-building features (ratings, condition reports).

## Security & Compliance
- OAuth2 scopes: asset:read, asset:write, booking:write; enforce role- and location-based access.
- Mandatory audit logs on bookings, check-in/out, condition reports, maintenance, and approvals.
- Require approvals for restricted/high-value assets; enforce usage terms and liability acknowledgments.

## Acceptance Criteria
- API standards: RFC7807 errors, cursor pagination, idempotency keys on POST (bookings, check-in/out), rate limits.
- Conflict handling: return 409 on overlapping bookings or unavailable assets.
- Cross-links: [Model](mdc:docs/models/assetshare.model.md) · [API](mdc:docs/api-specs/assetshare.openapi.md) · [Workflow](mdc:docs/architecture/assetshare.workflow.md)
- Validate booking lifecycle, cost allocation, and permission checks.
