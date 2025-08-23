---
description: OpenAPI spec outline for Credit Engine API endpoints
---

# Credit Engine API Spec
- Security: OAuth2, scopes: credit:read, credit:write, finance:read
- Standards: see [API Standards](STANDARDS.md)
	- Decision endpoints require `Idempotency-Key`; include explainability payloads
- POST /creditprofile/create: Create credit profile
- GET /creditprofile/{id}: Retrieve credit profile
- POST /creditprofile/{id}/score: Update credit score
- POST /creditprofile/{id}/decision: Make credit decision
- GET /creditprofile/{id}/audit: Retrieve audit log
