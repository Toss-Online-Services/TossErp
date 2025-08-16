<template>
  <div class="space-y-6">
    <!-- Page header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Dashboard</h1>
        <p class="text-gray-600 dark:text-gray-400">Stock management overview and insights</p>
      </div>
      <div class="flex space-x-3">
        <button class="btn-outline" @click="showBulkOperations = !showBulkOperations">
          <ArrowDownTrayIcon class="w-4 h-4 mr-2" />
          {{ showBulkOperations ? 'Hide' : 'Show' }} Bulk Operations
        </button>
        <button class="btn-primary" @click="showAddItemModal = true">
          <PlusIcon class="w-4 h-4 mr-2" />
          Add Item
        </button>
      </div>
    </div>

    <!-- Stats cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="stat-card">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-primary-100 dark:bg-primary-900 rounded-lg flex items-center justify-center">
              <CubeIcon class="w-5 h-5 text-primary-600 dark:text-primary-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="stat-label">Total Items</p>
            <p class="stat-value">{{ stockStore.stats.totalItems.toLocaleString() }}</p>
            <p class="stat-change-positive">
              <ArrowUpIcon class="w-4 h-4 inline mr-1" />
              +{{ Math.round((stockStore.stats.totalItems / 100) * 12.5) }}%
            </p>
          </div>
        </div>
      </div>

      <div class="stat-card">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-secondary-100 dark:bg-secondary-900 rounded-lg flex items-center justify-center">
              <TruckIcon class="w-5 h-5 text-secondary-600 dark:text-secondary-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="stat-label">Warehouses</p>
            <p class="stat-value">{{ stockStore.stats.warehousesCount }}</p>
            <p class="stat-change-positive">
              <ArrowUpIcon class="w-4 h-4 inline mr-1" />
              +{{ Math.max(0, stockStore.stats.warehousesCount - 6) }}
            </p>
          </div>
        </div>
      </div>

      <div class="stat-card">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-accent-100 dark:bg-accent-900 rounded-lg flex items-center justify-center">
              <ExclamationTriangleIcon class="w-5 h-5 text-accent-600 dark:text-accent-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="stat-label">Low Stock</p>
            <p class="stat-value">{{ stockStore.stats.lowStockCount }}</p>
            <p class="stat-change-negative">
              <ArrowDownIcon class="w-4 h-4 inline mr-1" />
              +{{ Math.max(0, stockStore.stats.lowStockCount - 18) }}
            </p>
          </div>
        </div>
      </div>

      <div class="stat-card">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
              <CurrencyDollarIcon class="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
          </div>
          <div class="ml-4">
            <p class="stat-label">Total Value</p>
            <p class="stat-value">${{ formatCurrency(stockStore.stats.totalValue) }}</p>
            <p class="stat-change-positive">
              <ArrowUpIcon class="w-4 h-4 inline mr-1" />
              +{{ Math.round((stockStore.stats.totalValue / 45000) * 8.3) }}%
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts and tables -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Stock movement chart -->
      <div class="card">
        <div class="card-header">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Stock Movement</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">Last 30 days</p>
        </div>
        <div class="card-body">
          <StockMovementChart :height="300" />
        </div>
      </div>

      <!-- Category distribution chart -->
      <div class="card">
        <div class="card-header">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Category Distribution</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">By item count</p>
        </div>
        <div class="card-body">
          <CategoryDistributionChart :height="300" />
        </div>
      </div>
    </div>

    <!-- Recent activities -->
    <div class="card">
      <div class="card-header">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Activities</h3>
        <button
          @click="refreshActivities"
          :disabled="isRefreshingActivities"
          class="text-sm text-primary-600 hover:text-primary-700 dark:text-primary-400 dark:hover:text-primary-300"
        >
          <ArrowPathIcon v-if="!isRefreshingActivities" class="w-4 h-4" />
          <div v-else class="w-4 h-4 border-2 border-primary-600 border-t-transparent rounded-full animate-spin"></div>
        </button>
      </div>
      <div class="card-body">
        <div class="space-y-4">
          <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-3">
            <div class="flex-shrink-0">
              <div :class="[
                'w-8 h-8 rounded-full flex items-center justify-center',
                activity.type === 'in' ? 'bg-green-100 dark:bg-green-900' : 'bg-red-100 dark:bg-red-900'
              ]">
                <component 
                  :is="activity.type === 'in' ? ArrowDownIcon : ArrowUpIcon" 
                  :class="[
                    'w-4 h-4',
                    activity.type === 'in' ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'
                  ]"
                />
              </div>
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-gray-900 dark:text-white">
                {{ activity.description }}
              </p>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ formatTimeAgo(activity.timestamp) }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- AI Insights Panel -->
    <AiInsightsPanel />

    <!-- Bulk Operations -->
    <div v-if="showBulkOperations">
      <BulkImportExport />
    </div>

    <!-- Quick actions -->
    <div class="card">
      <div class="card-header">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Quick Actions</h3>
      </div>
      <div class="card-body">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <button 
            @click="showAddItemModal = true"
            class="flex items-center p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors"
          >
            <PlusIcon class="w-6 h-6 text-primary-600 dark:text-primary-400 mr-3" />
            <div class="text-left">
              <p class="text-sm font-medium text-gray-900 dark:text-white">Add Item</p>
              <p class="text-xs text-gray-500 dark:text-gray-400">Create new inventory item</p>
            </div>
          </button>
          
          <button 
            @click="navigateToStock"
            class="flex items-center p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors"
          >
            <ArrowDownTrayIcon class="w-6 h-6 text-secondary-600 dark:text-secondary-400 mr-3" />
            <div class="text-left">
              <p class="text-sm font-medium text-gray-900 dark:text-white">Receive Stock</p>
              <p class="text-xs text-gray-500 dark:text-gray-400">Record incoming inventory</p>
            </div>
          </button>
          
          <button 
            @click="navigateToStock"
            class="flex items-center p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors"
          >
            <ArrowUpTrayIcon class="w-6 h-6 text-accent-600 dark:text-accent-400 mr-3" />
            <div class="text-left">
              <p class="text-sm font-medium text-gray-900 dark:text-white">Issue Stock</p>
              <p class="text-xs text-gray-500 dark:text-gray-400">Record outgoing inventory</p>
            </div>
          </button>
          
          <button 
            @click="showBulkOperations = true"
            class="flex items-center p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors"
          >
            <ChartBarIcon class="w-6 h-6 text-green-600 dark:text-green-400 mr-3" />
            <div class="text-left">
              <p class="text-sm font-medium text-gray-900 dark:text-white">Bulk Operations</p>
              <p class="text-xs text-gray-500 dark:text-gray-400">Import/Export data</p>
            </div>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import {
  ArrowDownTrayIcon,
  PlusIcon,
  CubeIcon,
  TruckIcon,
  ExclamationTriangleIcon,
  CurrencyDollarIcon,
  ArrowUpIcon,
  ArrowDownIcon,
  ArrowUpTrayIcon,
  ChartBarIcon,
  ArrowPathIcon
} from '@heroicons/vue/24/outline'
import { useStockStore } from '../stores/stock'
import StockMovementChart from '../components/charts/StockMovementChart.vue'
import CategoryDistributionChart from '../components/charts/CategoryDistributionChart.vue'
import AiInsightsPanel from '../components/AiInsightsPanel.vue'
import BulkImportExport from '../components/BulkImportExport.vue'

