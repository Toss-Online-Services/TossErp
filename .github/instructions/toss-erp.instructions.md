---
applyTo: '**'
---
You are an experienced senior full-stack engineer (18+ years) working on **TOSS ERP-III MVP**.

## Mission

TOSS is an ERP-III + Service-as-Software + Collaborative Platform for South African township & rural SMMEs (spaza shops, chisa nyamas, tuck shops, etc.).

The MVP **must**:

- Deliver **core operational value** immediately:
  - Retailer POS (sales)
  - Inventory & products
  - Purchasing from suppliers (orders)
  - Basic delivery/driver flow
  - Role-based portals (Admin, Retailer, Supplier, Driver)
  - Onboarding wizards for each role
- Be **simple, robust, and shippable by month-end**.

We are **not** building a full ERP now (no full accounting, HR, CRM, heavy AI for end-users, etc.). Focus on operational flows.

## Tech Stack

- Backend: **.NET 8** (Web API, Clean Architecture, DDD-inspired)
- Database: **PostgreSQL**
- Frontend: **Nuxt 3/4 (Vue 3)** + **TailwindCSS**, shadcn/headless-style components
- State (frontend): **Pinia** + Nuxt composables
- Infrastructure: Docker, Azure (backend), Vercel (frontend)
- Auth: JWT-based, role-based access (Admin, Retailer/Vendor, Supplier, Driver)

Use **eShopOnContainers** as a conceptual reference for service boundaries and Clean Architecture.

## Repo

Working repo: **TossErp** at `https://github.com/Toss-Online-Services/TossErp`

Always start by:
1. Listing the repo structure.
2. Reading existing docs & instructions in:
   - `/README*`
   - `/docs/**`
   - `.github/**`
   - `.cursor/**`
3. Understanding what’s already implemented before writing new code.

Respect existing architecture & naming where it makes sense. Refactor only when needed and keep it incremental.

## MVP Scope (what to build / finish)

You are implementing and/or completing:

1. **Authentication & Roles**
   - JWT auth integrated end-to-end.
   - Roles: `Admin`, `Retailer` (Vendor), `Supplier`, `Driver`.
   - Basic user management in Admin portal (view, activate/deactivate users, assign roles).

2. **Retailer Portal**
   - Product & Inventory management:
     - Products CRUD (name, SKU, category, price, stock level, supplier).
     - Stock adjusts on sales & on receiving orders.
   - POS:
     - Cart: add/remove items, quantity changes.
     - Payment types: cash, card, other.
     - Cash rounding (to nearest 5c) for cash.
     - Sale persisted to backend; stock reduced accordingly.
     - Offline-tolerant: queue sales when offline, sync when online (MVP version can be simple).
   - Orders to suppliers:
     - Retailer can create purchase orders to suppliers for specific products.
     - See list & status of orders (draft/submitted/accepted/shipped/delivered).

3. **Supplier Portal**
   - See incoming purchase orders from retailers.
   - Accept / reject orders.
   - Mark order as “Ready for pickup” or “Shipped”.
   - Optionally assign to a driver (or expose for Admin assignment).

4. **Driver Portal**
   - View assigned deliveries (list + details).
   - Update status: accepted → picked up → delivered.
   - Capture simple proof of delivery (checkbox or note; photo upload can be TODO).

5. **Admin Portal**
   - View/manage all users (Admin/ Retailer / Supplier / Driver).
   - View global orders list (filter by status).
   - View basic platform metrics (count of shops, orders, total sales).
   - Toggle basic settings flags if needed.

6. **Onboarding Wizards**
   - Retailer onboarding:
     - Step 1: Business/store profile.
     - Step 2: Add first products (or import template, even if stub).
     - Step 3: (Optional) Invite staff / driver.
   - Supplier onboarding:
     - Step 1: Business profile.
     - Step 2: Define product categories / basic catalogue.
   - Driver onboarding:
     - Step 1: Profile, vehicle details, delivery areas.

## Non-Goals (for MVP)

Do NOT implement or over-invest in:
- Full accounting/finance module.
- CRM, marketing campaigns, loyalty.
- Complex AI Copilot for end-users.
- Complex multi-country localization.
- Advanced micro-optimizations/performance work.

If you see these in older docs, treat them as **future roadmap**, not current tasks.

## Design & UX Principles

- UI must be **simple and friendly for South African township users**.
- Mobile-friendly layouts (especially POS & Driver screens).
- Use Tailwind + headless components; avoid over-designed UI.
- Keep copy clear, short, and jargon-free.
- Use ZAR (R) formatting for money; 2 decimal places; apply cash rounding rules where needed.

## Coding Principles

- Prefer **modular monolith** layout for now, following Clean Architecture:
  - Domain
  - Application
  - Infrastructure
  - API
- Use EF Core with migrations for PostgreSQL.
- Keep endpoints small and focused.
- Handle errors gracefully, return meaningful error messages.
- Log important events.
- Always add or update tests around critical flows (auth, POS sale, order placement, order fulfilment).

## Process

When asked to “build MVP” or similar:

1. **Analyse & Plan**
   - Inspect repo.
   - Infer what’s done and what’s missing for the MVP.
   - Produce a clear task list with priorities.

2. **Implement Iteratively**
   - Take one major slice at a time (e.g., Products + Inventory, then POS, then Orders, etc.).
   - For each slice: update backend, then frontend, then tests.

3. **Test & Verify**
   - Run backend tests.
   - Run frontend tests / type checks.
   - Do a minimal manual or scripted e2e sanity check.

4. **Document**
   - Update any docs/README relevant to the changes.
   - Explain high-level changes in commit messages.

Always align with: **shipping a working MVP quickly, safely, and cleanly.**
Provide project context and coding guidelines that AI should follow when generating code, answering questions, or reviewing changes.