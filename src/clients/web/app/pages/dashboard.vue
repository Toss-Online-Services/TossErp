<template>
  <div class="space-y-6">
    <!-- Material Design Page Header with Beautiful Gradient -->
    <div class="bg-gradient-to-r from-blue-600 via-blue-700 to-indigo-800 rounded-2xl shadow-2xl p-8 mb-8">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold text-white mb-2">Analytics Dashboard</h1>
          <p class="text-blue-100 text-lg">Welcome back! Here's what's happening with your business today.</p>
        </div>
        <div class="flex space-x-3">
          <button class="bg-white/20 hover:bg-white/30 text-white px-4 py-2 rounded-lg transition-all duration-200 backdrop-blur-sm">
            <ArrowDownTrayIcon class="w-5 h-5 inline mr-2" />
            Export
          </button>
          <button class="bg-white text-blue-600 hover:bg-blue-50 px-4 py-2 rounded-lg transition-all duration-200 font-medium">
            <PlusIcon class="w-5 h-5 inline mr-2" />
            Quick Actions
          </button>
        </div>
      </div>
    </div>

    <!-- Material Design Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <!-- Website Views Card -->
      <div class="material-card">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h6 class="material-card-title">Website Views</h6>
            <p class="material-card-subtitle">Last Campaign Performance</p>
          </div>
          <div class="stats-icon gradient-primary">
            <ChartBarIcon class="w-6 h-6" />
          </div>
        </div>
        <!-- Mini Chart Placeholder -->
        <div class="h-16 mb-4">
          <WebsiteViewsChart />
        </div>
        <div class="flex items-center text-xs text-gray-500">
          <ClockIcon class="w-4 h-4 mr-1" />
          campaign sent 2 days ago
        </div>
      </div>

      <!-- Daily Sales Card -->
      <div class="material-card">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h6 class="material-card-title">Daily Sales</h6>
            <p class="material-card-subtitle">(+15%) increase in today sales.</p>
          </div>
          <div class="stats-icon gradient-success">
            <CurrencyDollarIcon class="w-6 h-6" />
          </div>
        </div>
        <!-- Mini Chart Placeholder -->
        <div class="h-16 mb-4">
          <DailySalesChart />
        </div>
        <div class="flex items-center text-xs text-gray-500">
          <ClockIcon class="w-4 h-4 mr-1" />
          updated 4 min ago
        </div>
      </div>

      <!-- Completed Tasks Card -->
      <div class="material-card">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h6 class="material-card-title">Completed Tasks</h6>
            <p class="material-card-subtitle">Last Campaign Performance</p>
          </div>
          <div class="stats-icon gradient-warning">
            <CheckCircleIcon class="w-6 h-6" />
          </div>
        </div>
        <!-- Mini Chart Placeholder -->
        <div class="h-16 mb-4">
          <CompletedTasksChart />
        </div>
        <div class="flex items-center text-xs text-gray-500">
          <ClockIcon class="w-4 h-4 mr-1" />
          just updated
        </div>
      </div>

      <!-- Stock Overview Card -->
      <div class="material-card">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h6 class="material-card-title">Stock Level</h6>
            <p class="material-card-subtitle">Current inventory status</p>
          </div>
          <div class="stats-icon gradient-danger">
            <CubeIcon class="w-6 h-6" />
          </div>
        </div>
        <!-- Mini Chart Placeholder -->
        <div class="h-16 mb-4">
          <StockLevelChart />
        </div>
        <div class="flex items-center text-xs text-gray-500">
          <ClockIcon class="w-4 h-4 mr-1" />
          real-time data
        </div>
      </div>
    </div>

    <!-- Key Metrics Row -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mt-8">
      <!-- Bookings -->
      <div class="material-card text-center">
        <div class="flex justify-center mb-3">
          <div class="w-12 h-12 bg-gray-900 rounded-lg flex items-center justify-center">
            <CalendarIcon class="w-6 h-6 text-white" />
          </div>
        </div>
        <h3 class="text-2xl font-bold text-gray-900 mb-1">281</h3>
        <p class="text-sm text-gray-600 mb-2">Bookings</p>
        <!-- Revenue Card -->
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
                    Monthly Revenue
                  </dt>
                  <dd class="flex items-baseline">
                    <div class="text-2xl font-semibold text-gray-900 dark:text-white">
                      R {{ formatCurrency(monthlyRevenue) }}
                    </div>
                    <div class="ml-2 flex items-baseline text-sm font-semibold text-green-600">
                      <svg class="self-center flex-shrink-0 h-5 w-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span class="sr-only">Increased by</span>
                      12%
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <!-- Orders Card -->
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
                    Total Orders
                  </dt>
                  <dd class="flex items-baseline">
                    <div class="text-2xl font-semibold text-gray-900 dark:text-white">
                      {{ totalOrders }}
                    </div>
                    <div class="ml-2 flex items-baseline text-sm font-semibold text-green-600">
                      <svg class="self-center flex-shrink-0 h-5 w-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span class="sr-only">Increased by</span>
                      8%
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <!-- Stock Items Card -->
        <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                    Stock Items
                  </dt>
                  <dd class="flex items-baseline">
                    <div class="text-2xl font-semibold text-gray-900 dark:text-white">
                      {{ totalStockItems }}
                    </div>
                    <div class="ml-2 flex items-baseline text-sm font-semibold text-yellow-600">
                      <svg class="self-center flex-shrink-0 h-5 w-5 text-yellow-500" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clip-rule="evenodd" />
                      </svg>
                      <span class="sr-only">Decreased by</span>
                      {{ lowStockItems }} low
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <!-- Customers Card -->
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
                    Active Customers
                  </dt>
                  <dd class="flex items-baseline">
                    <div class="text-2xl font-semibold text-gray-900 dark:text-white">
                      {{ activeCustomers }}
                    </div>
                    <div class="ml-2 flex items-baseline text-sm font-semibold text-green-600">
                      <svg class="self-center flex-shrink-0 h-5 w-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span class="sr-only">Increased by</span>
                      5%
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Charts and Analytics -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <!-- Sales Chart -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Sales Overview</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">Last 7 days performance</p>
          </div>
          <div class="p-6">
            <div class="h-64 flex items-center justify-center bg-gray-50 dark:bg-gray-700 rounded-lg">
              <div class="text-center">
                <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
                <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">Sales chart will be displayed here</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Top Products -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Top Selling Products</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">Best performers this month</p>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="product in topProducts" :key="product.id" class="flex items-center justify-between">
                <div class="flex items-center">
                  <div class="w-10 h-10 bg-gray-200 dark:bg-gray-600 rounded-lg flex items-center justify-center">
                    <span class="text-sm font-medium text-gray-600 dark:text-gray-300">{{ product.name.charAt(0) }}</span>
                  </div>
                  <div class="ml-3">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">{{ product.name }}</p>
                    <p class="text-xs text-gray-500 dark:text-gray-400">{{ product.category }}</p>
                  </div>
                </div>
                <div class="text-right">
                  <p class="text-sm font-medium text-gray-900 dark:text-white">{{ product.sold }} sold</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">R {{ formatCurrency(product.revenue) }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activity and Quick Actions -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Recent Activity -->
        <div class="lg:col-span-2 bg-white dark:bg-gray-800 shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Recent Activity</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">Latest business transactions and updates</p>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-3">
                <div class="flex-shrink-0">
                  <div :class="[activity.type === 'sale' ? 'bg-green-100 dark:bg-green-900' : activity.type === 'stock' ? 'bg-blue-100 dark:bg-blue-900' : 'bg-yellow-100 dark:bg-yellow-900', 'w-8 h-8 rounded-full flex items-center justify-center']">
                    <svg v-if="activity.type === 'sale'" class="w-4 h-4 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    <svg v-else-if="activity.type === 'stock'" class="w-4 h-4 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                    </svg>
                    <svg v-else class="w-4 h-4 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
                    </svg>
                  </div>
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm text-gray-900 dark:text-white">{{ activity.description }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatTime(activity.timestamp) }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="bg-white dark:bg-gray-800 shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Quick Actions</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">Common tasks</p>
          </div>
          <div class="p-6">
            <div class="space-y-3">
              <NuxtLink
                to="/stock?action=add"
                class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-blue-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                  </svg>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Add Stock Item</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </NuxtLink>

              <NuxtLink
                to="/sales?action=new"
                class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                  </svg>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">New Sale</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </NuxtLink>

              <NuxtLink
                to="/finance?view=reports"
                class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-purple-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">View Reports</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </NuxtLink>

              <button
                @click="openAIAssistant"
                class="w-full flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                <div class="flex items-center">
                  <svg class="w-5 h-5 text-yellow-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                  </svg>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Ask AI Assistant</span>
                </div>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Page meta
