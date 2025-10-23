<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-6">
        <div class="flex flex-col space-y-3 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent truncate">
              Sales Dashboard
            </h1>
            <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400 line-clamp-1">
              Track sales and manage customer orders
            </p>
        </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <NuxtLink 
              to="/sales/pos" 
              class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-xl hover:from-blue-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all duration-200 text-xs sm:text-sm font-semibold whitespace-nowrap"
            >
              <ShoppingCartIcon class="w-4 h-4 sm:mr-2" />
              <span class="hidden sm:inline">New Sale</span>
            </NuxtLink>
            <button 
              @click="refreshStats" 
              class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 rounded-xl text-xs sm:text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 hover:shadow-md transition-all duration-200 whitespace-nowrap"
              title="Refresh"
            >
              <ArrowPathIcon class="w-4 h-4 sm:mr-2" :class="{ 'animate-spin': loading }" />
              <span class="hidden sm:inline">Refresh</span>
          </button>
          </div>
        </div>
        </div>
      </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-8 space-y-4 sm:space-y-6">
      <!-- Stats Grid -->
      <div class="grid grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-4">
        <!-- Today's Sales -->
        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Today's Sales</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">R{{ formatCurrency(todaysSales) }}</p>
              <p class="text-xs sm:text-sm text-green-600 mt-1">+{{ todaysGrowth }}%</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl shadow-lg flex-shrink-0">
              <CurrencyDollarIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>

        <!-- Orders -->
        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Orders</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">{{ totalOrders }}</p>
              <p class="text-xs sm:text-sm text-blue-600 mt-1">{{ pendingOrders }} pending</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl shadow-lg flex-shrink-0">
              <ShoppingBagIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>

        <!-- Invoices -->
        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Invoices</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">{{ totalInvoices }}</p>
              <p class="text-xs sm:text-sm text-orange-600 mt-1">{{ unpaidInvoices }} unpaid</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-orange-500 to-amber-600 rounded-xl shadow-lg flex-shrink-0">
              <DocumentTextIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>

        <!-- Average Order -->
        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Avg Order</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">R{{ formatCurrency(averageOrder) }}</p>
              <p class="text-xs sm:text-sm text-purple-600 mt-1">{{ conversionRate }}% rate</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl shadow-lg flex-shrink-0">
              <ArrowTrendingUpIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Sales Trends & Order Status -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6">
        <!-- Sales Trend Chart -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-shadow duration-300">
          <div class="mb-4 sm:mb-6">
            <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Sales Trends</h3>
            <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mt-1">Last 7 days sales activity</p>
          </div>
          <LineChart
            :labels="['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']"
            :data="[3.2, 4.1, 3.8, 5.2, 4.8, 6.5, 5.9]"
            label="Sales (R thousands)"
            color="#3B82F6"
            :height="280"
          />
        </div>

        <!-- Order Status Distribution -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-shadow duration-300">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white mb-4 sm:mb-6">Order Status</h3>
          <div class="space-y-4">
            <div>
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">Completed</span>
                <span class="font-medium text-slate-900 dark:text-white">67%</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-green-500 to-emerald-600 h-3 rounded-full transition-all duration-500" style="width: 67%"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">Processing</span>
                <span class="font-medium text-slate-900 dark:text-white">19%</span>
                    </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-blue-500 to-purple-600 h-3 rounded-full transition-all duration-500" style="width: 19%"></div>
                    </div>
                  </div>
            <div>
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">Pending</span>
                <span class="font-medium text-slate-900 dark:text-white">14%</span>
                  </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-orange-500 to-amber-600 h-3 rounded-full transition-all duration-500" style="width: 14%"></div>
              </div>
              </div>
            </div>
          </div>
        </div>

      <!-- Top Products & Category Performance -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6">
        <!-- Top Products Chart -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-shadow duration-300">
          <div class="flex items-center justify-between mb-4 sm:mb-6">
            <div>
              <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Top Selling Products</h3>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mt-1">This month</p>
            </div>
                  </div>
          <BarChart
            :labels="['Coca-Cola 2L', 'White Bread', 'Milk 1L', 'Simba Chips']"
            :data="[145, 132, 118, 95]"
            label="Units Sold"
            color="#8B5CF6"
            :height="280"
          />
                    </div>

        <!-- Category Sales -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-shadow duration-300">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white mb-4 sm:mb-6">Sales by Category</h3>
          <div class="space-y-4">
            <div>
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">🛒 Groceries</span>
                <span class="font-medium text-slate-900 dark:text-white">R8.5K (34%)</span>
                  </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-green-500 to-emerald-600 h-3 rounded-full transition-all duration-500" style="width: 34%"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">🥤 Beverages</span>
                <span class="font-medium text-slate-900 dark:text-white">R6.2K (25%)</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-blue-500 to-purple-600 h-3 rounded-full transition-all duration-500" style="width: 25%"></div>
          </div>
            </div>
                    <div>
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">🍿 Snacks</span>
                <span class="font-medium text-slate-900 dark:text-white">R4.8K (20%)</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-yellow-500 to-orange-600 h-3 rounded-full transition-all duration-500" style="width: 20%"></div>
                    </div>
                  </div>
                    <div>
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">🧹 Household</span>
                <span class="font-medium text-slate-900 dark:text-white">R3.2K (13%)</span>
                    </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-purple-500 to-pink-600 h-3 rounded-full transition-all duration-500" style="width: 13%"></div>
                    </div>
                  </div>
                    <div>
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">🧴 Personal Care</span>
                <span class="font-medium text-slate-900 dark:text-white">R2.0K (8%)</span>
                    </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-pink-500 to-rose-600 h-3 rounded-full transition-all duration-500" style="width: 8%"></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
        <h3 class="text-base sm:text-lg font-bold text-slate-900 dark:text-white mb-4">Quick Actions</h3>
        <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
          <NuxtLink 
            to="/sales/pos" 
            class="group p-4 bg-gradient-to-br from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 rounded-xl border-2 border-blue-200 dark:border-blue-800 hover:border-blue-400 dark:hover:border-blue-600 transition-all duration-200 hover:shadow-lg"
          >
            <div class="flex flex-col items-center text-center space-y-2">
              <div class="p-3 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform duration-200">
                <ShoppingCartIcon class="w-6 h-6 text-white" />
            </div>
                  <div>
                <p class="text-sm font-semibold text-slate-900 dark:text-white">New Sale</p>
                <p class="text-xs text-slate-600 dark:text-slate-400">Point of Sale</p>
                  </div>
                </div>
              </NuxtLink>

          <NuxtLink 
            to="/sales/orders" 
            class="group p-4 bg-gradient-to-br from-green-50 to-emerald-50 dark:from-green-900/20 dark:to-emerald-900/20 rounded-xl border-2 border-green-200 dark:border-green-800 hover:border-green-400 dark:hover:border-green-600 transition-all duration-200 hover:shadow-lg"
          >
            <div class="flex flex-col items-center text-center space-y-2">
              <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform duration-200">
                <ShoppingBagIcon class="w-6 h-6 text-white" />
                  </div>
                  <div>
                <p class="text-sm font-semibold text-slate-900 dark:text-white">Orders</p>
                <p class="text-xs text-slate-600 dark:text-slate-400">Manage orders</p>
                  </div>
                </div>
              </NuxtLink>

          <NuxtLink 
            to="/sales/invoices" 
            class="group p-4 bg-gradient-to-br from-orange-50 to-amber-50 dark:from-orange-900/20 dark:to-amber-900/20 rounded-xl border-2 border-orange-200 dark:border-orange-800 hover:border-orange-400 dark:hover:border-orange-600 transition-all duration-200 hover:shadow-lg"
          >
            <div class="flex flex-col items-center text-center space-y-2">
              <div class="p-3 bg-gradient-to-br from-orange-500 to-amber-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform duration-200">
                <DocumentTextIcon class="w-6 h-6 text-white" />
              </div>
              <div>
                <p class="text-sm font-semibold text-slate-900 dark:text-white">Invoices</p>
                <p class="text-xs text-slate-600 dark:text-slate-400">Manage billing</p>
              </div>
                </div>
          </NuxtLink>

          <NuxtLink 
            to="/stock/items" 
            class="group p-4 bg-gradient-to-br from-purple-50 to-pink-50 dark:from-purple-900/20 dark:to-pink-900/20 rounded-xl border-2 border-purple-200 dark:border-purple-800 hover:border-purple-400 dark:hover:border-purple-600 transition-all duration-200 hover:shadow-lg"
          >
            <div class="flex flex-col items-center text-center space-y-2">
              <div class="p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform duration-200">
                <CubeIcon class="w-6 h-6 text-white" />
              </div>
              <div>
                <p class="text-sm font-semibold text-slate-900 dark:text-white">Inventory</p>
                <p class="text-xs text-slate-600 dark:text-slate-400">Check stock</p>
              </div>
            </div>
          </NuxtLink>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  CurrencyDollarIcon, 
  ShoppingBagIcon, 
  DocumentTextIcon, 
  ArrowTrendingUpIcon,
  ShoppingCartIcon,
  CubeIcon,
  ArrowPathIcon
} from '@heroicons/vue/24/outline'
import LineChart from '~/components/charts/LineChart.vue'
import BarChart from '~/components/charts/BarChart.vue'

// Page metadata
useHead({
  title: 'Sales Dashboard - TOSS ERP',
  meta: [
    { name: 'description', content: 'Sales management dashboard for township businesses' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// State
const loading = ref(false)

// Sales statistics
const todaysSales = ref(24500)
const todaysGrowth = ref(15.8)
const totalOrders = ref(42)
const pendingOrders = ref(8)
const totalInvoices = ref(35)
const unpaidInvoices = ref(5)
const averageOrder = ref(580)
const conversionRate = ref(68)

// Helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const refreshStats = async () => {
  loading.value = true
  // Simulate API call
  await new Promise(resolve => setTimeout(resolve, 500))
  loading.value = false
}
</script>

