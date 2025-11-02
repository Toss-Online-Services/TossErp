# POS Page Browser Test Guide

## Testing Categories and Products Display

### Step 1: Access the POS Page
1. Open your browser and navigate to: `http://localhost:3001/sales/pos`
2. The page should load without errors

### Step 2: Verify Categories Display
1. **Check Category Section**: Look for the category filter buttons below the search bar
2. **Expected Behavior**:
   - Should see "All" category button first
   - Should see all categories from the API listed as buttons
   - Each category should show product count: `Category Name (X)`
   - Categories should be clickable and highlight when selected

### Step 3: Verify Products Display
1. **Check Products Grid**: Look for the product grid below categories
2. **Expected Behavior**:
   - Products should be displayed in a grid layout
   - Each product should show:
     - Product image (or placeholder icon)
     - Product name
     - SKU
     - Price (R format)
     - Stock status (with color coding)
   - Products should be clickable to add to cart

### Step 4: Test Category Filtering
1. Click on a specific category button (not "All")
2. **Expected Behavior**:
   - Products grid should filter to show only products from that category
   - "All" button should no longer be highlighted
   - Selected category button should be highlighted in blue

### Step 5: Test Search Functionality
1. Type a product name or SKU in the search box
2. **Expected Behavior**:
   - Products should filter in real-time
   - Search works across product name, SKU, and barcode

### Step 6: Check Browser Console
1. Open Developer Tools (F12)
2. Go to Console tab
3. **Look for these logs**:
   - ✅ `🔍 Fetching categories for shopId: X`
   - ✅ `📁 Categories response: [...]`
   - ✅ `🔍 Fetching products for shopId: X`
   - ✅ `📦 Products response received: [...]`
   - ✅ `📦 Raw products list length: X`
   - ✅ `Transformed products: X items`
   - ✅ `Final categories: X categories`
   - ✅ `Category IDs: [...]`

### Step 7: Verify Data Structure
In the browser console, check:
```javascript
// Should show categories array with structure:
{
  id: 'all' | number,
  name: string,
  productCount: number
}

// Should show products array with structure:
{
  id: number,
  name: string,
  sku: string,
  price: number,
  categoryId: number,
  category: string,
  stock: number,
  image: string | null,
  barcode: string
}
```

## Common Issues to Check

### Issue 1: Categories Not Displaying
- **Check**: Browser console for API errors
- **Fix**: Verify API is returning categories array
- **Look for**: `📁 Raw categories response: [...]` in console

### Issue 2: Products Not Displaying
- **Check**: Browser console for API errors
- **Fix**: Verify API is returning products array
- **Look for**: `📦 Products response received: [...]` in console

### Issue 3: Category Filter Not Working
- **Check**: Console logs for filtering logic
- **Fix**: Verify `categoryId` is properly mapped in products
- **Look for**: `🔄 Computing filteredProducts...` logs

### Issue 4: Type Mismatch Errors
- **Check**: Console for comparison errors
- **Fix**: Verify category IDs are numbers (not strings)
- **Look for**: `📝 Product categoryId: X Type: number` in console

## Expected Console Output

When page loads successfully, you should see:
```
🔍 Fetching categories for shopId: 1
📁 Categories response: [{id: 1, name: "Category 1", ...}, ...]
📁 Categories count: X
📊 Product count by category: {1: 5, 2: 3, ...}
✅ Final categories: X categories

🔍 Fetching products for shopId: 1
📦 Products response received: [...]
📦 Raw products list length: X
📝 Sample transformed product: {id: 1, name: "...", categoryId: 1, ...}
✅ Transformed products: X items

🔄 Computing filteredProducts...
   - Total products: X
   - Selected category: all
   ✅ Final filtered products: X
```

## Test Checklist

- [ ] Page loads without errors
- [ ] Categories are visible and displayed correctly
- [ ] Products are visible and displayed correctly
- [ ] Category buttons are clickable
- [ ] Category filtering works (click category → products filter)
- [ ] "All" category shows all products
- [ ] Product count displays on categories
- [ ] Search functionality works
- [ ] Products can be added to cart
- [ ] Browser console shows no errors
- [ ] Console logs show proper data transformation

## If Issues Persist

1. **Check API Response**: Open Network tab and check `/api/Inventory/categories` and `/api/Inventory/products` responses
2. **Verify Data Structure**: Compare actual API response with expected structure in console
3. **Check TypeScript Errors**: Look for any type errors in the console
4. **Refresh Page**: Sometimes a hard refresh (Ctrl+F5) helps clear cached issues

