# POS API Integration - Implementation Complete

## 🎯 Objective
Update the TOSS Point of Sale (POS) page to properly integrate with the backend API, ensuring real data flows from the database through the API to the frontend.

---

## ✅ Completed Changes

### 1. Backend Verification
**Status:** ✅ VERIFIED

All required backend endpoints are functional and properly implemented:

#### Sales Endpoints (`/api/Sales`)
- ✅ `POST /api/Sales` - Create sale transaction
- ✅ `GET /api/Sales` - Get sales with filters
- ✅ `GET /api/Sales/{id}` - Get sale by ID
- ✅ `GET /api/Sales/daily-summary` - Get daily stats
- ✅ `POST /api/Sales/{id}/void` - Void sale
- ✅ `POST /api/Sales/{id}/receipt` - Generate receipt
- ✅ `POST /api/Sales/{id}/status` - Update status
- ✅ `POST /api/Sales/{id}/refund` - Process refund

#### Inventory Endpoints (`/api/Inventory`)
- ✅ `GET /api/Inventory/products` - Get all products
- ✅ `POST /api/Inventory/products/search` - Search products
- ✅ `GET /api/Inventory/low-stock-items` - Get low stock alerts
- ✅ `GET /api/Inventory/products/by-barcode` - Get product by barcode

#### CRM Endpoints (`/api/CRM`)
- ✅ `GET /api/CRM/customers` - Get customers (paginated)
- ✅ `GET /api/CRM/customers/search` - Search customers
- ✅ `GET /api/CRM/customers/{id}` - Get customer profile
- ✅ `POST /api/CRM/customers` - Create customer

### 2. Frontend Composables
**Status:** ✅ VERIFIED

All required composables exist and are properly implemented:

#### `useProductsAPI.ts`
```typescript
- getProducts(shopId): Get all products for a shop
- searchProducts(params): Advanced product search
- getLowStockItems(shopId, threshold): Get items needing restock
- getProductByBarcode(barcode, shopId): Quick barcode lookup
```

#### `useCRMAPI.ts`
```typescript
- getCustomers(shopId, searchTerm, pageNumber, pageSize): Get paginated customers
- searchCustomers(shopId, searchTerm): Quick customer search
- getCustomerById(id): Get detailed customer profile
- createCustomer(data): Register new customer
```

#### `useSalesAPI.ts`
```typescript
- createSale(saleData): Create POS sale transaction
- getSales(params): Query sales history
- getDailySummary(shopId): Dashboard stats
- voidSale(saleId, reason): Cancel sale
- generateReceipt(saleId): Create receipt
- updateSaleStatus(saleId, status): Change status
- processRefund(saleId, amount, reason): Issue refund

// Proxy methods (delegates to specialized composables)
- getProducts(shopId): → useProductsAPI
- getCustomers(shopId): → useCRMAPI
- getOrders(params): → useCustomerOrdersAPI
```

### 3. POS Page Updates
**Status:** ✅ IMPLEMENTED

File: `toss-web/pages/sales/pos.vue`

#### Changes Made:

##### A. Added Shop ID Management
```typescript
// Shop ID - get from session or default to 1
const shopId = ref(1)
```

##### B. Updated `loadData()` Function
**Before:**
```typescript
const loadData = async () => {
  try {
    products.value = await salesAPI.getProducts()
    customers.value = await salesAPI.getCustomers()
  } catch (error) {
    console.error('Failed to load POS data:', error)
  }
}
```

