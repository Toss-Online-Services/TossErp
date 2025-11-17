# TOSS ERP Backend-Frontend Wiring Audit

## Executive Summary

This document tracks the wiring status of all frontend pages to backend endpoints.

## Status Legend
- ✅ Complete: Endpoint exists and is fully functional
- 🔶 Partial: Endpoint exists but needs enhancement
- ❌ Missing: Endpoint needs to be created
- 🔀 Redirect: Using different endpoint (e.g., `/vendors` instead of `/suppliers`)

---

## 1. Authentication (`/api/auth`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| POST `/api/auth/login` | ✅ | Complete |
| POST `/api/auth/refresh` | ✅ | Complete |
| POST `/api/auth/logout` | ✅ | Complete |
| GET `/api/auth/verify` | 🔶 | Exists but may need session enhancement |
| GET `/api/auth/session` | ❌ | **Need to create** |
| POST `/api/auth/session/activity` | ❌ | **Need to create** |
| POST `/api/auth/session/validate` | ❌ | **Need to create** |
| POST `/api/auth/session/terminate` | ❌ | **Need to create** |

**Pages**: `/auth/login`, `/auth/register`, `/auth/forgot-password`

---

## 2. AI Copilot (`/api/ai-copilot`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| POST `/api/ai-copilot/ask` | ✅ | Complete |
| GET `/api/ai-copilot/suggestions` | ✅ | Complete |
| POST `/api/ai-copilot/meta-tags` | ✅ | Complete |
| GET `/api/ai-copilot/settings/{shopId}` | ✅ | Complete |
| PUT `/api/ai-copilot/settings` | ✅ | Complete |

**Pages**: Global AI Assistant component

---

## 3. Dashboard (`/api/dashboard`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| GET `/api/dashboard/summary` | ✅ | Complete |
| GET `/api/dashboard/sales-trends` | ✅ | Complete |
| GET `/api/dashboard/top-products` | ✅ | Complete |
| GET `/api/dashboard/cash-flow` | ✅ | Complete |

**Pages**: `/dashboard/index`

---

## 4. Sales (`/api/sales`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| POST `/api/sales` | ✅ | Complete |
| GET `/api/sales` | ✅ | Complete (list) |
| GET `/api/sales/{id}` | ❌ | **Need to add** |
| GET `/api/sales/daily-summary` | ✅ | Complete |
| POST `/api/sales/{id}/void` | ✅ | Complete |
| POST `/api/sales/{id}/receipt` | ✅ | Complete |

**Pages**: `/sales/pos`, `/sales/orders/index`, `/sales/orders/create-order`, `/sales/orders/queue`, `/sales/invoices`

---

## 5. Inventory (`/api/inventory`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| GET `/api/inventory/products` | ✅ | Complete (list) |
| GET `/api/inventory/products/{id}` | ❌ | **Need to add** |
| POST `/api/inventory/products` | ✅ | Complete |
| GET `/api/inventory/stock-levels` | ✅ | Complete |
| GET `/api/inventory/low-stock-alerts` | ✅ | Complete |
| POST `/api/inventory/stock/adjust` | ✅ | Complete |
| GET `/api/inventory/stock/movements` | ✅ | Complete |
| GET `/api/inventory/categories` | ❌ | **Need to add** |
| GET `/api/inventory/products/by-sku` | ❌ | **Need to add** |
| GET `/api/inventory/products/by-barcode` | ❌ | **Need to add** |

**Pages**: `/stock/index`, `/stock/items`

---

## 6. Buying/Purchasing (`/api/buying`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| POST `/api/buying/purchase-orders` | ✅ | Complete |
| GET `/api/buying/purchase-orders/{id}` | ✅ | Complete (single) |
| GET `/api/buying/purchase-orders` | ❌ | **Need to add (list)** |
| POST `/api/buying/purchase-orders/{id}/approve` | ✅ | Complete |
| GET `/api/buying/aggregation/check` | ❌ | **Need to add** |
| GET `/api/buying/group-buys/active` | 🔀 | Use `/api/group-buying/pools/active` |
| POST `/api/buying/orders/aggregated` | ❌ | **Need to add** |

**Pages**: `/buying/orders/index`, `/buying/orders/create-order`, `/buying/invoices`

---

## 7. Suppliers/Vendors (`/api/suppliers` vs `/api/vendors`)

**Issue**: Frontend uses `/api/suppliers` but backend has `/api/vendors`

**Solution Options**:
1. Create alias endpoints `/api/suppliers` → `/api/vendors`
2. Update frontend composables to use `/api/vendors`

**Recommended**: Create alias endpoints for backward compatibility

| Frontend Expects | Backend Has | Status | Action |
|-----------------|-------------|--------|--------|
| GET `/api/suppliers` | GET `/api/vendors` | 🔀 | Create alias |
| GET `/api/suppliers/{id}` | GET `/api/vendors/{id}` | 🔀 | Create alias |
| POST `/api/suppliers` | POST `/api/vendors` | 🔀 | Create alias |
| GET `/api/suppliers/{id}/products` | GET `/api/vendors/{id}/products` | 🔀 | Create alias |
| POST `/api/suppliers/{id}/products` | POST `/api/vendors/{id}/products` | 🔀 | Create alias |
| PUT `/api/suppliers/products/{id}/pricing` | PUT `/api/vendors/products/{id}/pricing` | 🔀 | Create alias |

**Pages**: `/buying/suppliers/index`

---

