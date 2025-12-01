# TOSS ERP-III Combined Requirements (PRD + Functional Spec)

_Source references: `.taskmaster/docs/prd-platform-erp-iii-core.md` and `docs/functional-spec.md`_

## 1. Product Overview & Pillars

- **Audience:** South African township and rural SMMEs (spazas, tshisanyamas, bakeries, salons, mechanics, artisans, drivers, micro manufacturers). Extremely resource constrained, low digital literacy, intermittent connectivity, mostly low-end Android phones.
- **Platform Pillars:**
  1. **ERP-III Backbone:** Classic ERP domains (Sales, POS, Stock, Buying, Accounting, CRM, Projects, HR, Logistics, Manufacturing) with simplified, mobile-first UX.
  2. **SaaS 2.0 / AI Copilot:** Always-on assistant that monitors data, suggests or drafts actions (reorders, promotions, alerts, cross-sell, cashflow warnings) and answers natural language business questions.
  3. **Collaborative Network:** Multi-tenant features that let independent shops cooperate (group buying, pooled logistics, referrals, loyalty, community learning, shared credit signals).
- **Non-negotiables:** Offline-first PWA, multi-tenant row-level isolation, ZAR + VAT, event-driven architecture, Clean Architecture (.NET 8/9 backend, Nuxt 4 frontend), role-based access, POPIA compliance, auditable ledger, AI explainability, mobile-first UI referencing Material Dashboard Pro analytics aesthetic.

## 2. Platform Architecture & Cross-Cutting Requirements

- **Backend:** .NET 8/9, Clean Architecture layers (Domain, Application, Infrastructure, API). EF Core + PostgreSQL, MediatR CQRS, domain events for module reactions (sale -> stock -> accounting). JWT auth + OTP, Serilog structured logging, FluentValidation, caching (Redis), background services (Hangfire) for sync/AI jobs, SignalR websockets for realtime dashboard/notifications.
- **Frontend:** Nuxt 4 (Vue 3.5+, Vite 5), `<script setup>` + TypeScript, Tailwind + shadcn/radix components, Pinia stores, VueUse, PWA mode (workbox, background sync, IndexedDB). Auto-import composables/components, file-based routing, SEO via `useSEO`.
- **Security & Compliance:** JWT + refresh tokens, per-tenant encryption (sensitive fields), POPIA-friendly consent + audit logging, role-based modules (Owner, Manager, Cashier, Staff, Supplier, Driver, Accountant). All API calls must be tenant-scoped; add data-protection and E2E TLS.
- **Offline-first:** Local caching of catalog, customers, tasks, pending transactions with background sync + conflict resolution (first-confirm wins + notifications). POS, inventory adjustments, purchase receipts, sales orders, tasks, CRM updates must work offline and queue.
- **Mobile-first UI conventions:** Single column, large buttons, low-text high-icon, traffic-light indicators, copy uses plain language (“Money In/Out”, “People who owe you”). Dark/high-contrast theme for sunlight readability; reference Material Dashboard Pro analytics layout for cards/nav.
- **Testing & Quality:** Unit (xUnit/Vitest) + integration (Playwright/Cypress) per module. Add load tests for POS & inventory concurrency. All new modules require test strategies describing success, edge, failure cases.

## 3. Core Modules (summaries merge PRD + functional spec)

### 3.1 Operations Dashboard
- **Goal:** “Today view” showing daily sales, cash in/out, low stock, pending POs, overdue invoices, driver tasks, AI alerts, tasks queue.
- **Features:** KPI cards, sparkline charts (sales vs last week, cashflow), quick actions (New Sale, Receive Stock, Pay Supplier), offline timestamp, role-based widgets, activity timeline + AI summary (“You sold 90% of cement today”). Refresh via SignalR / polling fallback.
- **Rules:** Respect tenant & role filters, show VAT either excluded or clearly labelled, fallback to last synced data offline.

### 3.2 Task & Notification Layer
- Tasks linked polymorphically (Invoice, PO, JobCard, Delivery) with statuses (To Do/In Progress/Done), due dates, assignee, reminders, timeline log. Notifications via SignalR + digest email/SMS; filter by role; offline queue of “my tasks”.

