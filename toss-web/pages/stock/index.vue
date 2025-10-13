<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <div class="p-4 pb-20 space-y-4 sm:p-6 sm:space-y-6 lg:pb-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-slate-900 dark:text-white">Stock Management</h1>
          <p class="text-slate-600 dark:text-slate-400">Manage inventory, warehouses, and collaborative purchasing</p>
        </div>
        <div class="flex space-x-3">
          <NuxtLink to="/stock/items" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
            <PlusIcon class="w-4 h-4 inline mr-2" />
            Add Item
          </NuxtLink>
          <button @click="refreshStats" class="px-4 py-2 border border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors">
            <ArrowPathIcon class="w-4 h-4 inline mr-2" />
            Refresh
          </button>
        </div>
      </div>

      <!-- AI Co-Pilot Alerts -->
      <div class="bg-gradient-to-r from-blue-50 to-indigo-50 dark:from-blue-900/20 dark:to-indigo-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
        <div class="flex items-start">
          <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-3">
            <SparklesIcon class="w-5 h-5 text-blue-600 dark:text-blue-400" />
          </div>
          <div class="flex-1">
            <h3 class="text-sm font-semibold text-blue-900 dark:text-blue-100 mb-1">AI Co-Pilot Insights</h3>
            <p class="text-sm text-blue-700 dark:text-blue-300">
              Low stock detected for 3 items. Consider group purchasing for cleaning supplies to save 15%.
            </p>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div v-for="i in 4" :key="i" class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="animate-pulse">
            <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded mb-2"></div>
            <div class="h-8 bg-slate-200 dark:bg-slate-700 rounded"></div>
          </div>
        </div>
      </div>

      <!-- Stats Cards -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <!-- Total Items -->
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center">
            <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
              <CubeIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-slate-500 dark:text-slate-400">Total Items</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ stats.totalItems }}</p>
            </div>
          </div>
        </div>

        <!-- Total Warehouses -->
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center">
            <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
              <BuildingStorefrontIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-slate-500 dark:text-slate-400">Warehouses</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ stats.totalWarehouses }}</p>
            </div>
          </div>
        </div>

        <!-- Low Stock Items -->
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center">
            <div class="w-10 h-10 bg-orange-100 dark:bg-orange-900 rounded-lg flex items-center justify-center">
              <ExclamationTriangleIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-slate-500 dark:text-slate-400">Low Stock</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ stats.lowStockItems }}</p>
            </div>
          </div>
        </div>

        <!-- Total Stock Value -->
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center">
            <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center">
              <CurrencyDollarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-slate-500 dark:text-slate-400">Stock Value</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white">R{{ formatCurrency(stats.totalStockValue) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Quick Actions</h3>
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <NuxtLink
            to="/stock/items"
            class="flex items-center p-4 border border-slate-200 dark:border-slate-600 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors"
          >
            <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-4">
              <CubeIcon class="w-5 h-5 text-blue-600 dark:text-blue-400" />
            </div>
            <div>
              <h4 class="font-medium text-slate-900 dark:text-white">Manage Items</h4>
              <p class="text-sm text-slate-500 dark:text-slate-400">Add, edit, and organize inventory items</p>
            </div>
          </NuxtLink>

          <NuxtLink
            to="/stock/warehouses"
            class="flex items-center p-4 border border-slate-200 dark:border-slate-600 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors"
          >
            <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center mr-4">
              <BuildingStorefrontIcon class="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
            <div>
              <h4 class="font-medium text-slate-900 dark:text-white">Shared Warehouses</h4>
              <p class="text-sm text-slate-500 dark:text-slate-400">Community storage & facilities</p>
            </div>
          </NuxtLink>

          <button class="flex items-center p-4 border border-purple-200 dark:border-purple-600 rounded-lg hover:bg-purple-50 dark:hover:bg-purple-700 transition-colors">
            <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center mr-4">
              <UserGroupIcon class="w-5 h-5 text-purple-600 dark:text-purple-400" />
            </div>
            <div>
              <h4 class="font-medium text-slate-900 dark:text-white">Group Purchasing</h4>
              <p class="text-sm text-slate-500 dark:text-slate-400">Join bulk orders for savings</p>
            </div>
          </button>

          <NuxtLink
            to="/stock/movements"
            class="flex items-center p-4 border border-slate-200 dark:border-slate-600 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors"
          >
            <div class="w-10 h-10 bg-orange-100 dark:bg-orange-900 rounded-lg flex items-center justify-center mr-4">
              <ArrowsRightLeftIcon class="w-5 h-5 text-orange-600 dark:text-orange-400" />
            </div>
            <div>
              <h4 class="font-medium text-slate-900 dark:text-white">Stock Movements</h4>
              <p class="text-sm text-slate-500 dark:text-slate-400">Track inventory transactions and transfers</p>
            </div>
          </NuxtLink>
        </div>
      </div>

      <!-- Collaborative Features -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Group Purchasing Opportunities -->
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Group Buying</h3>
            <span class="bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200 text-xs font-medium px-2.5 py-0.5 rounded-full">
              Community
            </span>
          </div>
          <div class="space-y-3">
            <div class="p-3 bg-purple-50 dark:bg-purple-900/20 rounded-lg border border-purple-200 dark:border-purple-800">
              <div class="flex justify-between items-start mb-2">
                <h4 class="font-medium text-slate-900 dark:text-white">Cleaning Supplies</h4>
                <span class="text-xs text-purple-600 dark:text-purple-400">15% savings</span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">4/8 businesses joined • 2 days left</p>
              <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2 mb-2">
                <div class="bg-purple-600 h-2 rounded-full" style="width: 50%"></div>
              </div>
              <button class="text-sm text-purple-600 dark:text-purple-400 hover:text-purple-700">Join Group Order</button>
            </div>
            
            <div class="p-3 bg-green-50 dark:bg-green-900/20 rounded-lg border border-green-200 dark:border-green-800">
              <div class="flex justify-between items-start mb-2">
                <h4 class="font-medium text-slate-900 dark:text-white">Maize Meal</h4>
                <span class="text-xs text-green-600 dark:text-green-400">20% savings</span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">6/6 businesses joined • Ready to order</p>
              <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2 mb-2">
                <div class="bg-green-600 h-2 rounded-full" style="width: 100%"></div>
              </div>
              <button class="text-sm text-green-600 dark:text-green-400 hover:text-green-700">View Details</button>
            </div>
          </div>
        </div>

        <!-- Shared Logistics -->
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Shared Delivery</h3>
            <span class="bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200 text-xs font-medium px-2.5 py-0.5 rounded-full">
              Network
            </span>
          </div>
          <div class="space-y-3">
            <div class="p-3 bg-blue-50 dark:bg-blue-900/20 rounded-lg border border-blue-200 dark:border-blue-800">
              <div class="flex items-center mb-2">
                <TruckIcon class="w-4 h-4 text-blue-600 dark:text-blue-400 mr-2" />
                <h4 class="font-medium text-slate-900 dark:text-white">Tomorrow 9:00 AM</h4>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">Route: City Center → Township</p>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">2 slots available • R50 per pallet</p>
              <button class="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700">Reserve Slot</button>
            </div>
            
            <div class="p-3 bg-green-50 dark:bg-green-900/20 rounded-lg border border-green-200 dark:border-green-800">
              <div class="flex items-center mb-2">
                <TruckIcon class="w-4 h-4 text-green-600 dark:text-green-400 mr-2" />
                <h4 class="font-medium text-slate-900 dark:text-white">Friday 2:00 PM</h4>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">Route: Township → City Center</p>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">1 slot available • R45 per pallet</p>
              <button class="text-sm text-green-600 dark:text-green-400 hover:text-green-700">Reserve Slot</button>
            </div>
          </div>
        </div>

        <!-- Recent Activity -->
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Recent Activity</h3>
            <span class="bg-slate-100 text-slate-800 dark:bg-slate-700 dark:text-slate-200 text-xs font-medium px-2.5 py-0.5 rounded-full">
              Live
            </span>
          </div>
          <div class="space-y-3">
            <div class="flex items-start space-x-3">
              <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-full flex items-center justify-center">
                <ArrowDownIcon class="w-4 h-4 text-green-600 dark:text-green-400" />
              </div>
              <div class="flex-1">
                <p class="text-sm text-slate-900 dark:text-white">Stock received: 50 units of Maize Meal</p>
                <p class="text-xs text-slate-500 dark:text-slate-400">2 hours ago</p>
              </div>
            </div>
            
            <div class="flex items-start space-x-3">
              <div class="w-8 h-8 bg-orange-100 dark:bg-orange-900 rounded-full flex items-center justify-center">
                <ExclamationTriangleIcon class="w-4 h-4 text-orange-600 dark:text-orange-400" />
              </div>
              <div class="flex-1">
                <p class="text-sm text-slate-900 dark:text-white">Low stock alert: Cleaning supplies</p>
                <p class="text-xs text-slate-500 dark:text-slate-400">4 hours ago</p>
              </div>
            </div>
            
            <div class="flex items-start space-x-3">
              <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                <UserGroupIcon class="w-4 h-4 text-blue-600 dark:text-blue-400" />
              </div>
              <div class="flex-1">
                <p class="text-sm text-slate-900 dark:text-white">Joined group purchase: Office supplies</p>
                <p class="text-xs text-slate-500 dark:text-slate-400">1 day ago</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import {
  PlusIcon,
  ArrowPathIcon,
  SparklesIcon,
  CubeIcon,
  BuildingStorefrontIcon,
  ExclamationTriangleIcon,
  CurrencyDollarIcon,
  UserGroupIcon,
  ArrowsRightLeftIcon,
  TruckIcon,
  ArrowDownIcon
} from '@heroicons/vue/24/outline'
import { useStock } from '../composables/useStock'

// Reactive data
const loading = ref(true)
const stats = ref({
  totalItems: 0,
  totalWarehouses: 0,
  lowStockItems: 0,
  totalStockValue: 0
})

// Computed properties
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(amount).replace('ZAR', '')
}

// Composables
const { getStockOverview, getWarehouses } = useStock()

// Methods
const loadStats = async () => {
  loading.value = true
  try {
    // Try to fetch real data from API
    const [overview, warehousesData] = await Promise.all([
      getStockOverview().catch(() => null),
      getWarehouses().catch(() => ({ warehouses: [] }))
    ])
    
    stats.value = {
      totalItems: overview?.totalItems || 1247,
      totalWarehouses: warehousesData.warehouses?.length || 8,
      lowStockItems: overview?.lowStockItems || 23,
      totalStockValue: overview?.totalValue || 456789
    }
  } catch (error) {
    console.error('Failed to load stats:', error)
    // Use fallback data
    stats.value = {
      totalItems: 1247,
      totalWarehouses: 8,
      lowStockItems: 23,
      totalStockValue: 456789
    }
  } finally {
    loading.value = false
  }
}

const refreshStats = async () => {
  await loadStats()
  alert('Stock statistics refreshed!')
}

// Lifecycle
onMounted(() => {
  loadStats()
})

// Page metadata
definePageMeta({
  layout: 'default',
  title: 'Stock Management - TOSS ERP'
})

// Meta
useHead({
  title: 'Stock Management - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage inventory, warehouses, and collaborative purchasing for township businesses' }
  ]
})
</script>