---
description: Inventory & Procurement API
---

# Inventory API Outline
- Security: OAuth2, scopes: inventory:read, inventory:write, procurement:write
- Standards: see [API Standards](STANDARDS.md) for errors, pagination, idempotency, rate limits
  - Use `Idempotency-Key` on stock-moves and receipts
  - Include `X-Request-Id` in responses
- Endpoints:
  - GET /inventory/items?search=...&warehouse=...
  - POST /procurement/requisitions
  - POST /procurement/purchase-orders
  - POST /procurement/receipts
  - POST /inventory/stock-moves (idempotent-key)
  - GET /inventory/audit?entity=StockMove&id=...
- Errors: RFC7807; 409 on over-issuance or invalid status transitions; 412 on ETag mismatch where used
- Pagination: cursor-based per Standards, list envelope
