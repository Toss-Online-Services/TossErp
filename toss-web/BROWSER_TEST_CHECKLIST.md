# 🧪 Browser Testing Checklist - Purchase Orders

## 🎯 URL to Test
`http://localhost:3001/purchasing/orders`

---

## ✅ Visual Verification

### Page Layout
- [ ] Page loads without errors
- [ ] Header displays "Purchase Orders" with gradient text
- [ ] "Create Order" button visible in top right
- [ ] 5 stat cards displayed (Total Orders, Pending, In Transit, Delivered, Total Value)
- [ ] Search bar and filters visible
- [ ] At least 1 order card displayed (from previous test)

### Order Card Display
Each order card should show:
- [ ] Circular avatar with supplier's first letter
- [ ] Order number (e.g., "PO-1761045935289")
- [ ] Supplier name
- [ ] Status badge with correct color
- [ ] Total amount (e.g., "R1,250")
- [ ] Order Date
- [ ] Expected Delivery
- [ ] Item count
- [ ] Payment Terms

### Action Buttons
Each order card should have these buttons at the bottom:
- [ ] 👁️ **View** (Blue)
- [ ] ✅ **Approve** (Green - only if status is "pending")
- [ ] 🖨️ **Print** (Purple)
- [ ] 🚚 **Track** (Indigo)
- [ ] ❌ **Cancel** (Red - only if status is "pending" or "approved")

---

## 🧪 Functionality Tests

### 1. View Order
**Steps:**
1. Click the **"View"** button on any order
2. **Expected:** Modal opens with full order details
3. **Verify:**
   - [ ] Modal has gradient blue-purple header
   - [ ] Order number displayed
   - [ ] Supplier, status, dates all shown
   - [ ] Items table displays products
   - [ ] Subtotal, delivery fee, total calculated
   - [ ] Footer has "Print Order", "Track Order", "Close" buttons
4. Click "Close" or backdrop
5. **Expected:** Modal dismisses

---

### 2. Print Order
**Steps:**
1. Click the **"Print"** button on any order
2. **Expected:** New browser tab/window opens
3. **Verify:**
   - [ ] Print-friendly layout displayed
   - [ ] TOSS ERP header visible
   - [ ] Order details formatted professionally
   - [ ] Items table with quantities and prices
   - [ ] Totals section at bottom
   - [ ] "Print Order" button visible
4. Click "Print Order" button
5. **Expected:** System print dialog appears

**Alternative:** You can also print from inside the View modal.

---

### 3. Track Order
**Steps:**
1. Click the **"Track"** button on any order
2. **Expected:** Navigate to `/purchasing/track-orders`
3. **Verify:**
   - [ ] URL changes to `/purchasing/track-orders`
   - [ ] URL contains query parameter: `?order=PO-xxxxxxx`

---

### 4. Approve Order
**Steps:**
1. Find an order with **"pending"** status (orange badge)
2. Click the **"Approve"** button
3. **Expected:** 
   - [ ] Green success toast appears: "✅ Order Approved - Order PO-xxxxx approved successfully!"
   - [ ] Order status badge changes to "approved" (blue)
   - [ ] "Pending" stat decreases by 1
   - [ ] "Approve" button disappears from that order
4. **Verify toast:**
   - [ ] Toast appears in top-right
   - [ ] Toast shows for ~3 seconds
   - [ ] Toast auto-dismisses

---

### 5. Cancel Order
**Steps:**
1. Find an order with **"pending"** or **"approved"** status
2. Click the **"Cancel"** button
3. **Expected:** Confirmation dialog appears
4. **Verify:**
   - [ ] Dialog shows: "Are you sure you want to cancel order PO-xxxxx?"
5. Click **"OK"**
6. **Expected:**
   - [ ] Warning toast appears (yellow/orange)
   - [ ] Order status badge changes to "cancelled" (red)
   - [ ] Stats update automatically
   - [ ] "Cancel" button disappears from that order

**Alternative:** Click "Cancel" in dialog to abort.

---

## 🎨 UI/UX Checks

### Material Design Elements
- [ ] Gradient backgrounds on headers
- [ ] Smooth hover effects on buttons
- [ ] Card shadows and hover elevation
- [ ] Rounded corners on all elements
- [ ] Consistent color scheme (blue, purple, green, orange, red)
- [ ] Icons display correctly

### Responsive Layout
- [ ] Page adapts to browser width
- [ ] Stat cards stack on smaller screens
- [ ] Order cards remain readable
- [ ] Action buttons don't overflow

### Animations
- [ ] Toast slides in smoothly
- [ ] Modal fades in with scale effect
- [ ] Hover effects on buttons
- [ ] Status badges have proper contrast

---

## 🐛 Error Checking

### Console
Open browser DevTools (F12) → Console tab:
- [ ] No red errors
- [ ] No Vue warnings (or only minor ones)
- [ ] No 404 errors for assets

### Network
DevTools → Network tab:
- [ ] All resources load successfully
- [ ] No failed requests

### Performance
- [ ] Page loads quickly (< 2 seconds)
- [ ] Actions respond immediately
- [ ] No lag when clicking buttons

---

## 📊 Data Verification

### LocalStorage
DevTools → Application → Local Storage → `http://localhost:3001`:
- [ ] Key `toss-orders` exists
- [ ] Contains array of orders
- [ ] Order statuses update when approved/cancelled

---

## ✅ Success Criteria

**Page is working correctly if:**
1. ✅ No errors in browser console
2. ✅ All 5 stat cards display correct numbers
3. ✅ Order cards render with all information
4. ✅ All 5 action buttons work as expected
5. ✅ Toasts appear for approve/cancel actions
6. ✅ Modal opens/closes smoothly
7. ✅ Print window opens with formatted document
8. ✅ Track navigation works correctly
9. ✅ Stats update after status changes
10. ✅ Material Design styling applied throughout

---

## 🎉 Next Steps

If all checks pass:
1. ✅ Order management is fully functional
2. ✅ Ready for production deployment
3. ✅ Can proceed with additional features

If issues found:
1. Note which specific test failed
2. Check browser console for errors
3. Report back for further fixes

---

## 📝 Quick Test Workflow

**Fastest way to verify everything:**

1. **Load page** → Check for errors
2. **Click "View"** → Modal opens correctly
3. **Click "Print"** → Print window opens
4. **Click "Approve"** → Toast shows, status updates
5. **Click "Track"** → Navigation works
6. **Click "Cancel"** → Confirmation + status update

**Total time:** < 2 minutes

---

Happy testing! 🚀

