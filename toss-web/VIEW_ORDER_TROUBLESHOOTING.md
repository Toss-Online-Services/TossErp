# View Order Modal - Troubleshooting Guide

## 🔧 What I Fixed

1. **Added explicit import** for `OrderDetailsModal` component
2. **Added console logs** to debug the modal state
3. **Fixed TypeScript types** for better type safety

---

## 🧪 Testing Steps

### 1. Open Browser Console
Press **F12** to open DevTools → Go to **Console** tab

### 2. Click "View" Button
Click the "View" button on any order card

### 3. Check Console Output
You should see:
```
View Order clicked: {id: "...", number: "PO-...", ...}
Modal should be visible: true
```

---

## 🔍 Debugging Checklist

### If modal still doesn't show:

#### Check 1: Console Logs
- [ ] Do you see "View Order clicked:" in console?
  - **YES** → Click handler is working ✅
  - **NO** → Button click isn't firing ❌

- [ ] Do you see "Modal should be visible: true"?
  - **YES** → State is updating ✅
  - **NO** → State update failed ❌

#### Check 2: Component Registration
Open DevTools → **Vue DevTools** (if installed):
- [ ] Is `OrderDetailsModal` component registered?
- [ ] Does it appear in component tree when modal should be visible?

#### Check 3: Modal Props
In Console, type:
```javascript
document.querySelector('[class*="fixed inset-0 z-50"]')
```
- **Returns element** → Modal is in DOM but maybe hidden
- **Returns null** → Modal not rendering

#### Check 4: Z-Index Issues
Check if anything is covering the modal:
- [ ] Inspect element with DevTools
- [ ] Check z-index values (modal should be z-50)
- [ ] Look for overlapping elements

#### Check 5: CSS/Styling Issues
- [ ] Is backdrop visible? (should be black with 50% opacity)
- [ ] Is modal container visible? (white background, centered)
- [ ] Check for `display: none` or `opacity: 0` styles

---

## 🐛 Common Issues & Solutions

### Issue 1: Modal in DOM but not visible
**Solution:** Check CSS classes and z-index
```vue
<!-- Modal should have -->
<div class="fixed inset-0 z-50 overflow-y-auto">
```

### Issue 2: Click handler not firing
**Solution:** Check if button has correct event handler
```vue
<button @click="viewOrder(order)">View</button>
```

### Issue 3: Component not auto-importing
**Solution:** Restart dev server
```bash
# Stop server (Ctrl+C)
# Start again
pnpm run dev
```

### Issue 4: Props not passing correctly
**Solution:** Check modal component receives props
```vue
<OrderDetailsModal
  :show="showOrderDetails"
  :order="selectedOrder"
  @close="closeOrderDetails"
/>
```

---

## 🔬 Manual Test in Console

Open browser console and paste:

```javascript
// Check if modal state exists
console.log('Modal visible?', showOrderDetails)
console.log('Selected order:', selectedOrder)

// Manually trigger modal
showOrderDetails.value = true
selectedOrder.value = {
  id: 'TEST',
  number: 'PO-TEST-001',
  supplier: 'Test Supplier',
  status: 'pending',
  orderDate: new Date(),
  expectedDelivery: new Date(),
  totalAmount: 1000,
  itemCount: 5,
  paymentTerms: 'Net 30'
}
```

**Expected Result:** Modal should appear with test data

---

## ✅ Verification Steps

1. **Refresh the page** (`http://localhost:3001/purchasing/orders`)
2. **Open browser console** (F12)
3. **Click "View"** on any order
4. **Check console** for log messages
5. **Modal should appear** with full order details

---

## 📸 What You Should See

When modal is working correctly:
1. ✅ Backdrop appears (dark overlay covering page)
2. ✅ Modal slides in from center
3. ✅ Gradient blue-purple header visible
4. ✅ Order details displayed
5. ✅ Items table shows products
6. ✅ Three footer buttons: "Print Order", "Track Order", "Close"

---

## 🆘 Still Not Working?

If modal still doesn't show after all checks:

1. **Check browser console** for ANY errors (red text)
2. **Screenshot the console** and share
3. **Try in incognito mode** (to rule out extensions)
4. **Clear browser cache** (Ctrl+Shift+Delete)
5. **Restart dev server**

---

## 🎯 Quick Fix Commands

```bash
# 1. Stop dev server (Ctrl+C in terminal)

# 2. Clear Nuxt cache
rm -rf .nuxt

# 3. Restart dev server
pnpm run dev
```

---

## 📞 Report Back

Please check and report:
- [ ] Console log messages (screenshot)
- [ ] Any error messages in console
- [ ] Does modal DOM element exist? (inspect with DevTools)
- [ ] What happens when you click "View"?

This will help identify the exact issue! 🔍

