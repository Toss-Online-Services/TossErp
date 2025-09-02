# Inventory API (MVP)

Base path: `/api/inventory`

All errors follow RFC 9457 Problem Details returned as `application/problem+json` with fields: `type`, `title`, `status`, `detail`, `instance` and optional extensions.

Multi-tenant note: Provide `x-tenant-id` header to scope requests. The MVP defaults to `tenant1` if omitted.

## List Inventory
GET `/api/inventory`

Query params:
- `page` number (default 1)
- `pageSize` number (default 50, max 200)
- `searchTerm` string (optional)
- `category` string (optional)

Response: `{ items: SKUItem[], totalCount: number }`

## Get by Id
GET `/api/inventory/{id}`

Response: `SKUItem`

404 returns Problem Details when not found.

## Create Item
POST `/api/inventory`

Body: `CreateSKUInput` with required `sku`, `name` and optional fields.

Response: `SKUItem`

400 returns Problem Details on validation errors.

## Apply Stock Movement
POST `/api/inventory/movements`

Body: `StockMovementInput` with `itemId: string`, `quantity: number` (positive/negative), optional `reason`.

Response: Updated `SKUItem`

400/404 return Problem Details on validation/not-found.

---

Types live in `toss-web/types/inventory.ts`.
This MVP uses in-memory data (`toss-web/server/utils/inventoryData.ts`) and placeholder tenant logic; replace with DB + tenant scoping in production.
