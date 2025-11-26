<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              Business Analytics
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400 flex items-center space-x-2">
              <span>{{ formatDate(new Date()) }}</span>
              <span class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
                <span class="w-1.5 h-1.5 bg-green-500 rounded-full mr-1.5 animate-pulse"></span>
                Live
              </span>
            </p>
          </div>
          <div class="flex items-center space-x-3">
            <MaterialButton
              @click="refreshData"
              variant="outlined"
              size="md"
            >
              <ArrowPathIcon class="w-4 h-4 mr-2" :class="{ 'animate-spin': loading }" />
              Refresh
            </MaterialButton>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Loading State -->
      <div v-if="loading" class="space-y-6">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div v-for="i in 4" :key="i" class="bg-white dark:bg-slate-800 rounded-2xl p-6 animate-pulse shadow-lg">
            <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-1/2 mb-2"></div>
            <div class="h-8 bg-slate-200 dark:bg-slate-700 rounded w-3/4"></div>
          </div>
        </div>
      </div>

      <!-- Dashboard Content -->
      <div v-else class="space-y-6">
        <!-- Key Metrics - Material Design Cards -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <MaterialStatsCard
            label="Total Revenue"
            :value="metrics.totalRevenue"
            :icon="CurrencyDollarIcon"
            :change="metrics.revenueChange"
            gradient="green"
            :sparkline-data="revenueSparkline"
            prefix="R"
          />

          <MaterialStatsCard
            label="Total Orders"
            :value="metrics.totalOrders"
            :icon="ShoppingCartIcon"
            :change="metrics.ordersChange"
            gradient="blue"
            :sparkline-data="ordersSparkline"
          />

          <MaterialStatsCard
            label="Group Buy Savings"
            :value="metrics.groupBuySavings"
            :icon="UserGroupIcon"
            :change="15.3"
            gradient="purple"
            prefix="R"
          />

          <MaterialStatsCard
            label="Delivery Costs"
            :value="metrics.totalDeliveryCost"
            :icon="TruckIcon"
            :change="-12.5"
            gradient="orange"
            prefix="R"
          />
        </div>

        <!-- Charts Section -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <!-- Revenue Trend -->
          <MaterialCard variant="elevated" class="lg:col-span-2" hover>
            <div class="flex items-center justify-between mb-6">
              <div>
                <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Daily Sales</h3>
                <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">
                  <span class="font-semibold text-green-600 dark:text-green-400">(+{{ metrics.revenueChange.toFixed(1) }}%)</span> 
                  increase in today's sales
                </p>
              </div>
              <div class="flex items-center space-x-2 text-xs text-slate-500">
                <ClockIcon class="w-4 h-4" />
                <span>Updated {{ minutesAgo }} min ago</span>
              </div>
            </div>
            <LineChart
              :labels="dailySalesLabels"
              :data="dailySalesData"
              label="Revenue"
              color="#10B981"
              :height="280"
            />
          </MaterialCard>

          <!-- Quick Stats -->
          <MaterialCard variant="elevated" hover>
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-6">Quick Stats</h3>
            <div class="space-y-4">
              <div class="flex items-center justify-between p-4 bg-gradient-to-r from-blue-50 to-blue-100/50 dark:from-blue-900/20 dark:to-blue-900/10 rounded-xl border border-blue-200/50 dark:border-blue-800/50">
                <div>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Stock Value</p>
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ formatCurrency(metrics.stockValue) }}</p>
                </div>
                <div class="p-3 bg-blue-500 rounded-lg">
                  <CubeIcon class="w-6 h-6 text-white" />
                </div>
              </div>

              <div class="flex items-center justify-between p-4 bg-gradient-to-r from-purple-50 to-purple-100/50 dark:from-purple-900/20 dark:to-purple-900/10 rounded-xl border border-purple-200/50 dark:border-purple-800/50">
                <div>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Active Pools</p>
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ metrics.activePoolsCount }}</p>
                </div>
                <div class="p-3 bg-purple-500 rounded-lg">
                  <UsersIcon class="w-6 h-6 text-white" />
                </div>
              </div>

              <div class="flex items-center justify-between p-4 bg-gradient-to-r from-orange-50 to-orange-100/50 dark:from-orange-900/20 dark:to-orange-900/10 rounded-xl border border-orange-200/50 dark:border-orange-800/50">
                <div>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Deliveries</p>
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ metrics.totalDeliveries }}</p>
                </div>
                <div class="p-3 bg-orange-500 rounded-lg">
                  <TruckIcon class="w-6 h-6 text-white" />
                </div>
              </div>
            </div>
          </MaterialCard>
        </div>

        <!-- Sales by Category & Stock Levels -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Sales by Category -->
          <MaterialCard variant="elevated" hover>
            <div class="flex items-center justify-between mb-6">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Sales by Category</h3>
              <MaterialButton variant="text" color="primary" size="sm">
                View All
              </MaterialButton>
            </div>
            <BarChart
              :labels="categoryLabels"
              :data="categoryData"
              label="Sales"
              color="#3B82F6"
              :height="280"
            />
          </MaterialCard>

          <!-- Low Stock Alerts -->
          <MaterialCard variant="elevated" hover>
            <div class="flex items-center justify-between mb-6">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Low Stock Items</h3>
              <span class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400">
                {{ lowStockItems.length }} items
              </span>
            </div>
            <div class="space-y-3">
              <div
                v-for="item in lowStockItems.slice(0, 5)"
                :key="item.id"
                class="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-700/50 rounded-xl hover:bg-slate-100 dark:hover:bg-slate-700 transition-colors cursor-pointer"
              >
                <div class="flex items-center space-x-3">
                  <div class="p-2 bg-orange-100 dark:bg-orange-900/30 rounded-lg">
                    <ExclamationTriangleIcon class="w-5 h-5 text-orange-600 dark:text-orange-400" />
                  </div>
                  <div>
                    <p class="font-medium text-slate-900 dark:text-white">{{ item.name }}</p>
                    <p class="text-sm text-slate-600 dark:text-slate-400">{{ item.quantity }} units left</p>
                  </div>
                </div>
                <MaterialButton variant="text" color="primary" size="sm">
                  Reorder
                </MaterialButton>
              </div>
            </div>
            <NuxtLink
              to="/stock/items"
              class="block mt-4 text-center text-sm text-blue-600 dark:text-blue-400 hover:underline font-medium"
            >
              View all items →
            </NuxtLink>
          </MaterialCard>
        </div>

        <!-- AI Insights Card -->
        <MaterialCard gradient="purple" class="p-8 text-white relative overflow-hidden">
          <!-- Decorative elements -->
          <div class="absolute top-0 right-0 w-64 h-64 bg-white/10 rounded-full -mr-32 -mt-32"></div>
          <div class="absolute bottom-0 left-0 w-48 h-48 bg-black/10 rounded-full -ml-24 -mb-24"></div>
          
          <div class="relative z-10">
            <div class="flex items-start justify-between">
              <div class="flex items-center space-x-3 mb-4">
                <div class="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <SparklesIcon class="w-6 h-6 text-white" />
                </div>
                <div>
                  <h3 class="text-xl font-bold">AI Copilot Insights</h3>
                  <p class="text-white/80 text-sm">Smart recommendations for your business</p>
                </div>
              </div>
            </div>
            
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mt-6">
              <div class="bg-white/10 backdrop-blur-sm rounded-xl p-4 border border-white/20">
                <p class="text-white/80 text-sm mb-2">Cost Savings Opportunity</p>
                <p class="text-2xl font-bold">R{{ formatNumber(aiInsights.potentialSavings) }}</p>
                <p class="text-white/70 text-xs mt-2">Join 3 active group buying pools</p>
              </div>
              
              <div class="bg-white/10 backdrop-blur-sm rounded-xl p-4 border border-white/20">
                <p class="text-white/80 text-sm mb-2">Reorder Suggestion</p>
                <p class="text-2xl font-bold">{{ aiInsights.itemsToReorder }} items</p>
                <p class="text-white/70 text-xs mt-2">Will run out in 3-5 days</p>
              </div>
              
              <div class="bg-white/10 backdrop-blur-sm rounded-xl p-4 border border-white/20">
                <p class="text-white/80 text-sm mb-2">Delivery Optimization</p>
                <p class="text-2xl font-bold">-35%</p>
                <p class="text-white/70 text-xs mt-2">Share next delivery with 2 neighbors</p>
              </div>
            </div>
          </div>
        </MaterialCard>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  ArrowPathIcon,
  CurrencyDollarIcon,
  ShoppingCartIcon,
  UserGroupIcon,
  TruckIcon,
  CubeIcon,
  UsersIcon,
  ClockIcon,
  ExclamationTriangleIcon,
  SparklesIcon
} from '@heroicons/vue/24/outline'

