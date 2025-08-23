---
description: Group Buying API outline — endpoints, auth scopes, headers, idempotency, pagination, conflicts
---

# Group Buying API

Reference: See shared API standards in ../api-specs/STANDARDS.md for authentication, headers, RFC9457 errors, cursor pagination, idempotency keys, rate limits, webhooks, and auditing.

## Security & Scopes
- OAuth2 scopes: groupbuy:read, groupbuy:write, procurement:write
- Role- and company-based filtering enforced on all list/read endpoints

## Common Headers
- Authorization: Bearer <token>
- Idempotency-Key: required on POSTs that create/modify state (create, commit, rfq, allocate, settle)
- X-Request-Id: optional caller-generated for tracing
- Rate limit headers: X-RateLimit-*, Retry-After per standards

## Resource Model (high-level)
- GroupBuy: id, title, description, items[], target_quantity, min_commitment, deadline, status, rules, created_by, org_id
- Commitment: id, groupbuy_id, member_id, quantity, price_ceiling, status
- RFQ: id, groupbuy_id, vendors[], responses[], status, due_date
- Allocation: id, groupbuy_id, rules, results[], status

## Endpoints
- POST /groupbuy
  - Scopes: groupbuy:write
  - Headers: Idempotency-Key required
  - Body: { title, description?, items[], target_quantity, min_commitment, deadline, rules }
  - 201 with resource body; 409 if overlapping/invalid rules; 422 for validation

- GET /groupbuy/{id}
  - Scopes: groupbuy:read
  - Retrieve group buy details with current stats and commitments summary
  - 404 if not found or not visible

- GET /groupbuy
  - Scopes: groupbuy:read
  - Cursor-paginated list; filters: status, deadline_before/after, org_id, created_by
  - Response envelope per standards (items, next_cursor)

- POST /groupbuy/{id}/commit
  - Scopes: groupbuy:write
  - Headers: Idempotency-Key required
  - Add or upsert member commitment (idempotent)
  - 201 on new, 200 on update; 409 if capacity exceeded or after deadline; 403 if not eligible

- POST /groupbuy/{id}/rfq
  - Scopes: procurement:write
  - Initiate RFQ flow; may run async negotiation
  - 202 with Location header to job resource when async; webhooks for vendor-response events

- POST /groupbuy/{id}/allocate
  - Scopes: groupbuy:write
  - Run allocation rules and produce member/vendor allocations
  - 202 async (recommended); 409 on conflicting rules; emits allocation.created webhook

- POST /groupbuy/{id}/settle
  - Scopes: groupbuy:write, finance:write (implicit system role)
  - Finalize settlement and generate downstream AP/AR documents (links to Finance)
  - Idempotent; 409 if already settled or inconsistent state; emits settlement.completed webhook

- GET /groupbuy/{id}/audit
  - Scopes: groupbuy:read
  - RFC3339 timestamps; immutable audit entries; cursor pagination

## Errors & Conflicts
- RFC9457 problem+json per standards
- 409 when commitments exceed capacity, after deadlines, or invalid transitions
- 412 for ETag precondition failures on mutable endpoints (if-if-match)
- 403 for member eligibility violations; 422 validation

## Webhooks (select)
- vendor.response.received, allocation.created, settlement.completed
- Signed using HMAC per standards; include X-Request-Id passthrough

Payload example (allocation.created):
```json
{
  "id": "evt_01J6Q8KC2G",
  "type": "allocation.created",
  "created_at": "2025-08-23T12:40:01Z",
  "request_id": "01J6Q8K8DE",
  "data": {
    "groupbuy_id": "gb_123",
    "allocation": {
      "id": "alloc_99",
      "results": [ { "member_id": "mem_1", "quantity": 10 }, { "member_id": "mem_2", "quantity": 5 } ]
    }
  }
}
```
