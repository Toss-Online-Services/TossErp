# PRD: TOSS ERP III Core Platform – Operations Management & Core ERP Modules

## 1. Product Overview

**Product name:** TOSS ERP III Core Platform

**Owner:** TOSS Online Services

**Version:** v0.1 (Draft)

### 1.1 Vision

Build a modern ERP-III platform that helps small businesses in South African townships and rural areas run like professional retailers and manufacturers, while staying simple enough for non-technical, non-accounting users.

The platform combines:

* **Operations Management** (day-to-day running of the business)

* **Core ERP Modules:** Accounting, Procurement, Sales, CRM, Stock, Manufacturing, Projects, Assets, Point of Sale, Quality, Support, HR & Payroll

* **ERP-III Principles:** Collaboration, social feedback loops, and direct engagement with customers, vendors, and partners.

### 1.2 Problem Statement

Township and rural businesses (spaza shops, chisa nyamas, salons, mechanics, etc.) struggle with:

* No single view of their operations (stock, cash, staff, suppliers, jobs, etc.).

* Manual, paper-based processes and fragmented tools (WhatsApp, notebooks, Excel).

* Weak collaboration with suppliers, drivers, and customers.

* Lack of basic financial records, making it hard to get credit or grow.

They need **one simple platform** that covers daily operations and grows with them.

### 1.3 Goals & Non-Goals

**Goals**

1. Provide a **single operations hub** covering the core modules listed.

2. Make workflows **simple, guided, and mobile-first**, even for users without matric.

3. Enable **ERP-III style collaboration**:

   * Customers can give feedback and place orders.

   * Suppliers can share offers and stock availability.

   * Partners (drivers, service providers) can plug in.

4. Offer a **configurable but opinionated** setup tailored to township/rural SMEs.

**Non-Goals (Initial Releases)**

* Advanced enterprise features (complex multi-company consolidations, IFRS-heavy accounting).

* Deep country-by-country tax customisation beyond South Africa at first.

* Highly custom per-customer workflows (focus on templates, not bespoke builds).

---

## 2. Target Users & Personas

### 2.1 Primary Personas

1. **Spaza Shop / Small Retail Owner – "Mama D"**

   * Wants to track stock, sales, cash, and supplier orders.

   * Low patience for complex UIs; uses WhatsApp daily, not Excel.

   * Needs alerts: low stock, best-selling items, cash flow.

2. **Workshop / Mechanic / Artisan – "Bra Joe"**

   * Works on jobs/projects (repairs, installations).

   * Needs job cards, parts used, labour tracking, and basic invoicing.

3. **Small Manufacturer / Bakery – "Gogo Bakery"**

   * Produces goods from raw materials.

   * Needs to plan production, see raw material usage and costs.

4. **TOSS Partner / Consultant – "Admin Champion"**

   * Helps multiple small businesses set up and use the platform.

   * Needs multi-tenant dashboards, basic reporting, and setup tools.

---

## 3. Scope & Modules

### 3.1 Operations Management (Core Layer)

**Objective:** Provide one screen / dashboard to run the day: today's sales, stock alerts, tasks, deliveries, and messages.

**Key Features**

* **Today View / Home**

  * Snapshot: total sales today, cash in/out, low stock items, pending orders, tasks.

  * Alerts: "Stock running low on Bread 700g", "Payment overdue from Sipho".

* **Task & Workflow Orchestration**

  * Small "to-do" style tasks: receive stock, follow up a customer, complete a job.

  * Link tasks to records (invoice, purchase order, job card, etc.).

  * Simple statuses: To Do, In Progress, Done.

* **Notifications & Collaboration**

  * In-app notifications for events (stock in, payment received, delivery dispatched).

  * Simple comment/chat on a transaction (e.g., a delivery note or job card).

* **Basic Activity Timeline**

  * Timeline per business: "Today you did X sales, Y purchases, Z payments."

---

### 3.2 Accounting

**Objective:** Give owners a simple but correct view of money in and out.

**Key Features**

* **Cashbook & Bank**

  * Record cash sales, expenses, and bank transfers.

  * Bank accounts and petty cash accounts.

* **Invoices & Payments**

  * Create simple invoices and receipts.

  * Mark invoices as paid (cash, card, EFT, "on account").

* **Basic Financial Reports**

  * Profit & Loss (simple form).

  * Cashflow summary.

  * Debtors (who owes you) and creditors (who you owe).

* **Localisation**

  * ZAR as base currency (MVP).

  * Support VAT at a basic level.

---

### 3.3 Procurement

**Objective:** Help owners order stock and services from suppliers efficiently.

**Key Features**

* Supplier list (name, contact, payment terms).

* Purchase Requests → Purchase Orders → Goods Receipt.

* Capture supplier invoices and link to Accounting.

* Track expected deliveries, overdue orders, and pricing history.

---

### 3.4 Sales

**Objective:** Manage all non-POS sales (quotes, orders, invoices).

**Key Features**

* Quotations → Sales Orders → Delivery Notes → Invoices.

* Customer list with credit status and outstanding amounts.

* Integration with POS and CRM.

---

### 3.5 CRM (Customer Relationship Management)

**Objective:** Manage relationships and communication mainly in simple, non-technical terms.

**Key Features**

* Customer profiles (basic fields + tags: VIP, regular, "on account").

* Interaction log (calls, WhatsApps, visits; manual capture at MVP).

* Simple campaigns: e.g., send SMS/WhatsApp to a customer group (V2+).

