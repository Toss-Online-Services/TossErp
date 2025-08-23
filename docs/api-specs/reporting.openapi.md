---
description: Reporting & Analytics API
---

# Reporting API Outline
- Security: OAuth2, scopes: reporting:read, reporting:write
- Standards: see [API Standards](STANDARDS.md) for errors (RFC7807), pagination, idempotency, and rate limits
  - Long-running exports return async job with status endpoint; use 202 + Location
- Endpoints:
  - GET /reports?dataset=...&filters=...&format=...
  - POST /reports
  - POST /dashboards
  - GET /dashboards/{id}
  - POST /reports/{id}/export
  - POST /reports/{id}/schedule
  - GET /reporting/audit?entity=Report&id=...
- Errors: RFC7807; 413 for heavy exports (suggest pagination or async); 429 with rate-limit headers
- Pagination: cursor-based per Standards; async job IDs for heavy tasks
