# 🧪 POS Browser Testing Instructions

## ✅ System Status

**Frontend:** ✅ RUNNING on http://localhost:3001  
**Backend:** ⚠️ Running but Inventory API returning 500 error  
**POS Page:** ✅ Accessible at http://localhost:3001/sales/pos

---

## 🎯 Testing Instructions

### Step 1: Open the POS Page
Navigate to: **http://localhost:3001/sales/pos**

### Step 2: What to Test

#### A. Initial Page Load ✅
- [ ] Page renders without errors
- [ ] Navigation sidebar is visible
- [ ] POS interface displays (product grid, cart, payment section)
- [ ] Modern glass-morphism design is applied

#### B. Data Loading (May Show Errors) ⚠️
- [ ] Check browser console (F12) for API calls
- [ ] Look for:
  - `GET http://localhost:5000/api/Inventory/products?shopId=1`
  - `GET http://localhost:5000/api/CRM/customers?pageNumber=1&pageSize=100`
- [ ] If 500 errors appear, the backend database needs migration

#### C. Product Grid
- [ ] Should show products (if API successful)
- [ ] Each product card shows:
  - Product name
  - SKU
  - Price
  - Stock level
  - Category badge
- [ ] Click on product adds to cart

#### D. Shopping Cart
- [ ] Cart section on right side
- [ ] Add items to cart by clicking products
- [ ] Quantity can be increased/decreased
- [ ] Remove button works
- [ ] Subtotal, tax, total calculate correctly
- [ ] "Clear Cart" button works

#### E. Customer Selection
- [ ] Customer dropdown/search field visible
- [ ] Can search and select customers
- [ ] "Walk-in Customer" is default

#### F. Payment Processing
- [ ] Select payment method (Cash, Card, Mobile)
- [ ] "Process Payment" button enables when cart has items
- [ ] Click "Process Payment"
- [ ] Should call `POST http://localhost:5000/api/Sales` with sale data
- [ ] Success notification appears
- [ ] Cart clears after successful payment
- [ ] Receipt can be generated

#### G. Barcode Scanner
- [ ] Scanner icon visible in product search
- [ ] Clicking opens scanner modal
- [ ] (Optional) Test with webcam if available

#### H. Voice Commands (Optional)
- [ ] Voice input button visible
- [ ] Click to activate
- [ ] Speak commands like "add milk" or "clear cart"

---

## 🐛 Expected Issues (Backend Not Fully Running)

### 1. Products Not Loading
**Error in Console:**
```
Failed to load products: FetchError: [GET] "http://localhost:5000/api/Inventory/products?shopId=1": 500 Internal Server Error
```

**Cause:** Database tables don't exist or backend not migrated

**Fix:**
```powershell
cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss\src\Infrastructure
dotnet ef database update --startup-project ../Web/Web.csproj
```

### 2. Customers Not Loading
**Error in Console:**
```
Failed to load customers: FetchError: [GET] "http://localhost:5000/api/CRM/customers?pageNumber=1&pageSize=100": 500 Internal Server Error
```

**Same Cause & Fix as above**

### 3. CORS Errors
**Error:**
```
Access to fetch at 'http://localhost:5000/api/...' from origin 'http://localhost:3001' has been blocked by CORS policy
```

**Status:** ✅ Should be FIXED - CORS is configured for localhost:3001

---

## 📊 What You're Testing

### Frontend Changes (Just Implemented)
1. ✅ **Shop ID Management**: POS now uses `shopId = 1` for all API calls
2. ✅ **Real API Calls**: Removed all mock data
3. ✅ **Data Transformation**: Backend response → POS format
4. ✅ **Proper Error Handling**: Shows notifications for success/failure
5. ✅ **Correct API Method**: Uses `createSale()` not `createOrder()`
6. ✅ **Complete Sale Data**: Sends all required fields to backend

### Backend Integration Points
1. **GET /api/Inventory/products?shopId=1** - Load products
2. **GET /api/CRM/customers** - Load customers
3. **POST /api/Sales** - Create sale transaction
4. **GET /api/Sales/daily-summary** - Dashboard stats

---

## ✅ Success Criteria

**Minimal (Frontend Only):**
- [ ] Page loads without crashes
- [ ] UI is responsive and looks good
- [ ] Can add items to cart
- [ ] Cart calculations work
- [ ] Payment button is clickable

**Full Success (With Backend):**
- [ ] Products load from database
- [ ] Customers load from database
- [ ] Can complete a sale transaction
- [ ] Sale is saved to database
- [ ] Receipt can be generated
- [ ] Cart clears after successful payment

---

## 🚀 Quick Start Commands

### Start Frontend (if not running):
```powershell
cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\toss-web
pnpm run dev
```

### Start Backend (if needed):
```powershell
cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss\src\Web
.\start-web.ps1
```

### Fix Database Issues:
```powershell
cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss\src\Infrastructure
dotnet ef database update --startup-project ../Web/Web.csproj
```

---

## 📝 Testing Checklist

Copy this into your testing session:

```markdown
### POS Testing - Session Date: __________

#### Visual/UI ✓
- [ ] Page loads
- [ ] Glass-morphism design
- [ ] Responsive layout
- [ ] Navigation works

#### Products ✓
- [ ] Products display
- [ ] Can add to cart
- [ ] Stock levels shown
- [ ] Categories visible

#### Cart ✓
- [ ] Items appear in cart
- [ ] Quantities adjust
- [ ] Remove works
- [ ] Totals calculate

#### Payment ✓
- [ ] Payment methods selectable
- [ ] Process button works
- [ ] Sale creates successfully
- [ ] Cart clears after payment

#### Console (F12) ✓
- [ ] Check for API calls
- [ ] Note any errors
- [ ] Verify request/response data
```

---

## 🆘 Troubleshooting

### Frontend Won't Start
```powershell
cd toss-web
Get-Process -Name "node" | Stop-Process -Force
pnpm run dev
```

### Backend 500 Errors
1. Check if database is running (Docker)
2. Run migrations: `dotnet ef database update`
3. Check connection string in appsettings.json
4. Restart backend: `.\start-web.ps1`

### Port Already in Use
```powershell
# Kill process on port 3001
netstat -ano | Select-String ":3001"
# Find PID, then:
Stop-Process -Id <PID> -Force
```

---

## 📞 Report Back

After testing, please report:
1. ✅ What worked
2. ❌ What failed
3. 📋 Console errors (if any)
4. 🎯 Next steps needed