const router = useRouter()
const stockStore = useStockStore()

// State
const showBulkOperations = ref(false)
const showAddItemModal = ref(false)
const isRefreshingActivities = ref(false)

// Computed properties
const recentActivities = computed(() => {
  return stockStore.movements.slice(0, 5).map(movement => ({
    id: movement.id,
    type: movement.type === 'in' ? 'in' : 'out',
    description: `${movement.type === 'in' ? 'Received' : 'Issued'} ${movement.quantity} units of ${movement.itemName}`,
    timestamp: movement.timestamp
  }))
})

// Methods
const formatCurrency = (amount: number): string => {
  if (amount >= 1000000) {
    return (amount / 1000000).toFixed(1) + 'M'
  } else if (amount >= 1000) {
    return (amount / 1000).toFixed(1) + 'K'
  }
  return amount.toFixed(0)
}

const formatTimeAgo = (timestamp: string): string => {
  const now = new Date()
  const date = new Date(timestamp)
  const diffMs = now.getTime() - date.getTime()
  const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
  
  if (diffHours < 1) return 'Just now'
  if (diffHours === 1) return '1 hour ago'
  if (diffHours < 24) return `${diffHours} hours ago`
  
  const diffDays = Math.floor(diffHours / 24)
  if (diffDays === 1) return '1 day ago'
  return `${diffDays} days ago`
}

const refreshActivities = async () => {
  isRefreshingActivities.value = true
  
  try {
    // Simulate refresh delay
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // In real app, this would fetch new data from the API
    console.log('Activities refreshed')
  } catch (error) {
    console.error('Failed to refresh activities:', error)
  } finally {
    isRefreshingActivities.value = false
  }
}

const navigateToStock = () => {
  router.push('/stock')
}

// Initialize store data on mount
onMounted(() => {
  stockStore.loadMockData()
})
</script>

<style scoped>
.stat-card {
  @apply bg-white dark:bg-gray-900 p-6 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm;
}

.stat-label {
  @apply text-sm font-medium text-gray-600 dark:text-gray-400;
}

.stat-value {
  @apply text-2xl font-bold text-gray-900 dark:text-white;
}

.stat-change-positive {
  @apply text-sm text-green-600 dark:text-green-400 flex items-center;
}

.stat-change-negative {
  @apply text-sm text-red-600 dark:text-red-400 flex items-center;
}

.card {
  @apply bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-700 rounded-lg shadow-sm;
}

.card-header {
  @apply px-6 py-4 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between;
}

.card-body {
  @apply px-6 py-4;
}

.btn-primary {
  @apply inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500;
}

.btn-outline {
  @apply inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}
</style>
