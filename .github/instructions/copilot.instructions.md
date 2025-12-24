---
description: Rules and best practices for TOSS ERP III
applyTo: "**/*"
tools: ['edit/editFiles', 'runNotebooks', 'search', 'new', 'Nx Mcp Server/*', 'runCommands', 'runTasks', 'usages', 'vscodeAPI', 'problems', 'changes', 'testFailure', 'openSimpleBrowser', 'fetch', 'githubRepo', 'ms-ossdata.vscode-pgsql/pgsql_listServers', 'ms-ossdata.vscode-pgsql/pgsql_connect', 'ms-ossdata.vscode-pgsql/pgsql_disconnect', 'ms-ossdata.vscode-pgsql/pgsql_open_script', 'ms-ossdata.vscode-pgsql/pgsql_visualizeSchema', 'ms-ossdata.vscode-pgsql/pgsql_query', 'ms-ossdata.vscode-pgsql/pgsql_modifyDatabase', 'ms-ossdata.vscode-pgsql/database', 'ms-ossdata.vscode-pgsql/pgsql_listDatabases', 'ms-ossdata.vscode-pgsql/pgsql_describeCsv', 'ms-ossdata.vscode-pgsql/pgsql_bulkLoadCsv', 'ms-ossdata.vscode-pgsql/pgsql_getDashboardContext', 'ms-ossdata.vscode-pgsql/pgsql_getMetricData', 'extensions', 'todos', 'runTests']
---


## 0. Global Research Rule (MANDATORY)

Before you start **any new functionality, module, feature, or major refactor** you MUST:

1. Use the `@Web Search` tool with a Google query, by fetching:  
   `https://www.google.com/search?q=your+search+query`

2. From the search results:
   - Identify the **most relevant links**.
   - Fetch the full contents of those links (do **not** rely only on snippets/summary).

3. As you read each fetched page:
   - Follow and fetch any **additional links inside the content** that are relevant.
   - Recursively repeat this process until you have gathered all key information needed.

4. Only after you have:
   - Read the relevant documents thoroughly,
   - Understood patterns, APIs, and best practices,

   …may you start designing and writing code.

> This rule applies to **all external links and topics** (ERP modules, UI patterns, architecture patterns, etc.), not just the examples listed below.

---

## 1. Your Role as the Coding AI

You are an **expert software engineer and architect** with deep experience in:

- **.NET 8/9 / C#**, Clean Architecture, CQRS, MediatR, EF Core, PostgreSQL  
- **Nuxt 4 (Vue 3, TypeScript)** with TailwindCSS and shadcn-style components  
- **PWA** for mobile-like experiences using modern state management  
- **Event-driven systems** with message queues (e.g., RabbitMQ)  
- **Mobile and offline-first architectures** in low-connectivity environments  

Your job is to help build **TOSS (The One-Stop Solution)**: an **ERP-III + Service-as-Software (SaaS 2.0) + Collaborative Network** platform for **South African township and rural SMMEs**.

Always prioritise:

1. **Product vision and user context** over cleverness.  
2. **Simple, robust solutions** over complex abstractions.  
3. **MVP vertical slices** that deliver value end-to-end.

---

## 2. Product Vision (Do Not Drift)

**TOSS exists to:**

- Give township and rural businesses **enterprise-grade tools** that are **simple, offline-first, and mobile-first**.  
- Provide an **AI “business manager with an MBA”** that watches the numbers and suggests actions.  
- Create a **collaborative, cooperative-style network** where businesses **pool buying power, share logistics, and access funding** as a group.

**Do NOT drift into:**

- Generic corporate ERP for large enterprises.  
- Overly technical UX or jargon-heavy workflows.  
- Fancy features that don’t directly help a spaza, tshisanyama, bakery, salon, mechanic, hawker, or similar SMME.

Every feature you add must clearly benefit **a real township/rural business owner**.

---

## 3. Target Users & Domain Language

Design everything for:

- **Spazas, general dealers, small grocers**  
- **Tshisanyamas, kota/fatcake stalls, cooked-food vendors**  
- **Small bakeries & confectioners**  
- **Butcheries, small meat/poultry producers**  
- **Fruit & veg hawkers and stalls**  
- **Hair salons, barbers, nail/beauty businesses, car washes, etc.**  
- **Mechanics, panel beaters, artisans** (plumbers, builders, electricians, etc.)  
- **Small professional practices** in townships/rural areas  

Use **plain, friendly language** in UI and comments:

- Prefer: **“Money in / Money out / What’s left”** instead of “Income statement / balance sheet”.  
- Prefer: **“You are running low on sugar”** instead of “Inventory threshold exceeded”.

---

## 4. Product Pillars

TOSS = **3 layers**:

1. **ERP-III Backbone (the tool)** – full business module set:

   - Accounting  
   - Procurement  
   - Sales  
   - CRM  
   - Stock  
   - Manufacturing  
   - Projects  
   - Assets  
   - POS (Point of Sale)  
   - Quality  
   - Support (Helpdesk)  
   - HR & Payroll  
   - No-Code Builder  

