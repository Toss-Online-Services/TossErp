---
description: Asset Sharing API outline — endpoints, auth scopes, headers, idempotency, pagination, approvals
---

# Asset Sharing API

Reference: See shared API standards in ../api-specs/STANDARDS.md for authentication, headers, RFC7807 errors, cursor pagination, idempotency keys, rate limits, webhooks, and auditing.

## Security & Scopes
- OAuth2 scopes: asset:read, asset:write, booking:write
- Role/location-based scoping for assets and bookings

## Common Headers
- Authorization: Bearer <token>
- Idempotency-Key: required on POSTs that create/modify state (asset create, booking, check-in/out, condition reports)
- X-Request-Id: optional caller-generated for tracing

## Resource Model (high-level)
- Asset: id, name, category, owner_org_id, location_id, booking_terms, maintenance_schedule, status
- Booking: id, asset_id, user_id, start, end, status, cost, approvals
- ConditionReport: id, asset_id, booking_id, checklist[], photos[], status
- Maintenance: id, asset_id, type, due_at, status

## Endpoints
- POST /assets
  - Register new asset; 201 on success; approvals for restricted assets
- GET /assets/{id}
  - Retrieve asset details including availability windows and terms
- GET /assets
  - Cursor list with filters: category, status, location_id, owner_org_id

- POST /assets/{id}/bookings
  - Create booking (idempotent); 409 on overlapping windows; requires terms acceptance
- POST /assets/{id}/check-in
  - Idempotent state transition; requires existing booking; emits booking.checked_in webhook
- POST /assets/{id}/check-out
  - Idempotent; cost finalization; emits booking.checked_out webhook

- POST /assets/{id}/condition-reports
  - Submit a condition report with photos; may require approval

- GET /assets/{id}/usage
  - Usage and cost summary; cursor pagination for logs

- GET /assets/{id}/audit
  - Full audit trail per standards

## Errors & Conflicts
- RFC7807
- 409 on overlapping bookings or invalid transitions; 403 on permission; 422 validation

## Webhooks
- booking.created, booking.checked_in, booking.checked_out, maintenance.due
- HMAC-signed per standards; includes X-Request-Id
