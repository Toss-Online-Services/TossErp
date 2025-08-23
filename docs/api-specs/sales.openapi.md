---
description: Sales & CRM API
---

# Sales API Outline
- Security: OAuth2, scopes: sales:read, sales:write, crm:write
- Standards: see [API Standards](STANDARDS.md) for errors (RFC7807), pagination, idempotency, and rate limits
  - POST endpoints accept `Idempotency-Key`
  - Response headers include `X-Request-Id` and rate limit headers
- Endpoints:
  - POST /sales/leads
  - POST /sales/opportunities
  - POST /sales/quotes
  - POST /sales/orders
  - POST /sales/orders/{id}/approve
  - GET /sales/customers?search=...&page=...
  - GET /sales/audit?entity=SalesOrder&id=...
- Errors: RFC7807 problem+json, rate limit headers
- Pagination: cursor-based per Standards, simple additive filters (e.g., status, date ranges)
