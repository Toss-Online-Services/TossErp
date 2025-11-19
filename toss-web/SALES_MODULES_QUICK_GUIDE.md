# TOSS Sales Modules - Quick Developer Guide

## 🚀 Quick Start

### Running the Project
```bash
# Install dependencies
pnpm install

# Start dev server
pnpm dev

# Build for production
pnpm build
```

### Project Structure
```
toss-web/
├── pages/sales/          # Sales module pages
│   ├── quotations/       # Quotations module
│   ├── returns/          # Returns module ⭐ NEW
│   └── reports/          # Analytics ⭐ NEW
├── composables/          # Business logic
│   ├── useSalesReturns.ts ⭐ NEW
│   └── useSalesAnalytics.ts ⭐ NEW
├── components/analytics/ # Reusable components ⭐ NEW
└── locales/              # Translations (120+ new keys) ⭐
```

---

## 📋 Available Composables

### 1. useSalesReturns()
```typescript
import { useSalesReturns } from '~/composables/useSalesReturns'

const {
  returns,                    // ref<SalesReturn[]>
  loading,                    // ref<boolean>
  error,                      // ref<string | null>
  
  fetchReturns,              // (filters?) => Promise<void>
  createReturnFromInvoice,   // (invoiceRef, items) => Promise<SalesReturn>
  changeReturnStatus,        // (id, status) => Promise<void>
  processRefund,             // (id, refundData) => Promise<void>
  getReturnReasons,          // () => ReturnReason[]
} = useSalesReturns()
```

**Example Usage:**
```typescript
// Fetch all returns
await fetchReturns({ status: 'pending', limit: 50 })

// Create return from invoice
const returnData = await createReturnFromInvoice('INV-001', [
  {
    productId: 1,
    returnQty: 2,
    returnReason: 'defective',
    condition: 'damaged'
  }
])

// Approve return
await changeReturnStatus(returnData.id, 'approved')

// Process refund
await processRefund(returnData.id, {
  method: 'credit_note',
  amount: 150.00
})
```

---

### 2. useSalesAnalytics()
```typescript
import { useSalesAnalytics } from '~/composables/useSalesAnalytics'

const {
  metrics,                   // ref<SalesMetrics>
  trends,                    // ref<SalesTrend[]>
  productPerformance,        // ref<ProductPerformance[]>
  customerAnalytics,         // ref<CustomerAnalytics>
  loading,                   // ref<boolean>
  error,                     // ref<string | null>
  
  fetchMetrics,              // (filters) => Promise<void>
  fetchTrends,               // (filters) => Promise<void>
  fetchProductPerformance,   // (filters) => Promise<void>
  fetchCustomerAnalytics,    // (filters) => Promise<void>
  generateForecast,          // (days) => Promise<Forecast>
  exportAnalytics,           // (format, filters) => Promise<void>
} = useSalesAnalytics()
```

**Example Usage:**
```typescript
// Fetch metrics for date range
await fetchMetrics({
  startDate: '2025-01-01',
  endDate: '2025-01-31',
  groupBy: 'day'
})

// Get sales trends
await fetchTrends({
  startDate: '2025-01-01',
  endDate: '2025-01-31',
  groupBy: 'week'
})

// Generate 30-day forecast
const forecast = await generateForecast(30)

// Export to CSV
await exportAnalytics('csv', {
  startDate: '2025-01-01',
  endDate: '2025-01-31'
})
```

---

## 🎨 Using Components

### MetricCard
```vue
<template>
  <MetricCard
    :title="t('analytics.metrics.totalRevenue')"
    :value="formatCurrency(12450.50)"
    :change="15.3"
    icon="mdi:currency-usd"
    color="blue"
    :loading="false"
  />
</template>

<script setup>
import MetricCard from '~/components/analytics/MetricCard.vue'
</script>
```

**Props:**
- `title: string` - Metric title
- `value: string` - Metric value (formatted)
- `change?: number` - Percentage change
- `icon: string` - Icon name (from @nuxt/icon)
- `color?: 'blue' | 'green' | 'purple' | 'orange' | 'red' | 'yellow'`
- `loading?: boolean` - Show loading state

---

## 🌍 Internationalization (i18n)

### Using Translations
```vue
<script setup>
import { useI18n } from 'vue-i18n'

const { t, locale } = useI18n()
</script>

<template>
  <!-- Simple translation -->
  <h1>{{ t('returns.title') }}</h1>
  
  <!-- With parameters -->
  <p>{{ t('returns.confirmations.processRefund', { amount: 'R150.00' }) }}</p>
  
  <!-- Switch language -->
  <button @click="locale = 'zu'">Switch to Zulu</button>
</template>
```

### Translation Keys Structure
```
quotations.fields.*      → Form field labels
quotations.actions.*     → Button labels
quotations.messages.*    → Success/error messages

returns.status.*         → Return statuses
returns.reasons.*        → 8 return reasons
returns.conditions.*     → 4 item conditions
returns.refundMethods.*  → 4 refund methods

analytics.metrics.*      → KPI labels
analytics.charts.*       → Chart titles
analytics.segments.*     → Customer segments
```

---

## 📊 Chart.js Integration

