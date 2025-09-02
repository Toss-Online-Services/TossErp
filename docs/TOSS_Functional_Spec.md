# TOSS ERP III – Functional Specification (Source‑backed Summary)

This document distills the functional scope, AI co-pilot approach, and collaborative economy mechanics for TOSS (Third‑Generation ERP Service Platform) targeting township and rural SMMEs. It aligns the MVP scope with our current Nuxt app structure in `toss-web/` and anchors major claims to current external sources.

## Vision & Model

- ERP III foundation: Cloud‑first, modular, service‑oriented, and federated/postmodern ERP patterns enabling mixed best‑of‑breed integrations. Sources: Hurbean & Fotache (2014); subsequent literature on ERP III and Industry 4.0 trends.
- Service‑as‑Software (SaS): Deliver outcomes via automation/agents; customers “pay for results, not tools,” turning labor into software with human‑in‑the‑loop oversight. Source: Windward (2025).
- Embedded AI execution: AI agents operate inside core ERP flows to close the loop from insight to action (procurement, supplier onboarding, production issue response). Source: SupplyChain360 (2025, SAP briefing).
- Collaborative network: Group Purchasing Organizations (GPOs) and cooperative mechanics natively supported to unlock scale advantages for micro‑businesses. Source: Virto Commerce (2024) overview of GPOs and benefits across industries.
- Financial inclusion: Digital stokvels/tontines and pooled credit/savings to formalize transaction history and unlock credit access. Source: Microsoft News Center (2019) with South Africa stokvel stats (≈11M members saving ≈$3B/year).

## MVP Modules (Phase 1)

1) Inventory Management
- Real‑time stock, low‑stock alerts, basic forecasting; optional batch/expiry; multi‑location and shared storage.
- AI: demand signals, stockout prevention, group buy suggestions.

2) Sales & Order Management
- Quotes/orders, invoices/receipts, basic CRM, job status, simple analytics.
- AI: upsell prompts, follow‑ups, cross‑business sourcing cues.

3) Point of Sale (POS)
- Fast checkout, multi‑payment, receipts (print/digital), inventory sync, EoD reports.
- AI: real‑time restock prompts and performance nudges.

4) Purchasing & Supplier Management
- Supplier directory, POs, status, price history.
- Group Purchasing (pilot): join/initiate bulk orders, cost‑split, distribution.
- AI: reorder automation, negotiation assist, consolidation.

5) Logistics (Basic)
- Delivery scheduling, simple route planning, delivery status, inventory in‑transit.
- Foundation for shared delivery network.

6) Finance & Accounting
- Auto‑bookkeeping from sales/purchases, income/expense, P&L and cash flow.
- AR/AP reminders; pooled savings (basic) and simple internal loan tracking.
- AI: anomaly checks, cash flow alerts, optimization tips.

7) AI Business Co‑Pilots v1
- Contextual Q&A, recommendations, light automations under consent/limits.
- Role‑aware agents across inventory/sales/purchasing/logistics/finance, with audit trails.

8) Collaborative Basics
- Business discovery/connect, forum/chat, group purchasing (limited SKUs pilot), basic tool‑sharing listings, pooled savings club (MVP).

## Phase 2+ Highlights

- Scale group purchasing (broader categories, supplier bids, formal GPO constructs).
- Shared logistics (route sharing, delivery marketplace, shared warehousing with bookings and live tracking).
- Tool pooling marketplace (booking, deposits, ratings) and Shared Services portal (professional services, shared personnel, training).
- CRM/Marketing; E‑commerce integrations; HR/Payroll lite.
- AI autonomy uplift and multilingual/voice; analytics dashboards.
- Phase 3: AI‑driven B2B marketplace, gov/bank integrations, IoT, micro‑manufacturing/maker spaces, enhanced pooled finance, regional scaling, community governance.

## AI Co‑Pilot Design Principles

- Proactive task execution with guardrails (limits, consent, reversible actions) and full audit logs.
- Cross‑module orchestration (inventory→purchasing→logistics→finance) for supply chain synchronization.
- Natural language and role‑aware responses; personalization from usage patterns.
- Network intelligence: pooling opportunities (buys, logistics), peer benchmarks, and community insights.

Security & Compliance basics (aligns with `.github/instructions/copilot.instructions.md`):
- OAuth2 scopes per module, least privilege data access; PII masking/redaction where applicable.
- Audit every assistant action (actor, timestamp, consent).
- RFC9457‑style errors; rate limits/abuse protection; streaming for voice when supported.

