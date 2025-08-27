<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="border-b border-gray-200 dark:border-gray-700 pb-4">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Stock Dashboard</h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Overview of your inventory and stock management
          </p>
        </div>
        <div class="flex space-x-3">
          <NuxtLink
            to="/stock/items"
            class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-lg text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            <CubeIcon class="w-4 h-4 mr-2" />
            Manage Items
          </NuxtLink>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div v-for="i in 4" :key="i" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 animate-pulse">
        <div class="h-4 bg-gray-200 dark:bg-gray-700 rounded mb-2"></div>
        <div class="h-8 bg-gray-200 dark:bg-gray-700 rounded"></div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <!-- Total Items -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
            <CubeIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Total Items</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalItems }}</p>
          </div>
        </div>
      </div>

      <!-- Total Warehouses -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
            <BuildingStorefrontIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Warehouses</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalWarehouses }}</p>
          </div>
        </div>
      </div>

      <!-- Low Stock Items -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-orange-100 dark:bg-orange-900 rounded-lg flex items-center justify-center">
            <ExclamationTriangleIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Low Stock</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.lowStockItems }}</p>
          </div>
        </div>
      </div>

      <!-- Total Stock Value -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center">
            <CurrencyDollarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Stock Value</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ formatCurrency(stats.totalStockValue) }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Quick Actions</h3>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <NuxtLink
          to="/stock/items"
          class="flex items-center p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
        >
          <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-4">
            <CubeIcon class="w-5 h-5 text-blue-600 dark:text-blue-400" />
          </div>
          <div>
            <h4 class="font-medium text-gray-900 dark:text-white">Manage Items</h4>
            <p class="text-sm text-gray-500 dark:text-gray-400">Add, edit, and organize inventory items</p>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/stock/warehouses"
          class="flex items-center p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
        >
          <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center mr-4">
            <BuildingStorefrontIcon class="w-5 h-5 text-green-600 dark:text-green-400" />
          </div>
          <div>
            <h4 class="font-medium text-gray-900 dark:text-white">Warehouses</h4>
            <p class="text-sm text-gray-500 dark:text-gray-400">Manage storage locations and facilities</p>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/stock/movements"
          class="flex items-center p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
        >
          <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center mr-4">
            <ArrowsRightLeftIcon class="w-5 h-5 text-purple-600 dark:text-purple-400" />
          </div>
          <div>
            <h4 class="font-medium text-gray-900 dark:text-white">Stock Movements</h4>
            <p class="text-sm text-gray-500 dark:text-gray-400">Track inventory transactions and transfers</p>
          </div>
        </NuxtLink>
      </div>
    </div>

    <!-- Recent Activity -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Recent Stock Movements -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Movements</h3>
          <NuxtLink
            to="/stock/movements"
            class="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300"
          >
            View all
          </NuxtLink>
        </div>
        <div v-if="recentMovements.length === 0" class="text-center py-8">
          <ArrowsRightLeftIcon class="w-12 h-12 text-gray-400 mx-auto mb-4" />
          <p class="text-gray-500 dark:text-gray-400">No recent movements</p>
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="movement in recentMovements.slice(0, 5)"
            :key="movement.id"
            class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg"
          >
            <div class="flex items-center">
              <div 
                class="w-8 h-8 rounded-full flex items-center justify-center mr-3"
                :class="{
                  'bg-green-100 text-green-600 dark:bg-green-900 dark:text-green-400': movement.movementType === 'IN',
                  'bg-red-100 text-red-600 dark:bg-red-900 dark:text-red-400': movement.movementType === 'OUT',
                  'bg-blue-100 text-blue-600 dark:bg-blue-900 dark:text-blue-400': movement.movementType === 'TRANSFER',
                  'bg-yellow-100 text-yellow-600 dark:bg-yellow-900 dark:text-yellow-400': movement.movementType === 'ADJUSTMENT'
                }"
              >
                <ArrowUpIcon v-if="movement.movementType === 'IN'" class="w-4 h-4" />
                <ArrowDownIcon v-else-if="movement.movementType === 'OUT'" class="w-4 h-4" />
                <ArrowsRightLeftIcon v-else-if="movement.movementType === 'TRANSFER'" class="w-4 h-4" />
                <AdjustmentsHorizontalIcon v-else class="w-4 h-4" />
              </div>
              <div>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ movement.reference }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatDate(movement.movementDate) }}</p>
              </div>
            </div>
            <div class="text-right">
              <p class="text-sm font-medium text-gray-900 dark:text-white">
                {{ movement.quantity }} {{ movement.item?.unit }}
              </p>
              <p class="text-xs text-gray-500 dark:text-gray-400">{{ movement.item?.name }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Low Stock Alerts -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Low Stock Alerts</h3>
          <NuxtLink
            to="/stock/items?filter=low_stock"
            class="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300"
          >
            View all
          </NuxtLink>
        </div>
        <div v-if="lowStockItems.length === 0" class="text-center py-8">
          <CheckCircleIcon class="w-12 h-12 text-green-400 mx-auto mb-4" />
          <p class="text-gray-500 dark:text-gray-400">All items are well stocked</p>
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="item in lowStockItems.slice(0, 5)"
            :key="item.id"
            class="flex items-center justify-between p-3 bg-orange-50 dark:bg-orange-900/20 rounded-lg border border-orange-200 dark:border-orange-800"
          >
            <div class="flex items-center">
              <ExclamationTriangleIcon class="w-5 h-5 text-orange-600 dark:text-orange-400 mr-3" />
              <div>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ item.name }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ item.sku }}</p>
              </div>
            </div>
            <div class="text-right">
              <p class="text-sm font-medium text-orange-600 dark:text-orange-400">
                {{ item.quantityOnHand }} {{ item.unit }}
              </p>
              <p class="text-xs text-gray-500 dark:text-gray-400">
                Reorder at {{ item.reorderLevel }}
              </p>
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
  CubeIcon,
  BuildingStorefrontIcon,
  ExclamationTriangleIcon,
  CurrencyDollarIcon,
  ArrowsRightLeftIcon,
  ArrowUpIcon,
  ArrowDownIcon,
  AdjustmentsHorizontalIcon,
  CheckCircleIcon
} from '@heroicons/vue/24/outline'
import { useStock } from '~/composables/useStock'

