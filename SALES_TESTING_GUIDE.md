# Sales Pages Testing Guide

## System Status: ✅ READY FOR TESTING

### Backend Status
- ✅ Compiled successfully (all 5 projects)
- ✅ Running in background
- ✅ New Customer Orders endpoints added
- ✅ CORS configured for `http://localhost:3000` and `https://localhost:3001`
- 🌐 **Backend URL**: `http://localhost:5000` or `https://localhost:5001`
- 📚 **Swagger UI**: `http://localhost:5000/swagger/index.html`

### Frontend Status
- ✅ Running on port 3000
- ✅ API composables updated
- ✅ New composables created (useCRMAPI, useCustomerOrdersAPI)
- 🌐 **Frontend URL**: `http://localhost:3000`

---

## New API Endpoints to Test

### Customer Orders Endpoints
```
POST   /api/CustomerOrders              - Create customer order
GET    /api/CustomerOrders              - List customer orders (with filters)
POST   /api/CustomerOrders/{id}/status  - Update order status
POST   /api/CustomerOrders/{id}/cancel  - Cancel order
```

### Enhanced Sales Endpoints
```
POST   /api/Sales/status/{id}           - Update sale status
POST   /api/Sales/refund/{id}           - Process refund
```

### Enhanced Buying Endpoints
```
POST   /api/Buying/status/{id}          - Update PO status
POST   /api/Buying/receive/{id}         - Receive goods
```

---

## Testing Checklist

### 1. Swagger API Testing (Recommended First)
Open: `http://localhost:5000/swagger/index.html`

**Test in this order:**
- [ ] GET `/api/Stores` - Verify stores are loading
- [ ] GET `/api/CRM/customers` - Verify customers exist
- [ ] GET `/api/Inventory/products` - Verify products exist
- [ ] POST `/api/CustomerOrders` - Create a test order
  ```json
  {
    "shopId": 1,
    "customerId": 1,
    "orderItems": [
      {
        "productId": 1,
        "quantity": 2,
        "unitPrice": 100
      }
    ],
    "shippingAddress": "123 Test St, Johannesburg",
    "billingAddress": "123 Test St, Johannesburg"
  }
  ```
- [ ] GET `/api/CustomerOrders` - Verify order appears
- [ ] POST `/api/CustomerOrders/{id}/status` - Update order to "Processing"
- [ ] POST `/api/CustomerOrders/{id}/cancel` - Test cancellation

### 2. Sales Pages Testing

#### A. Sales Dashboard (`/sales`)
**URL**: `http://localhost:3000/sales`

**Expected Features:**
- Sales overview cards (today, week, month)
- Recent transactions table
- Quick actions (New Sale, View Orders, Reports)

**Tests:**
- [ ] Page loads without errors
- [ ] Dashboard displays stats
- [ ] Navigation buttons work

#### B. Point of Sale (`/sales/pos`)
**URL**: `http://localhost:3000/sales/pos`

**Expected Features:**
- Product grid with search
- Shopping cart
- Customer selection
- Payment processing
- Real-time stock updates

**Critical Tests:**
- [ ] Products load from API (not mock data)
- [ ] Can add products to cart
- [ ] Can select customer
- [ ] Can complete sale
- [ ] Cart clears after checkout

**Verify API Calls (Browser DevTools → Network tab):**
```
✅ GET /api/Inventory/products
✅ GET /api/CRM/customers  
✅ POST /api/ShoppingCart/checkout
✅ POST /api/Sales
```

#### C. Customer Orders (`/sales/orders`)
**URL**: `http://localhost:3000/sales/orders`

**Expected Features:**
- Order list with filters
- Create new order button
- Order status badges
- Action buttons (View, Update Status, Cancel)

**Tests:**
- [ ] Orders load from new CustomerOrders API
- [ ] Can filter by status
- [ ] Can create new order
- [ ] Can update order status
- [ ] Can cancel order

**Verify API Calls:**
```
✅ GET /api/CustomerOrders
✅ POST /api/CustomerOrders
✅ POST /api/CustomerOrders/{id}/status
✅ POST /api/CustomerOrders/{id}/cancel
```