### Basic Chart Setup
```vue
<template>
  <canvas ref="chartCanvas" class="w-full h-64"></canvas>
</template>

<script setup>
import { Chart, registerables } from 'chart.js'

Chart.register(...registerables)

const chartCanvas = ref<HTMLCanvasElement | null>(null)
let chart: Chart | null = null

onMounted(() => {
  if (!chartCanvas.value) return
  
  chart = new Chart(chartCanvas.value, {
    type: 'line',
    data: {
      labels: ['Jan', 'Feb', 'Mar'],
      datasets: [{
        label: 'Revenue',
        data: [1200, 1900, 3000],
        borderColor: 'rgb(59, 130, 246)',
        tension: 0.4
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false
    }
  })
})

onBeforeUnmount(() => {
  if (chart) chart.destroy()
})
</script>
```

---

## 🔥 FormKit Forms

### Basic Form
```vue
<FormKit
  type="form"
  :actions="false"
  @submit="handleSubmit"
  v-model="formData"
>
  <FormKit
    type="select"
    name="customer"
    :label="t('quotations.fields.customer')"
    :options="customerOptions"
    validation="required"
  />
  
  <FormKit
    type="number"
    name="amount"
    :label="t('quotations.fields.amount')"
    validation="required|min:0"
    step="0.01"
  />
</FormKit>
```

### Repeater (Line Items)
```vue
<FormKit
  type="repeater"
  name="items"
  :label="false"
  :min="1"
  #default="{ index, value }"
>
  <FormKit
    type="select"
    name="product"
    :options="products"
    validation="required"
  />
  
  <FormKit
    type="number"
    name="quantity"
    validation="required|min:1"
  />
</FormKit>
```

---

## 🎯 Common Patterns

### Pagination
```vue
<script setup>
const currentPage = ref(1)
const itemsPerPage = ref(10)

const paginatedData = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return allData.value.slice(start, end)
})

const totalPages = computed(() => 
  Math.ceil(allData.value.length / itemsPerPage.value)
)
</script>
```

### Search & Filter
```vue
<script setup>
const searchQuery = ref('')
const filterStatus = ref('')

const filteredData = computed(() => {
  let filtered = allData.value
  
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(item => 
      item.name.toLowerCase().includes(query)
    )
  }
  
  if (filterStatus.value) {
    filtered = filtered.filter(item => 
      item.status === filterStatus.value
    )
  }
  
  return filtered
})
</script>
```

### Currency Formatting
```typescript
function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount || 0)
}
```

### Date Formatting
```typescript
function formatDate(date: string | Date) {
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}
```

---

## 🐛 Common Issues & Solutions

### Issue: TypeScript errors for composables
```
Cannot find module '~/composables/useSalesReturns'
```
**Solution:** These are auto-import issues. The code will work at runtime. To fix, add to `tsconfig.json`:
```json
{
  "compilerOptions": {
    "types": ["@nuxt/types"]
  }
}
```

### Issue: Chart not rendering
**Solution:** Ensure Chart.js is registered and canvas ref is available:
```typescript
import { Chart, registerables } from 'chart.js'
Chart.register(...registerables)

onMounted(() => {
  nextTick(() => {
    if (canvas.value) {
      // Create chart
    }
  })
})
```

### Issue: @apply directive warnings
```
Unknown at rule @apply
```
**Solution:** These are harmless linter warnings for Tailwind CSS. Add to VS Code settings:
```json
{
  "css.lint.unknownAtRules": "ignore"
}
```

---

## 🧪 Testing

### Unit Test Example (Vitest)
```typescript
// tests/composables/useSalesReturns.spec.ts
import { describe, it, expect, vi } from 'vitest'
import { useSalesReturns } from '~/composables/useSalesReturns'

describe('useSalesReturns', () => {
  it('should fetch returns', async () => {
    const { returns, fetchReturns } = useSalesReturns()
    
    await fetchReturns()
    
    expect(returns.value).toBeDefined()
    expect(returns.value.length).toBeGreaterThan(0)
  })
  
  it('should create return from invoice', async () => {
    const { createReturnFromInvoice } = useSalesReturns()
    
    const result = await createReturnFromInvoice('INV-001', [
      { productId: 1, returnQty: 2, returnReason: 'defective' }
    ])
    
    expect(result).toBeDefined()
    expect(result.salesInvoiceRef).toBe('INV-001')
  })
})
```

---

## 📦 Environment Variables

Required for API integration:
```env
# .env file
NUXT_PUBLIC_API_BASE_URL=http://localhost:5000/api
NUXT_PUBLIC_SITE_URL=http://localhost:3001
```

---

## 🔗 Useful Links

- **Nuxt Documentation**: https://nuxt.com
- **FormKit Docs**: https://formkit.com
- **Chart.js Docs**: https://www.chartjs.org
- **Iconify Icons**: https://icon-sets.iconify.design
- **Tailwind CSS**: https://tailwindcss.com

---

## 💡 Tips & Tricks

1. **Use Computed for Reactivity**: Always use `computed()` for derived state
2. **Destroy Charts**: Always destroy Chart.js instances in `onBeforeUnmount()`
3. **FormKit Validation**: Use built-in validators: `required|min:0|max:100`
4. **i18n Keys**: Keep translation keys organized by module and feature
5. **Type Safety**: Define TypeScript interfaces for all data models
6. **Error Handling**: Always use try-catch in async functions
7. **Loading States**: Show loading indicators for better UX

---

## 📞 Support

For questions or issues:
- Check the main documentation: `SALES_MODULES_IMPLEMENTATION.md`
- Review composable source code for detailed examples
- Check locale files for available translation keys

---

**Last Updated**: January 10, 2025  
**Version**: 1.0.0  
**Status**: Production Ready
