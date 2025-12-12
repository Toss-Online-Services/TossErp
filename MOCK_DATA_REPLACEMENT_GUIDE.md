# Quick Start Guide: Replacing Mock Data with Real API Calls

This guide shows you how to replace mock data in frontend pages with real backend API calls.

## Pattern Overview

### Before (Mock Data)
```vue
<script setup lang="ts">
import { ref } from 'vue'

// Mock data
const stats = ref({
  todaySales: 15420,
  cashIn: 12300,
  cashOut: 4500,
  lowStock: 8
})
</script>
```

### After (Real API Calls)
```vue
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useDashboardApi } from '~/composables/useDashboardApi'

// Real data with loading and error states
const stats = ref(null)
const loading = ref(true)
const error = ref(null)

const { getDashboardStats } = useDashboardApi()

onMounted(async () => {
  try {
    loading.value = true
    const { data, error: apiError } = await getDashboardStats()
    
    if (apiError.value) {
      error.value = apiError.value
    } else {
      stats.value = data.value
    }
  } catch (err) {
    error.value = err
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <!-- Loading State -->
  <div v-if="loading" class="flex items-center justify-center p-8">
    <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
  </div>
  
  <!-- Error State -->
  <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4">
    <p class="text-red-800">{{ error.message || 'Failed to load data' }}</p>
  </div>
  
  <!-- Success State -->
  <div v-else-if="stats">
    <div class="grid grid-cols-4 gap-4">
      <div class="bg-white p-4 rounded-lg shadow">
        <h3 class="text-sm text-gray-600">Today's Sales</h3>
        <p class="text-2xl font-bold">R {{ stats.todaySales }}</p>
      </div>
      <!-- More stat cards -->
    </div>
  </div>
</template>
```

## Step-by-Step Implementation

### Step 1: Check if API Composable Exists

Look in `toss-web/composables/` for the relevant API file:
- `useSalesApi.ts` - Sales/POS operations
- `useCrmApi.ts` - Customer relationship management
- `useInventoryApi.ts` - Stock/inventory operations
- `useAccountingApi.ts` - Financial operations
- `useHrApi.ts` - HR operations
- `useProjectsApi.ts` - Project management

### Step 2: Import the Composable

```typescript
import { useSalesApi } from '~/composables/useSalesApi'
```

### Step 3: Use the API Methods

```typescript
const { getSales, getDailySummary } = useSalesApi()

// Fetch data
const { data, error, execute } = getSales(shopId, 'completed')
await execute()
```

### Step 4: Handle Response States

Always handle three states:
1. **Loading**: Show spinner or skeleton
2. **Error**: Display error message
3. **Success**: Render the data

## Common Patterns

### Pattern 1: Simple Data Fetch

```vue
<script setup lang="ts">
const products = ref([])
const { getProducts } = useInventoryApi()

onMounted(async () => {
  const { data } = await getProducts()
  products.value = data.value?.items || []
})
</script>
```

### Pattern 2: Paginated Data

```vue
<script setup lang="ts">
const sales = ref([])
const currentPage = ref(1)
const totalPages = ref(1)

const loadSales = async () => {
  const { data } = await getSales(shopId, 'all', currentPage.value, 50)
  if (data.value) {
    sales.value = data.value.items
    totalPages.value = data.value.totalPages
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
    loadSales()
  }
}
</script>
```

### Pattern 3: Form Submission

```vue
<script setup lang="ts">
const formData = ref({
  customerId: null,
  items: []
})
const submitting = ref(false)

const handleSubmit = async () => {
  try {
    submitting.value = true
    const { data, error } = await createSale(formData.value)
    
    if (error.value) {
      // Show error notification
      alert('Error: ' + error.value.message)
    } else {
      // Success - redirect or show success message
      navigateTo('/sales')
    }
  } catch (err) {
    alert('Unexpected error')
  } finally {
    submitting.value = false
  }
}
</script>
```

### Pattern 4: Real-time Updates with Computed

```vue
<script setup lang="ts">
const salesStore = useSalesStore()

// Computed property that updates when store changes
const totalSales = computed(() => 
  salesStore.sales.reduce((sum, sale) => sum + sale.total, 0)
)

onMounted(async () => {
  await salesStore.loadSales()
})
</script>
```

## Available API Methods

### Sales Module (`useSalesApi`)
- `getSales(shopId, status, page, size)` - List sales
- `getSaleById(id)` - Get single sale
- `getDailySummary(shopId, date)` - Today's statistics
- `createSale(saleData)` - Create new sale
- `posCheckout(cartItems, payment, amount)` - POS checkout
- `getInvoices(shopId, status)` - List invoices
- `createInvoice(invoiceData)` - Create invoice