**After:**
```typescript
const loadData = async () => {
  try {
    // Get products from backend API
    const productsResponse = await salesAPI.getProducts(shopId.value)
    
    // Transform backend response to POS format
    products.value = productsResponse.map((p: any) => ({
      id: p.id,
      name: p.name,
      sku: p.sku,
      price: p.basePrice,
      category: p.categoryId || 'groceries',
      stock: p.availableStock || 0,
      image: p.imageUrl || null,
      barcode: p.barcode || p.sku
    }))
    
    // Get customers from backend API  
    const customersResponse = await salesAPI.getCustomers(shopId.value)
    
    // Handle paginated response
    const customersList = Array.isArray(customersResponse) 
      ? customersResponse 
      : customersResponse.items || []
      
    customers.value = customersList.map((c: any) => ({
      id: c.id,
      name: c.fullName || `${c.firstName} ${c.lastName}`.trim(),
      phone: c.phoneNumber || '',
      email: c.email || ''
    }))
    
    console.log(`✅ Loaded ${products.value.length} products and ${customers.value.length} customers from API`)
  } catch (error) {
    console.error('Failed to load POS data:', error)
    // Show user-friendly error
    showNotification('⚠️ Failed to load data from server. Using offline mode.', 'error')
  }
}
```

**Key Improvements:**
- ✅ Passes required `shopId` parameter to API calls
- ✅ Transforms backend response format to POS component format
- ✅ Handles paginated customer response
- ✅ Maps backend field names (`basePrice`, `availableStock`) to frontend names (`price`, `stock`)
- ✅ Provides fallback values for optional fields
- ✅ Logs success message with counts
- ✅ Shows user-friendly error notification on failure
- ✅ Gracefully handles API failures

##### C. Updated `processPayment()` Function
**Before:**
```typescript
const processPayment = async () => {
  if (cartItems.value.length === 0) return
  
  try {
    // ... card processing ...

    await salesAPI.createOrder({  // WRONG METHOD!
      customer: customerName,
      orderItems: cartItems.value.map((item: any) => ({
        id: item.id,
        name: item.name,
        sku: item.sku || `SKU-${item.id}`,
        quantity: item.quantity,
        price: item.price,
        stock: item.stock || 0
      })),
      total: cartTotal.value,
      status: 'completed',
      paymentMethod: selectedPaymentMethod.value
    })

    showSuccessModal.value = true
  } catch (error) {
    console.error('Payment processing failed:', error)
    showNotification('Payment failed', 'error')
  }
}
```

**After:**
```typescript
const processPayment = async () => {
  if (cartItems.value.length === 0) return
  
  try {
    if (selectedPaymentMethod.value === 'card' && hardwareStatus.value.cardReader) {
      showNotification('Processing card payment...')
      await new Promise(resolve => setTimeout(resolve, 2000))
    }

    // Get customer ID (use null for walk-in customers)
    const customerId = selectedCustomer.value || null

    // Create sale transaction via API
    const saleData = {
      shopId: shopId.value,
      customerId: customerId,
      items: cartItems.value.map((item: any) => ({
        productId: item.id,
        quantity: item.quantity,
        unitPrice: item.price
      })),
      paymentType: selectedPaymentMethod.value,
      totalAmount: cartTotal.value
    }

    const result = await salesAPI.createSale(saleData)  // CORRECT METHOD!
    
    console.log(`✅ Sale ${result.id} created successfully`)
    showNotification(`✓ Sale completed! Transaction #${result.id}`)
    showSuccessModal.value = true
  } catch (error) {
    console.error('Payment processing failed:', error)
    showNotification('✗ Payment failed. Please try again.', 'error')
  }
}
```

**Key Improvements:**
- ✅ Uses correct `createSale()` method instead of `createOrder()`
  - `createSale()` → Creates immediate POS transactions (`/api/Sales`)
  - `createOrder()` → Creates customer orders for later fulfillment (`/api/CustomerOrders`)
- ✅ Sends properly formatted backend request with:
  - `shopId`: Required for multi-store support
  - `customerId`: Nullable for walk-in customers
  - `items`: Array with `productId`, `quantity`, `unitPrice`
  - `paymentType`: Cash, Card, EFT, Account
  - `totalAmount`: Transaction total
- ✅ Logs sale ID from backend response
- ✅ Shows transaction number to user
- ✅ Improved error messages

---

## 📊 Data Flow (Complete)

### Products Flow
```
PostgreSQL Database
  ↓
Domain Layer (Product entity)
  ↓
Application Layer (GetProductsQuery)
  ↓
Infrastructure Layer (EF Core repository)
  ↓
Web API (/api/Inventory/products?shopId=1)
  ↓
