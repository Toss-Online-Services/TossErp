# POS Browser Testing Guide 🧪

Quick reference for manual testing of the newly integrated POS cart features.

---

## 🚀 Quick Start

1. **Open POS Page**: Navigate to `http://localhost:3000/sales/pos`
2. **Open DevTools**: Press F12
3. **Open Console Tab**: To view Sentry breadcrumbs and logs

---

## ✅ Test Scenarios

### Scenario 1: Basic Cart Operations
**Goal**: Verify cart math calculates correctly

1. Add "Coca-Cola 500ml" to cart (R12.50)
   - **Expected**: Line total = R14.38 (R12.50 + 15% VAT = R1.88)
2. Add "Bread White" to cart (R15.00)
   - **Expected**: Line total = R17.25 (R15.00 + 15% VAT = R2.25)
3. Increase Coca-Cola quantity to 2
   - **Expected**: Line total = R28.75 (R25.00 + R3.75 VAT)
4. Check VAT Summary
   - **Expected**:
     - Subtotal: R37.50 (R25.00 + R15.00)
     - VAT (15%): R5.63 (15% of R37.50)
     - Grand Total: R43.13

**Console Check**: Look for Sentry breadcrumbs:
- "Added Coca-Cola 500ml to cart"
- "Updated quantity for Coca-Cola 500ml"
- "Added Bread White to cart"

---

### Scenario 2: Percentage Discounts
**Goal**: Verify percentage discount calculations

1. Add "Coca-Cola 500ml" to cart (quantity: 2, line subtotal: R25.00)
2. Click "Discount" button on Coca-Cola line
3. Enter 10% in the percentage field
4. Check line total
   - **Expected**:
     - Original subtotal: R25.00
     - Discount (10%): -R2.50
     - Taxable amount: R22.50
     - VAT (15% of R22.50): R3.38
     - Line total: R25.88 (R22.50 + R3.38)
5. Check VAT Summary
   - **Expected**:
     - Subtotal: R25.00
     - Discount Total: R2.50 (orange)
     - VAT (15%): R3.38
     - Grand Total: R25.88

**Console Check**: "Applied percent discount to Coca-Cola 500ml"

---

### Scenario 3: Fixed Amount Discounts
**Goal**: Verify fixed amount discount calculations

1. Add "Bread White" to cart (R15.00)
2. Click "Discount" button twice (to get to amount mode)
3. Enter R3.00 in the amount field
4. Check line total
   - **Expected**:
     - Original subtotal: R15.00
     - Discount: -R3.00
     - Taxable amount: R12.00
     - VAT (15% of R12.00): R1.80
     - Line total: R13.80 (R12.00 + R1.80)
5. Click discount button again (should clear discount)
   - **Expected**: Line total back to R17.25

**Console Check**: 
- "Applied amount discount to Bread White"
- "Cleared discount from Bread White"

---

### Scenario 4: Discount Validation
**Goal**: Ensure discounts can't exceed subtotal

1. Add "Coca-Cola 500ml" to cart (R12.50)
2. Apply 150% discount
   - **Expected**: Caps at 100%, line total = R0.00 + R0.00 VAT = R0.00
3. Clear discount, apply R20.00 fixed amount
   - **Expected**: Caps at R12.50, line total = R0.00 + R0.00 VAT = R0.00

---

### Scenario 5: Online Payment Processing
**Goal**: Verify payment succeeds when online

1. Add 2-3 products to cart
2. Select customer or leave as "Walk-in"
3. Select payment method (Cash/Card)
4. Click "Complete Sale"
5. **Expected**:
   - Success modal appears
   - Notification: "✓ Sale completed! Transaction #123"
   - Cart clears automatically
   - Console: "✅ Sale 123 created successfully (online)"

**Console Check**: 
- Sentry breadcrumb: "Sale processed online" with saleId, paymentMethod, total

---

### Scenario 6: Offline Payment Processing
**Goal**: Verify payment queues when offline

1. **Enable Offline Mode**:
   - DevTools → Network tab → Toggle "Offline" checkbox
2. Add products to cart
3. Click "Complete Sale"
4. **Expected**:
   - Success modal appears
   - Notification: "✓ Sale queued for sync (1 pending)"
   - Orange queue badge appears in header: "1 queued"
   - Cart clears automatically
   - Console: "Sale queued (offline)"

**Console Check**: 
- Sentry breadcrumb: "Sale queued (offline)" with paymentMethod, total

---

### Scenario 7: Offline Queue Auto-Sync
**Goal**: Verify queue syncs when online returns

1. Process 2 sales while offline (from Scenario 6)
2. **Expected**: Queue badge shows "2 queued"
3. **Re-enable Online Mode**:
   - DevTools → Network tab → Uncheck "Offline"
4. Wait ~30 seconds (auto-sync interval)
5. **Expected**:
   - Notification: "Synced 2 transactions"
   - Queue badge disappears
   - Console: "✅ Sale X created successfully", "✅ Sale Y created successfully"

---

### Scenario 8: Mixed Cart with Discounts
**Goal**: Verify complex cart calculations

1. Add "Coca-Cola 500ml" × 2 (R25.00 subtotal)
   - Apply 10% discount → -R2.50
