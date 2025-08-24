<template>
  <Card title="Profit & Loss Summary" subtitle="Monthly profit and loss analysis">
    <template #headerActions>
      <div class="flex items-center space-x-2">
        <select 
          v-model="selectedPeriod" 
          class="px-3 py-1 text-sm border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500"
        >
          <option value="current">Current Month</option>
          <option value="previous">Previous Month</option>
          <option value="quarter">This Quarter</option>
          <option value="year">This Year</option>
        </select>
        <button 
          @click="exportReport"
          class="px-3 py-1 text-sm bg-orange-600 text-white rounded hover:bg-orange-700"
        >
          Export
        </button>
      </div>
    </template>

    <div class="space-y-6">
      <!-- Revenue Section -->
      <div class="bg-green-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-green-900 mb-3">Revenue</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div class="text-center">
            <p class="text-2xl font-bold text-green-600">{{ formatCurrency(revenue.total) }}</p>
            <p class="text-sm text-green-700">Total Revenue</p>
          </div>
          <div class="text-center">
            <p class="text-lg font-semibold text-green-600">{{ formatCurrency(revenue.sales) }}</p>
            <p class="text-sm text-green-700">Sales</p>
          </div>
          <div class="text-center">
            <p class="text-lg font-semibold text-green-600">{{ formatCurrency(revenue.other) }}</p>
            <p class="text-sm text-green-700">Other Income</p>
          </div>
        </div>
      </div>

      <!-- Cost of Goods Sold Section -->
      <div class="bg-red-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-red-900 mb-3">Cost of Goods Sold</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div class="text-center">
            <p class="text-2xl font-bold text-red-600">{{ formatCurrency(cogs.total) }}</p>
            <p class="text-sm text-red-700">Total COGS</p>
          </div>
          <div class="text-center">
            <p class="text-lg font-semibold text-red-600">{{ formatCurrency(cogs.inventory) }}</p>
            <p class="text-sm text-red-700">Inventory</p>
          </div>
          <div class="text-center">
            <p class="text-lg font-semibold text-red-600">{{ formatCurrency(cogs.labor) }}</p>
            <p class="text-sm text-red-700">Direct Labor</p>
          </div>
        </div>
      </div>

      <!-- Gross Profit -->
      <div class="bg-blue-50 p-4 rounded-lg">
        <div class="text-center">
          <p class="text-3xl font-bold text-blue-600">{{ formatCurrency(grossProfit) }}</p>
          <p class="text-lg text-blue-700">Gross Profit</p>
          <p class="text-sm text-blue-600">Margin: {{ grossProfitMargin }}%</p>
        </div>
      </div>

      <!-- Operating Expenses -->
      <div class="bg-yellow-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-yellow-900 mb-3">Operating Expenses</h3>
        <div class="space-y-3">
          <div v-for="expense in operatingExpenses" :key="expense.category" class="flex justify-between items-center">
            <span class="text-sm text-yellow-800">{{ expense.category }}</span>
            <span class="font-medium text-yellow-900">{{ formatCurrency(expense.amount) }}</span>
          </div>
          <div class="border-t border-yellow-200 pt-2 flex justify-between items-center">
            <span class="font-medium text-yellow-900">Total Operating Expenses</span>
            <span class="text-lg font-bold text-yellow-900">{{ formatCurrency(totalOperatingExpenses) }}</span>
          </div>
        </div>
      </div>

      <!-- Net Profit -->
      <div class="bg-purple-50 p-4 rounded-lg">
        <div class="text-center">
          <p class="text-3xl font-bold text-purple-600">{{ formatCurrency(netProfit) }}</p>
          <p class="text-lg text-purple-700">Net Profit</p>
          <p class="text-sm text-purple-600">Net Margin: {{ netProfitMargin }}%</p>
        </div>
      </div>

      <!-- Period Comparison -->
      <div class="bg-gray-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-gray-900 mb-3">Period Comparison</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="text-center">
            <p class="text-sm text-gray-600">Previous Period</p>
            <p class="text-lg font-semibold text-gray-900">{{ formatCurrency(previousPeriod.netProfit) }}</p>
          </div>
          <div class="text-center">
            <p class="text-sm text-gray-600">Change</p>
            <p 
              class="text-lg font-semibold"
              :class="periodChange >= 0 ? 'text-green-600' : 'text-red-600'"
            >
              {{ periodChange >= 0 ? '+' : '' }}{{ formatCurrency(periodChange) }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="flex justify-between items-center text-sm text-gray-500">
        <span>Last updated: {{ lastUpdated }}</span>
        <span>Report generated for: {{ selectedPeriodLabel }}</span>
      </div>
    </template>
  </Card>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'

// Props
interface Props {
  period?: string
  autoRefresh?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  period: 'current',
  autoRefresh: false
})

