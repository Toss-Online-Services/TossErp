# TOSS MVP - Backend Complete, Frontend Integration Guide

## ✅ BACKEND - 100% COMPLETE

### What We Built
All backend API endpoints for TOSS MVP are implemented, tested for compilation, and ready for use:

#### 1. Shopping Cart System (`/api/ShoppingCart`)
- `POST /add` - Add product to cart
- `PUT /update` - Update cart item quantity
- `POST /get` - Retrieve cart contents
- `POST /checkout` - Process checkout & create sale
- `DELETE /clear` - Clear cart

**Commands**: `AddToCartCommand`, `UpdateCartItemCommand`, `CheckoutCommand`  
**Queries**: `GetCartQuery`  
**Database**: `ShoppingCartItem` table migrated ✅

#### 2. Enhanced Sales Management (`/api/Sales`)
- `POST /{id}/status` - Update sale status (Pending→Completed→Refunded)
- `POST /{id}/refund` - Process refunds with optional inventory restocking

**Commands**: `UpdateSaleStatusCommand`, `ProcessRefundCommand`

#### 3. Enhanced Purchase Orders (`/api/Buying`)
- `POST /purchase-orders/{id}/status` - Update PO status transitions
- `POST /purchase-orders/{id}/receive` - Receive goods with auto stock movements

**Commands**: `UpdatePurchaseOrderStatusCommand`, `ReceiveGoodsCommand`

#### 4. Product Search & Inventory (`/api/Inventory`)
- `POST /search` - Advanced search (name, SKU, barcode, category, stock filters)
- `GET /low-stock` - Reorder alerts and suggestions

**Queries**: `SearchProductsQuery`, `GetLowStockItemsQuery`

#### 5. Group Buying (`/api/GroupBuying`)
All 8 endpoints already complete:
- Pool management (create, join, confirm)
- Participation tracking
- Geographic pooling
- Aggregated PO generation

---

## 🔧 FRONTEND - NEEDS WIRING

### API Composables (Already Created) ✅
Located in `toss-web/composables/`:
- ✅ `useShoppingCartAPI.ts` - All 5 methods (add, update, get, checkout, clear)
- ✅ `useProductsAPI.ts` - Search, lookup by ID/SKU/barcode, categories, low stock
- ✅ `useStoresAPI.ts` - Store CRUD
- ⚠️ `useSalesAPI.ts` - Exists but needs new methods (updateStatus, processRefund)
- ⚠️ `useBuyingAPI.ts` - May need new methods (receiveGoods, updateStatus)

### Pages Requiring Integration

#### Priority 1: POS Page (`pages/sales/pos.vue`)
**Line**: ~945 lines  
**Current State**: Uses mock data and old `salesAPI.createOrder()`  

**Required Changes** (estimated 60-80 lines):

1. **Add Imports** (line ~397):
```typescript
import { useShoppingCartAPI } from '~/composables/useShoppingCartAPI'
import { useProductsAPI } from '~/composables/useProductsAPI'
```

2. **Initialize APIs** (line ~400):
```typescript
const cartAPI = useShoppingCartAPI()
const productsAPI = useProductsAPI()
const sessionId = ref(crypto.randomUUID())  // Cart session
const shopId = ref(1)  // From user context/auth
```

3. **Load Products** (line ~448, replace mock data):
```typescript
const loadProducts = async () => {
  try {
    const result = await productsAPI.searchProducts({
      shopId: shopId.value,
      inStock: true,
      pageSize: 100
    })
    products.value = result.items.map(p => ({
      id: p.id,
      name: p.name,
      sku: p.sku,
      price: p.basePrice,
      stock: p.availableStock,
      category: p.categoryId,
      image: p.imageUrl
    }))
  } catch (error) {
    console.error('Failed to load products:', error)
    showNotification('Failed to load products', 'error')
  }
}

// Call in onMounted
onMounted(async () => {
  await loadProducts()
  await loadCart()  // Load existing cart if any
  // ... rest of initialization
})
```

4. **Load Cart on Mount** (new function):
```typescript
const loadCart = async () => {
  try {
    const result = await cartAPI.getCart(sessionId.value, shopId.value)
    cartItems.value = result.items.map(item => ({
      id: item.productId,
      name: item.productName,
      sku: item.productSku,
      price: item.unitPrice,
      quantity: item.quantity
    }))
  } catch (error) {
    // Cart might be empty, that's okay
    console.log('No existing cart')
  }
}
```

5. **Update addToCart** (line ~496):
```typescript
const addToCart = async (product: any) => {
  if (product.stock === 0) return
  
  try {
    await cartAPI.addToCart(
      shopId.value,
      product.id,
      1,
      sessionId.value
    )
    
    // Update local cart
    const existingItem = cartItems.value.find(item => item.id === product.id)
    if (existingItem) {
      existingItem.quantity += 1
    } else {
      cartItems.value.push({
        id: product.id,
        name: product.name,
        sku: product.sku,
        price: product.price,
        quantity: 1
      })
    }
    
    showNotification(`${product.name} added to cart`)
  } catch (error) {
    console.error('Failed to add to cart:', error)
    showNotification('Failed to add item', 'error')
  }
}
```