### 3.3 Authentication, Multi-Tenancy & User Management
- Email/phone login + password + OTP, support invite links, user can belong to multiple businesses with quick switcher. Tenants have plan tier metadata, default modules, branding. Provide user admin UI (roles, permissions, deactivate/reactivate). Audit log per action (who/when/old vs new values).

### 3.4 Stock (Inventory)
- **Features:** Item master with barcode, UoM, cost/sell price, supplier, reorder levels per warehouse; multi-warehouse tracking; stock ledger with weighted average cost (default FIFO for valuation vs SAP style). Stock movements: PO receipt, sales issue, adjustments, transfers, manufacturing consumption/production, returns. Batch/serial support, expiry warnings, stock reconciliation with variance reason. Low-stock + fast/slow moving analysis feeding AI + dashboard. Offline support for item lookup, adjustments, transfers.
- **Rules:** Prevent negative stock unless override, mandatory warehouse, log all adjustments, unit conversions, reorder suggestions should exclude quantities already on open POs, tie valuation to accounting entries, optionally enforce FEFO for perishable goods.

### 3.5 POS (Point of Sale)
- Touch-first cart, barcode/camera scan, quick grid, hold/resume carts, multi-tenders (cash/card/mobile/credit), cash rounding to nearest 5c with rounding ledger, offline queue (IndexedDB) with retry sync, sequential invoice numbers per shop, refunds referencing original sale, receipt print/SMS/WhatsApp, shift/till management with Z-report (till open/close, expected vs counted). Sync flow: local sale -> queue -> sync -> confirm -> mark in ledger.

### 3.6 Sales Management (B2B/credit)
- Lead→Quotation→Sales Order→Delivery Note→Invoice workflow w/ status machine. Reserve stock on SO, support partial deliveries/backorders, recurring orders, delivery scheduling hooking to Logistics, credit limit enforcement, invoices track payment status (Paid/Partial/Overdue), supports zero rated vs standard VAT, pro-forma quotes shareable via WhatsApp/email, integrated returns (credit note). Provide 360° customer view (outstanding, last order date, loyalty tag).

### 3.7 CRM
- Lead capture (source, interest), opportunities w/ pipeline stages + probability, follow-up reminders, communication log, quick convert to customer/quotation, informal credit tracking, segmentation tags (VIP, wholesale, on-account), offline log of calls/notes, integrated WhatsApp deep-links. AI highlights dormant leads, suggests best next action, segments customers, warns if high spender hasn’t purchased lately. Collaborative referral + shared loyalty program support (opt-in shared data).

### 3.8 Buying / Procurement
- Material Request, RFQ, Supplier Quotation, Purchase Order, Goods Receipt, Purchase Invoice, payments. Group buying aggregator (combine requests across tenants, allocate deliveries, split invoices). Supplier portal UI (accept/reject, update status). Auto-reorder suggestions (AI). Track supplier performance (on-time, quality). Offline drafting of PO & receipt. Enforce three-way match, PO approval thresholds, budget checks, COD vs on-credit flag.

### 3.9 Accounting & Finance
- Chart of accounts tailored to township retail (Cash, Mobile Money, Inventory, COGS, Sales Income, Expenses, VAT Output/Input, Rounding Adjustment). Double-entry journals triggered automatically (sales, purchases, stock, payroll). Cashbook & bank accounts, payment entries, petty cash, supplier/customer ledgers. Reports: Profit & Loss (“Money In vs Money Out”), Cashflow, Debtors (“People who owe you”), Creditors (“Who you owe”), VAT 201 summary, Trial Balance, Balance Sheet (“What you Have/Owe”). Support accrual w/ cash-basis reporting toggle. Bank reconciliation, expense capture (photo of receipt). Localization: ZAR formatting, VAT 15%, zero-rated items. Provide manual JE UI (advanced) with guardrails.

### 3.10 Logistics & Delivery
- Driver profiles (availability, vehicle, service radius). Delivery tasks auto-created when PO shipped or sales order needs delivery. Driver app view (list, accept, mark picked up/delivered, attach proof photo/signature, offline queue). Status progression (Pending → Assigned → Picked Up → Out for Delivery → Delivered/Failed). Retailer view shows driver contact + ETA, confirm receipt or flag issue. Admin view for assignment / reassign / multi-drop route hints (basic ordering). Feed delivery completion back to procurement/inventory invoicing.

