# TOSS End-to-End Testing Complete Guide

## 🎯 Overview

This guide provides comprehensive instructions for testing the TOSS ERP system end-to-end, including backend API verification and frontend integration testing.

---

## ✅ Current System Status

### Backend Implementation - **COMPLETE**
- ✅ **Sales Module**: POS, Sales transactions, Sale items
- ✅ **Customer Orders Module**: Order lifecycle management (Create, Update Status, Cancel)
- ✅ **Shopping Cart Module**: Cart management, Checkout process
- ✅ **Inventory Module**: Products, Categories, Stock management, Low stock alerts
- ✅ **Buying Module**: Purchase orders, Vendor management
- ✅ **CRM Module**: Customer management, Search functionality
- ✅ **Payments Module**: M-Pesa, Airtel, MTN integrations
- ✅ **Stores Module**: Store management with SA townships
- ✅ **Logistics Module**: Driver management, Delivery tracking
- ✅ **Group Buying Module**: Pool creation, Participation management

### Frontend Implementation - **COMPLETE**
- ✅ **Sales Dashboard**: Overview, Quick actions
- ✅ **POS Page**: Product catalog, Cart, Checkout
- ✅ **Customer Orders**: Order creation and management
- ✅ **Stores Management**: CRUD operations, SA-specific features
- ✅ **CRM**: Customer search and management
- ✅ **API Composables**: useSalesAPI, useCustomerOrdersAPI, useCRMAPI, useStoresAPI, useProductsAPI

### Backend Startup Scripts - **COMPLETE**
- ✅ `start-backend.ps1`: Aspire Dashboard startup with auto-cleanup
- ✅ `start-web.ps1`: Web API only startup with auto-cleanup
- ✅ Automatic process detection and termination
- ✅ Port conflict resolution

---

## 🚀 Testing Prerequisites

### 1. Start Backend (Choose ONE option)

#### Option A: Using Startup Script (Recommended)
```powershell
# Navigate to Web directory
cd backend\Toss\src\Web

# Run startup script (automatically kills existing processes)
.\start-web.ps1
```

#### Option B: Manual Startup
```powershell
cd backend\Toss\src\Web
dotnet run
```

**Wait for:** `Now listening on: http://localhost:5000`

### 2. Start Frontend
```powershell
cd toss-web
pnpm run dev
```

**Wait for:** `Nuxt 4.0 ready in X ms`

### 3. Verify Services Running
```powershell
# Check backend
curl http://localhost:5000/api

# Check frontend
curl http://localhost:3000
```

---

## 🧪 Backend API Testing

### Test Suite 1: Inventory & Products

#### Test 1.1: Get Products
```powershell
$products = Invoke-RestMethod -Uri "http://localhost:5000/api/Inventory/products?shopId=1" -Method GET
Write-Host "✅ Found $($products.Count) products"
$products | Select-Object -First 3 | Format-Table id, name, price, sku
```

**Expected:** List of products with SA pricing (R prefix)

#### Test 1.2: Search Products
```powershell
$results = Invoke-RestMethod -Uri "http://localhost:5000/api/Inventory/search?searchTerm=milk&shopId=1" -Method GET
Write-Host "✅ Found $($results.products.Count) products matching 'milk'"
```

#### Test 1.3: Get Low Stock Items
```powershell
$lowStock = Invoke-RestMethod -Uri "http://localhost:5000/api/Inventory/low-stock?shopId=1&threshold=10" -Method GET
Write-Host "⚠️ $($lowStock.Count) items need restocking"
```

### Test Suite 2: Sales & POS

#### Test 2.1: Get Sales
```powershell
$sales = Invoke-RestMethod -Uri "http://localhost:5000/api/Sales?shopId=1" -Method GET
Write-Host "✅ Found $($sales.Count) sales transactions"
```

#### Test 2.2: Shopping Cart - Add Item
```powershell
$cartItem = @{
    productId = 1
    shopId = 1
    sessionId = "test-session-123"
    quantity = 2
    unitPrice = 25.50
    taxRate = 0.15
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/ShoppingCart/add" -Method POST -Body $cartItem -ContentType "application/json"
Write-Host "✅ Item added to cart"
```

#### Test 2.3: Get Cart
```powershell
$cart = Invoke-RestMethod -Uri "http://localhost:5000/api/ShoppingCart?sessionId=test-session-123&shopId=1" -Method GET
Write-Host "🛒 Cart has $($cart.items.Count) items, Total: R$($cart.total)"
```