## Mapping to `toss-web/` (Nuxt 3 + Tailwind)

- Pages present: `pages/` already includes top‑level placeholders for many modules (e.g., `inventory/`, `selling/`, `buying/`, `crm/`, `projects/`, `onboarding/`, etc.).
- Components present: `components/AICopilotChat.vue`, module modals, layout/navigation.
- Stores: `stores/` include `customers.ts`, `globalAI.ts`, `notifications.ts`, `settings.ts`, `user.ts`.
- Middleware: `middleware/auth.ts`, `tenant.global.ts` for auth/tenant scoping.

Initial wiring suggestions for MVP slices:
- Inventory: pages/inventory/* with stock list, item details, reorder thresholds; reuse `stores/settings.ts` for thresholds; add store `inventory.ts` (Pinia) and composables for fetch/mutations.
- Sales/Orders: pages/selling/* and `crm/` for quotes/orders and simple CRM; use `customers.ts` + new `orders.ts` store.
- POS: pages/pos/* for fast checkout UI, receipt print/share via Web Share API or WhatsApp deep link; real‑time stock decrement to Inventory.
- Purchasing: pages/buying/* for POs and suppliers; add `suppliers.ts` + `purchasing.ts` stores; group buy flows as MVP pilot.
- Logistics: pages/logistics/* to schedule deliveries and track statuses; integrate with orders/POs.
- Finance: pages/finance/* for income/expense, P&L, cash flow; simple AR/AP views; link to pooled savings.
- AI Co‑Pilot: ensure `AICopilotChat.vue` is accessible globally (layout slot/button) with role/context awareness per route; every action performed by AI must be logged server‑side with consent.

Note: Data/backend stubs should live under `server/api/` (Nitro) with clear DTOs and RFC9457 errors; expand incrementally.

## Acceptance Criteria (MVP)

- Inventory: Create/list/update SKUs; set reorder levels; low‑stock alert fired; AI suggests reorder qty; creating a PO updates status and reserved/in‑transit quantities.
- Sales/Orders: Create quote→order→invoice; email/SMS share; CRM list with last activity.
- POS: Add items, multi‑payment record, receipt share/print; inventory decrements; EoD summary.
- Purchasing: Create PO; supplier select with last price; basic group buy join/initiate with cost split preview.
- Logistics: Schedule delivery against order/PO; status updates reflect in UI; mark as delivered updates inventory/AR.
- Finance: Auto‑postings from sales/PO receiving; add manual expense; P&L and cash flow view; pooled savings group with contribution/withdraw record.
- AI Co‑Pilot: Contextual Q&A for sales/inventory; one light automation (e.g., draft PO from low‑stock); action audit entries created.

## Risks & Mitigations

- Data quality/permissions: enforce tenant scoping in `tenant.global.ts`; module‑scoped OAuth2; input validation on server routes; PII redaction for prompts.
- Agent overreach: default to recommendation‑first; require explicit consent for spend; configurable limits and undo.
- Group features trust: transparent cost‑split, immutable audit logs, ratings for shared logistics/tools, deposits for tool rentals.
- Connectivity: offline‑tolerant POS (deferred sync queue); SMS/WhatsApp fallbacks for receipts and notifications.

## Sources (validated)

- ERP III (federated/cloud/SOA, post‑modern ERP): Hurbean, L. & Fotache, D. (2014). ERP III – The Promise of a New Generation. ResearchGate (PDF link provided by RG).
- Service‑as‑Software (SaS): Windward AI (2025). “Service as Software (SaS)” – outcomes‑based services via automation/agents.
- Embedded AI agents in ERP execution: SupplyChain360 (2025‑07‑24). “SAP Embeds AI in ERP to Automate Supply Chain Execution.”
- Group Purchasing Organizations & marketplaces: Virto Commerce Blog (2024‑09‑05). “Why Group Purchasing Organizations Are Essential for Modern Procurement Success.”
- Stokvel statistics & digital inclusion: Microsoft News Center (2019‑06‑26). “Unbanked Africa: Ripe for a FinTech‑led Future.” (≈11M SA stokvel members; ≈$3B savings/year).

---
This file is a concise guide for product and engineering to align UX, backend stubs, and AI agent guardrails with the MVP scope already scaffolded in `toss-web/`. 