#### D. Invoices (`/sales/invoices`)
**URL**: `http://localhost:3000/sales/invoices`

**Expected Features:**
- Invoice list
- Invoice generation
- PDF export
- Payment tracking

**Tests:**
- [ ] Invoices load (mapping to Sales)
- [ ] Can view invoice details
- [ ] Status displays correctly

---

## Common Issues & Solutions

### Issue: "Failed to load data"
**Cause**: Backend not running or CORS issue
**Solution**:
1. Check backend is running: `Get-Process -Name "dotnet"`
2. Verify CORS in DevTools console
3. Restart backend if needed

### Issue: "Products not loading in POS"
**Cause**: API endpoint mismatch
**Solution**:
1. Open DevTools → Network tab
2. Verify GET request to `/api/Inventory/products`
3. Check response status and data

### Issue: "Cart checkout fails"
**Cause**: Missing required fields
**Solution**:
1. Ensure customer is selected
2. Verify shop ID is set
3. Check payment method is valid

---

## API Composable Architecture

### useSalesAPI (Unified Facade)
```typescript
// Core sales methods
createSale()
getSales()
getSaleById()
updateSaleStatus()
processRefund()

// Proxy methods (delegate to specialized composables)
getProducts() → useProductsAPI
getCustomers() → useCRMAPI
getOrders() → useCustomerOrdersAPI
createOrder() → useCustomerOrdersAPI
```

### Specialized Composables
- **useCRMAPI**: Customer management
- **useCustomerOrdersAPI**: Order lifecycle
- **useProductsAPI**: Product lookups
- **useShoppingCartAPI**: Cart operations

---

## Testing Flow Example

### Complete POS Transaction Flow
1. **Navigate to POS**: `http://localhost:3000/sales/pos`
2. **Select Customer**: Click "Select Customer" → Choose from list
3. **Add Products**: 
   - Search for product
   - Click "Add to Cart"
   - Adjust quantity if needed
4. **Review Cart**: Verify totals calculate correctly
5. **Process Payment**:
   - Select payment method (Cash, Card, M-Pesa)
   - Click "Complete Sale"
6. **Verify Sale**: 
   - Check success message
   - Cart should clear
   - New sale should appear in `/sales`

### Complete Customer Order Flow
1. **Navigate to Orders**: `http://localhost:3000/sales/orders`
2. **Create Order**:
   - Click "Create Order"
   - Fill in customer details
   - Add order items
   - Submit order
3. **Update Status**:
   - Find the new order
   - Click "Update Status"
   - Change to "Processing"
4. **Complete Order**:
   - Update status to "Complete"
   - Verify payment status updates

---

## Debug Tools

### Browser DevTools
- **Console**: Check for errors
- **Network**: Monitor API calls
- **Application**: Inspect localStorage/sessionStorage

### Backend Logs
Monitor backend terminal for:
- Database queries
- API endpoint hits
- Error messages

### Swagger UI
Test endpoints directly:
- No frontend required
- See exact request/response
- Test error scenarios

---

## Success Criteria

✅ **All tests pass when:**
1. POS loads products from backend (not mock data)
2. Can complete a sale successfully
3. Customer Orders page loads and creates orders
4. Can update order status
5. Can cancel orders
6. All API calls return 200 status codes
7. No CORS errors in console
8. Data persists across page refreshes

---

## Next Steps After Testing

1. **Report Issues**: Document any failures with screenshots
2. **E2E Tests**: Run Playwright tests for automated verification
3. **Performance**: Monitor response times
4. **Mobile Testing**: Test on mobile viewport
5. **Production Prep**: Environment variable configuration

---

## Quick Commands

```powershell
# Check backend status
Get-Process -Name "dotnet"

# Restart backend
cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss\src\Web
dotnet run --no-build

# Check frontend status
# (Already running on port 3000)

# Open Swagger
Start-Process "http://localhost:5000/swagger/index.html"

# Open Frontend
Start-Process "http://localhost:3000/sales"
```

---

## Contact/Support

If you encounter issues:
1. Check this guide first
2. Review browser console errors
3. Check backend logs
4. Verify database has seed data
5. Restart backend if needed

**Happy Testing! 🚀**

