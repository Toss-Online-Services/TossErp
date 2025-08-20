<template>
  <div class="min-h-screen bg-gray-50 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Page Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900">Financial Reports</h1>
        <p class="mt-2 text-gray-600">Comprehensive financial analysis and reporting for TOSS ERP III</p>
      </div>

      <!-- Report Navigation -->
      <div class="mb-8">
        <nav class="flex space-x-8 border-b border-gray-200">
          <button
            v-for="tab in reportTabs"
            :key="tab.id"
            @click="activeTab = tab.id"
            :class="[
              'py-2 px-1 border-b-2 font-medium text-sm',
              activeTab === tab.id
                ? 'border-orange-500 text-orange-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]"
          >
            {{ tab.name }}
          </button>
        </nav>
      </div>

      <!-- Report Content -->
      <div class="space-y-8">
        <!-- P&L Summary Report -->
        <div v-if="activeTab === 'pl-summary'">
          <div class="mb-6">
            <h2 class="text-2xl font-semibold text-gray-900 mb-2">Profit & Loss Summary</h2>
            <p class="text-gray-600">Comprehensive analysis of revenue, costs, and profitability</p>
          </div>
          <PlSummary :auto-refresh="true" />
        </div>

        <!-- Cash Position Report -->
        <div v-if="activeTab === 'cash-position'">
          <div class="mb-6">
            <h2 class="text-2xl font-semibold text-gray-900 mb-2">Cash Position</h2>
            <p class="text-gray-600">Current cash balances, liquidity metrics, and cash flow analysis</p>
          </div>
          <CashPosition :auto-refresh="true" />
        </div>

        <!-- Month-over-Month Report -->
        <div v-if="activeTab === 'month-over-month'">
          <div class="mb-6">
            <h2 class="text-2xl font-semibold text-gray-900 mb-2">Month-over-Month Analysis</h2>
            <p class="text-gray-600">Trend analysis and performance comparison across periods</p>
          </div>
          <MonthOverMonth :auto-refresh="true" />
        </div>

        <!-- Consolidated Dashboard -->
        <div v-if="activeTab === 'dashboard'">
          <div class="mb-6">
            <h2 class="text-2xl font-semibold text-gray-900 mb-2">Financial Dashboard</h2>
            <p class="text-gray-600">Overview of key financial metrics and KPIs</p>
          </div>
          
          <!-- Quick Stats Grid -->
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
            <div class="bg-white p-6 rounded-lg shadow border border-gray-200">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-green-100 rounded-full flex items-center justify-center">
                    <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1" />
                    </svg>
                  </div>
                </div>
                <div class="ml-4">
                  <p class="text-sm font-medium text-gray-500">Total Revenue</p>
                  <p class="text-2xl font-semibold text-gray-900">{{ formatCurrency(quickStats.revenue) }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white p-6 rounded-lg shadow border border-gray-200">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center">
                    <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                    </svg>
                  </div>
                </div>
                <div class="ml-4">
                  <p class="text-sm font-medium text-gray-500">Net Profit</p>
                  <p class="text-2xl font-semibold text-gray-900">{{ formatCurrency(quickStats.profit) }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white p-6 rounded-lg shadow border border-gray-200">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-purple-100 rounded-full flex items-center justify-center">
                    <svg class="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                  </div>
                </div>
                <div class="ml-4">
                  <p class="text-sm font-medium text-gray-500">Cash Balance</p>
                  <p class="text-2xl font-semibold text-gray-900">{{ formatCurrency(quickStats.cash) }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white p-6 rounded-lg shadow border border-gray-200">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-yellow-100 rounded-full flex items-center justify-center">
                    <svg class="w-5 h-5 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
                    </svg>
                  </div>
                </div>
                <div class="ml-4">
                  <p class="text-sm font-medium text-gray-500">Growth Rate</p>
                  <p class="text-2xl font-semibold text-gray-900">{{ quickStats.growth }}%</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Mini Reports Grid -->
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
            <div>
              <h3 class="text-lg font-medium text-gray-900 mb-4">Revenue Trend</h3>
              <div class="bg-white p-6 rounded-lg shadow border border-gray-200">
                <div class="h-48 flex items-center justify-center bg-gray-50 rounded">
                  <p class="text-gray-500">Revenue chart placeholder</p>
                </div>
              </div>
            </div>
            
            <div>
              <h3 class="text-lg font-medium text-gray-900 mb-4">Expense Breakdown</h3>
              <div class="bg-white p-6 rounded-lg shadow border border-gray-200">
                <div class="h-48 flex items-center justify-center bg-gray-50 rounded">
                  <p class="text-gray-500">Expense chart placeholder</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Export All Reports -->
      <div class="mt-12 bg-white p-6 rounded-lg shadow border border-gray-200">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-lg font-medium text-gray-900">Export All Reports</h3>
            <p class="text-sm text-gray-600">Generate comprehensive financial reports package</p>
          </div>
          <div class="flex space-x-3">
            <button 
              @click="exportAllReports"
              class="px-4 py-2 bg-orange-600 text-white rounded-md hover:bg-orange-700 focus:outline-none focus:ring-2 focus:ring-orange-500"
            >
              Export All
            </button>
            <button 
              @click="scheduleReport"
              class="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500"
            >
              Schedule Report
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Report tabs configuration
const reportTabs = [
  { id: 'dashboard', name: 'Dashboard' },
  { id: 'pl-summary', name: 'P&L Summary' },
  { id: 'cash-position', name: 'Cash Position' },
  { id: 'month-over-month', name: 'Month-over-Month' }
]

// Reactive state
const activeTab = ref('dashboard')
const loading = ref(false)

// Mock quick stats data
const quickStats = ref({
  revenue: 125000,
  profit: 25000,
  cash: 85000,
  growth: 12.5
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

const exportAllReports = async () => {
  try {
    loading.value = true
    console.log('Exporting all financial reports...')
    
    // Simulate export process
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // Create comprehensive report package
    const reportData = {
      timestamp: new Date().toISOString(),
      reports: ['pl-summary', 'cash-position', 'month-over-month'],
      period: 'current-month',
      format: 'pdf'
    }
    
    console.log('Reports exported:', reportData)
    
    // Show success message
    alert('All reports exported successfully!')
    
  } catch (error) {
    console.error('Export failed:', error)
    alert('Export failed. Please try again.')
  } finally {
    loading.value = false
  }
}

const scheduleReport = () => {
  // Implement report scheduling functionality
  console.log('Opening report scheduler...')
  alert('Report scheduling feature coming soon!')
}

// Lifecycle
onMounted(() => {
  console.log('Reports page mounted')
})
</script>

<style scoped>
/* Page-specific styles */
</style>
