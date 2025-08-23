---
description: Logistics API outline — endpoints, auth scopes, headers, idempotency, pagination, status flows
---

# Logistics API

Reference: See shared API standards in ../api-specs/STANDARDS.md for authentication, headers, RFC7807 errors, cursor pagination, idempotency keys, rate limits, webhooks, and auditing.

## Security & Scopes
- OAuth2 scopes: logistics:read, logistics:write, shipment:write
- Warehouse/carrier/shipper scoping on queries

## Common Headers
- Authorization: Bearer <token>
- Idempotency-Key: required on POSTs that create/modify state (shipment create/update, scans, inventory transfers)
- X-Request-Id for tracing

## Resource Model (high-level)
- Shipment: id, order_ref, origin, destination, items[], status, events[], carrier
- Warehouse: id, name, location, capacity, status
- InventoryMove: id, warehouse_id, sku, qty, batch/serial, status, doc_refs

## Endpoints
- POST /shipments
  - Create shipment; 201; emits shipment.created
- GET /shipments/{id}
  - Shipment details with events and tracking
- GET /shipments
  - Cursor list; filters: status, carrier, origin/destination, date ranges

- POST /shipments/{id}/events
  - Post tracking scan or status update (idempotent); 409 on invalid transitions

- POST /warehouses/{id}/inventory-moves
  - Record inventory move (idempotent); integrates with Inventory module

- GET /warehouses/{id}/analytics
  - Aggregated KPIs; may be async if heavy (202 + Location)

- GET /shipments/{id}/audit
  - Audit trail per standards

## Errors & Conflicts
- RFC7807
- 409 for double scans or invalid transitions; 412 on ETag precondition failures

## Webhooks
- shipment.created, shipment.updated, shipment.delivered, scan.recorded
- HMAC-signed per standards
