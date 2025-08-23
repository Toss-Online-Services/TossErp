description: Sales & CRM API
---

# Sales API Outline
- Security: OAuth2, scopes: sales:read, sales:write, crm:write
- Standards: see [API Standards](STANDARDS.md) for errors (RFC9457), pagination, idempotency, ETag/If-Match, and rate limits
  - POST/transition endpoints accept `Idempotency-Key`
  - Response headers include `X-Request-Id`, rate limit headers
  - Resources (Quote, SalesOrder, Customer) may include ETag; use `If-Match` on updates

## Resources
- Lead: id, name, email/phone, source, status, owner_id, created_at
- Opportunity: id, lead_id, amount, currency, stage, close_date, probability
- Quote: id, number, customer_id, currency, status(DRAFT|SENT|APPROVED|EXPIRED), totals, lines[], etag
- SalesOrder: id, number, customer_id, status(DRAFT|APPROVAL_PENDING|APPROVED|FULFILLED|CANCELLED), totals, lines[], reservation_id?, etag
- Customer: id, name, contacts[], billing/shipping addresses

## Endpoints
- POST /sales/leads (scope: crm:write, Idempotency-Key)
- GET /sales/leads (scope: sales:read; filters: owner_id, status, date ranges)

- POST /sales/opportunities (scope: sales:write, Idempotency-Key)
- GET /sales/opportunities (scope: sales:read)

- POST /sales/quotes (scope: sales:write, Idempotency-Key)
  - 201 with ETag; 422 validation; 409 duplicate number
- GET /sales/quotes/{id} (scope: sales:read)
- PUT /sales/quotes/{id} (scope: sales:write; If-Match)
  - 412 on ETag mismatch; allowed only while status=DRAFT
- POST /sales/quotes/{id}/approve (scope: sales:write; Idempotency-Key)
  - Transition to APPROVED; 409 if already approved/expired; emits sales.quote.approved

- POST /sales/orders (scope: sales:write; Idempotency-Key)
  - Create order from quote or direct; on success may reserve inventory via Inventory module
  - 201 with ETag; 409 insufficient inventory (from Inventory) or approval required
- GET /sales/orders (scope: sales:read; filters: status, customer_id, number)
- GET /sales/orders/{id} (scope: sales:read; ETag)
- PUT /sales/orders/{id} (scope: sales:write; If-Match; editable in DRAFT)
  - 412 precondition failed; 409 if not editable
- POST /sales/orders/{id}/approve (scope: sales:write; Idempotency-Key)
  - Approval workflow; 403 if user lacks authority; emits sales.order.approved

- GET /sales/customers (scope: sales:read) — filters: search, created_from/to

- GET /sales/audit (scope: sales:read) — entity, id

## Errors
- RFC9457 problem+json
  - 409 for invalid status transitions, duplicate numbers
  - 403 for approval threshold violations
  - 412 on ETag mismatch
  - 429 with rate-limit headers

## Webhooks
- sales.quote.approved
- sales.order.created
- sales.order.approved
  - Signed per Standards; payload includes minimal identifiers and version

## Example
- POST /sales/orders
  - Request: { "customer_id":"cus_1","currency":"USD","lines":[{"item_id":"sku_1","qty":2,"unit_price":50}]}
  - 201: { "id":"so_123","status":"DRAFT","totals":{"subtotal":100,"tax":8,"total":108} }