### 3.11 Projects / Job Cards
- Projects w/ start/end, type (internal/external), customer link, budget vs actual. Tasks with statuses, assignee, due date, progress %, attachments/photos, materials consumption pulling from stock, labour/time logs, invoices tie-in. Mobile “My Tasks” checklist, offline updates. Project completes automatically when all tasks done or manual override. Integration with CRM (opportunity → project) and Accounting (cost capture, profitability).

### 3.12 HR & Payroll
- Employee master (personal info, role, wage type, start date, banking, emergency contact). Attendance tracking (present/absent, clock-in/out optional) w/ mobile roster view, offline capture. Leave requests w/ approval, leave balances. Payroll cycles (weekly/monthly) supporting daily wage or salary, overtime, deductions (advances, UIF), payouts (cash/bank). Generates payslips + posts salary expense + liabilities + payments to accounting. Employee to user-link (optional). Role-based privacy (employees see own info). Reminders for payday & pending leave.

### 3.13 Manufacturing (optional future)
- BOM definitions (materials, labour, scrap), work orders, material reservation, production entry (consume raw, produce finished goods), cost variance, batch/expiry tracking for produced goods.

### 3.14 AI Copilot
- Natural language Q&A (“How were sales this week vs last?”), daily/weekly health summary cards, proactive suggestions (reorder, promotions, price anomalies, credit control, driver delays), autonomous drafts (draft PO, draft promotion message, draft collection reminder). Chat widget in dashboard w/ quick prompts, offline queue (respond when online). Provide reason/explanation + action buttons (accept/reject). Records actions in audit trail.

### 3.15 Collaborative Network
- Group buying orchestrator (collect intents, aggregate, assign lead shop, split deliveries/invoices). Shared logistics pooling (single driver multi shops, cost split). Referral marketplace (lead sharing, reward tracking). Shared loyalty programme (opt-in cross-tenant customer profiles). Financial services integration (credit scoring using anonymized metrics, lender portal). Community forum/events/mentorship board. Strict opt-in data sharing with scopes and consent logs.

## 4. Data & Integration Requirements

- **APIs:** Versioned REST + future webhooks. Document via Swagger/OpenAPI. Provide integration hooks for banks, payments, WhatsApp/SMS, potential government compliance (e.g., eFiling VAT). Rate limiting per tenant. Webhook signing secret.
- **Data Import/Export:** CSV import/export for items, customers, suppliers, stock balances, GL entries. Validate duplicates, preview errors, allow partial imports with error report. Provide templates.
- **Auditing:** Immutable audit log storing entity, action, before/after diff, user, timestamp. UI viewer with filters, export capability. Tamper-proof (append-only). Link audit entries to notifications if suspicious (e.g., large discount).
- **Observability:** Structured logs (Serilog) to console + file + optional Application Insights, metrics (Prometheus), basic tracing. Alerts for failed background jobs, sync failures, AI service issues.

## 5. UI / UX Standards

- Adopt Material Dashboard Pro analytics layout for nav + cards + charts. Provide global nav tabs: Home, Sales, Stock, Money, People, Jobs, Settings.
- Provide guided wizards for complex flows (new business onboarding, first stock import, payroll run).
- Localization-ready copy (English default but plan for isiZulu, isiXhosa). Use friendly terminology.
- Accessibility: high contrast, large tap targets, offline indicators, skeleton loaders.

## 6. Deployment & Ops

- Dockerized services, optional K8s, environment configs for dev/stage/prod. CI/CD pipelines (GitHub Actions) running lint/test/build/deploy. Seed data scripts. Feature flag system for progressive rollout. Backup/restore strategy. Telemetry for usage analytics feeding AI.

## 7. Success Criteria

- Provide prioritized backlog covering MVP scope (Stock, POS, Dashboard, Accounting basics, CRM lite, Procurement, Logistics lite, AI suggestions, collaborative seeds).
- Each task must include description, dependencies, acceptance criteria/test strategy, and alignment to pillars (ERP, AI, Collaboration).
- Ensure tasks cover backend, frontend, AI, DevOps, testing, documentation, onboarding, and go-live readiness.




