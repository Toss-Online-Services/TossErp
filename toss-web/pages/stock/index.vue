<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-purple-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-6">
        <div class="flex flex-col space-y-3 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
          <div class="flex-1 min-w-0 overflow-hidden">
            <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold bg-gradient-to-r from-purple-600 to-blue-600 bg-clip-text text-transparent truncate">
              Stock Management
            </h1>
            <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400 truncate">
              Track your inventory and manage stock
            </p>
        </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <NuxtLink 
              to="/stock/items" 
              class="inline-flex items-center justify-center p-2 sm:px-4 sm:py-2.5 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 text-xs sm:text-sm font-semibold"
            >
              <PlusIcon class="w-5 h-5 sm:w-4 sm:h-4 sm:mr-2" />
              <span class="hidden sm:inline">Add Item</span>
          </NuxtLink>
            <button 
              @click="refreshStats" 
              class="inline-flex items-center justify-center p-2 sm:px-4 sm:py-2.5 rounded-xl text-xs sm:text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 hover:shadow-md transition-all duration-200"
              title="Refresh"
            >
              <ArrowPathIcon class="w-5 h-5 sm:w-4 sm:h-4 sm:mr-2" :class="{ 'animate-spin': loading }" />
              <span class="hidden sm:inline">Refresh</span>
          </button>
          </div>
        </div>
        </div>
      </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-8">
      <!-- AI Co-Pilot Banner -->
      <div class="mb-4 sm:mb-6 bg-gradient-to-r from-blue-500 via-purple-500 to-pink-500 rounded-2xl shadow-2xl p-4 sm:p-6 text-white relative overflow-hidden">
        <div class="absolute top-0 right-0 w-32 h-32 sm:w-64 sm:h-64 bg-white/10 rounded-full -mr-16 sm:-mr-32 -mt-16 sm:-mt-32"></div>
        <div class="relative z-10 flex items-start gap-3 sm:gap-4">
          <div class="p-2 sm:p-3 bg-white/20 backdrop-blur-sm rounded-xl flex-shrink-0">
            <SparklesIcon class="w-5 h-5 sm:w-6 sm:h-6" />
          </div>
          <div class="flex-1 min-w-0 overflow-hidden">
            <h3 class="text-base sm:text-lg font-bold mb-1 sm:mb-2">AI Co-Pilot Insights</h3>
            <p class="text-white/90 text-xs sm:text-sm leading-relaxed break-words">
              Low stock detected for <strong>3 items</strong>. Consider joining a group buying pool for cleaning supplies to save <strong>15%</strong>.
            </p>
            <button class="mt-3 sm:mt-4 px-4 sm:px-5 py-2 sm:py-2.5 bg-white/20 hover:bg-white/30 backdrop-blur-sm rounded-lg text-xs sm:text-sm font-medium transition-all duration-200">
              View Suggestions →
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6 mb-4 sm:mb-6">
        <div v-for="i in 4" :key="i" class="bg-white dark:bg-slate-800 rounded-2xl p-4 sm:p-6 animate-pulse shadow-lg">
          <div class="h-3 sm:h-4 bg-slate-200 dark:bg-slate-700 rounded mb-2"></div>
          <div class="h-6 sm:h-8 bg-slate-200 dark:bg-slate-700 rounded"></div>
        </div>
      </div>

      <!-- Stats Cards -->
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6 mb-4 sm:mb-6">
        <StatsCard
          label="Total Items"
          :value="stats.totalItems"
          :icon="CubeIcon"
          :change="8.2"
          change-label="new items this month"
          gradient="blue"
        />

        <StatsCard
          label="Categories"
          :value="stats.totalCategories"
          :icon="ShoppingCartIcon"
          gradient="green"
        />

        <StatsCard
          label="Low Stock Items"
          :value="stats.lowStockItems"
          :icon="ExclamationTriangleIcon"
          :change="-12.3"
          change-label="vs last week"
          gradient="orange"
        />

        <StatsCard
          label="Stock Value"
          :value="stats.totalStockValue"
          :icon="CurrencyDollarIcon"
          :change="15.7"
          change-label="vs last month"
          gradient="purple"
          prefix="R"
        />
        </div>

      <!-- Charts & Quick Actions -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6 mb-4 sm:mb-6">
        <!-- Stock Levels Chart -->
        <div class="lg:col-span-2 bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-shadow duration-300">
          <div class="flex items-center justify-between mb-6">
            <div>
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Stock Movement</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">
                Last 7 days activity
              </p>
            </div>
          </div>
          <LineChart
            :labels="['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']"
            :data="[120, 135, 128, 145, 152, 148, 160]"
            label="Items Sold"
            color="#8B5CF6"
            :height="280"
          />
      </div>

      <!-- Quick Actions -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-shadow duration-300">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white mb-4 sm:mb-6">Quick Actions</h3>
          <div class="space-y-3">
          <NuxtLink
            to="/stock/items"
              class="flex items-center p-4 bg-gradient-to-r from-blue-50 to-blue-100/50 dark:from-blue-900/20 dark:to-blue-900/10 rounded-xl hover:shadow-md transition-all duration-200 group"
          >
              <div class="p-3 bg-blue-500 rounded-lg group-hover:scale-110 transition-transform">
                <CubeIcon class="w-5 h-5 text-white" />
            </div>
              <div class="ml-3 flex-1">
                <p class="font-medium text-slate-900 dark:text-white">Manage Items</p>
                <p class="text-sm text-slate-600 dark:text-slate-400">View all stock items</p>
            </div>
          </NuxtLink>

          <NuxtLink
              to="/stock/movements"
              class="flex items-center p-4 bg-gradient-to-r from-green-50 to-green-100/50 dark:from-green-900/20 dark:to-green-900/10 rounded-xl hover:shadow-md transition-all duration-200 group"
          >
              <div class="p-3 bg-green-500 rounded-lg group-hover:scale-110 transition-transform">
                <ArrowsRightLeftIcon class="w-5 h-5 text-white" />
            </div>
              <div class="ml-3 flex-1">
                <p class="font-medium text-slate-900 dark:text-white">Track Movements</p>
                <p class="text-sm text-slate-600 dark:text-slate-400">View stock changes</p>
            </div>
          </NuxtLink>

          <NuxtLink
              to="/purchasing/create-order"
              class="flex items-center p-4 bg-gradient-to-r from-purple-50 to-purple-100/50 dark:from-purple-900/20 dark:to-purple-900/10 rounded-xl hover:shadow-md transition-all duration-200 group"
            >
              <div class="p-3 bg-purple-500 rounded-lg group-hover:scale-110 transition-transform">
                <ShoppingCartIcon class="w-5 h-5 text-white" />
              </div>
              <div class="ml-3 flex-1">
                <p class="font-medium text-slate-900 dark:text-white">Create Order</p>
                <p class="text-sm text-slate-600 dark:text-slate-400">AI-powered ordering</p>
              </div>
            </NuxtLink>

            <NuxtLink
              to="/purchasing/group-buying"
              class="flex items-center p-4 bg-gradient-to-r from-orange-50 to-orange-100/50 dark:from-orange-900/20 dark:to-orange-900/10 rounded-xl hover:shadow-md transition-all duration-200 group"
            >
              <div class="p-3 bg-orange-500 rounded-lg group-hover:scale-110 transition-transform">
                <UserGroupIcon class="w-5 h-5 text-white" />
              </div>
              <div class="ml-3 flex-1">
                <p class="font-medium text-slate-900 dark:text-white">Group Buying</p>
                <p class="text-sm text-slate-600 dark:text-slate-400">Save with neighbors</p>
              </div>
            </NuxtLink>
            </div>
          </div>
        </div>

      <!-- Low Stock Alerts & Top Items -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Low Stock Alerts -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-shadow duration-300">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Low Stock Alerts</h3>
            <span class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400">
              {{ stats.lowStockItems }} items
            </span>
          </div>
          <div class="space-y-3">
            <div
              v-for="item in lowStockItemsList"
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
              <button class="text-sm text-blue-600 dark:text-blue-400 hover:underline">Reorder</button>
            </div>
          </div>
          <NuxtLink
            to="/stock/items"
            class="block mt-4 text-center text-sm text-blue-600 dark:text-blue-400 hover:underline"
          >
            View all items →
          </NuxtLink>
        </div>

        <!-- Top Selling Items -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-shadow duration-300">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-6">Top Selling Items</h3>
          <BarChart
            :labels="['White Bread', 'Milk 1L', 'Sugar 2.5kg', 'Maize Meal', 'Cooking Oil']"
            :data="[340, 280, 220, 190, 165]"
            label="Units Sold"
            color="#10B981"
            :height="280"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import {
  PlusIcon,
  ArrowPathIcon,
  SparklesIcon,
  CubeIcon,
  ExclamationTriangleIcon,
  CurrencyDollarIcon,
  ArrowsRightLeftIcon,
  UserGroupIcon,
  ShoppingCartIcon,
  TruckIcon
} from '@heroicons/vue/24/outline'
// Manual imports for chart components
import StatsCard from '~/components/charts/StatsCard.vue'
import LineChart from '~/components/charts/LineChart.vue'
import BarChart from '~/components/charts/BarChart.vue'