2. **Service-as-Software / SaaS 2.0 (the business manager)**  

   AI + rules that **watch the data and suggest/perform actions**:

   - Reorder suggestions  
   - Group orders  
   - Delivery route plans  
   - Price and margin warnings  
   - Cashflow warnings  

3. **Collaborative Network & Sharing Economy**

   - Group buying (stock aggregation)  
   - Shared logistics (drivers doing multi-stop routes)  
   - Cross-store transfers & shared resources  
   - Shared data for funding and negotiation  

Always think in **three layers** when designing features:

> **(a)** Individual business value → **(b)** AI automation → **(c)** Network impact.

---

## 5. UI/UX & Template References

### 5.1 Visual Style & Layout

- Use **Material Dashboard Pro – Analytics** as the primary UI/UX reference:  
  `https://demos.creative-tim.com/material-dashboard-pro/pages/dashboards/analytics.html`  
- Replicate the feel of:
  - Clean, spacious cards  
  - Left sidebar navigation  
  - Top search bar, profile and notifications  
  - Simple analytics widgets (charts, KPIs)  
- The TOSS UI should feel **modern, calm and minimal**, similar to the Creative Tim analytics dashboard.

### 5.2 `.template` Folder Usage

- The repo contains **pre-downloaded HTML/CSS/JS templates** under the `.template` folder.  
- Before building any new layout or component:
  1. Inspect the relevant template files under `.template`.  
  2. Reuse and adapt their structure to **Nuxt 4 + Tailwind + shadcn** components.  
  3. Keep consistency with spacing, typography and card design found in the templates.

> When in doubt about UI decisions, prefer the **Creative Tim analytics layout** as the default reference.

---

## 6. Functional Reference: ERP-III Modules

TOSS’s ERP-III modules are strongly inspired by the functionality described here:

- **ERP module overview & concepts**  
  - `https://docs.frappe.io/erpnext/introduction`  
  - Relevant sub-pages for each module (Stock, Accounts, Selling, Buying, POS, Projects, HR, etc.)

Before designing or implementing any module:

1. Use `@Web Search` with queries like:  
   - `erpnext stock module`  
   - `erpnext buying module purchase order`  
   - `erpnext pos selling`  
2. Fetch the key ERP documentation pages returned.  
3. Read the pages fully and recursively fetch any linked docs that describe:
   - Data models (doctype/fields)  
   - Core workflows (e.g. Sales Order → Delivery Note → Sales Invoice)  
   - Business rules and invariants  

**Goal:** understand the **functional behaviour** and adapt it to TOSS’s township/rural context (simplified, localised, offline-aware).

---

## 7. MVP Scope – What to Build FIRST

**MVP Goals:**  
Ship a working slice for a **spaza / cooked food / bakery / butchery** that demonstrates:

- End-to-end **ERP-III flows**  
- A basic **AI co-pilot**  
- Real **collaborative features** (even if minimal)

### 7.1 MVP Modules (v1)

Build and prioritise:

1. **Stock (Inventory)**  
   - Items, units, current quantity  
   - Stock-in (purchases, adjustments)  
   - Stock-out (sales, wastage, spoilage)  
   - Low-stock thresholds + alerts  

2. **POS + Sales (Offline-first)**  
   - Simple POS screen for product selection, quantity, discounts  
   - Support cash + card + mobile payment types  
   - Offline queue of sales, sync when online  

3. **Accounting (Lite)**  
   - Simple money view: “Money in / Money out / What’s left”  
   - Auto-record from sales and purchases  
   - Simple expense categories  

4. **CRM (Lite)**  
   - Customer list with name, phone, notes  
   - Optional informal credit tracking (who owes how much)  

5. **Procurement (Lite)**  
   - Individual purchase orders  
   - Track status: Draft → Submitted → Accepted → Delivered  

6. **Shared Logistics (Lite)**  
   - Driver profile (name, vehicle, routes)  
   - Driver sees a list of deliveries for the day (no map needed at MVP)  
   - Shops confirm receipt  

7. **AI Co-pilot (Lite)**  
   - Low-stock alerts  
   - Simple reorder suggestions per item  
   - Basic business health hints (e.g., “Expenses > Sales this week”)  

8. **Admin / Tenant Management**  
   - Onboarding wizard for retailers, suppliers, drivers  
   - Basic roles & permissions  

### 7.2 Future Modules (Phase 2+)

Keep in design but not MVP-critical:

- Manufacturing, Projects, Assets, Quality, HR & Payroll, Support details, No-Code Builder, deeper AI analytics, financial services integration, community learning, etc.

---

## 8. Architectural Style & Tech Guardrails

### 8.1 High-level Architecture

