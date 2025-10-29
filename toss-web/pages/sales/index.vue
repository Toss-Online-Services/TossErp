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
            :labels="salesTrendLabels.length > 0 ? salesTrendLabels : ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']"
            :data="salesTrendData.length > 0 ? salesTrendData : [0, 0, 0, 0, 0, 0, 0]"
            label="Sales (R thousands)"
            color="#3B82F6"
            :height="280"
          />
        </div>

        <!-- Order Status Distribution -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-shadow duration-300">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white mb-4 sm:mb-6">Order Status</h3>
          <div class="space-y-4">
            <div v-for="status in orderStatusDistribution" :key="status.status">
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400 capitalize">{{ status.status }}</span>
                <span class="font-medium text-slate-900 dark:text-white">{{ Math.round(status.percentage) }}%</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div 
                  class="h-3 rounded-full transition-all duration-500"
                  :class="{
                    'bg-gradient-to-r from-green-500 to-emerald-600': status.status === 'complete',
                    'bg-gradient-to-r from-blue-500 to-purple-600': status.status === 'processing',
                    'bg-gradient-to-r from-orange-500 to-amber-600': status.status === 'pending',
                    'bg-gradient-to-r from-red-500 to-pink-600': status.status === 'cancelled'
                  }"
                  :style="`width: ${status.percentage}%`"
                ></div>
              </div>
            </div>
            <div v-if="orderStatusDistribution.length === 0">
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">No orders</span>
                <span class="font-medium text-slate-900 dark:text-white">0%</span>
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
            :labels="topProducts.length > 0 ? topProducts.map(p => p.name) : ['No products']"
            :data="topProducts.length > 0 ? topProducts.map(p => p.quantity) : [0]"
            label="Units Sold"
            color="#8B5CF6"
            :height="280"
          />
                    </div>

        <!-- Category Sales -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-shadow duration-300">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white mb-4 sm:mb-6">Sales by Category</h3>
          <div class="space-y-4">
            <div v-for="(category, index) in categorySales" :key="category.name">
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">{{ category.name }}</span>
                <span class="font-medium text-slate-900 dark:text-white">R{{ formatCurrency(category.total / 1000) }}K ({{ Math.round(category.percentage) }}%)</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div 
                  class="h-3 rounded-full transition-all duration-500"
                  :class="{
                    'bg-gradient-to-r from-green-500 to-emerald-600': index === 0,
                    'bg-gradient-to-r from-blue-500 to-purple-600': index === 1,
                    'bg-gradient-to-r from-yellow-500 to-orange-600': index === 2,
                    'bg-gradient-to-r from-purple-500 to-pink-600': index === 3,
                    'bg-gradient-to-r from-pink-500 to-rose-600': index >= 4
                  }"
                  :style="`width: ${category.percentage}%`"
                ></div>
              </div>
            </div>
            <div v-if="categorySales.length === 0">
              <div class="flex justify-between text-xs sm:text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">No category data</span>
                <span class="font-medium text-slate-900 dark:text-white">0%</span>
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
import { ref, computed, onMounted } from 'vue'
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
import { getErrorNotification, logError } from '~/utils/errorHandler'
import { useSalesAPI } from '~/composables/useSalesAPI'
import { useDashboard } from '~/composables/useDashboard'
import { useCustomerOrdersAPI } from '~/composables/useCustomerOrdersAPI'

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

// API
const salesAPI = useSalesAPI()
const dashboardAPI = useDashboard()
const ordersAPI = useCustomerOrdersAPI()

// State
const loading = ref(false)

// Sales statistics
const todaysSales = ref(0)
const todaysGrowth = ref(0)
const totalOrders = ref(0)
const pendingOrders = ref(0)
const totalInvoices = ref(0)
const unpaidInvoices = ref(0)
const averageOrder = ref(0)
const conversionRate = ref(0)

// Chart data
const salesTrendData = ref<number[]>([])
const salesTrendLabels = ref<string[]>([])
const orderStatusDistribution = ref<Array<{status: string, percentage: number}>>([])
const topProducts = ref<Array<{name: string, quantity: number}>>([])
const categorySales = ref<Array<{name: string, total: number, percentage: number}>>([])

// Load data on mount
onMounted(async () => {
  await refreshStats()
})

// Load dashboard data
const loadDashboardData = async () => {
  try {
    const shopId = 1 // TODO: Get from session/auth
    
    // Get dashboard summary
    const summary = await dashboardAPI.getDashboardSummary(shopId)
    todaysSales.value = summary.todayRevenue || 0
    totalOrders.value = summary.todayTransactions || 0
    averageOrder.value = summary.todayTransactions > 0 ? (summary.todayRevenue / summary.todayTransactions) : 0
    
    // Get sales trends (last 7 days)
    const endDate = new Date()
    const startDate = new Date()
    startDate.setDate(startDate.getDate() - 7)
    const trends = await dashboardAPI.getSalesTrends({
      shopId,
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    })
    
    salesTrendData.value = trends.map(t => Number(t.totalSales) / 1000) // Convert to thousands
    salesTrendLabels.value = trends.map(t => {
      const date = new Date(t.date)
      return date.toLocaleDateString('en-US', { weekday: 'short' })
    })
    
    // Get top products
    const topProductsData = await dashboardAPI.getTopProducts({
      shopId,
      limit: 4
    })
    topProducts.value = topProductsData.map(p => ({
      name: p.productName,
      quantity: p.quantitySold
    }))
    
    // Get category sales
    const categoryData = await dashboardAPI.getCategorySales({
      shopId,
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    })
    categorySales.value = categoryData.map(c => ({
      name: c.categoryName,
      total: Number(c.totalSales),
      percentage: Number(c.percentage)
    }))
    
    // Get order status distribution
    const orderStatus = await dashboardAPI.getOrderStatusDistribution({ shopId })
    orderStatusDistribution.value = orderStatus.map(os => ({
      status: os.statusName.toLowerCase(),
      percentage: Number(os.percentage)
    }))
    
    // Get orders
    const orders = await ordersAPI.getOrders({ shopId })
    totalOrders.value = orders.length
    pendingOrders.value = orders.filter(o => o.orderStatus === 'Pending').length
    
    // Get invoices
    const invoices = await salesAPI.getInvoices(shopId)
    totalInvoices.value = invoices.items?.length || 0
    unpaidInvoices.value = invoices.items?.filter((i: any) => i.status !== 'paid').length || 0
    
    // Calculate growth (mock for now - would need previous period data)
    todaysGrowth.value = 0
    
    // Calculate conversion rate (mock)
    conversionRate.value = 68
    
  } catch (error) {
    logError(error, 'load_data', 'Failed to load dashboard data')
  }
}

// Helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const refreshStats = async () => {
  loading.value = true
  try {
    await loadDashboardData()
  } catch (error) {
    logError(error, 'load_data', 'Failed to refresh stats')
    // Show user-friendly notification
    const notification = document.createElement('div')
    notification.textContent = getErrorNotification(error, 'load_data')
    notification.className = 'fixed top-20 right-4 bg-red-600 text-white px-4 py-2 rounded-lg shadow-lg z-50'
    document.body.appendChild(notification)
    setTimeout(() => notification.remove(), 3000)
  } finally {
    loading.value = false
  }
}
</script>