6. **Update processPayment** (line ~691):
```typescript
const processPayment = async () => {
  if (cartItems.value.length === 0) return
  
  try {
    if (selectedPaymentMethod.value === 'card' && hardwareStatus.value.cardReader) {
      showNotification('Processing card payment...')
      await new Promise(resolve => setTimeout(resolve, 2000))
    }

    // Use shopping cart checkout API
    const result = await cartAPI.checkout(
      sessionId.value,
      shopId.value,
      selectedPaymentMethod.value,
      cartTotal.value,
      selectedCustomer.value || undefined,
      `POS Sale - ${new Date().toISOString()}`
    )

    // Show success
    showSuccessModal.value = true
    lastSaleTotal.value = result.total
    lastSaleChange.value = result.change
    
    // Clear local cart
    cartItems.value = []
    
    // Generate new session for next sale
    sessionId.value = crypto.randomUUID()
    
    showNotification('Sale completed successfully')
  } catch (error) {
    console.error('Payment processing failed:', error)
    showNotification('Payment failed', 'error')
  }
}
```

7. **Update updateQuantity** (line ~516):
```typescript
const updateQuantity = async (productId: number, newQuantity: number) => {
  const item = cartItems.value.find(i => i.id === productId)
  if (!item) return
  
  try {
    // Find cart item ID (would need to store this from addToCart response)
    // For now, update locally and sync with server
    if (newQuantity <= 0) {
      await removeFromCart(productId)
    } else {
      item.quantity = newQuantity
      // Optionally call cartAPI.updateCartItem() here
    }
  } catch (error) {
    console.error('Failed to update quantity:', error)
  }
}
```

#### Priority 2: Sales Pages (`pages/sales/*.vue`)
**Files**: `index.vue`, `[id].vue`  
**Required**: Wire up status updates, refund processing

**Update `useSalesAPI.ts`** first:
```typescript
async updateSaleStatus(id: number, newStatus: string, notes?: string) {
  return await $fetch(`${baseURL}/Sales/${id}/status`, {
    method: 'POST',
    body: { newStatus, notes }
  })
},

async processRefund(id: number, refundAmount: number, reason: string, restockItems: boolean = true) {
  return await $fetch(`${baseURL}/Sales/${id}/refund`, {
    method: 'POST',
    body: { refundAmount, reason, restockItems }
  })
}
```

#### Priority 3: Buying Pages (`pages/buying/*.vue`)
**Required**: Wire up goods receipt, status updates

**Update `useBuyingAPI.ts`** (if not exists, create it):
```typescript
async receiveGoods(poId: number, items: Array<{productId: number, quantityReceived: number}>, notes?: string) {
  return await $fetch(`${baseURL}/Buying/purchase-orders/${poId}/receive`, {
    method: 'POST',
    body: { items, notes }
  })
},

async updatePOStatus(poId: number, newStatus: string, notes?: string) {
  return await $fetch(`${baseURL}/Buying/purchase-orders/${poId}/status`, {
    method: 'POST',
    body: { newStatus, notes }
  })
}
```

#### Priority 4-7: Other Pages
Follow similar pattern for:
- `pages/stock/*.vue` - Use `useProductsAPI()`
- `pages/users/*.vue` - Check if API exists
- `pages/logistics/*.vue` - Check if API exists
- `pages/buying/group-buying/*.vue` - Use Group Buying API

---

## 🎯 Step-by-Step Action Plan

### Step 1: Verify Backend Running
```bash
# Should already be running in background
# Test: https://localhost:5001/api
```

### Step 2: Update POS Page (45 minutes)
1. Open `toss-web/pages/sales/pos.vue`
2. Apply changes listed above
3. Test:
   - Products load from API
   - Add to cart works
   - Checkout completes
   - Sale created in database

### Step 3: Update useSalesAPI (10 minutes)
Add `updateSaleStatus` and `processRefund` methods

### Step 4: Update Sales Pages (30 minutes)
Wire up status and refund buttons

### Step 5: Update Buying Pages (30 minutes)
Wire up receive goods and status updates

### Step 6: Create E2E Tests (60 minutes)
Test complete POS workflow end-to-end

---

## ✅ Success Checklist

Backend:
- [x] All endpoints implemented
- [x] Migrations applied
- [x] Build succeeds
- [x] Backend running

Frontend:
- [ ] POS loads real products
- [ ] Can add items to cart
- [ ] Cart persists (refresh page, cart remains)
- [ ] Checkout creates sale in DB
- [ ] Can update sale status
- [ ] Can process refunds
- [ ] Can receive PO goods
- [ ] All pages wired up
- [ ] E2E tests pass

---

**Current Status**: Backend complete and running. Frontend needs systematic API wiring starting with POS page.

**Estimated Time to Complete Frontend**: 3-4 hours

**Next Immediate Action**: Update POS page with real API calls