definePageMeta({
  title: 'Dashboard - TOSS ERP',
  description: 'Business dashboard with key metrics and insights'
})

// Reactive data
const monthlyRevenue = ref(45750.00)
const totalOrders = ref(1234)
const totalStockItems = ref(567)
const lowStockItems = ref(7)
const activeCustomers = ref(245)

// Top products data
const topProducts = ref([
  { id: 1, name: 'Product A', category: 'Electronics', sold: 145, revenue: 12500.00 },
  { id: 2, name: 'Product B', category: 'Clothing', sold: 98, revenue: 8750.00 },
  { id: 3, name: 'Product C', category: 'Food', sold: 87, revenue: 6200.00 },
  { id: 4, name: 'Product D', category: 'Books', sold: 65, revenue: 4300.00 },
  { id: 5, name: 'Product E', category: 'Home', sold: 52, revenue: 3800.00 }
])

// Recent activities
const recentActivities = ref([
  { id: 1, type: 'sale', description: 'Sale completed: R 234.50', timestamp: new Date(Date.now() - 5 * 60 * 1000) },
  { id: 2, type: 'stock', description: 'Stock updated: Added 50 units of Product A', timestamp: new Date(Date.now() - 15 * 60 * 1000) },
  { id: 3, type: 'alert', description: 'Low stock alert: Product C (5 units remaining)', timestamp: new Date(Date.now() - 30 * 60 * 1000) },
  { id: 4, type: 'sale', description: 'Sale completed: R 156.75', timestamp: new Date(Date.now() - 45 * 60 * 1000) },
  { id: 5, type: 'stock', description: 'New product added: Product F', timestamp: new Date(Date.now() - 60 * 60 * 1000) }
])