## 8. CRM/Customers (`/api/crm`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| GET `/api/crm/customers` | ✅ | Complete (list) |
| GET `/api/crm/customers/{id}` | ✅ | Complete (single) |
| POST `/api/crm/customers` | ✅ | Complete |
| GET `/api/crm/customers/search` | ❌ | **Need to add** |

**Pages**: `/crm/customers/index`, `/crm/customers/[id]`

---

## 9. Group Buying (`/api/group-buying`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| POST `/api/group-buying/pools` | ✅ | Complete |
| GET `/api/group-buying/pools/active` | ✅ | Complete |
| GET `/api/group-buying/pools/{id}` | ✅ | Complete |
| POST `/api/group-buying/pools/{poolId}/join` | ✅ | Complete |
| POST `/api/group-buying/pools/{poolId}/confirm` | ✅ | Complete |
| POST `/api/group-buying/pools/{poolId}/generate-po` | ✅ | Complete |
| GET `/api/group-buying/participations` | ✅ | Complete |
| GET `/api/group-buying/opportunities` | ✅ | Complete |

**Pages**: `/buying/group-buying/index`

---

## 10. Logistics/Delivery (`/api/logistics`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| POST `/api/logistics/delivery-runs` | ✅ | Complete |
| GET `/api/logistics/delivery-runs` | ✅ | Complete (list) |
| GET `/api/logistics/delivery-runs/{id}/driver-view` | ✅ | Complete |
| POST `/api/logistics/delivery-runs/{id}/status` | ✅ | Complete |
| POST `/api/logistics/delivery-runs/{id}/assign-driver` | ✅ | Complete |
| POST `/api/logistics/delivery-stops/{stopId}/proof` | ✅ | Complete |
| GET `/api/logistics/delivery-runs/{runId}/tracking` | ❌ | **Need to add** |

**Pages**: `/logistics/driver`, `/logistics/shared-runs`, `/logistics/tracking`

---

## 11. Payments (`/api/payments`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| POST `/api/payments/pay-links` | ✅ | Complete |
| POST `/api/payments/record` | ✅ | Complete |
| GET `/api/payments` | ✅ | Complete (list) |
| GET `/api/payments/{id}` | ❌ | **Need to add (single)** |
| POST `/api/payments/mpesa/initiate` | ❌ | **Need to add** |
| POST `/api/payments/airtel/initiate` | ❌ | **Need to add** |
| POST `/api/payments/mtn/initiate` | ❌ | **Need to add** |
| GET `/api/payments/{provider}/status/{transactionId}` | ❌ | **Need to add** |
| POST `/api/payments/qr/generate` | ❌ | **Need to add** |

**Pages**: Integrated in POS and various transaction pages

---

## 12. Users Management (`/api/users`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| GET `/api/users` | ❌ | **Need to create** |
| GET `/api/users/{id}` | ❌ | **Need to create** |
| POST `/api/users` | ❌ | **Need to create** |
| PUT `/api/users/{id}` | ❌ | **Need to create** |
| DELETE `/api/users/{id}` | ❌ | **Need to create** |
| PUT `/api/users/{id}/roles` | ❌ | **Need to create** |

**Pages**: `/users/index`

---

## 13. Settings (`/api/settings`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| GET `/api/settings/shop/{shopId}` | ✅ | Complete |
| PUT `/api/settings/shop/{shopId}` | ✅ | Complete |

**Pages**: `/settings/index`

---

## 14. Audit Logging (`/api/audit`)

| Endpoint | Status | Notes |
|----------|--------|-------|
| POST `/api/audit/log` | ❌ | **Need to create** |

**Pages**: Used by `useAudit` composable across multiple pages

---

## Priority Matrix

### P0 - Critical (Blocking Core Functionality)
1. GET `/api/auth/session` - Auth pages depend on this
2. GET `/api/buying/purchase-orders` - Buying orders list page
3. GET `/api/sales/{id}` - Sales order details
4. GET `/api/inventory/products/{id}` - Stock item details
5. Suppliers alias endpoints - Suppliers page

### P1 - High (Feature Incomplete)
1. GET `/api/crm/customers/search` - Customer search
2. GET `/api/inventory/categories` - Product categorization
3. GET `/api/inventory/products/by-sku` - SKU lookup
4. GET `/api/inventory/products/by-barcode` - Barcode scanning
5. Mobile money endpoints - Payment integration

### P2 - Medium (Enhancement)
1. Session management endpoints
2. GET `/api/logistics/delivery-runs/{runId}/tracking` - Tracking details
3. QR code generation
4. Audit logging

### P3 - Low (Nice to Have)
1. Advanced analytics endpoints
2. Reporting endpoints

---

## Implementation Plan

### Phase 1: Critical Endpoints (Week 1)
- [ ] Auth session management
- [ ] Suppliers/Vendors aliasing
- [ ] Missing GET by ID endpoints
- [ ] Purchase orders list

### Phase 2: Feature Complete (Week 2)
- [ ] Search endpoints
- [ ] Mobile money payments
- [ ] User management

### Phase 3: Polish & Enhancement (Week 3)
- [ ] Audit logging
- [ ] Advanced features
- [ ] Testing & validation

---

## Testing Strategy

For each endpoint:
1. Unit test the Command/Query handler
2. Integration test the endpoint
3. Test from frontend composable
4. E2E test from UI

---

## Notes

- Follow nopCommerce service patterns for business logic
- Use CQRS pattern (Commands/Queries) for all operations
- Ensure proper error handling and validation
- Add authorization checks using existing security middleware

