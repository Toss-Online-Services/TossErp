# 🧪 Order Actions Testing Guide

## Quick Test Checklist

### ✅ Prerequisites
1. Start the dev server: `pnpm run dev`
2. Navigate to: `http://localhost:3000/purchasing/orders`
3. Ensure you have at least one order created (from previous tests)

---

## 1. **Test VIEW Order** 👁️

**Steps:**
1. Click the **"View"** button on any order card
2. Verify modal opens with full order details
3. Check that all information is displayed:
   - Order number
   - Supplier name
   - Status badge (with correct color)
   - Order date
   - Expected delivery date
   - Payment terms
   - Items table with quantities and prices
   - Subtotal, delivery fee, and total calculations
4. Click **"Close"** or backdrop to dismiss modal

**Expected Result:** ✅ Modal displays complete order information with Material Design styling

---

## 2. **Test PRINT Order** 🖨️

**Steps:**
1. Click the **"Print"** button on any order card
2. New browser tab/window opens with print-friendly version
3. Verify the print layout includes:
   - TOSS ERP header
   - Order number prominently displayed
   - All order information in formatted blocks
   - Items table with quantities and prices
   - Totals section
   - "Print Order" button
4. Click **"Print Order"** button
5. System print dialog appears

**Expected Result:** ✅ Professional print-ready document opens in new window

**Alternative:** You can also print from the order details modal by clicking "Print Order" button in the footer.

---

## 3. **Test TRACK Order** 🚚

**Steps:**
1. Click the **"Track"** button on any order card
2. Verify navigation to `/purchasing/track-orders`
3. Check URL contains order number as query parameter: `?order=PO-xxxxxxx`

**Expected Result:** ✅ Navigates to tracking page with order number pre-filled

**Alternative:** You can also track from the order details modal by clicking "Track Order" button in the footer.

---

## 4. **Test APPROVE Order** ✅

**Steps:**
1. Find an order with **"pending"** status (orange badge)
2. Click the **"Approve"** button
3. Verify green success toast appears: "✅ Order Approved"
4. Check order status changes to **"approved"** (blue badge)
5. Verify stats update automatically:
   - Pending count decreases by 1
   - Total count remains the same
6. Notice **"Approve"** button disappears (only shows for pending orders)

**Expected Result:** ✅ Order status updates, toast notification shows, stats refresh

---

## 5. **Test CANCEL Order** ❌

**Steps:**
1. Find an order with **"pending"** or **"approved"** status
2. Click the **"Cancel"** button
3. Confirmation dialog appears: "Are you sure you want to cancel order PO-xxxxx?"
4. Click **"OK"**
5. Verify warning toast appears with order number
6. Check order status changes to **"cancelled"** (red badge)
7. Verify stats update automatically
8. Notice **"Cancel"** button disappears (only shows for pending/approved orders)

**Expected Result:** ✅ Order status updates, toast notification shows, stats refresh

**Alternative:** Click "Cancel" in the confirmation dialog to abort the cancellation.

---

## 6. **Test ORDER DETAILS Modal Actions** 📋

**Steps:**
1. Click **"View"** on any order
2. Modal opens with full details
3. Test all three footer buttons:
   - **"Print Order"** → Opens print window
   - **"Track Order"** → Navigates to tracking page
   - **"Close"** → Dismisses modal

**Expected Result:** ✅ All modal actions work correctly

---

## 🎯 Complete Workflow Test

### Create → View → Approve → Track → Print Flow

1. **Create a new order:**
   - Go to `/purchasing/create-order`
   - Add some items to cart
   - Click "Place Order"
   - Toast appears: "Order PO-xxxxx created successfully!"
   - Redirects to orders page

2. **View the order:**
   - Click "View" on the newly created order
   - Verify all details are correct
   - Close modal

3. **Approve the order:**
   - Click "Approve" button
   - Toast appears: "Order approved successfully!"
   - Status changes to "approved" (blue badge)

4. **Track the order:**
   - Click "Track" button
   - Navigate to tracking page
   - URL contains order number

5. **Print the order:**
   - Go back to orders page
   - Click "Print" button
   - Print window opens
   - Click "Print Order" to print

**Expected Result:** ✅ Complete order lifecycle works seamlessly

---

## 📊 Expected UI Elements

### Order Card Actions (Bottom of each card)
```
[👁️ View] [✓ Approve] [🖨️ Print] [🚚 Track]           [❌ Cancel]
```

- **View** (Blue) - Always visible
- **Approve** (Green) - Only for "pending" orders
- **Print** (Purple) - Always visible
- **Track** (Indigo) - Always visible
- **Cancel** (Red) - Only for "pending" or "approved" orders

### Order Status Badges
- **Pending** - Orange badge
- **Approved** - Blue badge
- **In Transit** - Purple badge
- **Delivered** - Green badge
- **Cancelled** - Red badge

---

## 🐛 Troubleshooting

### Issue: Modal doesn't open
- **Fix:** Check browser console for errors
- Ensure `OrderDetailsModal.vue` component exists in `components/purchasing/`

### Issue: Print window is blank
- **Fix:** Check if order has items in localStorage
- Open browser dev tools → Application → Local Storage → `toss-orders`

### Issue: Toast doesn't appear
- **Fix:** Ensure `ToastContainer` is in `app.vue`
- Check `useToast` composable is imported

### Issue: Order status doesn't update
- **Fix:** Check localStorage for order data
- Ensure `loadOrders()` is called after status change

---

## 🎉 Success Criteria

All functionality is working if:
- ✅ View modal displays complete order details
- ✅ Print opens formatted print-friendly window
- ✅ Track navigates to tracking page with order number
- ✅ Approve changes status and shows toast
- ✅ Cancel changes status after confirmation
- ✅ Stats update automatically after status changes
- ✅ All toasts appear with correct messages and colors
- ✅ UI is responsive and Material Design styled

---

## 📱 Mobile Testing

Test on mobile viewport (< 640px):
- Responsive order cards
- Touch-friendly action buttons
- Modal adapts to screen size
- Toast notifications positioned correctly

---

## 🚀 Ready for Production!

Once all tests pass, the order management system is fully functional and ready for deployment! 🎊

