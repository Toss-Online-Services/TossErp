# TOSS ERP Backend-Frontend Wiring Progress

## Summary
This document tracks the progress of wiring all frontend pages to backend endpoints.

**Last Updated**: 2025-10-26

---

## ✅ Completed (P0 - Critical)

### 1. Auth Session Management Endpoints ✅
**Status**: Complete  
**Endpoints Added**:
- ✅ GET `/api/auth/session` - Get current session info
- ✅ POST `/api/auth/session/activity` - Update session activity
- ✅ POST `/api/auth/session/validate` - Validate session
- ✅ POST `/api/auth/session/terminate` - Terminate session

**Implementation**:
- Added session endpoints to `backend/Toss/src/Web/Endpoints/Auth.cs`
- Implemented in-memory session store (use Redis/database in production)
- Returns `SessionInfoDto` matching frontend `SessionInfo` type

**Files Modified**:
- `backend/Toss/src/Web/Endpoints/Auth.cs`

### 2. Suppliers Alias Endpoints ✅
**Status**: Complete  
**Endpoints Added**:
- ✅ GET `/api/suppliers` → GET `/api/vendors` (alias)
- ✅ GET `/api/suppliers/{id}` → GET `/api/vendors/{id}` (alias)
- ✅ POST `/api/suppliers` → POST `/api/vendors` (alias)
- ✅ GET `/api/suppliers/{id}/products` → GET `/api/vendors/{id}/products` (alias)
- ✅ POST `/api/suppliers/{id}/products` → POST `/api/vendors/{id}/products` (alias)
- ✅ PUT `/api/suppliers/products/{productId}/pricing` → PUT `/api/vendors/products/{productId}/pricing` (alias)

**Implementation**:
- Created new endpoint file that routes `/api/suppliers` to existing Vendors commands/queries
- Maintains backward compatibility with frontend

**Files Created**:
- `backend/Toss/src/Web/Endpoints/Suppliers.cs`

### 3. Purchase Orders List Endpoint ✅
**Status**: Complete  
**Endpoints Added**:
- ✅ GET `/api/buying/purchase-orders` - List purchase orders with filtering

**Implementation**:
- Created `GetPurchaseOrdersQuery` in Application layer
- Added query handler with filtering by ShopId, Status, and pagination
- Returns `List<PurchaseOrderListDto>`

**Files Created**:
- `backend/Toss/src/Application/Buying/Queries/GetPurchaseOrders/GetPurchaseOrdersQuery.cs`

**Files Modified**:
- `backend/Toss/src/Web/Endpoints/Buying.cs`

### 4. Sales By ID Endpoint ✅
**Status**: Complete  
**Endpoints Added**:
- ✅ GET `/api/sales/{id}` - Get individual sale by ID

**Implementation**:
- Created `GetSaleByIdQuery` in Application layer
- Added query handler with EF Core includes for Shop and Items
- Returns `SaleDetailDto` with full sale information including items

**Files Created**:
- `backend/Toss/src/Application/Sales/Queries/GetSaleById/GetSaleByIdQuery.cs`

**Files Modified**:
- `backend/Toss/src/Web/Endpoints/Sales.cs`

### 5. Inventory Product By ID Endpoint ✅
**Status**: Complete  
**Endpoints Added**:
- ✅ GET `/api/inventory/products/{id}` - Get individual product by ID

**Implementation**:
- Created `GetProductByIdQuery` in Application layer
- Added query handler with EF Core includes for ProductCategory
- Returns `ProductDetailDto` with full product information

**Files Created**:
- `backend/Toss/src/Application/Inventory/Queries/GetProductById/GetProductByIdQuery.cs`

**Files Modified**:
- `backend/Toss/src/Web/Endpoints/Inventory.cs`

---

## 🔶 In Progress (P1 - High Priority)

### 6. CRM Customer Search Endpoint 🔶
**Status**: In Progress  
**Next Steps**:
- Create `SearchCustomersQuery` in Application layer
- Add endpoint to `CRM.cs`
- Implement full-text search on customer name, email, phone

### 7. Inventory Categories Endpoint
**Status**: Pending  
**Next Steps**:
- Create `GetCategoriesQuery` in Application layer
- Add endpoint to `Inventory.cs`

### 8. Inventory Product By SKU Endpoint
**Status**: Pending  
**Next Steps**:
- Create `GetProductBySkuQuery` in Application layer
- Add endpoint to `Inventory.cs`

### 9. Inventory Product By Barcode Endpoint
**Status**: Pending  
**Next Steps**:
- Create `GetProductByBarcodeQuery` in Application layer
- Add endpoint to `Inventory.cs`

### 10. Mobile Money Payment Endpoints
**Status**: Pending  
**Next Steps**:
- Create M-Pesa initiation endpoint
- Create Airtel Money initiation endpoint
- Create MTN Mobile Money initiation endpoint
- Create payment status check endpoint
- Integrate with respective payment provider SDKs/APIs

---

## ⏳ Pending (P2 - Medium Priority)

### 11. Users Management Endpoints
**Status**: Pending  
**Endpoints Needed**:
- GET `/api/users` - List users
- GET `/api/users/{id}` - Get user by ID
- POST `/api/users` - Create user
- PUT `/api/users/{id}` - Update user
- DELETE `/api/users/{id}` - Delete user
- PUT `/api/users/{id}/roles` - Update user roles

### 12. Logistics Tracking Endpoint
**Status**: Pending  
**Endpoint Needed**:
- GET `/api/logistics/delivery-runs/{runId}/tracking` - Get delivery tracking details

### 13. QR Code Generation Endpoint
**Status**: Pending  
**Endpoint Needed**:
- POST `/api/payments/qr/generate` - Generate QR code for payment

### 14. Audit Logging Endpoint
**Status**: Pending  
**Endpoint Needed**:
- POST `/api/audit/log` - Log audit events

---

## 📊 Statistics

### Overall Progress
- **Total Endpoints Needed**: 35+
- **Completed**: 11 (31%)
- **In Progress**: 1 (3%)
- **Pending**: 23 (66%)

### By Priority
- **P0 (Critical)**: 5/5 complete (100%) ✅
- **P1 (High)**: 1/5 in progress (20%)
- **P2 (Medium)**: 0/4 started (0%)

---

## 🎯 Next Actions

1. **Complete P1-1**: Finish CRM customer search endpoint
2. **Tackle P1-2 to P1-4**: Add inventory lookup endpoints (categories, by-sku, by-barcode)
3. **P1-5**: Research and implement mobile money integrations
4. **P2**: Create users management CRUD endpoints
5. **Testing**: Build comprehensive test suite for all endpoints
6. **Frontend Integration**: Test each composable with corresponding backend

---

## 📝 Notes

- All P0 endpoints are following CQRS pattern (Commands/Queries)
- Using MediatR for request/response handling
- DTOs are defined in each Query/Command file
- Error handling uses `NotFoundException` from Common
- All endpoints require proper authentication/authorization
- Session management currently uses in-memory store (move to Redis/DB for production)

---

## 🔗 Related Documents

- [Endpoint Audit](ENDPOINT_AUDIT.md) - Comprehensive endpoint inventory
- [AI Integration Complete](AI_INTEGRATION_COMPLETE.md) - AI copilot integration summary
- [Migration Instructions](APPLY_MIGRATION_INSTRUCTIONS.md) - Database migration guide

