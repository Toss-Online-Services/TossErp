# nopCommerce vs TOSS Feature Comparison

## Shopping Cart Features

### nopCommerce Has:
1. ✅ Add to cart from catalog
2. ✅ Add to cart from product details
3. ✅ Update cart quantities
4. ✅ Remove items from cart
5. ✅ Apply discount coupons
6. ✅ Apply gift cards
7. ✅ Estimate shipping
8. ✅ Product attribute handling
9. ✅ File uploads for attributes
10. ✅ Wishlist management
11. ✅ Move to wishlist
12. ✅ Email wishlist

### TOSS Currently Has:
- ❌ None of the above shopping cart features

## Order Management Features

### nopCommerce Has:
1. ✅ List customer orders
2. ✅ Order details
3. ✅ Print order
4. ✅ PDF invoice
5. ✅ Cancel order
6. ✅ Re-order
7. ✅ Recurring payments
8. ✅ Reward points
9. ✅ Shipment tracking
10. ✅ Re-post payment

### TOSS Currently Has:
- ✅ Basic Sales list (GetSales)
- ✅ Basic Purchase Orders list (GetPurchaseOrders)
- ❌ Most advanced order management features missing

## Implementation Plan

### Phase 1: Shopping Cart/POS (Priority 1)
- [ ] Create ShoppingCart entity and DbSet
- [ ] AddToCart endpoint
- [ ] UpdateCart endpoint
- [ ] RemoveFromCart endpoint
- [ ] GetCart endpoint
- [ ] Checkout endpoint
- [ ] Wire up POS page

### Phase 2: Product Search & Inventory (Priority 2)
- [ ] SearchProducts endpoint (with filters, pagination)
- [ ] GetLowStockItems endpoint
- [ ] GetProductsByCategory endpoint
- [ ] BarcodeScanner integration endpoint
- [ ] Wire up inventory pages

### Phase 3: Order Management (Priority 3)
- [ ] Enhance Sales endpoints (details, cancel, refund)
- [ ] Enhance Purchase Order endpoints
- [ ] PDF invoice generation
- [ ] Order tracking
- [ ] Wire up sales/buying order pages

### Phase 4: Group Buying (Priority 4)
- [ ] CreateGroupBuy endpoint
- [ ] JoinGroupBuy endpoint
- [ ] GetActiveGroupBuys endpoint
- [ ] CompleteGroupBuy endpoint
- [ ] Wire up group buying pages

### Phase 5: Testing (Priority 5)
- [ ] Create E2E tests for all flows
- [ ] Test in browser with real data

