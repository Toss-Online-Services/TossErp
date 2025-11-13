# TOSS Sales Module Analysis & ERPNext Comparison

## Current State Analysis

### Implemented Features ✅

#### Sales Transactions
- **Sales Orders**: Basic CRUD operations with listing, creation, and viewing
- **Quotations**: Complete quotation management workflow
- **Sales Invoices**: Invoice generation and management
- **Delivery Notes**: Basic delivery note functionality
- **Point of Sale (POS)**: Core POS functionality implemented
- **Sales Analytics**: Basic analytics dashboard

#### API Integration
- **Sales API**: Comprehensive `useSalesAPI` composable
- **Queue Orders**: Support for chisa nyama/food preparation workflow
- **Held Sales**: Support for incomplete transactions
- **Refunds**: Refund processing capability
- **Multi-payment methods**: Cash, Card, Mobile Money, etc.

### ERPNext Selling Module Features vs TOSS Implementation

#### Master Data (Customer Management) ✅ Partially Implemented
- **Customer**: Basic customer management through CRM module ✅
- **Customer Group**: ❌ Missing - Need customer segmentation for SMME context
- **Sales Person**: ❌ Missing - Important for multi-staff spaza shops
- **Sales Partner**: ❌ Missing - Critical for TOSS collaborative network
- **Territory**: ❌ Missing - Important for township/rural delivery zones
- **Promotional Scheme**: ❌ Missing - Essential for spaza shop promotions

#### Transaction Workflow ✅ Partially Implemented
- **Quotation → Sales Order → Delivery Note → Invoice**: Basic flow exists ✅
- **Credit Note**: ❌ Missing - Important for returns/adjustments
- **Blanket Order**: ❌ Missing - Good for regular supplier relationships
- **Drop Shipping**: ❌ Missing - Could be useful for TOSS network
- **Sales Return**: ❌ Missing - Essential for spaza shop operations

## Critical Missing Features for South African SMMEs

### 1. Customer Segmentation & Groups ❌
**Business Impact**: High
- Township customers often buy in groups (stokvels, burial societies)
- Different pricing for regular vs walk-in customers
- Credit management for trusted customers

### 2. Sales Territory Management ❌
**Business Impact**: High
- Delivery zones for different townships
- Regional pricing variations
- Local driver assignments

### 3. Sales Person/Agent Management ❌
**Business Impact**: Medium
- Multi-staff operations (family members, part-time help)
- Commission tracking
- Performance monitoring

### 4. Credit Notes & Returns ❌
**Business Impact**: High
- Product returns (expired goods, wrong items)
- Credit adjustments for loyal customers
- Damage claims

### 5. Promotional Schemes ❌
**Business Impact**: High
- Volume discounts (buy 2 get 1 free)
- Loyalty programs
- Seasonal promotions
- Group buying incentives

### 6. Sales Partner Network ❌
**Business Impact**: Critical for TOSS
- Supplier partnerships
- Driver partnerships  
- Cross-selling between shops

### 7. Advanced Pricing Rules ❌
**Business Impact**: High
- Customer-specific pricing
- Bulk pricing tiers
- Time-based pricing (happy hour discounts)
- Payment method discounts

### 8. Blanket Orders ❌
**Business Impact**: Medium
- Regular weekly/monthly orders
- Seasonal inventory planning
- Supplier agreements

## Required Enhancements

### Immediate Priority (Week 1-2)

#### 1. Customer Groups & Segmentation
```typescript
interface CustomerGroup {
  id: string
  name: string
  description: string
  defaultDiscount: number
  creditLimit: number
  paymentTerms: number // days
  priceList: string
  territory: string
}
```

#### 2. Sales Territory Management
```typescript
interface Territory {
  id: string
  name: string
  parentTerritory?: string
  manager?: string
  deliveryCharges: number
  taxRate: number
  isActive: boolean
}
```

#### 3. Credit Notes & Returns
```typescript
interface CreditNote {
  id: string
  returnedInvoice: string
  returnDate: Date
  returnedItems: Array<{
    productId: string
    quantityReturned: number
    reason: string
    condition: 'damaged' | 'expired' | 'wrong_item'
  }>
  creditAmount: number
  refundMethod: 'cash' | 'store_credit' | 'replacement'
}
```

### Medium Priority (Week 3-4)

#### 4. Promotional Schemes
```typescript
interface PromotionalScheme {
  id: string
  name: string
  type: 'percentage' | 'amount' | 'buy_x_get_y' | 'bulk_discount'
  validFrom: Date
  validTo: Date
  applicableCustomerGroups: string[]
  conditions: {
    minimumQuantity?: number
    minimumAmount?: number
    applicableProducts?: string[]
  }
  discount: {
    percentage?: number
    amount?: number
    freeQuantity?: number
  }
}
```