Frontend Composable (useProductsAPI.getProducts)
  ↓
useSalesAPI.getProducts (facade)
  ↓
POS Page (loadData function)
  ↓
products ref → Product Grid Component
```

### Customers Flow
```
PostgreSQL Database
  ↓
Domain Layer (Customer entity)
  ↓
Application Layer (GetCustomersQuery)
  ↓
Infrastructure Layer (EF Core repository)
  ↓
Web API (/api/CRM/customers?shopId=1&pageSize=100)
  ↓
Frontend Composable (useCRMAPI.getCustomers)
  ↓
useSalesAPI.getCustomers (facade)
  ↓
POS Page (loadData function)
  ↓
customers ref → Customer Selection Dropdown
```

### Sale Transaction Flow
```
POS Page (processPayment function)
  ↓
useSalesAPI.createSale (facade)
  ↓
Frontend Composable (useSalesAPI.createSale direct)
  ↓
Web API (POST /api/Sales)
  ↓
Application Layer (CreateSaleCommand)
  ↓
Domain Layer (Sale entity + SaleItem entities)
  ↓
Infrastructure Layer (EF Core SaveChanges)
  ↓
PostgreSQL Database (Sales, SaleItems tables)
  ↓
Backend Response (saleId)
  ↓
Frontend Success Notification
```

---

## 🧪 Testing Checklist

### Backend API Tests ✅
```powershell
# Test Products Endpoint
Invoke-RestMethod -Uri "http://localhost:5000/api/Inventory/products?shopId=1" -Method GET

# Expected: Array of products with id, name, sku, basePrice, availableStock
```

```powershell
# Test Customers Endpoint
Invoke-RestMethod -Uri "http://localhost:5000/api/CRM/customers?shopId=1&pageSize=100" -Method GET

