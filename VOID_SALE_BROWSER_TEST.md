# 🧪 Void Sale Browser Test Guide

## Prerequisites
- ✅ Backend running on `http://localhost:5001`
- ✅ Frontend running on `http://localhost:3001`
- ✅ Open Browser Developer Tools (F12) → Network tab

## Test Scenarios

### Scenario 1: Void Current Cart (Before Sale Completion)

This tests voiding a sale that hasn't been saved yet (current cart).

**Steps:**

1. **Navigate to POS**
   - Go to `http://localhost:3001/sales/pos`

2. **Add Items to Cart**
   - Search for products in the search bar
   - Click on products to add them to cart
   - Add at least 2-3 items

3. **Initiate Void**
   - Scroll down to the "Quick Actions" section
   - Click the **red "🗑️ Void Sale"** button
   - The button is below the "Hold Sale" button

4. **Enter Void Reason**
   - A modal will appear: "Void Sale"
   - Enter a reason in the textarea, e.g., "Customer changed mind"
   - Click **"Void Sale"** button

5. **Verify Results**
   - ✅ Cart should be cleared
   - ✅ Notification appears: "✓ Sale voided successfully"
   - ✅ All items removed from cart
   - ✅ Total resets to R0.00

**Expected Console Output:**
```
Sale voided. Reason: Customer changed mind
```

---

### Scenario 2: Void Completed Sale (After Database Save)

This tests voiding a sale that has already been completed and saved to the database.

**Steps:**

1. **Complete a Sale First**
   - Add items to cart
   - Select a payment method (Cash, Card, etc.)
   - Click **"💰 Process Payment"**
   - Wait for success notification
   - Note the Sale ID from the success message (e.g., "Transaction #5")

2. **Open Browser Console**
   - Press F12 → Console tab
   - Keep this open to run commands

3. **Call Void Sale API**
   - In the console, run this code (replace `5` with your actual sale ID):

```javascript
// Get the sales API composable
const { $fetch } = useNuxtApp()

// Void the sale
await $fetch('http://localhost:5001/api/Sales/5/void', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    reason: 'Testing void functionality'
  })
})
.then(() => console.log('✅ Sale voided successfully'))
.catch(err => console.error('❌ Void failed:', err))
```

4. **Verify in Network Tab**
   - Switch to Network tab
   - Look for the request to `/api/Sales/5/void`
   - Check response:
     - Status: `200 OK` ✅
     - Or error status with reason ❌

5. **Verify in Database/API**
   - Fetch the sale again to check status:

```javascript
await $fetch('http://localhost:5001/api/Sales/5')
.then(sale => console.log('Sale status:', sale.status))
```

Expected result: Status should be "Voided" or "Cancelled"

---

## Alternative: Test via Swagger UI

1. **Open Swagger**
   - Go to `http://localhost:5001/swagger`

2. **Find Void Endpoint**
   - Expand **"Sales"** section
   - Find **`POST /api/Sales/{id}/void`**

3. **Execute Request**
   - Click **"Try it out"**
   - Enter Sale ID (e.g., `5`)
   - In the request body, add:
     ```json
     {
       "reason": "Browser test via Swagger"
     }
     ```
   - Click **"Execute"**

4. **Check Response**
   - Response Code: `200` ✅ (Success)
   - Response Code: `400` ❌ (Cannot void - sale may already be voided or in wrong status)

---

## Test Using PowerShell (Direct API Call)

If you want to test the API directly:

```powershell
# Complete a sale first, then void it
$saleId = 5  # Replace with actual sale ID

$body = @{
    reason = "PowerShell test void"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "http://localhost:5001/api/Sales/$saleId/void" `
        -Method POST `
        -Body $body `
        -ContentType "application/json"
    Write-Host "✅ Sale $saleId voided successfully"
} catch {
    Write-Host "❌ Failed to void sale: $($_.Exception.Message)"
}
```

---

## Common Issues & Troubleshooting

### ❌ "Sale cannot be voided"
**Cause:** Sale is already voided, completed with refund, or in a state that doesn't allow voiding

**Solution:** 
- Check sale status first
- Only "Completed" or "Pending" sales can typically be voided
- Voided sales cannot be voided again

### ❌ Network Error / CORS Issue
**Cause:** Backend not running or CORS not configured

**Solution:**
- Ensure backend is running: `cd backend/Toss && dotnet run --project src/Web/Web.csproj`
- Check backend console for errors

### ❌ 404 Not Found
**Cause:** Sale ID doesn't exist

**Solution:**
- Verify the sale ID exists in the database
- Use a valid sale ID from a completed transaction

---

## Expected Network Traffic

When voiding a sale, you should see:

**Request:**
```http
POST http://localhost:5001/api/Sales/5/void
Content-Type: application/json

{
  "reason": "Customer changed mind"
}
```

**Response (Success):**
```http
HTTP/1.1 200 OK
```

**Response (Failure):**
```http
HTTP/1.1 400 Bad Request

"Sale cannot be voided"
```

---

## Test Checklist

- [ ] Void current cart (before saving)
- [ ] Void completed sale (after saving)
- [ ] Verify cart clears on void
- [ ] Verify notification appears
- [ ] Check Network tab shows no errors
- [ ] Verify API returns 200 OK
- [ ] Test void with different sale statuses
- [ ] Test void with invalid sale ID
- [ ] Test void already voided sale

---

## Quick Test Script

Copy and paste this into your browser console while on the POS page:

```javascript
// Quick Void Sale Test
async function testVoidSale() {
  console.log('🧪 Starting Void Sale Test...');
  
  // Test 1: Get a completed sale
  try {
    const sales = await $fetch('http://localhost:5001/api/Sales?shopId=1');
    const completedSale = sales.find(s => s.status === 'Completed');
    
    if (!completedSale) {
      console.warn('⚠️ No completed sales found. Complete a sale first!');
      return;
    }
    
    console.log(`Found sale #${completedSale.id} with status: ${completedSale.status}`);
    
    // Test 2: Void the sale
    await $fetch(`http://localhost:5001/api/Sales/${completedSale.id}/void`, {
      method: 'POST',
      body: JSON.stringify({ reason: 'Automated test void' }),
      headers: { 'Content-Type': 'application/json' }
    });
    
    console.log('✅ Sale voided successfully!');
    
    // Test 3: Verify sale is voided
    const voidedSale = await $fetch(`http://localhost:5001/api/Sales/${completedSale.id}`);
    console.log(`Sale status after void: ${voidedSale.status}`);
    
    if (voidedSale.status === 'Voided' || voidedSale.status === 'Cancelled') {
      console.log('✅ All tests passed!');
    } else {
      console.error('❌ Sale status did not change to Voided');
    }
    
  } catch (error) {
    console.error('❌ Test failed:', error.message);
  }
}

// Run the test
testVoidSale();
```

---

## Success Indicators

✅ **All working correctly when:**
- Void button is enabled with items in cart
- Modal appears with reason input
- Cart clears after void
- Success notification shows
- API returns 200 OK
- Sale status changes to "Voided"
- No errors in console or Network tab

🎉 **Your void sale functionality is working perfectly!**

