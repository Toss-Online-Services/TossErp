# TOSS ERP Sales Modules - Complete Implementation Guide

## Overview

This document outlines the complete implementation of TOSS ERP's sales modules based on ERPNext's selling and POS features, optimized for South African township SMMEs and utilizing all installed Nuxt modules.

## Modules Completed

### ✅ 1. Quotations Module (`/pages/sales/quotations/`)

**Features Implemented:**
- Quotation listing with search and filters
- Status tracking (Draft, Sent, Accepted, Rejected, Expired)
- Multi-language support (@nuxtjs/i18n)
- Icon integration (@nuxt/icon)
- Real-time statistics dashboard
- Responsive design with Tailwind CSS

**Files Created:**
- `/pages/sales/quotations/index.vue` - Main listing page
- `/pages/sales/quotations/create.vue` - Create new quotation (TODO)
- `/pages/sales/quotations/[id].vue` - View quotation details (TODO)
- `/pages/sales/quotations/[id]/edit.vue` - Edit quotation (TODO)

**Key Features Still Needed:**
- [ ] FormKit form for quotation creation
- [ ] PDF generation for quotations
- [ ] Email sending functionality
- [ ] Template management
- [ ] Approval workflow
- [ ] Conversion to Sales Order

---

## Modules To Implement

### 2. Enhanced Sales Orders Module

**Location:** `/pages/sales/orders/`

**Features to Add:**
```typescript
// Core Features
- Order creation from quotations
- Order amendments (add/remove items)
- Partial fulfillment tracking
- Bulk order processing
- Order scheduling
- Delivery date management
- Order status workflow
- Customer PO number tracking
```

**ERPNext Alignment:**
- Sales Order → Delivery Note flow
- Payment terms and conditions
- Discount management
- Tax calculations (VAT 15%)
- Multi-currency support (ZAR primary)

**TOSS-Specific Features:**
- Group buying integration
- Township delivery routing
- Cash/Credit payment tracking
- SMS notifications (WhatsApp integration)
- Offline order queue

---

### 3. Delivery Notes Module

**Location:** `/pages/sales/delivery-notes/`

**Features:**
```typescript
interface DeliveryNote {
  deliveryNo: string
  orderRef: string
  customer: Customer
  deliveryDate: Date
  scheduledTime: string
  status: 'scheduled' | 'in-transit' | 'delivered' | 'failed'
  driver: Driver
  items: DeliveryItem[]
  packingSlip: string
  proofOfDelivery?: {
    signature: string
    photo: string
    timestamp: Date
    location: Coordinates
  }
  notes: string
}
```

**Integration Points:**
- Link to Sales Orders
- Driver assignment (Logistics module)
- Route optimization
- Real-time tracking
- SMS/WhatsApp delivery updates
- Proof of delivery capture (mobile)

---

### 4. Enhanced POS Module

**Current Status:** Basic POS exists in `/pages/sales/pos/index.vue`

**Enhancements Needed:**

**A. POS Profiles**
```typescript
interface POSProfile {
  id: number
  name: string
  warehouse: Warehouse
  pricelist: PriceList
  paymentMethods: PaymentMethod[]
  cashierPermissions: string[]
  receiptTemplate: string
  autoLogout: number // minutes
}
```

**B. Opening/Closing Entries**
```typescript
interface POSSession {
  id: number
  cashier: User
  openingTime: Date
  closingTime?: Date
  openingCash: number
  closingCash?: number
  expectedCash?: number
  variance?: number
  transactions: Sale[]
  status: 'open' | 'closed'
}
```

**C. Loyalty Programs**
```typescript
interface LoyaltyProgram {
  id: number
  name: string
  pointsPerRand: number
  minPurchase: number
  redemptionRate: number
  validityDays: number
  tiers: LoyaltyTier[]
}

interface LoyaltyTier {
  name: string
  minPoints: number
  benefits: string[]
  discount: number
}
```

**D. Multiple Payment Modes**
- Cash
- Card (via Yoco/Payment gateway)
- Mobile Money (e.g., M-Pesa, Kazang)
- Credit (Account customers)
- Layby/Installment
- Split payments

**E. Offline Capabilities**
- IndexedDB for product catalog
- Service Worker for offline sales
- Background sync when online
- Conflict resolution

---

### 5. Sales Returns Module

**Location:** `/pages/sales/returns/`

