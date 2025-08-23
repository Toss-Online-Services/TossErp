---
description: Logistics API outline — endpoints, auth scopes, headers, idempotency, pagination, status flows
---

# Logistics API

Reference: See shared API standards in ../api-specs/STANDARDS.md for authentication, headers, RFC9457 errors, cursor pagination, idempotency keys, rate limits, webhooks, and auditing.

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

### POST /shipments
- Scopes: logistics:write, shipment:write
- Headers: Idempotency-Key required
- Body: { order_ref, origin, destination, items: [ { sku, qty } ], carrier? }
- Responses: 201 Created with Shipment JSON; Webhook shipment.created

### GET /shipments/{id}
- Scopes: logistics:read
- Responses: 200 OK with Shipment JSON including events

### GET /shipments
- Scopes: logistics:read
- Query: status, carrier, origin, destination, date_from, date_to, limit, cursor
- Response: list envelope with Shipment[]

### PUT /shipments/{id}
- Scopes: logistics:write
- Headers: If-Match required, Idempotency-Key required
- Body: partial updates (carrier, destination, status when allowed)
- Responses: 200 OK new `ETag`; 412 on ETag mismatch (problem+json)

### POST /shipments/{id}/events
- Scopes: shipment:write
- Headers: Idempotency-Key required
- Body: { type: "SCAN"|"STATUS", code?: string, occurred_at, location? }
- Responses: 201 Created; 409 invalid transitions or duplicate scans; Webhook scan.recorded or shipment.updated

### POST /warehouses/{id}/inventory-moves
- Scopes: logistics:write
- Headers: Idempotency-Key required
- Body: { sku, qty, batch?: string, serial?: string, to_warehouse_id?, from_warehouse_id?, doc_refs?: [] }
- Responses: 201 Created; integrates with Inventory module via events

### GET /warehouses/{id}/analytics
- Scopes: logistics:read
- Query: since, window
- Responses: 202 Accepted with Location: /jobs/{job_id} for heavy aggregations; 200 OK for light ones

### GET /shipments/{id}/audit
- Scopes: logistics:read
- Response: list envelope of audit entries

## Errors & Conflicts
- RFC9457
- 409 for double scans or invalid transitions; 412 on ETag precondition failures

## Webhooks
- shipment.created, shipment.updated, shipment.delivered, scan.recorded
- HMAC-signed per standards

Payload example (shipment.delivered):
```json
{
  "id": "evt_01J6QA5JZP",
  "type": "shipment.delivered",
  "created_at": "2025-08-23T13:05:44Z",
  "request_id": "01J6QA5FQX",
  "data": {
    "shipment": { "id": "shp_789", "status": "DELIVERED", "delivered_at": "2025-08-23T13:05:40Z" }
  }
}
```
