# TOSS PRD — Product Requirements (Enhanced)

**Product:** TOSS — Township Operations Support System  
**Type:** Operations Management **Service-as-Software** (ERP-III + AI Copilot)  
**Primary Value:** Automate daily operations **and** unlock **collective buying + shared delivery** to cut costs.

---

## 1) Vision & Value

* **Vision:** A community-powered operations manager that keeps shops stocked, lowers costs through **group buying**, and reduces delivery expenses via **shared drivers/routes**—all in simple, mobile-first flows.
* **Value:**
  * Automates **Sales, Stock, Buying**.
  * Aggregates demand to **negotiate better prices**.
  * Shares logistics so **one driver serves many shops** on a single run.
  * Uses WhatsApp alerts and pay links for low-friction action.

---

## 2) Core Product Pillars

1. **Do the work:** Suggestions → one-tap approvals → real actions (orders, messages).
2. **Collective power:** **Group buying** raises order volumes and unlocks discounts.
3. **Shared logistics:** **Driver resource sharing** drops to multiple shops in one trip.
4. **Simple first:** Plain language, big buttons, offline-tolerant, WhatsApp friendly.
5. **Trust & transparency:** Clear prices, split rules, delivery status, audit trail.

---

## 3) Target Users & Roles

* **Shop Owner (primary):** joins/creates pools, approves reorders, pays, receives.
* **Group Lead (optional):** starts pools, sets split rules, invites others.
* **Supplier Admin:** receives POs, confirms quantities/dates.
* **Driver/Runner:** accepts a **shared run**, follows drop list, confirms delivery.
  * **Driver Onboarding:** Vehicle type, capacity, license verification
  * **Driver Availability:** Calendar with recurring availability windows
  * **Driver Earnings:** Transparent fee split (70% driver, 20% platform, 10% buffer)
  * **Driver Ratings:** 5-star rating by shops; auto-suspend at <3.5 avg
* **Cashier:** uses POS-lite sales.

---

## 4) MVP Scope (Must / Should / Could)

### Must (MVP)

* **Sales:** quick sale entry, today summary.
* **Stock:** on-hand, low-stock alerts, adjustments.
* **Buying:** create POs; **approve AI reorder suggestions**.
* **Group Buying (core):**
  * Create/Join **Pools** for specific SKUs (qty + deadline + area).
  * **Progress meter** (e.g., 4/6 crates reached).
  * **Invite via WhatsApp link**.
  * Pool states: **Open → Pending → Confirmed → Fulfilled/Cancelled**.
  * Simple **cost-split rules** (flat per participant or by units).
  * **Pool extension logic:** If 70% filled at deadline, auto-extend 24hrs (once).
  * **Early closure:** Allow lead to close pool early if 100% filled.
  * **Participant removal:** Lead can remove non-paying participants after 24hr grace.
  * **Pool templates:** Common SKUs (bread, milk, mealie meal) as one-click pool creation.
* **Shared Logistics (core):**
  * Create **Shared Driver Run** (capacity, pickup, drop list).
  * Assign to driver; **single route** with multiple drops.
  * **Proof of Delivery**: PIN or photo.
  * Deliveries tied to pool or solo orders.
* **Payments:** hosted **pay link**; order reflects **Paid/Pending/Failed**.
* **Alerts:** WhatsApp for pay links, pool progress, run scheduled, out-for-delivery, delivered.
* **AI Copilot (rules):** low-stock → draft reorder; suggest joining/opening a pool; propose shared run if multiple nearby drops.

### Should (near-term)

* Pool price tiers (better price at higher volumes).
* Driver fee split presets (by stops / by weight / by distance).
* Simple ratings for drivers/suppliers.
* **Batch tracking** for perishables (bread, milk) - critical for township shops.
* **Quality inspections** on receipt (especially for pooled orders).
* **Three-way matching** (PO → Receipt → Invoice) for financial transparency.

### Could (roadmap)

* Multi-SKU pooled baskets.
* Route optimization hints (Google Maps integration).
* Credits/wallet for pooled prepayments.
* **Serial number tracking** for high-value items.
* **Supplier portals** for PO confirmation.
* **Returns/RMA workflow** for defective goods.