#### 5. Sales Person Management
```typescript
interface SalesPerson {
  id: string
  name: string
  employeeId?: string
  commissionRate: number
  territory: string
  targetAmount: number
  isActive: boolean
}
```

### Long-term Priority (Week 5-8)

#### 6. Sales Partner Network
```typescript
interface SalesPartner {
  id: string
  name: string
  type: 'supplier' | 'driver' | 'shop' | 'aggregator'
  commissionRate: number
  territory: string
  contactInfo: {
    email: string
    phone: string
    address: string
  }
  rating: number
  isActive: boolean
}
```

#### 7. Advanced Pricing Rules
```typescript
interface PricingRule {
  id: string
  name: string
  applicableFor: 'customer' | 'customer_group' | 'territory' | 'item'
  conditions: {
    minQuantity?: number
    validFrom?: Date
    validTo?: Date
    paymentMethod?: string
  }
  priceOrDiscount: {
    type: 'percentage' | 'amount' | 'fixed_price'
    value: number
  }
  priority: number
}
```

## Navigation Updates Required

### Current Sales Navigation
```typescript
{
  label: 'Sales Workspace',
  icon: 'mdi:shopping-outline',
  children: [
    { label: 'Sales Analytics', to: '/sales/analytics' },
    { label: 'Orders', to: '/sales/orders' },
    { label: 'Point of Sale', to: '/sales/pos' },
    { label: 'Invoices', to: '/sales/invoices' },
  ],
}
```

### Enhanced Sales Navigation
```typescript
{
  label: 'Sales & CRM',
  icon: 'mdi:shopping-outline',
  children: [
    { label: 'Sales Dashboard', to: '/sales' },
    { label: 'Point of Sale', to: '/sales/pos' },
    
    // Sales Transactions
    { label: 'Quotations', to: '/sales/quotations' },
    { label: 'Sales Orders', to: '/sales/orders' },
    { label: 'Delivery Notes', to: '/sales/delivery-notes' },
    { label: 'Sales Invoices', to: '/sales/invoices' },
    { label: 'Credit Notes', to: '/sales/credit-notes' },
    { label: 'Returns & Refunds', to: '/sales/returns' },
    
    // Master Data
    { label: 'Customers', to: '/sales/customers' },
    { label: 'Customer Groups', to: '/sales/customer-groups' },
    { label: 'Sales Territories', to: '/sales/territories' },
    { label: 'Sales Partners', to: '/sales/partners' },
    
    // Pricing & Promotions  
    { label: 'Pricing Rules', to: '/sales/pricing-rules' },
    { label: 'Promotional Schemes', to: '/sales/promotions' },
    
    // Analytics
    { label: 'Sales Analytics', to: '/sales/analytics' },
    { label: 'Sales Reports', to: '/sales/reports' },
  ],
}
```

## Implementation Roadmap

### Phase 1: Core Missing Features (2 weeks)
1. Customer Groups & Segmentation
2. Sales Territory Management  
3. Credit Notes & Returns
4. Basic promotional schemes

### Phase 2: Advanced Features (2 weeks)
1. Sales Person management
2. Advanced pricing rules
3. Blanket orders
4. Enhanced analytics

### Phase 3: TOSS-Specific Features (2 weeks)
1. Sales Partner network integration
2. Group buying functionality
3. Mobile money integration enhancements
4. Offline-first improvements

### Phase 4: Mobile & Local Features (2 weeks)
1. WhatsApp integration for orders
2. Local language support
3. Voice ordering capabilities
4. Barcode scanning enhancements

## Business Impact Assessment

### High Impact Missing Features
1. **Customer Groups** - Essential for different pricing tiers
2. **Territory Management** - Critical for delivery and regional ops
3. **Credit Notes/Returns** - Daily operational necessity
4. **Promotional Schemes** - Key competitive advantage

### Medium Impact Missing Features  
1. **Sales Person Management** - Important for growing businesses
2. **Advanced Pricing Rules** - Flexibility for competitive pricing
3. **Sales Partners** - Foundation of TOSS network effect

### TOSS-Specific Competitive Advantages
1. **Group Buying Integration** - Unique to TOSS platform
2. **Mobile-First Design** - Optimized for township usage
3. **Offline Capability** - Critical for rural areas
4. **AI Recommendations** - Smart business insights

This analysis provides the foundation for completing the TOSS sales module to match ERPNext capabilities while adding SMME-specific enhancements.