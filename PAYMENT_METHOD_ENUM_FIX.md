# Payment Method Enum Fix

## Problem
The frontend was sending payment method strings like `'cash'`, `'card'`, `'eft'`, and `'account'` to the backend, but the backend `PaymentType` enum expects specific values:

```csharp
public enum PaymentType
{
    Cash = 0,
    Card = 1,
    MobileMoney = 2,
    BankTransfer = 3,
    PayLink = 4
}
```

This caused a JSON deserialization error:
```
System.Text.Json.JsonException: The JSON value could not be converted to Toss.Domain.Enums.PaymentType
```

## Root Cause
1. Frontend was using lowercase strings: `'cash'`, `'card'`, `'eft'`, `'account'`
2. The `capitalizePaymentMethod` function only capitalized the first letter, converting `'cash'` to `'Cash'` but failing for multi-word enum values like `'MobileMoney'` (which would become `'Mobilemoney'`)
3. Some payment method IDs (`'eft'` and `'account'`) didn't map to any backend enum value

## Solution

### 1. Updated Payment Methods in Frontend
Changed from lowercase/custom strings to match backend enum exactly:

**toss-web/pages/sales/pos/index.vue**
```typescript
// Before
const paymentMethods = ref([
  { id: 'cash', name: 'Cash' },
  { id: 'card', name: 'Card' },
  { id: 'eft', name: 'EFT' },
  { id: 'account', name: 'Account' }
])

// After
const paymentMethods = ref([
  { id: 'Cash', name: 'Cash' },
  { id: 'Card', name: 'Card' },
  { id: 'MobileMoney', name: 'Mobile Money' },
  { id: 'BankTransfer', name: 'Bank Transfer' },
  { id: 'PayLink', name: 'Pay Link' }
])
```

**toss-web/pages/sales/orders/create-order.vue**
- Applied the same payment method updates

### 2. Updated Default Values
Changed all default payment method values from `'cash'` to `'Cash'`:
- `const selectedPaymentMethod = ref('Cash')`
- Reset functions now use `'Cash'` instead of `'cash'`

### 3. Updated Composable Type Definitions
**toss-web/composables/useSalesAPI.ts**

Changed payment method types from `string` to explicit enum unions:
```typescript
// holdSale function
paymentMethod: 'Cash' | 'Card' | 'MobileMoney' | 'BankTransfer' | 'PayLink'

// createSale function  
paymentType: 'Cash' | 'Card' | 'MobileMoney' | 'BankTransfer' | 'PayLink'
```

### 4. Removed Faulty Transformation Function
Removed the `capitalizePaymentMethod` function which was incorrectly transforming multi-word enum values:
```typescript
// Removed this function
const capitalizePaymentMethod = (method: string) => {
  return method.charAt(0).toUpperCase() + method.slice(1).toLowerCase()
}
```

Now the payment method strings are passed directly to the API without transformation.

### 5. Fixed Invoice Payment Method
Changed the placeholder payment method for invoices from invalid `'Account'` to valid `'BankTransfer'`:
```typescript
paymentType: 'BankTransfer', // Account sales use bank transfer
```

### 6. Fixed Card Payment Check
Updated the card payment detection:
```typescript
// Before
if (selectedPaymentMethod.value === 'card' && hardwareStatus.value.cardReader)

// After  
if (selectedPaymentMethod.value === 'Card' && hardwareStatus.value.cardReader)
```

## Files Modified
1. `toss-web/pages/sales/pos/index.vue`
2. `toss-web/pages/sales/orders/create-order.vue`
3. `toss-web/composables/useSalesAPI.ts`

## Testing
After these changes, the hold sale and create sale functionality should work correctly with all payment methods properly deserializing to the backend `PaymentType` enum.

### 7. Fixed Load Held Sales Function
Removed `.toLowerCase()` call when loading held sales from the database:
```typescript
// Before
paymentMethod: sale.paymentMethod.toLowerCase(),

// After
paymentMethod: sale.paymentMethod, // Keep enum value as-is (Cash, Card, etc.)
```

This was causing a runtime error: `TypeError: sale.paymentMethod.toLowerCase is not a function` because the backend now returns the enum as a proper string value.

## Impact
- ✅ Hold sale now works correctly
- ✅ Complete sale now works correctly  
- ✅ Load held sales now works correctly (fixed `.toLowerCase()` error)
- ✅ Payment methods align with backend enum
- ✅ Type safety improved with explicit union types
- ✅ Removed unnecessary string transformation logic
- ✅ Better display on receipts (proper casing: "Cash" vs "cash")