2. Add "Bread White" × 1 (R15.00 subtotal)
   - Apply R2.00 discount → -R2.00
3. Add "Sugar 2kg" × 1 (R45.00 subtotal, no discount)

**Expected VAT Summary**:
- Subtotal: R85.00 (R25.00 + R15.00 + R45.00)
- Discount Total: R4.50 (R2.50 + R2.00)
- Taxable: R80.50 (R85.00 - R4.50)
- VAT (15%): R12.08 (15% of R80.50)
- Grand Total: R92.58 (R80.50 + R12.08)

---

## 🎨 Visual Checks

### VAT Summary Box
- [ ] Blue background with border
- [ ] Dark mode: darker blue, lighter text
- [ ] All amounts align right
- [ ] Discount shown in orange (if > 0)
- [ ] Grand total emphasized (larger, bold)
- [ ] Item count at bottom

### Discount Controls
- [ ] "Discount" button is orange
- [ ] Input appears when button clicked
- [ ] Percentage mode: shows "%" after input
- [ ] Amount mode: shows "R" before input
- [ ] Red × button to clear
- [ ] Original price shown with strikethrough when discount applied

### Queue Status Badge
- [ ] Only visible when queuePendingCount > 0
- [ ] Orange background
- [ ] Animated pulse icon
- [ ] Shows count: "X queued"

---

## 🐛 Edge Cases to Test

### Discount Edge Cases
- [ ] Enter negative discount → should cap at 0
- [ ] Enter discount > 100% → should cap at 100%
- [ ] Enter fixed amount > subtotal → should cap at subtotal
- [ ] Switch from percent to amount → opposite discount clears
- [ ] Remove item with discount → discount removed too

### Cart Edge Cases
- [ ] Remove last item → cart shows "No items in cart"
- [ ] Update quantity to 0 → item removed
- [ ] Clear cart → all items removed, customer reset
- [ ] Add same product twice → quantity increases (not duplicate lines)

### Offline Edge Cases
- [ ] Process sale offline → queue grows
- [ ] Go online → auto-sync triggers
- [ ] Go offline mid-sync → retries on next interval
- [ ] Queue with 3+ pending → all sync in batch

---

## 📊 Console Log Patterns

### Successful Flow (Online)
```
✅ Sale 123 created successfully (online)
Sentry breadcrumb: pos.payment | Sale processed online | { saleId: 123, paymentMethod: 'Cash', total: 92.58 }
```

### Queued Flow (Offline)
```
Sale queued (offline)
Sentry breadcrumb: pos.payment | Sale queued (offline) | { paymentMethod: 'Cash', total: 92.58 }
```

### Auto-Sync Success
```
Synced 2 transactions
✅ Sale 124 created successfully
✅ Sale 125 created successfully
```

### Auto-Sync Failure
```
Synced 0 transactions, 1 failed
Error: API call failed
```

---

## 🚨 Error Scenarios

### API Failure During Online Mode
**Setup**: 
1. Keep network online
2. Stop backend server (Ctrl+C in backend terminal)
3. Process sale

**Expected**:
- API call fails
- Sale queues automatically
- Notification: "✓ Sale queued for sync (1 pending)"
- Console: "API call failed, queueing for later"

### IndexedDB Not Available
**Setup**: 
1. Open incognito/private browsing mode
2. Some browsers block IndexedDB in incognito

**Expected**:
- Error in console
- Sale might fail entirely (graceful degradation needed)

---

## ✅ Success Criteria

All scenarios should:
- ✅ Calculate totals correctly (no float errors)
- ✅ Display VAT summary with accurate breakdown
- ✅ Apply and clear discounts properly
- ✅ Queue sales when offline
- ✅ Auto-sync when online returns
- ✅ Log Sentry breadcrumbs for all operations
- ✅ Clear cart after successful payment
- ✅ Show appropriate notifications (success/error)

---

## 📸 Screenshots to Capture

1. **Cart with VAT Summary** (before payment)
2. **Line Item with Discount Applied** (showing strikethrough)
3. **Queue Badge in Header** (1+ queued)
4. **Success Modal** (after payment)
5. **DevTools Console** (showing breadcrumbs)

---

## 🔧 Debugging Tips

**Cart Math Not Updating**:
- Check Vue DevTools → Components → cartTotals computed
- Verify cart items have taxRate, discountPercent/Amount fields

**Discount Not Applying**:
- Check item.showDiscount state (should be 'percent' or 'amount')
- Verify handleDiscountChange fires (add console.log)

**Queue Not Working**:
- Check IndexedDB in DevTools → Application → Storage → IndexedDB
- Verify 'toss-offline-queue' database exists
- Check 'transactions' object store for pending records

**Auto-Sync Not Triggering**:
- Verify online status: `navigator.onLine` in console
- Check interval is set: `autoSyncInterval` should not be null
- Wait full 30 seconds (interval delay)

**Sentry Breadcrumbs Not Appearing**:
- Verify `sentry` is defined: `typeof sentry !== 'undefined'`
- Check Sentry client config is loaded
- Look in Network tab for Sentry API calls

---

**Happy Testing!** 🎉
