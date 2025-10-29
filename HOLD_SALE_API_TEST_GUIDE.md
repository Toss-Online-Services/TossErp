# Hold Sale API Test Guide

## Current Status
- **Backend API**: Running at https://localhost:5001/api/index.html?url=/api/specification.json
- **Frontend**: Should be accessible at http://localhost:3001/sales/pos
- **Issue**: 500 Internal Server Error when attempting to Hold Sale

## Test Steps

### 1. Browser Testing (Primary Method)

1. **Open POS Interface**
   - Navigate to: http://localhost:3001/sales/pos
   - Verify the page loads correctly

2. **Add Items to Cart**
   - Add at least one item to the cart
   - Set quantity and verify total calculation

3. **Test Hold Sale**
   - Select payment method (ensure it's "Cash", "Card", etc. - matching backend enum)
   - Click "Hold Sale" button
   - **Expected**: Sale should be held successfully
   - **Current Issue**: 500 Internal Server Error

4. **Check Browser Developer Tools**
   - Open F12 Developer Tools
   - Go to Network tab
   - Attempt Hold Sale operation
   - Check the request/response details for the `/api/Sales/hold` endpoint

### 2. API Direct Testing

#### Using Browser Console
```javascript
// Test Hold Sale API directly
fetch('https://localhost:5001/api/Sales/hold', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    shopId: 1,
    customerId: null,
    items: [
      {
        productId: 1,
        quantity: 2,
        unitPrice: 10.00
      }
    ],
    paymentMethod: "Cash",
    totalAmount: 20.00,
    notes: "Test hold sale"
  })
})
.then(response => {
  console.log('Status:', response.status);
  return response.text();
})
.then(data => console.log('Response:', data))
.catch(error => console.error('Error:', error));
```

#### Using PowerShell (Alternative)
```powershell
$body = @{
    shopId = 1
    customerId = $null
    items = @(
        @{
            productId = 1
            quantity = 2
            unitPrice = 10.00
        }
    )
    paymentMethod = "Cash"
    totalAmount = 20.00
    notes = "Test hold sale"
} | ConvertTo-Json -Depth 3

Invoke-RestMethod -Uri 'https://localhost:5001/api/Sales/hold' -Method POST -ContentType 'application/json' -Body $body
```

### 3. Common Issues to Check

#### Payment Method Enum Values
Ensure frontend sends exact enum values:
- ✅ "Cash" (not "cash")
- ✅ "Card" (not "card") 
- ✅ "MobileMoney"
- ✅ "BankTransfer"
- ✅ "PayLink"

#### Required Fields
Verify all required fields are present:
- `shopId` (integer)
- `paymentMethod` (enum string)
- `totalAmount` (decimal)
- `items` (array, can be empty for hold)

#### Database Connection
- Backend might be trying to connect to PostgreSQL
- Check if in-memory database configuration is working
- Verify database initialization

### 4. Debugging Steps

1. **Check Backend Logs**
   - Look for detailed error messages in the console where the API is running
   - Check for database connection errors
   - Look for validation errors

2. **Verify API Endpoint**
   - Confirm the endpoint exists: https://localhost:5001/api/Sales/hold
   - Check the Swagger documentation for required parameters

3. **Test with Minimal Data**
   ```json
   {
     "shopId": 1,
     "paymentMethod": "Cash",
     "totalAmount": 0.00,
     "items": []
   }
   ```

### 5. Expected Successful Response

A successful Hold Sale should return:
- **Status Code**: 200 or 201
- **Response Body**: Sale ID or confirmation object
- **Frontend Behavior**: 
  - Success message displayed
  - Cart cleared
  - Sale appears in held sales list

### 6. Next Steps if Still Failing

1. Check database connectivity and initialization
2. Verify all required services are registered in DI container
3. Check for missing migrations or database schema issues
4. Validate request payload matches exactly what backend expects
5. Review any authentication/authorization requirements

## Files Modified for Payment Method Fix
- `toss-web/pages/sales/pos/index.vue` - Updated payment method enum values
- `toss-web/composables/useSalesAPI.ts` - Fixed payment method handling
- `toss-web/pages/sales/orders/create-order.vue` - Updated payment methods

## Previous Fixes Applied
- ✅ Fixed payment method enum mismatch (Cash vs cash)
- ✅ Fixed `.toLowerCase()` error in held sales loading
- ✅ Updated frontend to use exact backend enum values
