<template>
  <Card title="Cash Position" subtitle="Current cash balances and liquidity analysis">
    <template #headerActions>
      <div class="flex items-center space-x-2">
        <select 
          v-model="selectedDate" 
          class="px-3 py-1 text-sm border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500"
        >
          <option value="today">Today</option>
          <option value="week">This Week</option>
          <option value="month">This Month</option>
          <option value="quarter">This Quarter</option>
        </select>
        <button 
          @click="refreshData"
          class="px-3 py-1 text-sm bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          Refresh
        </button>
      </div>
    </template>

    <div class="space-y-6">
      <!-- Cash Balance Overview -->
      <div class="bg-blue-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-blue-900 mb-3">Cash Balance Overview</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div class="text-center">
            <p class="text-2xl font-bold text-blue-600">{{ formatCurrency(cashBalance.total) }}</p>
            <p class="text-sm text-blue-700">Total Cash</p>
          </div>
          <div class="text-center">
            <p class="text-lg font-semibold text-blue-600">{{ formatCurrency(cashBalance.available) }}</p>
            <p class="text-sm text-blue-700">Available</p>
          </div>
          <div class="text-center">
            <p class="text-lg font-semibold text-blue-600">{{ formatCurrency(cashBalance.reserved) }}</p>
            <p class="text-sm text-blue-700">Reserved</p>
          </div>
        </div>
      </div>

      <!-- Bank Accounts -->
      <div class="bg-green-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-green-900 mb-3">Bank Accounts</h3>
        <div class="space-y-3">
          <div v-for="account in bankAccounts" :key="account.id" class="flex justify-between items-center p-3 bg-white rounded-lg">
            <div>
              <p class="font-medium text-green-900">{{ account.name }}</p>
              <p class="text-sm text-green-700">{{ account.accountNumber }}</p>
            </div>
            <div class="text-right">
              <p class="font-bold text-green-600">{{ formatCurrency(account.balance) }}</p>
              <p class="text-xs text-green-600">{{ account.status }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Cash Flow -->
      <div class="bg-purple-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-purple-900 mb-3">Cash Flow ({{ selectedDateLabel }})</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="text-center">
            <p class="text-lg font-semibold text-green-600">{{ formatCurrency(cashFlow.inflow) }}</p>
            <p class="text-sm text-green-700">Cash Inflow</p>
          </div>
          <div class="text-center">
            <p class="text-lg font-semibold text-red-600">{{ formatCurrency(cashFlow.outflow) }}</p>
            <p class="text-sm text-red-700">Cash Outflow</p>
          </div>
        </div>
        <div class="mt-4 text-center">
          <p class="text-xl font-bold" :class="netCashFlow >= 0 ? 'text-green-600' : 'text-red-600'">
            {{ netCashFlow >= 0 ? '+' : '' }}{{ formatCurrency(netCashFlow) }}
          </p>
          <p class="text-sm text-purple-700">Net Cash Flow</p>
        </div>
      </div>

      <!-- Liquidity Metrics -->
      <div class="bg-yellow-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-yellow-900 mb-3">Liquidity Metrics</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="text-center">
            <p class="text-lg font-semibold text-yellow-600">{{ currentRatio.toFixed(2) }}</p>
            <p class="text-sm text-yellow-700">Current Ratio</p>
          </div>
          <div class="text-center">
            <p class="text-lg font-semibold text-yellow-600">{{ quickRatio.toFixed(2) }}</p>
            <p class="text-sm text-yellow-700">Quick Ratio</p>
          </div>
        </div>
      </div>

      <!-- Cash Forecast -->
      <div class="bg-indigo-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-indigo-900 mb-3">30-Day Cash Forecast</h3>
        <div class="space-y-3">
          <div v-for="forecast in cashForecast" :key="forecast.date" class="flex justify-between items-center">
            <span class="text-sm text-indigo-800">{{ forecast.date }}</span>
            <span class="font-medium text-indigo-900">{{ formatCurrency(forecast.balance) }}</span>
            <span class="text-xs text-indigo-600">{{ forecast.change >= 0 ? '+' : '' }}{{ formatCurrency(forecast.change) }}</span>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="flex justify-between items-center text-sm text-gray-500">
        <span>Last updated: {{ lastUpdated }}</span>
        <span>Data as of: {{ selectedDateLabel }}</span>
      </div>
    </template>
  </Card>
</template>

<script setup lang="ts">
// Props
interface Props {
  date?: string
  autoRefresh?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  date: 'today',
  autoRefresh: false
})

// Reactive state
const selectedDate = ref(props.date)
const loading = ref(false)
const lastUpdated = ref('')

// Mock data - replace with actual API calls
const cashBalance = ref({
  total: 85000,
  available: 75000,
  reserved: 10000
})

const bankAccounts = ref([
  { id: 1, name: 'Main Business Account', accountNumber: '1234-5678-9012', balance: 60000, status: 'Active' },
  { id: 2, name: 'Savings Account', accountNumber: '9876-5432-1098', balance: 20000, status: 'Active' },
  { id: 3, name: 'Petty Cash', accountNumber: 'N/A', balance: 5000, status: 'Active' }
])

const cashFlow = ref({
  inflow: 45000,
  outflow: 32000
})

const cashForecast = ref([
  { date: 'Today', balance: 85000, change: 0 },
  { date: 'Day 7', balance: 92000, change: 7000 },
  { date: 'Day 14', balance: 88000, change: -4000 },
  { date: 'Day 21', balance: 95000, change: 7000 },
  { date: 'Day 30', balance: 102000, change: 7000 }
])

// Computed properties
const netCashFlow = computed(() => cashFlow.value.inflow - cashFlow.value.outflow)

const currentRatio = computed(() => {
  // Mock calculation - replace with actual data
  const currentAssets = cashBalance.value.total + 50000 // cash + receivables
  const currentLiabilities = 30000 // payables + short-term debt
  return currentAssets / currentLiabilities
})

const quickRatio = computed(() => {
  // Mock calculation - replace with actual data
  const quickAssets = cashBalance.value.total + 25000 // cash + receivables (excluding inventory)
  const currentLiabilities = 30000
  return quickAssets / currentLiabilities
})

const selectedDateLabel = computed(() => {
  const labels = {
    today: 'Today',
    week: 'This Week',
    month: 'This Month',
    quarter: 'This Quarter'
  }
  return labels[selectedDate.value as keyof typeof labels] || 'Today'
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

// Watch for date changes
watch(selectedDate, () => {
  refreshData()
})
</script>

<style scoped>
/* Component-specific styles */
</style>