// Reactive state
const selectedPeriod = ref(props.period)
const loading = ref(false)
const lastUpdated = ref('')

// Mock data - replace with actual API calls
const revenue = ref({
  total: 125000,
  sales: 120000,
  other: 5000
})

const cogs = ref({
  total: 75000,
  inventory: 60000,
  labor: 15000
})

const operatingExpenses = ref([
  { category: 'Salaries & Wages', amount: 25000 },
  { category: 'Rent & Utilities', amount: 8000 },
  { category: 'Marketing', amount: 5000 },
  { category: 'Office Supplies', amount: 2000 },
  { category: 'Insurance', amount: 3000 },
  { category: 'Other', amount: 2000 }
])

const previousPeriod = ref({
  netProfit: 15000
})

// Computed properties
const grossProfit = computed(() => revenue.value.total - cogs.value.total)
const grossProfitMargin = computed(() => ((grossProfit.value / revenue.value.total) * 100).toFixed(1))

const totalOperatingExpenses = computed(() => 
  operatingExpenses.value.reduce((sum: number, expense: any) => sum + expense.amount, 0)
)

const netProfit = computed(() => grossProfit.value - totalOperatingExpenses.value)
const netProfitMargin = computed(() => ((netProfit.value / revenue.value.total) * 100).toFixed(1))

const periodChange = computed(() => netProfit.value - previousPeriod.value.netProfit)

const selectedPeriodLabel = computed(() => {
  const labels = {
    current: 'Current Month',
    previous: 'Previous Month',
    quarter: 'This Quarter',
    year: 'This Year'
  }
  return labels[selectedPeriod.value as keyof typeof labels] || 'Current Month'
})

// Methods
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const exportReport = async () => {
  try {
    loading.value = true
    // Implement export functionality
    console.log('Exporting P&L report for period:', selectedPeriod.value)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Create and download CSV
    const csvContent = generateCSV()
    downloadCSV(csvContent, `pl-summary-${selectedPeriod.value}.csv`)
    
  } catch (error) {
    console.error('Export failed:', error)
  } finally {
    loading.value = false
  }
}

const generateCSV = (): string => {
  const headers = ['Category', 'Amount', 'Percentage']
  const rows = [
    ['Total Revenue', revenue.value.total, '100%'],
    ['Cost of Goods Sold', cogs.value.total, ((cogs.value.total / revenue.value.total) * 100).toFixed(1) + '%'],
    ['Gross Profit', grossProfit.value, grossProfitMargin.value + '%'],
    ['Operating Expenses', totalOperatingExpenses.value, ((totalOperatingExpenses.value / revenue.value.total) * 100).toFixed(1) + '%'],
    ['Net Profit', netProfit.value, netProfitMargin.value + '%']
  ]
  
  return [headers, ...rows].map(row => row.join(',')).join('\n')
}

const downloadCSV = (content: string, filename: string) => {
  const blob = new Blob([content], { type: 'text/csv' })
  const url = window.URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = filename
  a.click()
  window.URL.revokeObjectURL(url)
}

const refreshData = async () => {
  try {
    loading.value = true
    // Implement actual API call here
    await new Promise(resolve => setTimeout(resolve, 1000))
    lastUpdated.value = new Date().toLocaleString()
  } catch (error) {
    console.error('Failed to refresh data:', error)
  } finally {
    loading.value = false
  }
}

// Lifecycle
onMounted(() => {
  lastUpdated.value = new Date().toLocaleString()
  if (props.autoRefresh) {
    setInterval(refreshData, 300000) // Refresh every 5 minutes
  }
})

// Watch for period changes
watch(selectedPeriod, () => {
  refreshData()
})
</script>

<style scoped>
/* Component-specific styles */
</style>