#### Test 2.4: Checkout
```powershell
$checkout = @{
    sessionId = "test-session-123"
    shopId = 1
    customerId = 1
    paymentMethod = "Cash"
    amountPaid = 60.00
    notes = "Test sale"
} | ConvertTo-Json

$result = Invoke-RestMethod -Uri "http://localhost:5000/api/ShoppingCart/checkout" -Method POST -Body $checkout -ContentType "application/json"
Write-Host "✅ Sale completed! Sale ID: $($result.saleId)"
```

### Test Suite 3: Customer Orders

#### Test 3.1: Create Customer Order
```powershell
$order = @{
    customerId = 1
    shopId = 1
    orderItems = @(
        @{
            productId = 1
            quantity = 3
            unitPrice = 45.00
        }
    )
    shippingAddress = "123 Main St, Soweto, 1801"
    billingAddress = "123 Main St, Soweto, 1801"
    notes = "Test order"
} | ConvertTo-Json -Depth 5

$newOrder = Invoke-RestMethod -Uri "http://localhost:5000/api/CustomerOrders" -Method POST -Body $order -ContentType "application/json"
Write-Host "✅ Order created! Order ID: $($newOrder.id)"
```

#### Test 3.2: Get Customer Orders
```powershell
$orders = Invoke-RestMethod -Uri "http://localhost:5000/api/CustomerOrders?shopId=1" -Method GET
Write-Host "✅ Found $($orders.Count) customer orders"
$orders | Format-Table id, customerName, totalAmount, status
```

#### Test 3.3: Update Order Status
```powershell
$statusUpdate = @{
    orderId = 1
    newStatus = "Processing"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/CustomerOrders/1/status" -Method POST -Body $statusUpdate -ContentType "application/json"
Write-Host "✅ Order status updated"
```

### Test Suite 4: Stores Management

#### Test 4.1: Get Stores
```powershell
$stores = Invoke-RestMethod -Uri "http://localhost:5000/api/Stores" -Method GET
Write-Host "✅ Found $($stores.Count) stores"
$stores | Format-Table id, name, area, city
```

#### Test 4.2: Create Store
```powershell
$newStore = @{
    name = "Soweto Spaza Shop"
    area = "Soweto"
    city = "Johannesburg"
    stateProvince = "Gauteng"
    postalCode = "1801"
    country = "South Africa"
    phoneNumber = "+27 11 123 4567"
    email = "soweto@toss.co.za"
    isActive = $true
} | ConvertTo-Json

$store = Invoke-RestMethod -Uri "http://localhost:5000/api/Stores" -Method POST -Body $newStore -ContentType "application/json"
Write-Host "✅ Store created! ID: $($store.id)"
```

### Test Suite 5: CRM & Customers

#### Test 5.1: Search Customers
```powershell
$customers = Invoke-RestMethod -Uri "http://localhost:5000/api/CRM/customers/search?searchTerm=John" -Method GET
Write-Host "✅ Found $($customers.Count) customers"
```

---

## 🌐 Frontend Testing (Browser)

### Setup for Browser Testing
1. Open browser: `http://localhost:3000`
2. Open DevTools (F12)
3. Go to Network tab
4. Filter by "Fetch/XHR"

### Test Flow 1: POS Complete Transaction

**URL:** `http://localhost:3000/sales/pos`

**Steps:**
1. ✅ Page loads with product catalog
2. ✅ Search for product (e.g., "milk")
3. ✅ Add product to cart (click + button)
4. ✅ Verify cart updates (item count, total)
5. ✅ Click "Checkout"
6. ✅ Select payment method
7. ✅ Complete transaction
8. ✅ Verify sale appears in sales list

**Expected Network Calls:**
- `GET /api/Inventory/products?shopId=1`
- `POST /api/ShoppingCart/add`
- `GET /api/ShoppingCart?sessionId=...&shopId=1`
- `POST /api/ShoppingCart/checkout`

### Test Flow 2: Customer Order Management

**URL:** `http://localhost:3000/sales/orders`

**Steps:**
1. ✅ Page loads with orders list
2. ✅ Click "Create Order"
3. ✅ Select customer
4. ✅ Add products to order
5. ✅ Enter shipping address
6. ✅ Submit order
7. ✅ Verify order appears in list
8. ✅ Click order to view details
9. ✅ Update order status
10. ✅ Verify status change reflected

**Expected Network Calls:**
- `GET /api/CustomerOrders?shopId=1`
- `POST /api/CustomerOrders`
- `POST /api/CustomerOrders/{id}/status`

### Test Flow 3: Store Management

**URL:** `http://localhost:3000/stores`

**Steps:**
1. ✅ Page loads with stores grid
2. ✅ View store statistics
3. ✅ Click "Create Store"
4. ✅ Fill in store details (SA township)
5. ✅ Select GPS location
6. ✅ Set business hours
7. ✅ Save store
8. ✅ Verify store appears in list
9. ✅ Click store to edit
10. ✅ Update details and save