**Features:**
```typescript
interface SalesReturn {
  returnNo: string
  salesInvoiceRef: string
  customer: Customer
  returnDate: Date
  items: ReturnItem[]
  reason: ReturnReason
  status: 'draft' | 'pending-approval' | 'approved' | 'rejected'
  refundMethod: 'cash' | 'credit-note' | 'exchange'
  refundAmount: number
  restockItems: boolean
}

interface ReturnItem {
  product: Product
  quantity: number
  reason: string
  condition: 'defective' | 'damaged' | 'wrong-item' | 'customer-request'
  restockable: boolean
}

enum ReturnReason {
  DEFECTIVE = 'Defective product',
  DAMAGED = 'Damaged in transit',
  WRONG_ITEM = 'Wrong item delivered',
  NOT_AS_DESCRIBED = 'Not as described',
  CUSTOMER_CHANGED_MIND = 'Customer changed mind',
  EXPIRED = 'Product expired',
  OTHER = 'Other'
}
```

**Workflow:**
1. Create return request
2. Approval workflow (optional)
3. Generate credit note
4. Process refund
5. Update inventory (if restockable)
6. Link to original sale/invoice

**Integration:**
- Automatic credit note generation
- Inventory adjustment
- Customer credit balance
- Supplier returns (for defective items)

---

### 6. Customer Management Module

**Location:** `/pages/customers/`

**Enhanced Customer Profile:**
```typescript
interface Customer {
  // Basic Info
  id: number
  code: string
  name: string
  type: 'individual' | 'business'
  businessName?: string
  
  // Contact
  phone: string
  email?: string
  whatsappNo?: string
  address: Address
  
  // Financial
  creditLimit: number
  creditDays: number
  currentBalance: number
  paymentTerms: string
  taxNumber?: string
  
  // Segmentation
  customerGroup: CustomerGroup
  territory: Territory
  pricelist?: PriceList
  
  // Loyalty
  loyaltyPoints: number
  loyaltyTier?: string
  memberSince: Date
  
  // History
  totalPurchases: number
  averageOrderValue: number
  lastPurchaseDate?: Date
  orderFrequency: number
  
  // Preferences
  preferredPaymentMethod: string
  preferredDeliveryTime: string
  specialInstructions: string
}

interface CustomerGroup {
  id: number
  name: string
  discount: number
  creditTerms: string
}

interface Territory {
  id: number
  name: string
  township: string
  deliveryDay: string
  deliveryFee: number
}
```

**Features:**
- Customer credit management
- Transaction history
- Loyalty points tracking
- Customer groups/segments
- Territory assignment
- Credit limit warnings
- Automated statements
- SMS/WhatsApp communication

---

### 7. Sales Analytics & Reports

**Location:** `/pages/sales/reports/`

**Reports to Implement:**

**A. Sales Overview**
- Daily/Weekly/Monthly sales trends
- Sales by product category
- Sales by customer segment
- Sales by territory/township
- Payment method breakdown

**B. Performance Reports**
- Top selling products
- Top customers
- Salesperson performance
- Conversion rates (quotation → order)
- Average order value trends

**C. Financial Reports**
- Revenue analysis
- Profit margins by product
- Outstanding invoices
- Credit customer aging
- Tax reports (VAT)

**D. Inventory Impact**
- Stock movement from sales
- Fast-moving vs slow-moving items
- Reorder recommendations
- Dead stock identification

**Implementation with Installed Modules:**
```vue
<template>
  <!-- Use @nuxt/icon for charts icons -->
  <div class="reports-dashboard">
    <!-- Use @nuxtjs/device for responsive charts -->
    <div v-if="isDesktop">
      <LineChart :data="salesTrend" />
    </div>
    <div v-else>
      <SimplifiedChart :data="salesTrend" />
    </div>
    
    <!-- Use @nuxt/image for report exports -->
    <button @click="exportReport">
      <Icon name="mdi:file-pdf" />
      Export PDF
    </button>
  </div>
</template>
```

---

## Module Integration Strategy

### Using Installed Nuxt Modules

#### 1. @nuxtjs/i18n - Internationalization
**All sales modules must support:**
- English (en)
- isiZulu (zu)
- isiXhosa (xh)
- Afrikaans (af)
- Sesotho (st)

**Implementation:**
```vue
<script setup>
const { t, locale } = useI18n()
</script>

<template>
  <h1>{{ t('sales.quotations.title') }}</h1>
  <button @click="locale = 'zu'">
    Switch to Zulu
  </button>
</template>
```