// Composables
const { getItems, getWarehouses, getStockMovements } = useStock()

// State
const loading = ref(true)
const items = ref([])
const warehouses = ref([])
const movements = ref([])

// Computed
const stats = computed(() => {
  const totalItems = items.value.length
  const totalWarehouses = warehouses.value.length
  const lowStockItems = items.value.filter(item => 
    (item.quantityOnHand || 0) <= item.reorderLevel
  ).length
  const totalStockValue = items.value.reduce((total, item) => {
    const value = (item.quantityOnHand || 0) * (item.costPrice || item.sellingPrice || 0)
    return total + value
  }, 0)

  return {
    totalItems,
    totalWarehouses,
    lowStockItems,
    totalStockValue
  }
})

const recentMovements = computed(() => {
  return movements.value
    .sort((a, b) => new Date(b.movementDate).getTime() - new Date(a.movementDate).getTime())
    .slice(0, 10)
})

const lowStockItems = computed(() => {
  return items.value
    .filter(item => (item.quantityOnHand || 0) <= item.reorderLevel)
    .sort((a, b) => (a.quantityOnHand || 0) - (b.quantityOnHand || 0))
})

// Methods
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const loadDashboardData = async () => {
  try {
    loading.value = true
    
    // Load data in parallel
    const [itemsResponse, warehousesResponse, movementsResponse] = await Promise.all([
      getItems(),
      getWarehouses(),
      getStockMovements()
    ])
    
    items.value = itemsResponse.data || []
    warehouses.value = warehousesResponse.data || []
    movements.value = movementsResponse.data || []
  } catch (error) {
    console.error('Error loading dashboard data:', error)
  } finally {
    loading.value = false
  }
}

// Lifecycle
onMounted(() => {
  loadDashboardData()
})
</script>
