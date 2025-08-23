---
description: Asset Sharing API outline — endpoints, auth scopes, headers, idempotency, pagination, approvals
---

# Asset Sharing API

Reference: See shared API standards in ../api-specs/STANDARDS.md for authentication, headers, RFC9457 errors, cursor pagination, idempotency keys, rate limits, webhooks, and auditing.

## Security & Scopes
- OAuth2 scopes: asset:read, asset:write, booking:write
- Role/location-based scoping for assets and bookings

## Common Headers
- Authorization: Bearer <token>
- Idempotency-Key: required on POSTs that create/modify state (asset create, booking, check-in/out, condition reports)
- X-Request-Id: optional caller-generated for tracing

## Resource Model (high-level)
- Asset: id, name, category, owner_org_id, location_id, booking_terms, maintenance_schedule, status, created_at, updated_at, etag
- Booking: id, asset_id, user_id, start, end, status, cost_minor, currency, approvals[], created_at, updated_at
- ConditionReport: id, asset_id, booking_id, checklist[], photos[], notes, status, created_at
- Maintenance: id, asset_id, type, due_at, status

## Endpoints

### POST /assets
- Scopes: asset:write
- Headers: Idempotency-Key required
- Body:
  - name (string, required)
  - category (string, required)
  - owner_org_id (string, required)
  - location_id (string, required)
  - booking_terms (object, optional)
- Responses:
  - 201 Created with Asset JSON and `ETag` header
  - 422 validation problem+json

Example request:
```json
{ "name": "Forklift A2", "category": "EQUIPMENT", "owner_org_id": "org_1", "location_id": "loc_7" }
```

### GET /assets/{id}
- Scopes: asset:read
- Headers: Optional `If-None-Match`
- Responses:
  - 200 OK with Asset JSON and `ETag`
  - 304 Not Modified if `If-None-Match` matches
  - 404 if not found or not visible

### GET /assets
- Scopes: asset:read
- Query: category, status, location_id, owner_org_id, q, limit, cursor
- Response: Standard list envelope with Asset[]

### PUT /assets/{id}
- Scopes: asset:write
- Headers: `If-Match` required, `Idempotency-Key` required
- Body: Partial update fields (name, category, location_id, status)
- Responses:
  - 200 OK with updated Asset; new `ETag`
  - 412 Precondition Failed on ETag mismatch (problem+json)

### POST /assets/{id}/bookings
- Scopes: booking:write
- Headers: Idempotency-Key required
- Body:
  - user_id (string, required)
  - start (ISO8601, required)
  - end (ISO8601, required)
  - accept_terms (boolean, required)
- Responses:
  - 201 Created (new booking) or 200 OK (idempotent replay)
  - 409 on overlapping windows or after deadline
  - 403 if user lacks permission or terms not accepted
  - Webhook: booking.created

Example request:
```json
{ "user_id": "usr_9", "start": "2025-09-01T08:00:00Z", "end": "2025-09-01T16:00:00Z", "accept_terms": true }
```

### POST /assets/{id}/check-in
- Scopes: booking:write
- Headers: Idempotency-Key required
- Body: { booking_id: string, condition_notes?: string }
- Responses: 200 OK; 409 invalid transition; Webhook booking.checked_in

### POST /assets/{id}/check-out
- Scopes: booking:write
- Headers: Idempotency-Key required
- Body: { booking_id: string, extra_cost_minor?: integer }
- Responses: 200 OK; cost finalization; Webhook booking.checked_out

### POST /assets/{id}/condition-reports
- Scopes: asset:write
- Headers: Idempotency-Key required
- Body: { booking_id?: string, checklist: array, photos: array, notes?: string }
- Responses: 201 Created; 422 validation; may require approval

### GET /assets/{id}/usage
- Scopes: asset:read
- Query: date_from, date_to, limit, cursor
- Response: list envelope with usage logs and cost aggregation summary

### GET /assets/{id}/audit
- Scopes: asset:read
- Response: list envelope with immutable audit entries

## Errors & Conflicts
- RFC9457
- 409 on overlapping bookings or invalid transitions; 403 on permission; 422 validation

Example 409 problem:
```json
{
  "type": "https://api.toss.erp/problems/booking-overlap",
  "title": "Booking window overlaps",
  "status": 409,
  "detail": "Requested window overlaps with existing booking bkg_123.",
  "code": "BOOKING_OVERLAP",
  "errors": [ { "name": "start", "reason": "overlaps existing #bkg_123" } ]
}
```

## Webhooks
- booking.created, booking.checked_in, booking.checked_out, maintenance.due
- HMAC-signed per standards; includes X-Request-Id

Payload example (booking.created):
```json
{
  "id": "evt_01J6Q7A9ZB",
  "type": "booking.created",
  "created_at": "2025-08-23T12:34:56Z",
  "request_id": "01J6Q7A8V4",
  "data": {
    "booking": {
      "id": "bkg_123",
      "asset_id": "ast_456",
      "user_id": "usr_9",
      "start": "2025-09-01T08:00:00Z",
      "end": "2025-09-01T16:00:00Z",
      "status": "CONFIRMED"
    }
  }
}
```
