<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800 p-6">
    <!-- Page Header -->
    <div class="max-w-7xl mx-auto">
      <div class="flex items-center justify-between mb-6">
        <div>
          <h1 class="text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
             Order Queue
          </h1>
          <p class="text-slate-600 dark:text-slate-400 mt-1">
            Manage preparation and serving of customer orders
          </p>
        </div>
        <div class="flex gap-3">
          <button 
            @click="refreshOrders"
            class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg shadow-lg transition-colors"
          >
            <ArrowPathIcon class="w-5 h-5 inline mr-2" />
            Refresh
          </button>
          <NuxtLink 
            to="/sales/pos" 
            class="px-4 py-2 bg-gray-600 hover:bg-gray-700 text-white rounded-lg shadow-lg transition-colors"
          >
            <ArrowLeftIcon class="w-5 h-5 inline mr-2" />
            Back to POS
          </NuxtLink>
        </div>
      </div>

      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-lg p-6 border-l-4 border-yellow-500">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-gray-600 text-sm font-medium">Pending Orders</p>
              <p class="text-3xl font-bold text-yellow-600">{{ pendingOrders.length }}</p>
            </div>
            <div class="bg-yellow-100 p-3 rounded-full">
              <ClockIcon class="w-8 h-8 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-lg p-6 border-l-4 border-blue-500">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-gray-600 text-sm font-medium">In Progress</p>
              <p class="text-3xl font-bold text-blue-600">{{ inProgressOrders.length }}</p>
            </div>
            <div class="bg-blue-100 p-3 rounded-full">
              <FireIcon class="w-8 h-8 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-lg p-6 border-l-4 border-green-500">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-gray-600 text-sm font-medium">Ready for Pickup</p>
              <p class="text-3xl font-bold text-green-600">{{ readyOrders.length }}</p>
            </div>
            <div class="bg-green-100 p-3 rounded-full">
              <CheckCircleIcon class="w-8 h-8 text-green-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="flex justify-center items-center py-20">
        <div class="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-600"></div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-xl p-6 text-center">
        <p class="text-red-600 font-medium">{{ error }}</p>
        <button @click="refreshOrders" class="mt-4 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700">
          Try Again
        </button>
      </div>

      <!-- Orders List -->
      <div v-else class="space-y-6">
        <!-- Pending Orders -->
        <div v-if="pendingOrders.length > 0">
          <h2 class="text-xl font-bold text-gray-900 mb-4 flex items-center">
            <ClockIcon class="w-6 h-6 text-yellow-600 mr-2" />
            Pending Orders ({{ pendingOrders.length }})
          </h2>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <OrderCard 
              v-for="order in pendingOrders" 
              :key="order.id" 
              :order="order"
              status-color="yellow"
              @start-preparation="startPreparation"
            />
          </div>
        </div>

        <!-- In Progress Orders -->
        <div v-if="inProgressOrders.length > 0">
          <h2 class="text-xl font-bold text-gray-900 mb-4 flex items-center">
            <FireIcon class="w-6 h-6 text-blue-600 mr-2" />
            In Progress ({{ inProgressOrders.length }})
          </h2>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <OrderCard 
              v-for="order in inProgressOrders" 
              :key="order.id" 
              :order="order"
              status-color="blue"
              @mark-ready="markReady"
            />
          </div>
        </div>

        <!-- Ready Orders -->
        <div v-if="readyOrders.length > 0">
          <h2 class="text-xl font-bold text-gray-900 mb-4 flex items-center">
            <CheckCircleIcon class="w-6 h-6 text-green-600 mr-2" />
            Ready for Pickup ({{ readyOrders.length }})
          </h2>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <OrderCard 
              v-for="order in readyOrders" 
              :key="order.id" 
              :order="order"
              status-color="green"
              @complete-order="completeOrder"
            />
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="allOrders.length === 0" class="bg-white rounded-xl shadow-lg p-12 text-center">
          <div class="text-gray-400 mb-4">
            <InboxIcon class="w-16 h-16 mx-auto" />
          </div>
          <h3 class="text-xl font-semibold text-gray-700 mb-2">No Active Orders</h3>
          <p class="text-gray-500">Queue is empty. New orders will appear here.</p>
          <NuxtLink to="/sales/pos" class="mt-4 inline-block px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
            Go to POS
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { 
  ArrowPathIcon, 
  ArrowLeftIcon,
  ClockIcon,
  FireIcon,
  CheckCircleIcon,
  InboxIcon
} from '@heroicons/vue/24/outline'
import { useSalesAPI } from '~/composables/useSalesAPI'
import OrderCard from '~/components/queue/OrderCard.vue'

const salesAPI = useSalesAPI()

// State
const allOrders = ref<any[]>([])
const isLoading = ref(true)
const error = ref('')
const shopId = ref(1) // TODO: Get from session

// Auto-refresh interval (10 seconds)
let refreshInterval: ReturnType<typeof setInterval> | null = null

// Computed filtered orders by status
const pendingOrders = computed(() => 
  allOrders.value.filter(order => order.status === 'Pending')
)

const inProgressOrders = computed(() => 
  allOrders.value.filter(order => order.status === 'InProgress')
)

const readyOrders = computed(() => 
  allOrders.value.filter(order => order.status === 'Ready')
)

// Fetch orders from API
const refreshOrders = async () => {
  try {
    isLoading.value = true
    error.value = ''
    const orders = await salesAPI.getQueueOrders(shopId.value)
    allOrders.value = orders || []
  } catch (err: any) {
    console.error('Failed to load queue orders:', err)
    error.value = err.message || 'Failed to load orders'
  } finally {
    isLoading.value = false
  }
}

// Start preparation (Pending  InProgress)
const startPreparation = async (orderId: number) => {
  try {
    await salesAPI.updateQueueOrderStatus(orderId, 'InProgress')
    await refreshOrders()
  } catch (err: any) {
    console.error('Failed to start preparation:', err)
    error.value = 'Failed to update order status'
  }
}

// Mark as ready (InProgress  Ready)
const markReady = async (orderId: number) => {
  try {
    await salesAPI.updateQueueOrderStatus(orderId, 'Ready')
    await refreshOrders()
  } catch (err: any) {
    console.error('Failed to mark as ready:', err)
    error.value = 'Failed to update order status'
  }
}

// Complete order (Ready  Completed)
const completeOrder = async (orderId: number) => {
  try {
    await salesAPI.completeQueueOrder(orderId)
    await refreshOrders()
  } catch (err: any) {
    console.error('Failed to complete order:', err)
    error.value = 'Failed to complete order'
  }
}

// Lifecycle
onMounted(async () => {
  await refreshOrders()
  
  // Set up auto-refresh every 10 seconds
  refreshInterval = setInterval(refreshOrders, 10000)
})

onUnmounted(() => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
  }
})

// Page metadata
definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Order Queue - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage customer orders in preparation queue' }
  ]
})
</script>

<style scoped>
/* Add any custom styles here */
</style>