---

## 5) Key User Journeys

### A) **Create a Group-Buy Pool**

1. Owner taps **Group Buying → New Pool**.
2. Select SKU, target qty, deadline, area, split rule.
3. TOSS generates **WhatsApp invite link**.
4. Others join; pool shows **progress meter**.
5. At deadline or target met: pool **Confirms** → **PO created** → pay links sent.

**Acceptance:** pool easy to start/join; progress obvious; one-tap confirm creates PO and triggers payment links.

---

### B) **Join an Existing Pool**

1. Owner opens invite or finds pools nearby.
2. Chooses qty; sees **share of cost** and **estimated savings**.
3. Joins; gets WhatsApp updates.
4. On confirm, pays via **pay link**.

**Acceptance:** join flow ≤ 3 steps; transparent cost and savings; payment status reflected.

---

### C) **Shared Driver Run (Resource Sharing)**

1. After PO confirmation, TOSS suggests **Shared Run** (same supplier, same area).
2. Driver accepts run; run shows **pickup location + drop list** (shops).
3. Driver completes drops; captures **POD** (PIN/photo) per stop.
4. Owners receive **Delivered** message; stock updated.

**Acceptance:** driver sees clear itinerary; each stop confirmed; shops notified in real time.

---

### D) **Low-Stock → Suggest Pool or Solo Reorder**

1. Copilot flags low stock on common SKUs.
2. Suggests: *"Join Pool A (3/6 crates, closes 5pm) or Solo Reorder now."*
3. Owner picks one; TOSS continues flow (pool join or PO draft).

**Acceptance:** relevant suggestions; action completes in ≤ 2 taps.

---

## 6) Feature Requirements (Functional)

### 6.1 Group Buying

* **Discover:** list of nearby pools (SKU, price, progress, closes at).
* **Create:** set target qty, deadline, area, split rule (flat/units).
* **Join:** choose qty; preview cost and savings.
* **Confirm:** auto-lock when threshold met or at deadline.
* **Cancel/Fail:** if not met, propose alternatives (extend, convert solo).
* **Comms:** WhatsApp notifications at key milestones.
* **Visibility:** see participants (shop name or alias), not personal info.

**Pool Management Rules:**
* **Pool extension logic:** If 70% filled at deadline, auto-extend 24hrs (once only)
* **Early closure:** Lead can close pool early if 100% filled
* **Participant removal:** Lead can remove non-paying participants after 24hr grace period
* **Pool templates:** Common SKUs (bread, milk, mealie meal) as one-click pool creation
* **Cancellation:** Lead can cancel with reason; participants notified immediately

### 6.2 Shared Logistics

* **Run setup:** pickup point, capacity, drops (shops & ETAs).
* **Assignment:** select driver; share run link; driver mobile view.
* **On-trip:** mark "Out for delivery"; per-stop **POD**.
* **Split fees:** show rule; display each shop's share; show **"You saved Rxx"**.
* **Completion:** all drops done → run closed; shops notified.

### 6.3 Sales / Stock / Buying (unchanged core)

* Sales entry; day totals; top items.
* Stock levels; low-stock banners; adjustments.
* PO lifecycle: Draft → Sent → Confirmed → Received.

### 6.4 Payments & Alerts

* **Pay links** for pools and solo orders.
* Alerts: pool created/joined/confirmed, run scheduled/out/delivered, payment received/failed.

### 6.5 AI Copilot (rules for MVP)

* Suggest pool on trending low-stock SKUs across shops in same area.
* Nudge to join active pool nearing threshold.
* Suggest shared run if ≥2 drops in same zone/day.
* Always explain reason in plain text (e.g., "3 shops nearby buying same item today").

### 6.6 Conflict Resolution (NEW)

* **Payment disputes:** 48hr window for payment completion before auto-cancellation
* **Delivery disputes:** POD challenge process with photo evidence required
* **Quality issues:** Returns handled by lead shop, reimbursed from pool credit or direct refund
* **Non-payment:** Automatic removal from pool after 24hr grace + 2 reminders
* **Driver no-show:** Reassignment protocol + notification to all participants
* **Split disputes:** Clear rules shown before join; per-shop total visible at all times