* Feedback capture: compliments, complaints linked to sales or tickets.

---

### 3.6 Stock (Inventory)

**Objective:** Keep track of what is in the shop, storeroom, and in transit.

**Key Features**

* Items catalog (name, barcode, unit, cost, selling price, category).

* Stock locations (shop, storeroom, truck).

* Stock movements:

  * Purchases, sales (including POS), adjustments, transfers.

* Stock alerts:

  * Min/max levels.

  * "Items that are moving fast/slow".

* Stock valuation (basic weighted average).

---

### 3.7 Manufacturing

**Objective:** For small producers (e.g., bakeries, butchers) to manage simple production.

**Key Features**

* Bill of Materials (BOM) per product.

* Production Orders (planned & actual).

* Material consumption and finished goods receipt into stock.

* Basic production costing (material + simple overhead).

---

### 3.8 Projects

**Objective:** Track jobs and projects (e.g., car repairs, building jobs).

**Key Features**

* Project / Job card creation with customer, description, due date.

* Add tasks, materials (from stock), labour, and notes.

* Link invoices and receipts.

* Project/job status: New, In Progress, On Hold, Completed, Closed.

---

### 3.9 Assets

**Objective:** Track important business assets (fridges, machinery, vehicles).

**Key Features**

* Register of assets with value, date, location, and condition.

* Maintenance history (e.g., "serviced on X date").

* Optional simple depreciation (later release).

---

### 3.10 Point of Sale (POS)

**Objective:** Fast checkout for walk-in customers, works offline where possible.

**Key Features**

* Simple POS interface:

  * Product search by barcode or name.

  * Cart, discounts, and multiple payment types (cash, card, on account).

* Offline-first approach (local cache; sync when online).

* Daily till summary (Z-report).

* Link to stock and accounting automatically.

---

### 3.11 Quality

**Objective:** Support basic quality checks for food/service businesses.

**Key Features**

* Checklists (cleanliness, food safety, equipment).

* Record incidents (spoiled stock, customer complaints).

* Simple actions/follow-ups assigned to staff.

---

### 3.12 Support (Service Desk)

**Objective:** Track issues and support requests from customers.

**Key Features**

* Tickets with type (complaint, query, request).

* Status: New, In Progress, Resolved.

* Link to customer, order, or project.

* Internal notes and simple chat/comment thread.

---

### 3.13 HR & Payroll

**Objective:** Basic people management, MVP-level.

**Key Features**

* Employee records (contact, role, rate).

* Attendance/shift tracking (simple time in/out or days worked).

* Payroll summary (gross salary, deductions, net pay; export/report, not full SARS integration at MVP).

* Leave tracking (optional V2).

---

## 4. User Experience & Design

### 4.1 Principles

* **Plain language** (avoid "accounts receivable"; say "customers who still owe you").

* **Mobile-first**, but works on desktop.

* **Guided flows**: step-by-step wizards for complex actions like "Receive stock" or "Run payroll".

* **Dashboards with minimal numbers**, more traffic light indicators: red/amber/green.

### 4.2 Navigation

* Global navigation: Home, Sales, Stock, Money, People, Jobs, Settings.

* Each module has:

  * "What do you want to do today?" quick actions (e.g., "Sell", "Order stock", "Pay a supplier").

---

## 5. Functional Requirements (High-Level)

### 5.1 Cross-Cutting

* Multi-business (one user can manage more than one shop/workshop).

* Role-based access (Owner, Manager, Cashier, Staff).

* Audit trail per transaction (who did what, when).

* Basic import/export (CSV for items, customers, suppliers).

### 5.2 Data & Integrations (Future-Facing)

* Prepare for integration with:

  * Banks (statement import).

  * Payment gateways.

  * Messaging (WhatsApp / SMS APIs).

* API layer for future partners (drivers, suppliers, etc.).

---

## 6. Non-Functional Requirements

* **Performance:** Common actions (open POS, save invoice, lookup item) should feel instant on mid-range Android.

* **Availability:** 99% uptime target for SaaS platform.

* **Security:**

  * Secure authentication (email/phone + OTP and password).

  * Encrypted at rest and in transit.

* **Scalability:** Design for growing from a few shops to thousands of SMEs.

---

## 7. Success Metrics

* Onboard **10 pilot businesses** actively using at least 3 modules (e.g., POS, Stock, Accounting) within 3 months of MVP.

* **Daily active usage:** 70% of pilot businesses log in at least 4 days a week.

* **Reduction in stock-outs:** At least 25% fewer reported "out of stock" events over 3 months (based on user feedback / logs).

* **Time savings:** Users report "it saves me time" in >70% of post-onboarding surveys.

---

## 8. Release Plan (Phases)

### Phase 1 – MVP (3–6 months)

Focus on **Retail & Service** businesses:

* Operations Dashboard

* POS

* Stock

* Sales (basic)

* Customers (basic CRM)

* Simple Accounting (cashbook, invoices, payments)

* Suppliers & basic Procurement

* Simple Projects/Jobs

* User management & roles

### Phase 2 – Extended Operations (6–12 months)

* Manufacturing (simple BOM + production)

* Assets

* Quality & Support (tickets, checklists)

* HR & Payroll (basic)

* Improved analytics and dashboards

* Basic ERP-III collaboration surfaces (feedback, offers, basic vendor/customer portal)

### Phase 3 – ERP-III Collaboration & Ecosystem

* Customer self-service portal (order/feedback).

* Supplier / driver portal.

* Group purchasing, shared logistics, and advanced analytics.