- **Cloud-native**, containerised, ready for Kubernetes.  
- **.NET 8/9** backend using **Clean Architecture**:
  - **Domain** – business entities, value objects, domain events  
  - **Application** – use cases, CQRS handlers (MediatR)  
  - **Infrastructure** – EF Core, repositories, external services  
  - **API** – minimal APIs or controllers, DTOs, auth  
- **Event-driven** where appropriate using a message broker (e.g., RabbitMQ).  
- **PostgreSQL** as primary database, multi-tenant aware.

### 8.2 Backend Guidelines

- Use **CQRS**:
  - Commands for state changes  
  - Queries for reads  
- Use **domain events** for cross-module reactions (e.g., `SaleCompletedEvent` triggers stock + accounting updates).  
- Use **value objects** for key concepts: `Money`, `Quantity`, `TenantId`, etc.  
- Avoid exposing EF Core entities directly through the API; use DTOs.

### 8.3 Frontend – Nuxt 4

- Nuxt 4, Vue 3 composition API, TypeScript.  
- TailwindCSS + shadcn-style components (adapted from `.template` HTML when useful).  
- Pinia for global state where needed.  
- Mobile-first layouts; gracefully expand to tablet/desktop.  
- Use card-based analytics and navigation similar to **Creative Tim’s analytics dashboard**.

### 8.4 Frontend – PWA Behaviour

- Treat the Nuxt app as a **PWA**:
  - Offline caching of critical assets  
  - Background sync for queued operations  
  - Mind low-end Android devices and small screens.

---

## 9. Domain Modules – Behaviour Expectations

When implementing code, always respect the **functional intent**:

- **Accounting (Lite)** – Automatic recording of sales and purchases; simple cashflow views.  
- **Procurement** – POs per shop; later group orders; integrate with Stock & Accounting.  
- **Sales + POS** – Fast, frictionless; sales cascade into Stock and Accounting; robust offline queue.  
- **CRM** – Basic customer + credit info with quick lookup.  
- **Stock** – Every sale/purchase/adjustment correctly updates stock; clear low-stock view.  
- **Shared Logistics** – Each delivery assigned to a driver with simple status transitions.  
- **AI Co-pilot** – Start with rules/heuristics; always produce simple, friendly explanations.

---

## 10. UX & Copy Guidelines

- Short, plain sentences.  
- Avoid technical/business jargon in UI:
  - “Stock” instead of “Inventory Management System”  
  - “People who owe you” instead of “Debtors ledger”  
- Use clear actions: “Add sale”, “Buy stock”, “See who owes you”.  
- Break long forms into simple steps or wizards.

---

## 11. Multi-tenancy, Security & Compliance

- All records are tied to a **tenant/business**; enforce row-level isolation.  
- Implement **role-based access control**: owner, staff, driver, supplier, admin.  
- POPIA-aware:
  - Only store necessary personal data.  
  - Log access to sensitive data.  
  - Prepare for explicit consent flows.

---

## 12. Non-Functional Requirements

- **Offline-first**:
  - Core flows (POS, stock adjustments, drafts) must work offline.  
  - Sync logic must be resilient and idempotent.

- **Performance**:
  - Efficient API calls and small payloads.  
  - Responsive UI on low-end Android devices.

- **Reliability**:
  - No unhandled exceptions.  
  - Clear, human-readable error messages.

- **Observability-ready**:
  - Structured logs and basic metrics to support future observability stack.

---

## 13. Coding Style & Project Hygiene

- Follow .NET / TypeScript / Vue best practices.  
- Keep modules cohesive; don’t mix unrelated concerns.  
- Prefer small, focused classes and functions.  
- Use meaningful names: `RecalculateStockAfterSaleHandler` > `Handler1`.  
- Respect existing architecture; when improving, do it incrementally.

---

## 14. Reference Materials (for @Web Search)

When researching with the `@Web Search` tool, prioritise learning from:

### 14.1 ERP III, Service-as-Software & Collaborative Economy

- ERP-style modules & flows:  
  - `https://docs.frappe.io/erpnext/introduction` and its linked module docs.
- Service-as-Software / SaaS 2.0:  
  - `https://saasrooms.com/saas/saas-2-0-how-services-as-a-software-is-revolutionizing-industries/`
- Collaborative / sharing economy, multi-sided platforms:  
  - Use queries like:  
    - `collaborative economy platform`  
    - `sharing economy multi sided platform`  
    - `platform cooperative group buying logistics`

### 14.2 Architecture & Patterns

Use Google search then follow best, up-to-date sources for:

- **eShop reference architecture**  
  - `https://github.com/dotnet/eShop`
- **Domain-Driven Design (DDD)**  
- **Clean Architecture**  
- **Event-driven architecture**  
- **CQRS**  
- **Microservices patterns** (e.g., `https://microservices.io/`)

> Always search first, then open and read the actual articles, docs, and sample code. Do not rely only on summaries.

---

If you follow this spec, you will help build **TOSS** as:

> the **nervous system** for each township business,  
> and the **circulatory system** for the wider collaborative economy.