---

## 7) UX Principles

* **Mobile-first:** single column, bottom nav; 48px touch targets.
* **Clarity:** progress meters, simple badges ("Open", "Confirmed", "Delivered").
* **Transparency:** always show cost split basis and savings ("You saved R45!").
* **Local:** EN/isiZulu labels; WhatsApp links/buttons.
* **Accessible:** icons + text; don't rely on colour only; offline-tolerant states.

---

## 8) Success Metrics

| Metric                                  | Target                       |
| --------------------------------------- | ---------------------------- |
| Pool fill rate                          | ≥ 70% of created pools       |
| Avg delivery cost per order             | −30% vs solo                 |
| "You saved" displayed                   | ≥ 80% of pooled/ shared runs |
| Copilot pool/join suggestion acceptance | ≥ 50%                        |
| On-time delivery (shared runs)          | ≥ 90%                        |
| Pay-link completion                     | ≥ 70%                        |

---

## 9) Constraints & Assumptions

* MVP: **single-SKU pools**; one supplier per pool; one route per shared run.
* Simple split rules (flat or units).
* Payments via hosted pay links (low device friction).
* **Geographic zones:** Define explicit zones with distance constraints:

```typescript
const TOWNSHIP_ZONES = {
  'soweto-north': { center: [-26.2309, 27.8559], radius: 5 },
  'soweto-south': { center: [-26.2870, 27.8559], radius: 5 },
  'alexandra': { center: [-26.1023, 28.0897], radius: 3 }
}
```

Pools and Runs respect zone boundaries. Cross-zone participation requires opt-in (higher delivery fees).

---

## 10) Risks & Mitigations

* **Pool not filled:** auto-extend once; offer solo reorder; notify early.
* **Driver no-show:** reassign protocol; notify all shops; allow pickup window extension.
* **Disputes on split:** clear rules shown before join; per-shop total visible.
* **Trust:** show participant list with shop names/aliases; simple driver ratings; reliability scores.
* **Payment failures:** 24hr grace period + 2 reminders before removal.
* **Quality issues:** Clear returns process; lead shop manages; pool credit or refund.

---

## 11) Launch Plan (focused on collective features)

1. **Pilot Zone:** 1–2 high-density areas; pre-seed 3 common SKUs for pools.
2. **Driver Network:** onboard 5-10 reliable drivers; test shared run flow with real routes.
3. **Measure savings:** show "You saved" on every pooled/ shared delivery with actual amounts.
4. **Iterate:** tune thresholds (70% extension rule), deadlines, and default split rules based on data.
5. **Onboard shops:** Start with 20-30 shops in pilot zone; train on pool creation and joining.
6. **Monitor metrics:** Track pool fill rate, delivery on-time %, payment completion weekly.

---

## 12) Technical Implementation Notes

### Data Models
- **Pool** as first-class entity (not a PO variant)
- **Pool generates PO** when confirmed (PO.metadata.poolId = pool.id)
- **State machine:** open → pending → confirmed → fulfilled | cancelled
- **Geographic zones** hard-coded for MVP, database-driven in production

### Integration Points
- **Stock Management:** Pools linked to items via itemId
- **Purchase Orders:** Pools generate consolidated POs with poolId metadata
- **AI Copilot:** Suggests pool actions for low-stock items
- **WhatsApp:** Pool invites, progress updates, delivery tracking
- **Payments:** PayFast/Yoco/Ozow integration for pay links

### Security & Privacy
- Participant shop names visible, not personal contact info
- Payment links expire after 24 hours
- POD photos stored securely, auto-delete after 90 days
- Driver location tracking only during active deliveries

---

### One-line product definition (updated)

**TOSS helps township businesses sell, restock, and **buy together**, while **sharing drivers** to cut delivery costs—guided by an AI copilot that turns suggestions into one-tap actions.**

---

**Version:** 1.1 Enhanced  
**Date:** January 20, 2025  
**Status:** ✅ Approved for Implementation  
**Implementation Status:** Phase 1 Complete (Core Infrastructure)

