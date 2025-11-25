# TOSS ERP Sales Module - Implementation Guide

This document serves as a comprehensive guide for implementing the Sales module based on ERPNext's selling module, adapted for TOSS ERP using Nuxt 4, Vue 3, and shadcn-vue components.

## Overview

The Sales module implements the complete order-to-cash cycle:
1. **Quotations** → 2. **Sales Orders** → 3. **Delivery Notes** → 4. **Sales Invoices** → 5. **Payments**

Based on ERPNext documentation:
- [ERPNext Selling Module](https://docs.frappe.io/erpnext/user/manual/en/selling)
- [ERPNext Sales Features](https://frappe.io/erpnext/open-source-sales-invoicing)

## Module Structure

```
app/pages/sales/
├── index.vue                    # Sales Dashboard
├── quotations/
│   ├── index.vue               # List quotations
│   ├── create.vue              # Create new quotation
│   └── [id].vue                # View/edit quotation
├── orders/
│   ├── index.vue               # List sales orders
│   ├── create.vue              # Create new order
│   └── [id].vue                # View/edit order
├── invoices/
│   ├── index.vue               # List invoices
│   ├── create.vue              # Create new invoice
│   └── [id].vue                # View/edit invoice
└── customers/
    ├── index.vue               # List customers
    └── [id].vue                # View/edit customer
```

## Core Components

### 1. Sales Dashboard (`/sales/index.vue`)

**Features:**
- Key metrics cards (Revenue, Orders, Pending Invoices, Customers)
- Revenue trend chart
- Recent quotations, orders, and invoices
- Quick actions

**Components Used:**
- Card, CardHeader, CardTitle, CardContent
- Charts (vue-chartjs)
- Button, Badge

### 2. Quotations Module

**Based on ERPNext Quotation Doctype:**
- Customer selection
- Item lines with pricing
- Discounts and taxes
- Validity period
- Status workflow: Draft → Sent → Accepted/Rejected
- Convert to Sales Order

**Key Fields:**
- `customer_name`: Customer reference
- `transaction_date`: Quote date
- `valid_till`: Expiry date
- `items`: Array of items with qty, rate, amount
- `taxes_and_charges`: Tax template
- `grand_total`: Final amount
- `status`: Draft/Sent/Accepted/Rejected/Expired

### 3. Sales Orders Module

**Based on ERPNext Sales Order Doctype:**
- Create from Quotation
- Delivery date scheduling
- Status tracking: Draft → To Deliver → Completed
- Link to delivery notes and invoices

**Key Fields:**
- `quotation_no`: Reference to quotation (optional)
- `delivery_date`: Scheduled delivery
- `items`: Order items
- `status`: Draft/To Deliver/Completed/Cancelled

### 4. Sales Invoices Module

**Based on ERPNext Sales Invoice:**
- Generate from Sales Order
- Payment tracking
- Accounts receivable management
- Print/PDF generation

**Key Fields:**
- `customer_name`: Customer
- `posting_date`: Invoice date
- `due_date`: Payment due date
- `items`: Invoice items
- `outstanding_amount`: Unpaid amount
- `status`: Draft/Submitted/Paid/Overdue

## Implementation Patterns

### Data Structure

```typescript
// Quotation interface
interface Quotation {
  id: string
  name: string // e.g., "QUO-00001"
  customer_name: string
  customer: Customer
  transaction_date: Date
  valid_till: Date
  items: QuotationItem[]
  subtotal: number
  discount_amount: number
  tax_amount: number
  grand_total: number
  status: 'Draft' | 'Sent' | 'Accepted' | 'Rejected' | 'Expired'
  created_at: Date
  updated_at: Date
}

interface QuotationItem {
  item_code: string
  item_name: string
  description?: string
  qty: number
  rate: number
  amount: number
  discount_percentage?: number
}
```

### Component Pattern

Each module follows this structure:

```vue
<script setup lang="ts">
// 1. Define types/interfaces
// 2. Set up composables (useState, useFetch)
// 3. Define computed properties
// 4. Define methods (CRUD operations)
// 5. Lifecycle hooks (onMounted, etc.)

useHead({
  title: 'Module Name - TOSS ERP'
})
</script>

<template>
  <!-- 1. Page header with title and actions -->
  <!-- 2. Stats/metrics cards -->
  <!-- 3. Filters and search -->
  <!-- 4. Data table or form -->
  <!-- 5. Dialogs for create/edit -->
</template>
```

### Data Table Pattern

```vue
<script setup lang="ts">
import { useVueTable } from '@tanstack/vue-table'

const columns = [
  // Define columns
]

const table = useVueTable({
  data: items,
  columns,
  getCoreRowModel: getCoreRowModel(),
  getPaginationRowModel: getPaginationRowModel(),
  // ... other config
})
</script>
```

## UI/UX Guidelines

### Styling
- Use Material Dashboard theme colors
- Follow shadcn-vue component patterns
- Responsive design (mobile-first)
- Dark mode support

### Forms
- Use FormKit or native Vue forms with vee-validate
- Real-time validation
- Auto-calculations (totals, taxes)
- Save as draft capability

### Tables
- Sortable columns
- Search and filters
- Pagination
- Row actions (view, edit, delete)
- Status badges with colors

### Status Colors
- Draft: Gray
- Sent: Blue
- Accepted/Completed: Green
- Rejected/Cancelled: Red
- Overdue: Orange

## API Integration (Future)

The frontend will integrate with backend APIs:

```
GET    /api/sales/quotations        # List
POST   /api/sales/quotations        # Create
GET    /api/sales/quotations/:id    # Get one
PUT    /api/sales/quotations/:id    # Update
DELETE /api/sales/quotations/:id    # Delete
POST   /api/sales/quotations/:id/convert  # Convert to SO
```

## Testing Checklist

- [ ] Dashboard loads and displays metrics
- [ ] Create quotation with items
- [ ] Edit quotation
- [ ] Convert quotation to sales order
- [ ] Create sales order
- [ ] Create invoice from order
- [ ] Search and filter work
- [ ] Calculations are correct
- [ ] Status updates work
- [ ] Mobile responsive

## Documentation Template

This guide serves as a template for other modules:
- Replace "Sales" with module name
- Update structure and features
- Adapt to module-specific requirements
- Keep consistent patterns and components

## References

- ERPNext Selling: https://docs.frappe.io/erpnext/user/manual/en/selling
- ERPNext Sales Features: https://frappe.io/erpnext/open-source-sales-invoicing
- Nuxt 4 Docs: https://nuxt.com/docs
- shadcn-vue: https://www.shadcn-vue.com/

