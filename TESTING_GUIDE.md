# TOSS MVP Testing Guide

## Prerequisites

1. **Database:** PostgreSQL must be running
2. **Backend:** Run `dotnet run --project src/Web` from `backend/Toss`
3. **Frontend:** Run `npm run dev` from `toss-web`
4. **Migration:** Create OnboardingStatus migration:
   ```bash
   cd backend/Toss
   dotnet ef migrations add AddOnboardingStatus --project src/Infrastructure --startup-project src/Web
   ```

## Test User Accounts

Based on the seed data, you can use:
- **Admin:** `admin@toss.local` / `Admin1!`
- **Retailer:** `storeowner1@toss.local` / `StoreOwner1!`
- **Supplier:** (Create via admin or registration)
- **Driver:** (Create via admin or registration)

## End-to-End Test Flows

### 1. Retailer Flow ✅

1. **Login as Retailer**
   - URL: `/login`
   - Email: `storeowner1@toss.local`
   - Password: `StoreOwner1!`

2. **Onboarding (First Time)**
   - Should redirect to `/retailer/onboarding`
   - Complete Step 1: Business profile
   - Complete Step 2: Add at least one product
   - Complete Step 3: Skip staff invitation
   - Should redirect to `/retailer/dashboard`

3. **Add Products**
   - Navigate to `/retailer/products`
   - Click "+ Add Product"
   - Fill in: Name, SKU, Price, Category
   - Save

4. **Make a Sale (POS)**
   - Navigate to `/sales/pos`
   - Search/select products
   - Add to cart
   - Select payment method (Cash/Card)
   - Click "Process Payment"
   - Verify: Sale created, inventory reduced

5. **Create Purchase Order**
   - Navigate to `/retailer/orders`
   - Click "+ New Purchase Order"
   - Select supplier
   - Add products with quantities
   - Create order
   - Submit order (if Draft)

6. **View Inventory**
   - Navigate to `/retailer/inventory`
   - Verify stock levels updated after sale
   - Check low-stock indicators

### 2. Supplier Flow ✅

1. **Login as Supplier**
   - (Create supplier account via admin first)

2. **Onboarding**
   - Complete business profile
   - Select product categories

3. **View Incoming Orders**
   - Navigate to `/supplier/orders`
   - See orders with "Submitted" status

4. **Accept Order**
   - Click on an order
   - Click "Accept Order"
   - Verify status changes to "Accepted"

5. **Mark as Shipped**
   - After accepting, click "Mark as Shipped"
   - Verify status updates

### 3. Driver Flow ✅

1. **Login as Driver**
   - (Create driver account via admin first)

2. **Onboarding**
   - Complete profile: Name, phone, vehicle type, registration, area

3. **View Deliveries**
   - Navigate to `/driver/deliveries`
   - See assigned deliveries

4. **Update Delivery Status**
   - Click on a delivery
   - Click "Mark as Picked Up"
   - Add delivery notes
   - Click "Mark as Delivered"
   - Verify status updates

### 4. Admin Flow ✅

1. **Login as Admin**
   - URL: `/login`
   - Email: `admin@toss.local`
   - Password: `Admin1!`

2. **View Dashboard**
   - Navigate to `/admin/dashboard`
   - Verify stats: retailers, suppliers, drivers, sales

3. **Manage Users**
   - Navigate to `/admin/users`
   - Search/filter by role
   - Activate/deactivate users
   - Edit user roles

4. **View All Orders**
   - Navigate to `/admin/orders`
   - Filter by status
   - View order details

## API Testing (Postman/Thunder Client)

### Authentication
```
POST /api/auth/login
Body: { "email": "admin@toss.local", "password": "Admin1!" }
Response: { "token": "...", "user": {...} }
```

### Products
```
GET /api/Inventory/products?ShopId=1
POST /api/Inventory/products
PUT /api/Inventory/products/{id}
DELETE /api/Inventory/products/{id}
```

### Sales (POS)
```
POST /api/Sales
Body: {
  "shopId": 1,
  "items": [...],
  "paymentMethod": "Cash",
  "saleType": 0
}
```

### Purchase Orders
```
GET /api/Buying/purchase-orders?shopId=1
POST /api/Buying/purchase-orders
GET /api/Buying/purchase-orders/{id}
POST /api/Buying/purchase-orders/{id}/approve
POST /api/Buying/purchase-orders/{id}/status
```

### Onboarding
```
GET /api/onboarding/{userId}?role=Retailer
PUT /api/onboarding/{userId}
POST /api/onboarding/{userId}/complete?role=Retailer
```

## Verification Checklist

- [ ] Retailer can complete onboarding
- [ ] Retailer can add products
- [ ] Retailer can make POS sale
- [ ] Inventory decreases after POS sale
- [ ] Retailer can create purchase order
- [ ] Supplier can see incoming orders
- [ ] Supplier can accept/reject orders
- [ ] Supplier can mark order as shipped
- [ ] Driver can see assigned deliveries
- [ ] Driver can update delivery status
- [ ] Admin can view dashboard stats
- [ ] Admin can manage users (activate/deactivate, roles)
- [ ] Admin can view all orders
- [ ] Offline POS queue syncs when online
- [ ] Role-based route protection works
- [ ] Onboarding gates main UI until completed

## Common Issues & Solutions

### Issue: "OnboardingStatuses not found"
**Solution:** Create and run migration:
```bash
dotnet ef migrations add AddOnboardingStatus --project src/Infrastructure --startup-project src/Web
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

### Issue: "JWT token invalid"
**Solution:** Check `appsettings.json` has JWT configuration, ensure token is being sent in Authorization header

### Issue: "Product not found in POS"
**Solution:** Ensure products are created with correct shopId, check API response format

### Issue: "Onboarding redirect loop"
**Solution:** Check onboarding status API returns correct format, verify role parameter is passed

### Issue: "Inventory not updating"
**Solution:** Verify StockLevel entity exists for product, check StockMovement records are created

## Performance Notes

- Product list loads all products (pagination TODO for large catalogs)
- Order lists show up to 50 items (configurable via API)
- POS offline queue syncs every 30 seconds when online
- Dashboard stats load on page mount (consider caching for production)

## Security Notes

- All endpoints require authentication (except login/register)
- Role-based authorization enforced on backend and frontend
- JWT tokens expire after 60 minutes (configurable)
- User activation/deactivation prevents login