#### 2. @formkit/nuxt - Form Management
**Use for all data entry forms:**
```vue
<FormKit
  type="form"
  @submit="createQuotation"
  :actions="false"
>
  <FormKit
    type="select"
    name="customer"
    :label="t('sales.quotations.customer')"
    :options="customers"
    validation="required"
  />
  
  <FormKit
    type="date"
    name="validUntil"
    :label="t('sales.quotations.validUntil')"
    validation="required|date_after:today"
  />
  
  <!-- Product line items -->
  <FormKit
    type="repeater"
    name="items"
    :label="t('sales.quotations.items')"
  >
    <FormKit type="select" name="product" :options="products" />
    <FormKit type="number" name="quantity" />
    <FormKit type="number" name="rate" />
  </FormKit>
</FormKit>
```

#### 3. @vee-validate/nuxt - Validation
**Advanced validation for complex rules:**
```typescript
import { useForm } from 'vee-validate'
import * as yup from 'yup'

const schema = yup.object({
  customer: yup.number().required(),
  validUntil: yup.date()
    .min(new Date(), 'Date must be in the future')
    .required(),
  items: yup.array().of(
    yup.object({
      product: yup.number().required(),
      quantity: yup.number().min(1).required(),
      rate: yup.number().min(0).required()
    })
  ).min(1, 'Add at least one item')
})

const { handleSubmit, errors } = useForm({ validationSchema: schema })
```

#### 4. @nuxt/icon - Icons
**Consistent iconography:**
```vue
<!-- Status icons -->
<Icon name="mdi:check-circle" v-if="status === 'completed'" />
<Icon name="mdi:clock-outline" v-if="status === 'pending'" />
<Icon name="mdi:truck-delivery" v-if="status === 'in-transit'" />

<!-- Action icons -->
<Icon name="mdi:eye" @click="view" />
<Icon name="mdi:pencil" @click="edit" />
<Icon name="mdi:delete" @click="remove" />
<Icon name="mdi:printer" @click="print" />
```

#### 5. @nuxt/image - Image Optimization
**Product images in sales:**
```vue
<NuxtImg
  :src="product.image"
  :alt="product.name"
  width="200"
  height="200"
  format="webp"
  quality="80"
  placeholder
  loading="lazy"
/>
```

#### 6. @nuxtjs/device - Responsive Design
**Adaptive UI:**
```vue
<script setup>
const { isMobile, isTablet, isDesktop } = useDevice()
</script>

<template>
  <!-- Mobile POS layout -->
  <div v-if="isMobile" class="pos-mobile">
    <!-- Simplified touch-friendly interface -->
  </div>
  
  <!-- Desktop POS layout -->
  <div v-else class="pos-desktop">
    <!-- Full featured interface -->
  </div>
</template>
```

#### 7. nuxt-swiper - Product Carousels
**Featured products in POS:**
```vue
<Swiper
  :slides-per-view="isMobile ? 2 : 4"
  :space-between="20"
  :loop="true"
  :autoplay="{ delay: 3000 }"
>
  <SwiperSlide v-for="product in featuredProducts" :key="product.id">
    <ProductCard :product="product" />
  </SwiperSlide>
</Swiper>
```

#### 8. @nuxt/content - Help Documentation
**In-app help:**
```vue
<ContentDoc path="/help/sales/quotations" />
```

Create markdown files in `/content/help/sales/`:
```markdown
---
title: Creating Quotations
description: Step-by-step guide
---

# How to Create a Quotation

1. Click "New Quotation"
2. Select customer...
```

#### 9. nuxt-lodash - Data Manipulation
```vue
<script setup>
// Group sales by customer
const salesByCustomer = _groupBy(sales, 'customerId')

// Calculate totals
const totalRevenue = _sumBy(sales, 'grandTotal')

// Filter and sort
const topProducts = _orderBy(
  _filter(products, p => p.sales > 0),
  ['sales'],
  ['desc']
).slice(0, 10)
</script>
```

#### 10. nuxt-security - Data Protection
**Configured in `nuxt.config.ts`:**
```typescript
security: {
  headers: {
    contentSecurityPolicy: {
      'base-uri': ["'self'"],
      'img-src': ["'self'", 'data:', 'https:'],
      'script-src': ["'self'"]
    }
  },
  rateLimiter: {
    tokensPerInterval: 150,
    interval: 60000
  }
}
```

---

## Database Schema

### Core Tables

