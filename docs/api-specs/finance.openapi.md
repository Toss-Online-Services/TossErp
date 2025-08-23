---
description: Finance API (GL, AP, AR, Tax)
---

# Finance API Outline
- Security: OAuth2, scopes: finance:read, finance:write, payments:write
- Standards: see [API Standards](STANDARDS.md) for errors (RFC7807), pagination, idempotency, and rate limits
  - Idempotency on POST/PUT via `Idempotency-Key`
  - Response headers: `X-Request-Id`, rate limit headers
- Endpoints:
  - POST /finance/journal-entries (idempotent-key)
  - GET /finance/journal-entries?account=...&date_from=...&date_to=...&page=...
  - POST /finance/invoices
  - POST /finance/payments
  - GET /finance/tax-codes
  - GET /finance/audit?entity=JournalEntry&id=...
- Errors: RFC7807 problem+json with code, detail, traceId
- Pagination: cursor-based per Standards; list endpoints return envelope
