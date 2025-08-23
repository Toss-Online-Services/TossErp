description: Inventory & Procurement API
---

# Inventory API Outline
- Security: OAuth2, scopes: inventory:read, inventory:write, procurement:write
- Standards: see [API Standards](STANDARDS.md) for errors (RFC9457), pagination, idempotency, ETag, and rate limits
  - Use `Idempotency-Key` on stock-moves, receipts, and PO creation
  - Include `X-Request-Id` in responses
  - ETag/If-Match on mutable resources (PO, Receipt) to avoid lost updates

## Resources
- Item: id, sku, name, uom, track_by(BATCH|SERIAL|NONE), active
- StockLevel: item_id, warehouse_id, on_hand, reserved, available
- StockMove: id, type(ISSUE|RECEIPT|TRANSFER|ADJUSTMENT), from_wh?, to_wh?, lines[{item_id, qty, batch?, serials[]}], status, idempotency_key
- Requisition: id, requester_id, lines, status(DRAFT|APPROVED|ORDERED)
- PurchaseOrder: id, number, vendor_id, currency, lines, status(DRAFT|SENT|RECEIVED|CLOSED), etag
- Receipt: id, po_id, lines[{po_line_id, qty_received, batch?, serials[]}], status(PENDING|POSTED), etag

## Endpoints
- GET /inventory/items (scope: inventory:read)
  - Filters: search, warehouse, active

- POST /procurement/requisitions (scope: procurement:write, Idempotency-Key)
- POST /procurement/purchase-orders (scope: procurement:write, Idempotency-Key)
  - 201 with ETag; 409 duplicate number; 422 validation
- GET /procurement/purchase-orders/{id} (scope: inventory:read) — ETag
- PUT /procurement/purchase-orders/{id} (scope: procurement:write, If-Match)
  - 412 on ETag mismatch; allowed in DRAFT

- POST /procurement/receipts (scope: inventory:write, Idempotency-Key)
  - Records receipt against a PO; validates batch/serial tracking and does 3-way match
  - 201 with ETag; 409 over-receipt beyond ordered qty; 422 invalid serial/batch
  - Webhook: inventory.receipt.posted

- POST /inventory/stock-moves (scope: inventory:write, Idempotency-Key)
  - Issues/transfers/adjusts stock; 409 on over-issuance; 422 invalid warehouse/item
  - Webhook: inventory.stock_move.posted

- GET /inventory/audit (scope: inventory:read) — entity, id

## Errors
- RFC9457 problem+json; common cases: 409 over-issuance/over-receipt; 412 ETag mismatch; 403 permission/warehouse scope

## Webhooks
- inventory.stock_move.posted
- inventory.receipt.posted
  - HMAC-signed per Standards

## Examples
- POST /inventory/stock-moves
  - Request: { "type":"TRANSFER","from_wh":"WH-A","to_wh":"WH-B","lines":[{"item_id":"sku-1","qty":10}] }
  - 201: { "id":"sm_123","status":"POSTED" }