```sql
-- Quotations
CREATE TABLE quotations (
  id SERIAL PRIMARY KEY,
  quotation_no VARCHAR(50) UNIQUE NOT NULL,
  customer_id INTEGER REFERENCES customers(id),
  quotation_date DATE NOT NULL,
  valid_until DATE NOT NULL,
  status VARCHAR(20) NOT NULL,
  subtotal DECIMAL(15,2),
  tax_amount DECIMAL(15,2),
  discount_amount DECIMAL(15,2),
  grand_total DECIMAL(15,2),
  terms_and_conditions TEXT,
  notes TEXT,
  created_by INTEGER,
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE quotation_items (
  id SERIAL PRIMARY KEY,
  quotation_id INTEGER REFERENCES quotations(id) ON DELETE CASCADE,
  product_id INTEGER REFERENCES products(id),
  description TEXT,
  quantity DECIMAL(10,2),
  rate DECIMAL(15,2),
  discount_percent DECIMAL(5,2),
  amount DECIMAL(15,2)
);

-- Sales Orders
CREATE TABLE sales_orders (
  id SERIAL PRIMARY KEY,
  order_no VARCHAR(50) UNIQUE NOT NULL,
  quotation_id INTEGER REFERENCES quotations(id),
  customer_id INTEGER REFERENCES customers(id),
  customer_po_no VARCHAR(50),
  order_date DATE NOT NULL,
  delivery_date DATE,
  status VARCHAR(20) NOT NULL,
  payment_terms VARCHAR(50),
  subtotal DECIMAL(15,2),
  tax_amount DECIMAL(15,2),
  discount_amount DECIMAL(15,2),
  grand_total DECIMAL(15,2),
  advance_paid DECIMAL(15,2),
  notes TEXT,
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE sales_order_items (
  id SERIAL PRIMARY KEY,
  order_id INTEGER REFERENCES sales_orders(id) ON DELETE CASCADE,
  product_id INTEGER REFERENCES products(id),
  description TEXT,
  quantity DECIMAL(10,2),
  delivered_qty DECIMAL(10,2) DEFAULT 0,
  rate DECIMAL(15,2),
  discount_percent DECIMAL(5,2),
  amount DECIMAL(15,2)
);

-- Delivery Notes
CREATE TABLE delivery_notes (
  id SERIAL PRIMARY KEY,
  delivery_no VARCHAR(50) UNIQUE NOT NULL,
  sales_order_id INTEGER REFERENCES sales_orders(id),
  customer_id INTEGER REFERENCES customers(id),
  delivery_date DATE NOT NULL,
  scheduled_time VARCHAR(20),
  status VARCHAR(20) NOT NULL,
  driver_id INTEGER,
  vehicle_no VARCHAR(20),
  notes TEXT,
  proof_of_delivery JSONB,
  created_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE delivery_note_items (
  id SERIAL PRIMARY KEY,
  delivery_note_id INTEGER REFERENCES delivery_notes(id) ON DELETE CASCADE,
  order_item_id INTEGER REFERENCES sales_order_items(id),
  product_id INTEGER REFERENCES products(id),
  quantity DECIMAL(10,2),
  warehouse_id INTEGER
);

-- Sales Returns
CREATE TABLE sales_returns (
  id SERIAL PRIMARY KEY,
  return_no VARCHAR(50) UNIQUE NOT NULL,
  sales_invoice_id INTEGER,
  customer_id INTEGER REFERENCES customers(id),
  return_date DATE NOT NULL,
  status VARCHAR(20) NOT NULL,
  return_reason VARCHAR(100),
  refund_method VARCHAR(20),
  refund_amount DECIMAL(15,2),
  restock_items BOOLEAN DEFAULT true,
  notes TEXT,
  created_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE sales_return_items (
  id SERIAL PRIMARY KEY,
  return_id INTEGER REFERENCES sales_returns(id) ON DELETE CASCADE,
  product_id INTEGER REFERENCES products(id),
  quantity DECIMAL(10,2),
  rate DECIMAL(15,2),
  condition VARCHAR(20),
  restockable BOOLEAN DEFAULT true
);

-- POS Sessions
CREATE TABLE pos_sessions (
  id SERIAL PRIMARY KEY,
  session_no VARCHAR(50) UNIQUE NOT NULL,
  cashier_id INTEGER,
  pos_profile_id INTEGER,
  opening_time TIMESTAMP NOT NULL,
  closing_time TIMESTAMP,
  opening_cash DECIMAL(15,2),
  closing_cash DECIMAL(15,2),
  expected_cash DECIMAL(15,2),
  variance DECIMAL(15,2),
  status VARCHAR(20) NOT NULL,
  notes TEXT
);

-- Loyalty Programs
CREATE TABLE loyalty_programs (
  id SERIAL PRIMARY KEY,
  name VARCHAR(100) NOT NULL,
  points_per_rand DECIMAL(10,2),
  min_purchase DECIMAL(15,2),
  redemption_rate DECIMAL(10,2),
  validity_days INTEGER,
  active BOOLEAN DEFAULT true
);

CREATE TABLE customer_loyalty_points (
  id SERIAL PRIMARY KEY,
  customer_id INTEGER REFERENCES customers(id),
  program_id INTEGER REFERENCES loyalty_programs(id),
  points_earned DECIMAL(10,2) DEFAULT 0,
  points_redeemed DECIMAL(10,2) DEFAULT 0,
  points_balance DECIMAL(10,2) DEFAULT 0,
  last_activity_date TIMESTAMP
);
```

