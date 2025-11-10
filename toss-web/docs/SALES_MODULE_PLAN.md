# Sales Module – Analysis and Implementation Plan

This plan aligns our Sales module with ERPNext Selling and POS best practices, adapted for TOSS ERP III (Nuxt 4 PWA, offline-first, ZAR).

Sources reviewed:
- ERPNext – Selling: https://docs.frappe.io/erpnext/user/manual/en/selling
- ERPNext – Selling Essentials: https://docs.frappe.io/erpnext/user/manual/en/selling-essentials
- ERPNext – Selling Transactions: https://docs.frappe.io/erpnext/user/manual/en/selling-transactions
- ERPNext – Point of Sale: https://docs.frappe.io/erpnext/user/manual/en/point-of-sale and https://docs.frappe.io/erpnext/user/manual/en/point-of-sales

## 1) Scope and Feature Map

| Domain | ERPNext Reference | TOSS UI (current) | MVP Scope |
|---|---|---|---|
| Quotations | Selling Transactions → Quotation | sales/orders/create-order.vue | Draft quote with lines, discount, tax, convert to Sales Order |
| Sales Orders | Selling Transactions → Sales Order | sales/orders/index.vue, queue.vue | Create SO from quote or POS restock; status timeline; basic approval |
| Delivery Note | Selling Transactions → Delivery Note | N/A explicit page | Defer; handled via order status and future logistics integration |
| Sales Invoice | Selling Transactions → Sales Invoice | sales/invoices.vue | Issue invoice from SO or POS; ZAR rounding, VAT, PDF/print |
| Returns/Credit Note | Credit Note / Sales Return | POS recent orders return flow | Allow POS returns (negative qty) and credit note issuance |
| POS | Point of Sales | sales/pos/index.vue, queue.vue | Cart, barcode/scan, tender (cash/card/wallet), change, receipt, hold/resume |
| Reports | Selling Reports | sales/reports/index.vue | Daily sales, item performance, export CSV/XLSX/PDF |
| Master Data | Selling Essentials (Customer, Group, Sales Person, Territory) | Customers via shared module | Minimal: customer select/create inline in POS; enhanced later |

## 2) User Flows (condensed)

- POS Sale
  1. Select/scan item → cart row (qty +/-), per-line discount enabled by profile flag
  2. Tender selection (cash/card/wallet), change calculation
  3. Complete order → generate POS Invoice (client) and persist; print/share
  4. Offline: queue writes; sync when online

- POS Return (Credit Note)
  1. Open Recent Orders → choose invoice → Return → items added with negative qty
  2. Complete → issue credit note; reflect stock and ledger indicators (client-level)

- Quotation → Sales Order → Sales Invoice
  1. Draft quotation from create-order page → confirm to SO
  2. Invoice from SO; apply VAT, rounding; export PDF/print

## 3) Data Shapes (frontend contracts)

```ts
// Product (subset)
export interface Product { id: string; sku?: string; name: string; barcode?: string; price: number; taxRate?: number; }

export type PaymentMethod = 'cash' | 'card' | 'wallet';

export interface CartLine { productId: string; name: string; qty: number; unitPrice: number; discount?: number; taxRate?: number; total: number; }

export interface PosInvoice {
  id: string;
  date: string; // YYYY-MM-DDTHH:mm:ssZ
  customerId?: string;
  lines: CartLine[];
  subTotal: number;
  discountTotal?: number;
  taxTotal?: number;
  grandTotal: number;
  payment: { method: PaymentMethod; paid: number; change: number };
  status: 'draft' | 'paid' | 'return' | 'consolidated';
}

export interface SalesOrder { id: string; customerId?: string; lines: CartLine[]; status: 'draft'|'confirmed'|'fulfilled'|'invoiced'; totals: { sub: number; tax: number; grand: number } }
export interface SalesInvoice { id: string; orderId?: string; totals: { sub: number; tax: number; grand: number }; paid: number; balance: number }
```

Notes:
- Currency: ZAR; formatting via `useMoney.formatCurrency` (already implemented with locale tolerance in tests)
- VAT: simple rate per line initially; cumulative `taxTotal` on invoice

## 4) Offline & Sync

- Read models cached; write operations enqueued: `queue.add({ type: 'pos.invoice.create', payload })`
- Conflict policy: idempotent keys (`id` as UUID v4 generated client-side)
- On reconnect, POST queue flush; retries with backoff; mark items `syncedAt` on success

## 5) Pages and Work Items

- sales/pos/index.vue
  - Implement barcode input, item search, cart (qty +/-), line discount toggle
  - Tender modal (cash/card/wallet), change logic, complete order
  ️ - Recent Orders sidebar and Return flow (negative qty), print/share

- sales/orders/create-order.vue
  - Create quotation: add items, discount, tax; Save draft
  - Convert to Sales Order; maintain status timeline

- sales/invoices.vue
  - List invoices; view details; print PDF via jsPDF/autotable

- sales/reports/index.vue
  - Daily totals, top items, CSV/XLSX export, chart.js over date range

## 6) Acceptance Criteria (MVP)

- POS:
  - Scan or search adds item; qty adjust; discount per line (when enabled)
  - Completing order creates POS invoice with correct change
  - Return flow from recent orders creates a negative-qty credit invoice
  - Works offline after first load; queued writes sync

- Orders/Invoices:
  - Quotation → Sales Order conversion; basic invoice issuance
  - VAT computed; totals in ZAR; PDF export

- Reports:
  - Daily sales and top items; export CSV/XLSX; simple chart

## 7) Telemetry & Errors

- Sentry breadcrumbs for cart actions, tender submit, sync success/failure
- User-visible errors:
  - "No stock available for {item}" (informational in MVP)
  - "Payment insufficient by R{amount}"; "Offline: saved for sync"

## 8) Tests

- Unit (Vitest):
  - Cart calculations (discounts, tax, change) – happy path + edge (zero/negative qty) + error (insufficient payment)
  - useMoney formatting remains correct

- E2E (Playwright):
  - POS add/adjust, tender, print; return flow; offline simulate

## 9) Settings & Profiles (Future)

- POS Profile flags: allow discount editing, default warehouse, change account, receipt template
- Loyalty program & consolidation on closing voucher (post-MVP)

---
Updated: 2025-11-10