// Meta - useHead is auto-imported by Nuxt
useHead({
  title: 'Business Analytics - TOSS ERP',
  meta: [
    { name: 'description', content: 'Real-time business analytics and insights for your township business' }
  ]
})

// State
const loading = ref(false)
const minutesAgo = ref(4)

// Metrics
const metrics = ref({
  totalRevenue: 45678,
  revenueChange: 15.3,
  totalOrders: 234,
  ordersChange: 12.7,
  groupBuySavings: 8945,
  activePoolsCount: 12,
  totalDeliveryCost: 1250,
  stockValue: 89456,
  totalDeliveries: 156
})

// AI Insights
const aiInsights = ref({
  potentialSavings: 3450,
  itemsToReorder: 23,
  deliveryOptimization: -35
})

// Chart Data
const dailySalesLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
const dailySalesData = [3200, 4100, 3800, 5200, 4800, 6100, 5500]

const categoryLabels = ['Groceries', 'Beverages', 'Snacks', 'Toiletries', 'Airtime']
const categoryData = [12500, 8900, 6700, 5400, 4200]

const revenueSparkline = [3200, 3600, 3400, 4100, 4500, 4800, 5200]
const ordersSparkline = [45, 52, 48, 61, 58, 67, 72]

// Low Stock Items
const lowStockItems = ref([
  { id: 1, name: 'White Bread', quantity: 12, reorderLevel: 30 },
  { id: 2, name: 'Fresh Milk 1L', quantity: 8, reorderLevel: 20 },
  { id: 3, name: 'Sugar 2.5kg', quantity: 5, reorderLevel: 15 },
  { id: 4, name: 'Cooking Oil 750ml', quantity: 10, reorderLevel: 25 },
  { id: 5, name: 'Maize Meal 10kg', quantity: 6, reorderLevel: 20 }
])

// Methods
const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(value).replace('ZAR', 'R')
}

const formatNumber = (value: number) => {
  return new Intl.NumberFormat('en-ZA').format(value)
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  }).format(date)
}

const refreshData = async () => {
  loading.value = true
  // Simulate API call
  setTimeout(() => {
    loading.value = false
    minutesAgo.value = 0
  }, 1000)
}

// Update minutes ago every minute
onMounted(() => {
  setInterval(() => {
    if (minutesAgo.value < 60) {
      minutesAgo.value++
    }
  }, 60000)
})
</script>

<style scoped>
/* Additional animations */
@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-slideUp {
  animation: slideUp 0.3s ease-out;
}
</style>