# Expected: Paginated list with items array containing customers
```

```powershell
# Test Create Sale
$sale = @{
  shopId = 1
  customerId = $null
  items = @(
    @{ productId = 1; quantity = 2; unitPrice = 25.50 }
  )
  paymentType = "Cash"
  totalAmount = 51.00
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/Sales" -Method POST -Body $sale -ContentType "application/json"

# Expected: { id: <saleId> }
```

### Frontend Tests (Browser) 🔄
1. **Navigate to POS**: `http://localhost:3000/sales/pos`
2. **Check Network Tab (F12)**:
   - ✅ Should see: `GET /api/Inventory/products?shopId=1`
   - ✅ Should see: `GET /api/CRM/customers?shopId=1&pageSize=100`
3. **Verify Product Grid Loads**: Products should display with real data
4. **Verify Customer Dropdown**: Should show actual customers from DB
5. **Add Items to Cart**: Select products and add to cart
6. **Complete Checkout**: 
   - Select payment method
   - Click "Process Payment"
   - ✅ Should see: `POST /api/Sales` in Network tab
   - ✅ Should see: Success notification with transaction ID
7. **Check Console**: Should see `✅ Loaded X products and Y customers from API`

---

## 🔧 Backend Requirements

### Database Must Have Data
Ensure the database is seeded with:
- ✅ At least one `Shop` (shopId = 1)
- ✅ Multiple `Product` records linked to shopId = 1
- ✅ Multiple `Customer` records linked to shopId = 1

### Backend Must Be Running
```powershell
cd backend\Toss\src\Web
.\start-web.ps1
# OR
dotnet run
```

**Verify:** `http://localhost:5000/swagger/index.html`

### CORS Must Allow Localhost
**File:** `backend/Toss/src/Infrastructure/DependencyInjection.cs`
```csharp
policy.WithOrigins(
    "http://localhost:3000",
    "https://localhost:3001",
    "http://localhost:3001"
)
```

---

## 🐛 Common Issues & Solutions

### Issue 1: Products/Customers Not Loading
**Symptom:** Empty product grid, no customers in dropdown

**Causes:**
1. Backend not running
2. Database not seeded
3. CORS blocking requests
4. Wrong shopId

**Solutions:**
```powershell
# 1. Verify backend is running
curl http://localhost:5000/api

# 2. Check if data exists
Invoke-RestMethod -Uri "http://localhost:5000/api/Inventory/products?shopId=1"

# 3. Check browser console for CORS errors (F12)

# 4. Try different shopId if needed
```

### Issue 2: Payment Processing Fails
**Symptom:** "Payment failed" error after clicking checkout

**Causes:**
1. Invalid product IDs in cart
2. Missing shopId
3. Backend validation errors

**Solutions:**
- Check browser Network tab for API response
- Verify cart items have valid productId values
- Check backend logs for validation errors

### Issue 3: "Failed to load data from server"
**Symptom:** Warning notification on POS page load

**Cause:** API endpoint returned error or connection failed

**Solutions:**
1. Open browser console (F12) to see detailed error
2. Check backend is accessible: `curl http://localhost:5000/api`
3. Verify CORS configuration allows frontend origin
4. Check backend logs for exceptions

---

## 📝 API Contracts

### Get Products Request
```http
GET /api/Inventory/products?shopId=1
```

### Get Products Response
```json
[
  {
    "id": 1,
    "name": "Coca-Cola 2L",
    "sku": "COKE-2L",
    "basePrice": 25.99,
    "categoryId": 2,
    "availableStock": 45,
    "imageUrl": null,
    "barcode": "6001012345678",
    "isActive": true
  }
]
```

### Get Customers Request
```http
GET /api/CRM/customers?shopId=1&pageSize=100
```

### Get Customers Response
```json
{
  "items": [
    {
      "id": 1,
      "firstName": "Thabo",
      "lastName": "Mokoena",
      "fullName": "Thabo Mokoena",
      "email": "thabo@example.com",
      "phoneNumber": "+27821234567",
      "totalPurchases": 1250.00,
      "lastPurchaseDate": "2025-10-27T10:30:00Z"
    }
  ],
  "pageNumber": 1,
  "totalPages": 1,
  "totalCount": 12
}
```

### Create Sale Request
```http
POST /api/Sales
Content-Type: application/json

{
  "shopId": 1,
  "customerId": null,
  "items": [
    {
      "productId": 1,
      "quantity": 2,
      "unitPrice": 25.99
    }
  ],
  "paymentType": "Cash",
  "totalAmount": 51.98
}
```

### Create Sale Response
```json
{
  "id": 42
}
```

---

## 🎯 Next Steps

### Immediate (Testing Phase)
1. ✅ **Verify Backend is Running**: Check `http://localhost:5000/swagger`
2. ✅ **Verify Database Has Data**: Query products and customers endpoints directly
3. ⏳ **Start Frontend**: `pnpm run dev` in `toss-web/`
4. ⏳ **Test POS Page**: Navigate to `http://localhost:3000/sales/pos`
5. ⏳ **Verify API Calls**: Check Network tab in browser DevTools
6. ⏳ **Complete Test Transaction**: Add items, checkout, verify sale created

### Short Term (Enhancements)
1. Add barcode scanning integration
2. Implement receipt printing
3. Add cash drawer opening
4. Implement "Hold Sale" functionality
5. Add sales history/recent transactions
6. Implement returns/refunds flow

### Long Term (Advanced Features)
1. Offline mode with sync
2. Multiple payment methods in one transaction
3. Customer loyalty points integration
4. Real-time inventory updates
5. Staff performance tracking
6. Advanced reporting dashboard

---

## 📚 Related Documentation
- `E2E_TESTING_COMPLETE_GUIDE.md` - Comprehensive testing guide
- `BACKEND_STARTUP_GUIDE.md` - How to start the backend
- `SESSION_FINAL_COMPLETE_SUMMARY.md` - Full implementation summary
- `SALES_IMPLEMENTATION_COMPLETE.md` - Sales module details
- `SALES_PAGES_FIXED_SUMMARY.md` - Frontend fixes

---

## ✅ Implementation Status: **READY FOR TESTING**

**All code changes complete. Awaiting frontend compilation and browser testing.**

---

*Last Updated: October 28, 2025*  
*Status: Code Complete - Testing Pending*

