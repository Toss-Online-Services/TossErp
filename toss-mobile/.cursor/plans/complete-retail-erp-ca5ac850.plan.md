<!-- ca5ac850-64b6-474e-a00a-d0334788aa21 c9b2d5a9-28af-4da8-8199-27ac67eb701e -->
## TOSS ERP: Backend + POS/Web Admin Alignment

### 1) Target Architecture

- Backend: ASP.NET Core (eShop-style modular monolith now, evolvable to microservices)
- Clients: Flutter Mobile POS (minimal), Flutter Web Admin (full ERP)
- Data: SQL Server (primary), Redis (optional cache), Firestore (lightweight realtime cache/notifications), SQLite (mobile offline)
- Auth: ASP.NET Identity + JWT; roles: Admin, Manager, Cashier
- Sync: Mobile queued actions → Backend APIs; optional Firestore listeners for realtime updates

### 2) Backend Solution Structure (create here: `C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp`)

- `TossErp.sln`
- `src/BuildingBlocks/`
- `TossErp.BuildingBlocks` (base entities, Result, pagination, specs)
- `TossErp.Infrastructure` (EF Core, SQL Server, DI, Outbox, Redis cache)
- `TossErp.Identity` (ASP.NET Identity, JWT, role policies)
- `src/Modules/` (modular folders; separate DbContexts)
- `Catalog` (Products, Categories, Prices, Barcodes)
- `Inventory` (Stock, Locations, Movements)
- `Sales` (Carts, Orders, Payments, Receipts)
- `Customers` (Basic customer profiles & loyalty balance view)
- `Procurement` (POs) [Admin only]
- `Accounting` (Financial Tx/Expenses) [Admin only]
- `Support` (Tickets) [Admin only]
- `src/Api/`
- `TossErp.Api` (single WebAPI host, versioned endpoints, OpenAPI, CORS)
- `tests/` (unit/integration per module)

### 3) Minimal POS Feature Set (Mobile)

- Product browse/search/scan (SKU/barcode)
- Simple cart (discounts, taxes from backend)
- Payments: cash, card (external terminal integration stub), mobile money
- Checkout: create order, reduce stock, receipt generation (PDF/ESC-POS)
- Basics: view customer info, check stock level by location
- Offline: queue sales; sync when online

### 4) Web Admin Feature Set (Flutter Web)

- Full ERP: CRM, Inventory, Procurement, Accounting, Support, Reports, Settings
- Management only (create/update/delete entities), dashboards, approvals

### 5) API Surface (v1) – POS-critical first

- Auth
- POST `/api/v1/auth/login` → JWT
- GET `/api/v1/auth/profile`
- Catalog
- GET `/api/v1/catalog/products?query=&barcode=&page=&size=`
- GET `/api/v1/catalog/products/{id}`
- Inventory
- GET `/api/v1/inventory/stock/{productId}?locationId=`
- Customers
- GET `/api/v1/customers?query=`
- GET `/api/v1/customers/{id}`
- Sales (POS)
- POST `/api/v1/pos/carts/price` (validate/price cart)
- POST `/api/v1/pos/checkout` (create order, payments, inventory movements)
- GET `/api/v1/pos/orders/{orderNumber}`
- GET `/api/v1/pos/orders/{orderNumber}/receipt` (PDF/ESC-POS text)
- Sync
- POST `/api/v1/sync/actions` (batch offline actions: sales, payments)
- GET `/api/v1/sync/changes?since=timestamp` (delta for products/prices/stock)
- Admin-only endpoints (for Web Admin) exposed in separate areas/controllers (CRUD: products, inventory, POs, expenses, tickets, reports)

### 6) Data Model (initial)

- Catalog: Product(id, sku, barcode, name, price, taxRate, isActive)
- Inventory: Stock(productId, locationId, qtyOnHand), Movement(type, qty, ref)
- Sales: Order(orderNumber, customerId, lines[], totals, payments[], status)
- Customers: Customer(id, name, phone, email, loyaltyPoints)
- Payments: Payment(id, orderNumber, method, amount, ref)

### 7) Security & Roles

- Roles: `Admin`, `Manager`, `Cashier`
- POS endpoints require Cashier+; Admin endpoints require Admin/Manager
- JWT with refresh tokens; device binding optional for POS terminals

### 8) Mobile Integration Strategy

- Keep SQLite as source for browsing offline; nightly delta pull via `/sync/changes`
- Use current queued actions to POST to `/sync/actions`
- Keep Firestore as optional realtime cache (product/price updates) and store receipts; backend can publish updates to Firestore via service account

### 9) Flutter App Changes

- Mobile (POS):
- Remove CRM/Inventory/Procurement/Accounting UI flows
- Keep: product list/grid with scan, cart, payment screen, receipt screen, customer lookup, stock check
- Providers: simplify to CatalogProvider, CartProvider, CheckoutProvider, AuthProvider
- Routing: `home → scan/list → cart → payment → receipt`
- Web Admin: new Flutter web project
- Material 3, Riverpod, GoRouter, feature folders per module
- Pages: Dashboard, Catalog, Inventory, Procurement, Accounting, CRM, Support, Reports, Settings

### 10) Delivery Steps

1. Backend scaffolding (solution, projects, DI, logging, Swagger, EF Core, SQL Server)
2. Identity & roles + JWT; seed Admin, Manager, Cashier
3. Catalog + Inventory read models (products, stock) with endpoints
4. POS Sales: cart pricing, checkout, payments, receipt generation
5. Sync endpoints (batch actions + deltas)
6. Admin APIs (catalog/inventory CRUD) for Web Admin MVP
7. Flutter Mobile refactor to POS-only
8. New Flutter Web Admin app scaffold + auth + basic CRUD pages
9. Printing integrations (ESC/POS over LAN; share to PDF)
10. CI/CD (GitHub Actions), environment configs, docs

### 11) Non-Functional

- Observability: Serilog + OpenTelemetry + Health checks
- Pagination, caching headers (ETag/If-None-Match) for catalogs
- Concurrency: optimistic concurrency tokens on inventories/orders
- Testing: unit tests per module, integration tests for checkout

### 12) Environment & Config

- Local: SQL Server Developer, `appsettings.Development.json`
- Secrets: dotnet user-secrets for JWT keys and Firestore service account
- CORS: allow `toss-mobile` and `toss-web-admin` origins

### 13) Risks & Mitigations

- Offline sync conflicts → server wins with merge hints; show conflict UI
- Payment integrations vary → abstract payment provider; start with mock
- Firestore consistency → use it only as cache/notifications, backend is source of truth

### To-dos

- [ ] Create ASP.NET Core solution (eShop-style) with modules and API host
- [ ] Add Identity, roles, JWT (Admin/Manager/Cashier) and seed data
- [ ] Implement products and stock read endpoints for POS
- [ ] Implement cart pricing, checkout, payments, receipt APIs
- [ ] Implement offline sync: batch actions and delta changes APIs
- [ ] Expose admin CRUD endpoints for Catalog/Inventory to web admin
- [ ] Refactor Flutter mobile to POS-only flows and providers
- [ ] Create new Flutter web admin app with auth and base modules
- [ ] Add ESC/POS LAN and PDF receipt generation
- [ ] Wire GitHub Actions, migrations, health checks and docs