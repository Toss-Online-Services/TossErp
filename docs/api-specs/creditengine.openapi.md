---
description: OpenAPI spec outline for Credit Engine API endpoints
---

# Credit Engine API Spec
- Security: OAuth2, scopes: credit:read, credit:write, finance:read
- Standards: see [API Standards](STANDARDS.md); RFC9457 errors; ETag/If-Match on profile updates
	- Decision endpoints require `Idempotency-Key`; include explainability payloads

## Endpoints
- POST /creditprofile (credit:write; Idempotency-Key) -> 201
- GET /creditprofile/{id} (credit:read)
- PUT /creditprofile/{id} (credit:write; If-Match) -> 200 or 412
- POST /creditprofile/{id}/score (credit:write; Idempotency-Key) -> 202 Location: /jobs/{job_id}
- POST /creditprofile/{id}/decision (credit:write; Idempotency-Key) -> 201 decision with reasons
- GET /creditprofile/{id}/audit (credit:read) -> cursor list

## Webhooks
- creditengine.profile.created
- creditengine.decision.created

## Example
- POST /creditprofile/{id}/decision { amount, tenor_months } -> 201 { id, approved, reasons: ["DTI_TOO_HIGH"] }
