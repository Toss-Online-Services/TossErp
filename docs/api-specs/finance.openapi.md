description: Finance API (GL, AP, AR, Tax)
---

# Finance API Outline
- Security: OAuth2, scopes: finance:read, finance:write, payments:write
- Standards: see [API Standards](STANDARDS.md) for errors (RFC9457), pagination, idempotency, ETag/If-Match, and rate limits
  - Idempotency on POST/PUT via `Idempotency-Key`
  - Response headers: `X-Request-Id`, standard rate limit headers
  - Conditional updates via `ETag` + `If-Match` on mutable resources (Invoice, Payment)

## Resources (high-level)
- JournalEntry: id, date, currency, status(DRAFT|POSTED|VOID), lines[{account_id, debit, credit, description, entity_ref}], source_doc, posted_at, created_by
- Invoice: id, number, customer_id, currency, issue_date, due_date, status(DRAFT|ISSUED|PAID|VOID), totals{subtotal, tax, total}, lines[{item_id, qty, unit_price, tax_code, amount}], etag
- Payment: id, reference, customer_id, amount, currency, method, applied_to[{invoice_id, amount}], status(PENDING|APPLIED|FAILED), etag
- TaxCode: id, jurisdiction, rate, inclusive, valid_from, valid_to

## Headers
- Authorization: Bearer <token>
- Idempotency-Key: required on POST (create) and POST state transitions
- If-Match: required on PUT/PATCH where `ETag` is provided

## Endpoints (scopes, headers, semantics)
- POST /finance/journal-entries  (scope: finance:write, Idempotency-Key)
  - Request: { date, currency, lines[...], source_doc? }
  - Rules: lines must balance (sum(debit) == sum(credit)), valid accounts, open period
  - Responses: 201 { id, status:"POSTED" | "DRAFT" } · 422 validation · 403 period closed
  - Webhook: finance.journal_entry.posted

- GET /finance/journal-entries (scope: finance:read)
  - Filters: account_id, date_from, date_to, source_doc, status
  - Pagination: cursor per Standards

- POST /finance/invoices (scope: finance:write, Idempotency-Key)
  - Request: { customer_id, currency, issue_date, due_date, lines[...], number? }
  - Responses: 201 (ETag header set) · 409 duplicate number · 422 validation
  - Webhook: finance.invoice.issued

- GET /finance/invoices (scope: finance:read)
  - Filters: status, customer_id, number, date_from/date_to

- GET /finance/invoices/{id} (scope: finance:read)
  - Returns resource with ETag header

- PUT /finance/invoices/{id} (scope: finance:write, If-Match required)
  - Updates allowed while status=DRAFT; 412 on ETag mismatch; 409 if status not editable

- POST /finance/payments (scope: payments:write finance:write, Idempotency-Key)
  - Request: { reference, customer_id, amount, currency, method, apply: [{ invoice_id, amount }] }
  - Responses: 201 (ETag) · 409 over-application · 422 validation
  - Webhook: finance.payment.applied

- GET /finance/tax-codes (scope: finance:read)
  - Filters: jurisdiction, active

- GET /finance/audit (scope: finance:read)
  - Query: entity, id | source_doc; returns audit trail entries

## Errors
- RFC9457 problem+json; examples include:
  - 422 validation: missing account_id, unbalanced lines
  - 403 period closed or permissions
  - 409 duplicate invoice number or payment over-application
  - 412 ETag precondition failed

## Webhooks
- finance.journal_entry.posted
- finance.invoice.issued
- finance.payment.applied
  - All signed (HMAC) per Standards; include minimal payload with resource id and type

## Examples
- POST /finance/journal-entries
  - Request: { "date":"2025-01-15","currency":"USD","lines":[{"account_id":"4000","debit":0,"credit":100,"description":"Sale"},{"account_id":"1100","debit":100,"credit":0,"description":"Cash"}]}
  - 201: { "id":"je_123","status":"POSTED" }

## Notes
- Large GL exports should use Reporting API async jobs (see Jobs API)
- All list endpoints use cursor pagination and support consistent sorting per Standards
