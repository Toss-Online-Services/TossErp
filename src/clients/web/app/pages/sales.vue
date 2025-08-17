<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              Sales & POS
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Process sales, manage customers, and track your business performance
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <button
              @click="openPOS"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-green-600 hover:bg-green-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
              </svg>
              Open POS
            </button>
            <button
              @click="showNewSaleModal = true"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              New Sale
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Sales Overview Cards -->
    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
        <!-- Today's Sales -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Today's Sales
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    R {{ formatCurrency(todaysSales) }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <!-- Total Orders -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Today's Orders
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ todaysOrders }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <!-- Average Order Value -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Avg Order Value
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    R {{ formatCurrency(averageOrderValue) }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <!-- Active Customers -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-indigo-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Total Customers
                  </dt>
                  <dd class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ totalCustomers }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Recent Sales -->
        <div class="lg:col-span-2 bg-white dark:bg-gray-800 shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Recent Sales</h3>
              <div class="flex space-x-2">
                <select
                  v-model="salesFilter"
                  class="text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white rounded-md focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value="today">Today</option>
                  <option value="week">This Week</option>
                  <option value="month">This Month</option>
                </select>
              </div>
            </div>
          </div>
          <div class="p-6">
            <div v-if="loading" class="text-center py-4">
              <div class="inline-block animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
              <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">Loading sales data...</p>
            </div>
            <div v-else-if="error" class="text-center py-4">
              <div class="text-red-500 dark:text-red-400">
                <svg class="mx-auto h-12 w-12 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.704-.833-2.464 0L4.35 16.5c-.77.833.192 2.5 1.732 2.5z" />
                </svg>
                <p class="text-sm">{{ error }}</p>
                <button @click="refreshData" class="mt-2 text-blue-600 hover:text-blue-700 text-sm underline">
                  Try again
                </button>
              </div>
            </div>
            <div v-else class="space-y-4">
              <div v-for="sale in filteredRecentSales" :key="sale.id" class="flex items-center justify-between p-4 border border-gray-200 dark:border-gray-600 rounded-lg">
                <div class="flex items-center space-x-4">
                  <div class="flex-shrink-0">
                    <div :class="[getStatusBadgeClass(sale.status), 'w-8 h-8 rounded-full flex items-center justify-center']">
                      <svg v-if="sale.status === 'completed'" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                      </svg>
                      <svg v-else-if="sale.status === 'pending'" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                      <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                      </svg>
                    </div>
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-900 dark:text-white">
                      Sale #{{ sale.orderNumber }}
                    </p>
                    <p class="text-xs text-gray-500 dark:text-gray-400">
                      {{ sale.customer?.name || 'Walk-in Customer' }} • {{ formatTime(sale.createdAt) }}
                    </p>
                  </div>
                </div>
                <div class="text-right">
                  <p class="text-sm font-medium text-gray-900 dark:text-white">
                    R {{ formatCurrency(sale.total) }}
                  </p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">
                    {{ sale.items.length }} item{{ sale.items.length > 1 ? 's' : '' }}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions & Top Products -->
        <div class="space-y-6">
          <!-- Quick Actions -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Quick Actions</h3>
            </div>
            <div class="p-6">
              <div class="space-y-3">
                <button
                  @click="openPOS"
                  class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  <div class="flex items-center">
                    <svg class="w-5 h-5 text-green-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                    </svg>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">Start POS Sale</span>
                  </div>
                  <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </button>

                <button
                  @click="showNewCustomerModal = true"
                  class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  <div class="flex items-center">
                    <svg class="w-5 h-5 text-blue-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
                    </svg>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">Add Customer</span>
                  </div>
                  <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </button>

                <NuxtLink
                  to="/sales/reports"
                  class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  <div class="flex items-center">
                    <svg class="w-5 h-5 text-purple-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                    </svg>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">Sales Reports</span>
                  </div>
                  <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </NuxtLink>

                <button
                  @click="showCustomerList = true"
                  class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  <div class="flex items-center">
                    <svg class="w-5 h-5 text-indigo-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">Customer List</span>
                  </div>
                  <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </button>
              </div>
            </div>
          </div>

          <!-- Top Products -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
            <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Top Selling Products</h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">This month's best performers</p>
            </div>
            <div class="p-6">
              <div v-if="loading" class="text-center py-4">
                <div class="inline-block animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600"></div>
              </div>
              <div v-else-if="topProductsDisplay.length === 0" class="text-center py-4">
                <p class="text-sm text-gray-500 dark:text-gray-400">No top products data available</p>
              </div>
              <div v-else class="space-y-4">
                <div v-for="(product, index) in topProductsDisplay" :key="product.id" class="flex items-center justify-between">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 w-8 h-8 bg-gray-200 dark:bg-gray-600 rounded-lg flex items-center justify-center">
                      <span class="text-xs font-medium text-gray-600 dark:text-gray-300">{{ index + 1 }}</span>
                    </div>
                    <div class="ml-3">
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ product.name }}</p>
                      <p class="text-xs text-gray-500 dark:text-gray-400">{{ product.sold }} sold</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">R {{ formatCurrency(product.revenue) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modals -->
    <!-- New Sale Modal -->
    <div
      v-show="showNewSaleModal"
      class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4"
      @click.self="showNewSaleModal = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg max-w-md w-full p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">New Sale</h3>
          <button
            @click="showNewSaleModal = false"
            class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        <p class="text-gray-500 dark:text-gray-400 mb-4">
          Choose how you want to process this sale.
        </p>
        <div class="space-y-3">
          <button
            @click="openPOS"
            class="w-full flex items-center justify-center p-3 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            <svg class="w-5 h-5 text-green-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
            Use POS System
          </button>
          <button
            @click="createQuickSale"
            class="w-full flex items-center justify-center p-3 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            <svg class="w-5 h-5 text-blue-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
            Quick Sale
          </button>
        </div>
      </div>
    </div>

    <!-- New Customer Modal -->
    <div
      v-show="showNewCustomerModal"
      class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4"
      @click.self="showNewCustomerModal = false"
    >
      <div class="bg-white dark:bg-gray-800 rounded-lg max-w-md w-full p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">Add New Customer</h3>
          <button
            @click="showNewCustomerModal = false"
            class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        <p class="text-gray-500 dark:text-gray-400 mb-4">
          Customer management form will be implemented here.
        </p>
        <div class="flex justify-end space-x-3">
          <button
            @click="showNewCustomerModal = false"
            class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            Cancel
          </button>
          <button
            class="px-4 py-2 bg-blue-600 text-white rounded-md text-sm font-medium hover:bg-blue-700"
          >
            Add Customer
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Page meta
definePageMeta({
  title: 'Sales & POS - TOSS ERP',
  description: 'Process sales, manage customers, and track business performance'
})

