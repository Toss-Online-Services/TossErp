---
agent: beast mode
---


### 0. Context

- Repo: `https://github.com/Toss-Online-Services/TossErp`
- Backend: .NET 8 (Web API, Clean Architecture/DDD-inspired), PostgreSQL via EF Core.
- Frontend: Nuxt 3/4 + Vue 3, TailwindCSS, headless/shadcn-style components, Pinia for state.
- Deployment target: Azure backend, Vercel frontend.
- Users: South African township & rural SMMEs (spaza shops, chisa nyamas, tuck shops, etc.).
- Deadline: MVP in users’ hands **by end of this month**.

Before making any changes:

1. List the repo structure.  
2. Read existing docs in:
   - Root `README*`
   - `/docs/**`
   - `.github/**`
   - `.cursor/**`
3. Summarise in your own words:
   - What is already implemented.
   - What is partially done.
   - What is missing for the MVP.

Print that summary first.

---

### 1. MVP Functional Scope

Implement or complete the following features.

#### 1.1 Authentication & Roles

- Ensure JWT auth is fully wired end-to-end.
- Support roles:
  - `Admin`
  - `Retailer` (Vendor)
  - `Supplier`
  - `Driver`
- Enforce role-based access on backend APIs and frontend routes.
- Admin can:
  - View user list.
  - Filter by role.
  - Activate / deactivate users.
  - Assign/change roles.

---

#### 1.2 Retailer Portal

**Product & Inventory Management**

- CRUD products:
  - name, SKU, category, unit, price, current stock, default supplier.
- Inventory changes:
  - Decrease on POS sale.
  - Increase on receiving supplier orders.
- Simple stock view:
  - current stock,
  - low-stock indicator.

**POS (Point of Sale)**

- POS screen with:
  - searchable product list,
  - cart to add/remove items and change quantities.
- Payment types:
  - cash,
  - card,
  - other.
- For cash:
  - apply rounding to nearest 5c where relevant.
- On checkout:
  - persist sale & line items to backend,
  - update inventory,
  - return receipt data.
- Offline-tolerant behaviour (MVP level):
  - if network unavailable, queue sales locally (localStorage / IndexedDB),
  - sync queued sales when connectivity returns.

**Orders to Suppliers**

- Retailer can create purchase orders:
  - choose supplier,
  - choose products & quantities.
- View list of purchase orders with statuses:
  - Draft
  - Submitted
  - Accepted (by supplier)
  - Shipped
  - Delivered

---

#### 1.3 Supplier Portal

- List incoming purchase orders from retailers.
- View order details.
- Update status:
  - Accept / Reject.
  - Mark “Ready for pickup” / “Shipped”.
- Optionally assign to driver (if driver exists) or leave for Admin assignment.

---

#### 1.4 Driver Portal

- Driver sees:
  - list of assigned deliveries,
  - each delivery: supplier location, retailer location, items, contact numbers.
- Driver can update status:
  - Accepted
  - Picked up
  - Delivered
- Capture simple delivery confirmation (boolean + note).  
  Photo upload can be left as `TODO`.

---

#### 1.5 Admin Portal

- Admin dashboard:
  - number of active retailers, suppliers, drivers,
  - number of orders by status,
  - total sales today (aggregate).
- Management screens:
  - Users: list, filter, activate/deactivate, assign roles.
  - Orders: list all orders, filter by status, drill into details.

---

#### 1.6 Onboarding Wizards

Persist onboarding data in the backend and ensure the wizard only appears until completed.

**Retailer Onboarding (after first login)**

- Step 1: Business/store profile (name, address, contact).
- Step 2: Add first few products (at least name + price).
- Step 3: Optional – invite staff / driver.

**Supplier Onboarding**

- Step 1: Business profile.
- Step 2: Define product categories and at least one product.

**Driver Onboarding**

- Step 1: Name, phone, vehicle type, registration, typical area.

---

### 2. Architecture & Code Constraints

- Follow **Clean Architecture** in .NET:
  - Domain
  - Application
  - Infrastructure
  - API
- Use **EF Core** with migrations for PostgreSQL.
- Keep service boundaries simple:
  - For MVP, a modular monolith is acceptable, but maintain clear logical boundaries for future extraction:
    - Identity/Auth
    - Products/Inventory
    - Sales/POS
    - Orders/Suppliers/Deliveries

On the frontend:

- Use Nuxt layouts to separate:
  - auth,
  - retailer,
  - supplier,
  - driver,
  - admin.
- Use **Pinia** stores where shared state makes sense (e.g. POS cart, current user/session).
- Use TailwindCSS and existing design tokens.
- Maintain South African context:
  - Always display currency in **ZAR (R)** with 2 decimals.
  - Friendly copy, non-technical language in the UI.

---

### 3. Process: How I Want You To Work

Follow this process strictly.

#### 3.1 Analysis Phase

1. Inspect repo.
2. Summarise what’s implemented vs. missing for the MVP items above.
3. Generate a clear TODO list grouped by:
   - Backend
   - Frontend
   - Tests
4. Print that plan and wait for my confirmation.  
   **Assume confirmation if I don’t explicitly deny it.**

---

#### 3.2 Implementation Phase (Iterative)

For each major feature group **in order**, do:

- **Backend:**
  - Add/adjust domain models and EF Core migrations.
  - Add/adjust application services and API endpoints.
  - Add/adjust auth/authorization attributes.

- **Frontend:**
  - Build/complete pages, components, and stores.
  - Wire them to the APIs.
  - Ensure forms have validation and user-friendly error messages.

- **Tests:**
  - Add/update unit and integration tests for core logic.
  - Add minimal e2e/API tests for happy-path flows.

Work in **vertical slices**:

- e.g. complete “Product CRUD” end-to-end (backend + frontend + tests)  
  before jumping to “POS sale”.

---

#### 3.3 Testing & Verification

- Run:
  - `dotnet test` for backend.
  - Any frontend tests / type checks (Nuxt, Vitest, etc.) if present.
- Fix failing tests or adjust them if the behaviour change is intentional and correct.
- Manually simulate:
  - Retailer logging in, setting up store, adding products, making a sale.
  - Retailer creating a purchase order.
  - Supplier accepting an order.
  - Driver seeing and delivering an order.
  - Admin reviewing data.

---

#### 3.4 Documentation & Clean-up

- Update any relevant docs (MVP status, how to run, feature overview).
- Ensure code is formatted and linted.
- Remove or clearly label TODOs and dead code.

---

### 4. Quality Bar

- It’s okay to **defer non-critical edge cases** but:
  - No obvious crashes in core flows.
  - No unprotected endpoints leaking other tenants’ data.
  - No completely untested core logic (auth, POS, orders).

- Prefer simple, clear solutions over over-engineering.  
  We can refactor after MVP.

If there is a trade-off between **perfect** and **shipping a stable MVP**, choose **shipping**, and leave clear TODOs for later.

---

### 5. First Action

Start now:

1. Print repo structure and summarise current implementation vs MVP scope.  
2. Propose your TODO list for implementing everything above (Backend, Frontend, Tests).  
3. Then begin implementing according to that plan.