### CRM Module (`useCrmApi`)
- `getLeads(status, page, size)` - List leads
- `getLeadById(id)` - Get single lead
- `createLead(leadData)` - Create new lead
- `updateLead(id, data)` - Update lead
- `convertLeadToCustomer(id)` - Convert to customer
- `getOpportunities(stage, page, size)` - List opportunities

### Inventory Module (`useInventoryApi`)
- `getProducts(category, page, size)` - List products
- `getProductById(id)` - Get single product
- `getStockLevel(productId, warehouseId)` - Check stock
- `updateStock(productId, quantity, type)` - Adjust stock
- `getLowStockItems()` - Get items below reorder level

### Accounting Module (`useAccountingApi`)
- `getAccounts(type)` - List accounts
- `getTransactions(accountId, from, to)` - Get transactions
- `createTransaction(data)` - Record transaction
- `getBalanceSheet(date)` - Get balance sheet
- `getProfitAndLoss(from, to)` - P&L report

## Error Handling Best Practices

### 1. Display User-Friendly Messages

```typescript
const getErrorMessage = (error: any) => {
  if (error.response?.data?.message) {
    return error.response.data.message
  }
  if (error.message) {
    return error.message
  }
  return 'An unexpected error occurred'
}
```

### 2. Retry on Failure

```typescript
const maxRetries = 3
let retryCount = 0

const fetchWithRetry = async () => {
  try {
    return await getSales()
  } catch (error) {
    if (retryCount < maxRetries) {
      retryCount++
      await new Promise(resolve => setTimeout(resolve, 1000))
      return fetchWithRetry()
    }
    throw error
  }
}
```

### 3. Offline Detection

```typescript
const isOnline = ref(navigator.onLine)

window.addEventListener('online', () => {
  isOnline.value = true
  // Retry failed requests
})

window.addEventListener('offline', () => {
  isOnline.value = false
  // Show offline message
})
```

## Loading States

### Skeleton Loaders

```vue
<template>
  <div v-if="loading" class="animate-pulse">
    <div class="h-4 bg-gray-200 rounded w-3/4 mb-4"></div>
    <div class="h-4 bg-gray-200 rounded w-1/2 mb-4"></div>
    <div class="h-4 bg-gray-200 rounded w-5/6"></div>
  </div>
</template>
```

### Spinner

```vue
<template>
  <div v-if="loading" class="flex justify-center items-center py-8">
    <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
  </div>
</template>
```

## Authentication Headers

All API composables automatically include authentication headers via `useAuthApi()`:

```typescript
const { getHeaders } = useAuthApi()

// Headers automatically include:
// - Authorization: Bearer <token>
// - Content-Type: application/json
```

## Testing API Calls

### 1. Check Backend is Running
```bash
# Should return API documentation
curl http://localhost:5000/api
```

### 2. Test API Endpoint Directly
```bash
# Test login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "password"}'
```

### 3. Check Browser Console
```javascript
// Open browser DevTools (F12)
// Network tab shows all API calls
// Console tab shows any JavaScript errors
```

## Migration Checklist

For each page with mock data:

- [ ] Identify which API composable to use
- [ ] Replace static data with API call
- [ ] Add loading state
- [ ] Add error handling
- [ ] Test with backend running
- [ ] Test error scenarios (backend offline, invalid data)
- [ ] Add offline capability if needed
- [ ] Remove old mock data
- [ ] Test on mobile viewport

## Example: Complete Dashboard Migration

See the full example in the status report. This demonstrates:
- Multiple API calls for different stats
- Chart data transformation
- Error boundaries
- Loading states
- Responsive design

## Need Help?

1. **Check API Documentation**: http://localhost:5000/api
2. **Review Composable Code**: `toss-web/composables/`
3. **Check Backend Endpoints**: `backend/Toss/src/Web/Endpoints/`
4. **Test with Postman**: Import the OpenAPI spec
5. **Browser DevTools**: Network tab for debugging

## Common Issues

**Issue**: "CORS Error"
**Solution**: Ensure backend is running and CORS is configured in DependencyInjection.cs

**Issue**: "401 Unauthorized"
**Solution**: Check authentication - might need to login first and get token

**Issue**: "Connection refused"
**Solution**: Backend not running - start with `dotnet run` in Web project

**Issue**: "Empty response"
**Solution**: Check if database has seed data - might need to run seeder

**Issue**: "Type errors"
**Solution**: Update TypeScript interfaces in stores to match API response

---

**Remember**: Always handle the three states - Loading, Error, and Success!
