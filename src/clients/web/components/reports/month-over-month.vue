<template>
  <Card title="Month-over-Month Analysis" subtitle="Financial performance comparison across months">
    <template #headerActions>
      <div class="flex items-center space-x-2">
        <select 
          v-model="selectedMetric" 
          class="px-3 py-1 text-sm border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500"
        >
          <option value="revenue">Revenue</option>
          <option value="profit">Profit</option>
          <option value="expenses">Expenses</option>
          <option value="cashflow">Cash Flow</option>
        </select>
        <select 
          v-model="selectedPeriods" 
          class="px-3 py-1 text-sm border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500"
        >
          <option value="3">Last 3 Months</option>
          <option value="6">Last 6 Months</option>
          <option value="12">Last 12 Months</option>
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
      <!-- Metric Overview -->
      <div class="bg-gradient-to-r from-blue-50 to-purple-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-gray-900 mb-3">{{ selectedMetricLabel }} Trend</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div class="text-center">
            <p class="text-2xl font-bold text-blue-600">{{ formatCurrency(currentPeriod.value) }}</p>
            <p class="text-sm text-blue-700">Current Period</p>
          </div>
          <div class="text-center">
            <p class="text-lg font-semibold text-purple-600">{{ formatCurrency(previousPeriod.value) }}</p>
            <p class="text-sm text-purple-700">Previous Period</p>
          </div>
          <div class="text-center">
            <p 
              class="text-xl font-bold"
              :class="percentageChange >= 0 ? 'text-green-600' : 'text-red-600'"
            >
              {{ percentageChange >= 0 ? '+' : '' }}{{ percentageChange.toFixed(1) }}%
            </p>
            <p class="text-sm text-gray-700">Change</p>
          </div>
        </div>
      </div>

      <!-- Monthly Chart Placeholder -->
      <div class="bg-white p-6 rounded-lg border border-gray-200">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Monthly Trend Chart</h3>
        <div class="h-64 flex items-center justify-center bg-gray-50 rounded">
          <p class="text-gray-500">Chart implementation placeholder for {{ selectedMetricLabel }}</p>
        </div>
      </div>

      <!-- Monthly Breakdown Table -->
      <div class="bg-white rounded-lg border border-gray-200 overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">Monthly Breakdown</h3>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Month</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">{{ selectedMetricLabel }}</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Change</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">% Change</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Trend</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="(month, index) in monthlyData" :key="month.period" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                  {{ month.period }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatCurrency(month.value) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm">
                  <span 
                    :class="month.change >= 0 ? 'text-green-600' : 'text-red-600'"
                    class="font-medium"
                  >
                    {{ month.change >= 0 ? '+' : '' }}{{ formatCurrency(month.change) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm">
                  <span 
                    :class="month.percentageChange >= 0 ? 'text-green-600' : 'text-red-600'"
                    class="font-medium"
                  >
                    {{ month.percentageChange >= 0 ? '+' : '' }}{{ month.percentageChange.toFixed(1) }}%
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  <div class="flex items-center">
                    <svg 
                      v-if="month.trend === 'up'" 
                      class="w-4 h-4 text-green-500 mr-1" 
                      fill="none" 
                      stroke="currentColor" 
                      viewBox="0 0 24 24"
                    >
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 10l7-7m0 0l7 7m-7-7v18" />
                    </svg>
                    <svg 
                      v-else-if="month.trend === 'down'" 
                      class="w-4 h-4 text-red-500 mr-1" 
                      fill="none" 
                      stroke="currentColor" 
                      viewBox="0 0 24 24"
                    >
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 14l-7 7m0 0l-7-7m7 7V3" />
                    </svg>
                    <svg 
                      v-else 
                      class="w-4 h-4 text-gray-400 mr-1" 
                      fill="none" 
                      stroke="currentColor" 
                      viewBox="0 0 24 24"
                    >
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 12h14" />
                    </svg>
                    <span class="capitalize">{{ month.trend }}</span>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Key Insights -->
      <div class="bg-yellow-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-yellow-900 mb-3">Key Insights</h3>
        <div class="space-y-2">
          <div v-for="insight in keyInsights" :key="insight.id" class="flex items-start">
            <svg class="w-5 h-5 text-yellow-600 mr-2 mt-0.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <p class="text-sm text-yellow-800">{{ insight.message }}</p>
          </div>
        </div>
      </div>

      <!-- Performance Summary -->
      <div class="bg-gray-50 p-4 rounded-lg">
        <h3 class="text-lg font-medium text-gray-900 mb-3">Performance Summary</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="text-center">
            <p class="text-lg font-semibold text-gray-900">{{ bestMonth.period }}</p>
            <p class="text-sm text-gray-600">Best Performing Month</p>
            <p class="text-lg font-bold text-green-600">{{ formatCurrency(bestMonth.value) }}</p>
          </div>
          <div class="text-center">
            <p class="text-lg font-semibold text-gray-900">{{ worstMonth.period }}</p>
            <p class="text-sm text-gray-600">Lowest Performing Month</p>
            <p class="text-lg font-bold text-red-600">{{ formatCurrency(worstMonth.value) }}</p>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="flex justify-between items-center text-sm text-gray-500">
        <span>Last updated: {{ lastUpdated }}</span>
        <span>Analysis period: {{ selectedPeriods }} months</span>
      </div>
    </template>
  </Card>
</template>

<script setup lang="ts">
// Props
interface Props {
  metric?: string
  periods?: number
  autoRefresh?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  metric: 'revenue',
  periods: 6,
  autoRefresh: false
})

// Reactive state
const selectedMetric = ref(props.metric)
const selectedPeriods = ref(props.periods)
const loading = ref(false)
const lastUpdated = ref('')

// Mock data - replace with actual API calls
const monthlyData = ref([
  { period: 'Jan 2024', value: 120000, change: 0, percentageChange: 0, trend: 'stable' },
  { period: 'Feb 2024', value: 135000, change: 15000, percentageChange: 12.5, trend: 'up' },
  { period: 'Mar 2024', value: 128000, change: -7000, percentageChange: -5.2, trend: 'down' },
  { period: 'Apr 2024', value: 142000, change: 14000, percentageChange: 10.9, trend: 'up' },
  { period: 'May 2024', value: 138000, change: -4000, percentageChange: -2.8, trend: 'down' },
  { period: 'Jun 2024', value: 150000, change: 12000, percentageChange: 8.7, trend: 'up' }
])

const keyInsights = ref([
  { id: 1, message: 'Revenue has grown 25% over the last 6 months' },
  { id: 2, message: 'February and April showed the strongest growth' },
  { id: 3, message: 'March and May experienced seasonal dips' },
  { id: 4, message: 'Overall trend is positive with 4 out of 6 months showing growth' }
])

// Computed properties
const selectedMetricLabel = computed(() => {
  const labels = {
    revenue: 'Revenue',
    profit: 'Profit',
    expenses: 'Expenses',
    cashflow: 'Cash Flow'
  }
  return labels[selectedMetric.value as keyof typeof labels] || 'Revenue'
})

const currentPeriod = computed(() => monthlyData.value[monthlyData.value.length - 1]?.value || 0)
const previousPeriod = computed(() => monthlyData.value[monthlyData.value.length - 2]?.value || 0)

const percentageChange = computed(() => {
  if (previousPeriod.value === 0) return 0
  return ((currentPeriod.value - previousPeriod.value) / previousPeriod.value) * 100
})

const bestMonth = computed(() => {
  return monthlyData.value.reduce((best, current) => 
    current.value > best.value ? current : best
  )
})

const worstMonth = computed(() => {
  return monthlyData.value.reduce((worst, current) => 
    current.value < worst.value ? current : worst
  )
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
    console.log('Exporting month-over-month report for metric:', selectedMetric.value)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Create and download CSV
    const csvContent = generateCSV()
    downloadCSV(csvContent, `month-over-month-${selectedMetric.value}.csv`)
    
  } catch (error) {
    console.error('Export failed:', error)
  } finally {
    loading.value = false
  }
}

const generateCSV = (): string => {
  const headers = ['Month', selectedMetricLabel.value, 'Change', '% Change', 'Trend']
  const rows = monthlyData.value.map(month => [
    month.period,
    month.value,
    month.change,
    month.percentageChange.toFixed(1) + '%',
    month.trend
  ])
  
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

// Watch for changes
watch([selectedMetric, selectedPeriods], () => {
  refreshData()
})
</script>

<style scoped>
/* Component-specific styles */
</style>