// Meta
useHead({
  title: 'Stock Management - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage your inventory and track stock levels' }
  ]
})

// Composables
const { getStockOverview } = useStock()

// State
const loading = ref(false)
const stats = ref({
  totalItems: 1247,
  totalCategories: 32,
  lowStockItems: 23,
  totalStockValue: 456789
})

const lowStockItemsList = ref([
  { id: 1, name: 'White Bread', quantity: 12, reorderLevel: 30 },
  { id: 2, name: 'Fresh Milk 1L', quantity: 8, reorderLevel: 20 },
  { id: 3, name: 'Sugar 2.5kg', quantity: 5, reorderLevel: 15 },
  { id: 4, name: 'Cooking Oil 750ml', quantity: 10, reorderLevel: 25 }
])

// Methods
const loadStats = async () => {
  loading.value = true
  try {
    const overview = await getStockOverview().catch(() => null)
    
    if (overview) {
    stats.value = {
        totalItems: overview.totalItems || stats.value.totalItems,
        totalCategories: overview.totalCategories || stats.value.totalCategories,
        lowStockItems: overview.lowStockItems || stats.value.lowStockItems,
        totalStockValue: overview.totalValue || stats.value.totalStockValue
      }
    }
  } catch (error) {
    console.error('Failed to load stats:', error)
  } finally {
    loading.value = false
  }
}

const refreshStats = () => {
  loadStats()
}

// Lifecycle
onMounted(() => {
  loadStats()
})
</script>