---

## API Endpoints

### Quotations API
```typescript
// GET /api/quotations
// POST /api/quotations
// GET /api/quotations/:id
// PUT /api/quotations/:id
// DELETE /api/quotations/:id
// POST /api/quotations/:id/send-email
// POST /api/quotations/:id/convert-to-order
```

### Sales Orders API
```typescript
// GET /api/sales-orders
// POST /api/sales-orders
// GET /api/sales-orders/:id
// PUT /api/sales-orders/:id
// PUT /api/sales-orders/:id/status
// POST /api/sales-orders/:id/create-delivery-note
// GET /api/sales-orders/:id/delivery-notes
```

### Delivery Notes API
```typescript
// GET /api/delivery-notes
// POST /api/delivery-notes
// GET /api/delivery-notes/:id
// PUT /api/delivery-notes/:id
// POST /api/delivery-notes/:id/proof-of-delivery
// PUT /api/delivery-notes/:id/complete
```

### Returns API
```typescript
// GET /api/sales-returns
// POST /api/sales-returns
// GET /api/sales-returns/:id
// PUT /api/sales-returns/:id
// POST /api/sales-returns/:id/approve
// POST /api/sales-returns/:id/process-refund
```

---

## Next Steps

### Immediate (This Session)
1. ✅ Install all required Nuxt modules
2. ✅ Create quotations listing page
3. ✅ Add i18n translations
4. ⏳ Create quotation creation form with FormKit
5. ⏳ Add delivery notes module
6. ⏳ Enhance POS with profiles and sessions

### Short Term (Next Session)
1. Complete all sales modules
2. Add comprehensive validation
3. Implement offline capabilities
4. Create mobile-optimized layouts
5. Add print/PDF generation

### Medium Term
1. Backend API implementation
2. Database migrations
3. Integration testing
4. User acceptance testing with township SMMEs
5. Deployment to staging

---

## TOSS-Specific Considerations

### Township Business Context
- **Simple Language**: Use everyday terms in local languages
- **Low Literacy**: Heavy use of icons and visual indicators
- **Cash Focus**: Prominently display cash payments
- **Credit Tracking**: Clear visibility of customer credit status
- **Mobile First**: Optimized for smartphones
- **Offline Priority**: All critical features work offline

### South African Specifics
- **Currency**: ZAR (R) formatting
- **Tax**: 15% VAT
- **Payment**: DebiCheck integration for debit orders
- **Delivery**: Township addressing (landmarks, not street addresses)
- **Communication**: WhatsApp >> Email

### AI Copilot Integration Points
1. **Smart Quotation Templates**: AI suggests products based on customer history
2. **Pricing Recommendations**: AI suggests competitive pricing
3. **Delivery Optimization**: AI recommends best delivery routes/times
4. **Return Predictions**: AI flags high-return-risk items
5. **Customer Insights**: AI provides next-best-action suggestions

---

## Success Metrics

### For Mama Dlamini (Target User)
- Can create quotation in < 2 minutes
- Can process sale offline
- Can track all deliveries on one screen
- Can see customer credit status at a glance
- Can switch between languages effortlessly

### Technical Metrics
- Page load < 2s on 3G
- Offline mode 100% functional for core features
- Form validation 100% coverage
- Mobile responsiveness score > 95
- Accessibility score > 90

---

**Last Updated**: November 10, 2025
**Status**: In Progress - Quotations module created, remaining modules to be implemented
**Priority**: High - Core business functionality for TOSS ERP MVP