// Use sales management composable
const {
  salesOverview,
  topProducts,
  loading,
  error,
  todaysSales,
  todaysOrders,
  averageOrderValue,
  totalCustomers,
  recentSales,
  loadSalesOverview,
  loadTopProducts,
  formatCurrency,
  formatTime,
  getStatusBadgeClass
} = useSalesManagement()

// Reactive data
const salesFilter = ref('today')
const showNewSaleModal = ref(false)
const showNewCustomerModal = ref(false)
const showCustomerList = ref(false)

// Computed properties
const filteredRecentSales = computed(() => {
  return recentSales.value.slice(0, 5) // Show only 5 most recent
})

const topProductsDisplay = computed(() => {
  return topProducts.value.slice(0, 5) // Show only top 5
})

// Watch sales filter changes
watch(salesFilter, async (newFilter) => {
  await loadSalesOverview(newFilter as 'today' | 'week' | 'month')
})

// Methods
const openPOS = () => {
  console.log('Opening POS system...')
  showNewSaleModal.value = false
  // TODO: Navigate to POS interface or open POS modal
}

const createQuickSale = () => {
  console.log('Creating quick sale...')
  showNewSaleModal.value = false
  // TODO: Implement quick sale functionality
}

const refreshData = async () => {
  await Promise.all([
    loadSalesOverview(salesFilter.value as 'today' | 'week' | 'month'),
    loadTopProducts({ period: salesFilter.value as 'today' | 'week' | 'month', limit: 5 })
  ])
}

// Load sales data on mount
onMounted(async () => {
  await refreshData()
})

// SEO
useHead({
  title: 'Sales & POS - TOSS ERP',
  meta: [
    { name: 'description', content: 'Complete sales management system with POS functionality, customer management, and sales analytics' }
  ]
})
</script>

<style scoped>
/* Component-specific styles */
.transition-colors {
  transition-property: color, background-color, border-color;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 200ms;
}

/* Hover effects */
.hover\:bg-gray-50:hover {
  background-color: #f9fafb;
}

.dark .hover\:bg-gray-700:hover {
  background-color: #374151;
}
</style>