**Expected Network Calls:**
- `GET /api/Stores`
- `POST /api/Stores`
- `GET /api/Stores/{id}`
- `PUT /api/Stores/{id}`

---

## 🔍 Debugging Failed Tests

### Issue: "No API calls in Network tab"

**Causes:**
1. Backend not running
2. CORS misconfigured
3. Wrong API URL in frontend

**Solutions:**
```powershell
# 1. Verify backend is running
curl http://localhost:5000/api

# 2. Check CORS in browser console (should NOT see CORS errors)

# 3. Verify frontend config
cd toss-web
grep -r "localhost:5000" composables/
```

### Issue: "CORS Policy Error"

**Error:** `Access to fetch at 'http://localhost:5000/...' has been blocked by CORS policy`

**Solution:**
```csharp
// In backend/Toss/src/Infrastructure/DependencyInjection.cs
// Verify CORS policy includes correct ports:
policy.WithOrigins("http://localhost:3000", "https://localhost:3001")
```

### Issue: "401 Unauthorized"

**Cause:** Missing or invalid authentication token

**Solution:**
```typescript
// In frontend composables, verify token is being sent
const config = useRuntimeConfig()
const token = sessionStorage.getItem('token')
// Add to request headers
```

### Issue: "404 Not Found" on API endpoint

**Causes:**
1. Endpoint not registered
2. Wrong route in frontend

**Solution:**
```powershell
# Check Swagger for available endpoints
# Open: http://localhost:5000/swagger/index.html
# Verify endpoint exists and correct HTTP method
```

---

## 📊 Automated Testing Script

Save as `test-complete-flow.ps1`:

```powershell
# TOSS Complete E2E Test Script
Write-Host "🧪 TOSS End-to-End Test Suite" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5000/api"
$shopId = 1
$sessionId = "test-session-" + (Get-Date -Format "yyyyMMddHHmmss")
$testResults = @()

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Uri,
        [object]$Body = $null
    )
    
    try {
        $params = @{
            Uri = "$baseUrl$Uri"
            Method = $Method
            ContentType = "application/json"
            ErrorAction = "Stop"
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
        }
        
        $response = Invoke-RestMethod @params
        Write-Host "✅ $Name" -ForegroundColor Green
        $script:testResults += @{ Test = $Name; Result = "PASS" }
        return $response
    }
    catch {
        Write-Host "❌ $Name - $($_.Exception.Message)" -ForegroundColor Red
        $script:testResults += @{ Test = $Name; Result = "FAIL"; Error = $_.Exception.Message }
        return $null
    }
}

Write-Host "🔍 Test Suite 1: Inventory" -ForegroundColor Yellow
$products = Test-Endpoint "Get Products" "GET" "/Inventory/products?shopId=$shopId"
Test-Endpoint "Search Products" "GET" "/Inventory/search?searchTerm=milk&shopId=$shopId"
Test-Endpoint "Get Low Stock" "GET" "/Inventory/low-stock?shopId=$shopId&threshold=10"

Write-Host ""
Write-Host "🛒 Test Suite 2: Shopping Cart" -ForegroundColor Yellow
if ($products -and $products.Count -gt 0) {
    $productId = $products[0].id
    $cartItem = @{
        productId = $productId
        shopId = $shopId
        sessionId = $sessionId
        quantity = 2
        unitPrice = 25.50
        taxRate = 0.15
    }
    Test-Endpoint "Add to Cart" "POST" "/ShoppingCart/add" $cartItem
    $cart = Test-Endpoint "Get Cart" "GET" "/ShoppingCart?sessionId=$sessionId&shopId=$shopId"
    
    if ($cart) {
        $checkout = @{
            sessionId = $sessionId
            shopId = $shopId
            customerId = 1
            paymentMethod = "Cash"
            amountPaid = 60.00
            notes = "Automated test sale"
        }
        Test-Endpoint "Checkout" "POST" "/ShoppingCart/checkout" $checkout
    }
}

Write-Host ""
Write-Host "📦 Test Suite 3: Customer Orders" -ForegroundColor Yellow
$orders = Test-Endpoint "Get Orders" "GET" "/CustomerOrders?shopId=$shopId"
if ($products -and $products.Count -gt 0) {
    $order = @{
        customerId = 1
        shopId = $shopId
        orderItems = @(
            @{
                productId = $products[0].id
                quantity = 3
                unitPrice = 45.00
            }
        )
        shippingAddress = "123 Test St, Soweto, 1801"
        billingAddress = "123 Test St, Soweto, 1801"
        notes = "Automated test order"
    }
    $newOrder = Test-Endpoint "Create Order" "POST" "/CustomerOrders" $order
    
    if ($newOrder) {
        $statusUpdate = @{
            orderId = $newOrder.id
            newStatus = "Processing"
        }
        Test-Endpoint "Update Order Status" "POST" "/CustomerOrders/$($newOrder.id)/status" $statusUpdate
    }
}

Write-Host ""
Write-Host "🏪 Test Suite 4: Stores" -ForegroundColor Yellow
Test-Endpoint "Get Stores" "GET" "/Stores"

Write-Host ""
Write-Host "👥 Test Suite 5: CRM" -ForegroundColor Yellow
Test-Endpoint "Search Customers" "GET" "/CRM/customers/search?searchTerm=test"

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "📊 Test Results Summary" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

$passed = ($testResults | Where-Object { $_.Result -eq "PASS" }).Count
$failed = ($testResults | Where-Object { $_.Result -eq "FAIL" }).Count
$total = $testResults.Count

Write-Host "Total Tests: $total" -ForegroundColor White
Write-Host "Passed: $passed" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor $(if ($failed -gt 0) { "Red" } else { "Green" })
Write-Host ""

if ($failed -gt 0) {
    Write-Host "❌ Failed Tests:" -ForegroundColor Red
    $testResults | Where-Object { $_.Result -eq "FAIL" } | ForEach-Object {
        Write-Host "  • $($_.Test): $($_.Error)" -ForegroundColor Gray
    }
}

Write-Host ""
if ($failed -eq 0) {
    Write-Host "🎉 All tests passed!" -ForegroundColor Green
} else {
    Write-Host "⚠️ Some tests failed. Review errors above." -ForegroundColor Yellow
}
```