// Utility functions
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatTime = (date: Date): string => {
  const now = new Date()
  const diffInMinutes = Math.floor((now.getTime() - date.getTime()) / (1000 * 60))
  
  if (diffInMinutes < 1) return 'Just now'
  if (diffInMinutes < 60) return `${diffInMinutes} minutes ago`
  
  const diffInHours = Math.floor(diffInMinutes / 60)
  if (diffInHours < 24) return `${diffInHours} hour${diffInHours > 1 ? 's' : ''} ago`
  
  const diffInDays = Math.floor(diffInHours / 24)
  return `${diffInDays} day${diffInDays > 1 ? 's' : ''} ago`
}

// Methods
const openAIAssistant = () => {
  // This will be implemented to open the AI assistant
  console.log('Opening AI Assistant...')
}

// Load dashboard data
onMounted(async () => {
  try {
    // TODO: Replace with actual API calls to gateway
    // const dashboardData = await $fetch('/api/dashboard/overview')
    // Update reactive data with API response
    
    console.log('Dashboard overview loaded')
  } catch (error) {
    console.error('Failed to load dashboard data:', error)
  }
})

// SEO
useHead({
  title: 'Dashboard - TOSS ERP',
  meta: [
    { name: 'description', content: 'Business dashboard with key metrics, analytics, and insights for your township business' }
  ]
})
</script>

<style scoped>
/* Smooth transitions for interactive elements */
.transition-colors {
  transition-property: color, background-color, border-color;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 200ms;
}

/* Hover effects */
.hover\:bg-gray-50:hover {
  background-color: #f9fafb;
}

.dark .hover\:bg-gray-600:hover {
  background-color: #4b5563;
}

.hover\:bg-gray-100:hover {
  background-color: #f3f4f6;
}

.dark .hover\:bg-gray-700:hover {
  background-color: #374151;
}
</style>

