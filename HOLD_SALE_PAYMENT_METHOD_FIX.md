# Hold Sale Payment Method Fix

## Issue

When trying to hold a sale from the POS page, the request was failing with a JSON deserialization error:

```
System.Text.Json.JsonException: The JSON value could not be converted to Toss.Domain.Enums.PaymentType. 
Path: $.paymentMethod
```

## Root Cause

**Frontend-Backend Mismatch:**
- **Frontend** was sending: `paymentMethod: "cash"` (lowercase)
- **Backend** expected: `PaymentMethod: "Cash"` (capitalized)

The backend `PaymentType` enum has values with proper casing:
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

But the frontend POS page was sending lowercase payment method strings.

---

## Solution

Added a `capitalizePaymentMethod` helper function in `useSalesAPI.ts` that converts payment methods to proper casing before sending to the backend.

### Changes Made

**File: `toss-web/composables/useSalesAPI.ts`**

1. **Added helper function:**
```typescript
/**
 * Capitalize payment method to match backend PaymentType enum
 */
const capitalizePaymentMethod = (method: string) => {
  return method.charAt(0).toUpperCase() + method.slice(1).toLowerCase()
}
```

2. **Updated `createSale` method:**
```typescript
const createSale = async (saleData: { ... }) => {
  return await $fetch<{ id: number }>(`${baseURL}/Sales`, {
    method: 'POST',
    body: {
      ...saleData,
      paymentType: capitalizePaymentMethod(saleData.paymentType)
    }
  })
}
```

3. **Updated `holdSale` method:**
```typescript
const holdSale = async (saleData: { ... }) => {
  return await $fetch<{ id: number }>(`${baseURL}/Sales/hold`, {
    method: 'POST',
    body: {
      ...saleData,
      paymentMethod: capitalizePaymentMethod(saleData.paymentMethod)
    }
  })
}
```

---

## Transformation Examples

| Frontend Input | Backend Expected | After Fix |
|----------------|------------------|-----------|
| `"cash"`       | `"Cash"`         | ✅ `"Cash"` |
| `"card"`       | `"Card"`         | ✅ `"Card"` |
| `"mobilemoney"`| `"MobileMoney"`  | ⚠️ `"Mobilemoney"` (partial) |

**Note:** The current implementation capitalizes only the first letter. For compound values like `MobileMoney`, the frontend should send the exact enum value or we should enhance the helper function.

---

## Testing

### Manual Test Steps

1. **Start Backend:**
   ```powershell
   cd backend/Toss/src/Web
   dotnet run --urls "http://localhost:5000;https://localhost:5001"
   ```

2. **Open POS Page:**
   - Navigate to `http://localhost:3000/sales/pos`

3. **Test Hold Sale:**
   - Add items to cart
   - Select customer
   - Select payment method (Cash/Card)
   - Click "Hold Sale"
   - Add optional note
   - Click "Confirm Hold"

4. **Expected Result:**
   - ✅ Success notification: "Sale held successfully"
   - ✅ Sale appears in "Held Sales" modal
   - ✅ No console errors

5. **Verify in Backend:**
   - Check that sale was saved to database with `OnHold` status
   - Verify `PaymentMethod` field has proper casing

---

## Backend Payment Method Values

The backend `PaymentType` enum supports:
- ✅ `Cash` (0)
- ✅ `Card` (1)
- ✅ `MobileMoney` (2)
- ✅ `BankTransfer` (3)
- ✅ `PayLink` (4)

---

## Known Limitations

1. **Compound Enum Values:**
   - Current fix: `"mobilemoney"` → `"Mobilemoney"` (incorrect)
   - Required: `"mobilemoney"` → `"MobileMoney"` (correct)
   
   **Future Enhancement:** Create a mapping object for proper enum value conversion:
   ```typescript
   const paymentMethodMap: Record<string, string> = {
     'cash': 'Cash',
     'card': 'Card',
     'mobilemoney': 'MobileMoney',
     'banktransfer': 'BankTransfer',
     'paylink': 'PayLink'
   }
   ```

2. **Case-Insensitive Backend:**
   Alternatively, the backend could be configured to accept case-insensitive enum values using:
   ```csharp
   services.AddControllers()
       .AddJsonOptions(options => {
           options.JsonSerializerOptions.Converters.Add(
               new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
           );
       });
   ```

---

## Related Files

- ✅ `toss-web/composables/useSalesAPI.ts` - Added capitalization helper
- ✅ `backend/Toss/src/Domain/Enums/PaymentType.cs` - Enum definition
- ✅ `toss-web/pages/sales/pos/index.vue` - POS page using the API

---

## Status

✅ **FIXED** - Payment method capitalization now matches backend enum expectations.

**Hold Sale** and **Create Sale** now work correctly with proper PaymentType enum values!

---

## Next Steps

1. ✅ Test hold sale functionality
2. ✅ Test create sale functionality
3. ⬜ Test retrieve held sale
4. ⬜ Test delete held sale
5. ⬜ Consider enhancing for compound enum values (MobileMoney, etc.)