**Run the script:**
```powershell
.\test-complete-flow.ps1
```

---

## 📝 Manual Test Checklist

Print this checklist and check off as you test:

### Backend API Tests
- [ ] Products list loads (GET /api/Inventory/products)
- [ ] Product search works (GET /api/Inventory/search)
- [ ] Low stock alerts work (GET /api/Inventory/low-stock)
- [ ] Add item to cart (POST /api/ShoppingCart/add)
- [ ] View cart (GET /api/ShoppingCart)
- [ ] Checkout completes (POST /api/ShoppingCart/checkout)
- [ ] Sales list loads (GET /api/Sales)
- [ ] Customer orders list (GET /api/CustomerOrders)
- [ ] Create customer order (POST /api/CustomerOrders)
- [ ] Update order status (POST /api/CustomerOrders/{id}/status)
- [ ] Stores list loads (GET /api/Stores)
- [ ] Customer search works (GET /api/CRM/customers/search)

### Frontend Tests
- [ ] Sales dashboard loads
- [ ] POS page loads with products
- [ ] Can add products to cart
- [ ] Cart updates correctly
- [ ] Checkout flow completes
- [ ] Orders page loads
- [ ] Can create new order
- [ ] Can update order status
- [ ] Stores page loads
- [ ] Can create new store
- [ ] Navigation works (sidebar links)

### Integration Tests
- [ ] Frontend calls backend APIs (check Network tab)
- [ ] No CORS errors in console
- [ ] Authentication works (if implemented)
- [ ] Error messages display correctly
- [ ] Loading states show during API calls
- [ ] Success messages display after operations

---

## 🎯 Success Criteria

Your TOSS system is ready for production when:

1. ✅ **All backend endpoints return 200 OK**
2. ✅ **All frontend pages load without errors**
3. ✅ **API calls visible in Network tab**
4. ✅ **No CORS errors in browser console**
5. ✅ **Complete POS transaction flow works**
6. ✅ **Complete customer order flow works**
7. ✅ **Store management CRUD works**
8. ✅ **Data persists in PostgreSQL database**

---

## 📚 Additional Resources

- **Backend API Docs**: `http://localhost:5000/swagger/index.html`
- **Startup Scripts Guide**: `BACKEND_STARTUP_GUIDE.md`
- **Sales Implementation**: `SALES_IMPLEMENTATION_COMPLETE.md`
- **Customer Orders**: `SALES_PAGES_FIXED_SUMMARY.md`

---

## 🆘 Getting Help

If tests fail:
1. Check backend is running: `curl http://localhost:5000/api`
2. Check frontend is running: `curl http://localhost:3000`
3. Review browser console for errors (F12)
4. Check Network tab for failed requests
5. Review backend logs for exceptions
6. Verify PostgreSQL is running and accessible

**Still stuck?** Review the troubleshooting sections in each module's documentation.

