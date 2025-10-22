<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-green-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-green-600 to-blue-600 bg-clip-text text-transparent">
              Order Queue
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Manage and track pending orders
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total in Queue</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ queueOrders.length }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl">
              <QueueListIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Pending</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ pendingCount }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-xl">
              <ClockIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">In Progress</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ inProgressCount }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl">
              <SparklesIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Ready</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ readyCount }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <CheckCircleIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Queue List -->
      <div class="space-y-4">
        <div v-for="order in queueOrders" :key="order.id" 
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden hover:shadow-xl transition-all duration-300"
        >
          <div class="bg-gradient-to-r from-green-50 to-blue-50 dark:from-green-900/20 dark:to-blue-900/20 px-6 py-4">
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-12 w-12">
                  <div class="h-12 w-12 rounded-xl bg-gradient-to-br from-green-500 to-blue-600 flex items-center justify-center">
                    <span class="text-lg font-bold text-white">{{ order.customer.charAt(0) }}</span>
                  </div>
                </div>
                <div>
                  <h3 class="text-lg font-bold text-slate-900 dark:text-white">Order #{{ order.orderNumber }}</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400">{{ order.customer }}</p>
                </div>
              </div>
              <div class="flex items-center space-x-3">
                <span 
                  class="px-3 py-1 rounded-full text-sm font-medium"
                  :class="getStatusClass(order.status)"
                >
                  {{ getStatusLabel(order.status) }}
                </span>
                <div class="text-right">
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">R{{ order.total.toFixed(2) }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-400">{{ order.orderItems?.length || 0 }} items</p>
                </div>
              </div>
            </div>
          </div>

          <div class="px-6 py-4">
            <div class="flex items-center justify-between">
              <div class="text-sm text-slate-600 dark:text-slate-400">
                <span>Created: {{ formatTime(order.createdAt) }}</span>
              </div>
              <div class="flex space-x-2">
                <button 
                  v-if="order.status === 'pending'"
                  @click="updateStatus(order, 'in-progress')"
                  class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-semibold transition-colors"
                >
                  ⚙️ Start
                </button>
                <button 
                  v-if="order.status === 'in-progress'"
                  @click="updateStatus(order, 'ready')"
                  class="px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg text-sm font-semibold transition-colors"
                >
                  ✅ Mark Ready
                </button>
                <button 
                  v-if="order.status === 'ready'"
                  @click="completeOrder(order)"
                  class="px-4 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg text-sm font-semibold transition-colors"
                >
                  ✓ Complete
                </button>
                <button 
                  @click="cancelOrder(order)"
                  class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded-lg text-sm font-semibold transition-colors"
                >
                  ✕ Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="queueOrders.length === 0" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
        <div class="flex flex-col items-center justify-center">
          <div class="p-4 bg-gradient-to-br from-green-100 to-blue-100 dark:from-green-900/20 dark:to-blue-900/20 rounded-full mb-4">
            <QueueListIcon class="w-12 h-12 text-green-600 dark:text-green-400" />
          </div>
          <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">Queue is empty</p>
          <p class="text-slate-600 dark:text-slate-400 mb-4">All orders have been completed!</p>
          <NuxtLink
            to="/sales/orders/create-order"
            class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-xl hover:from-green-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold"
          >
            <PlusIcon class="w-5 h-5 mr-2" />
            Create Order
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  QueueListIcon,
  ClockIcon,
  SparklesIcon,
  CheckCircleIcon,
  PlusIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Order Queue - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage pending orders in queue' }
  ]
})

definePageMeta({
  layout: 'default'
})

// Sample queue data
const queueOrders = ref([
  {
    id: '1',
    orderNumber: '001',
    customer: 'John Doe',
    total: 250.50,
    status: 'pending',
    createdAt: new Date(Date.now() - 15 * 60 * 1000),
    orderItems: [
      { id: 1, name: 'Coca Cola 2L', quantity: 2 },
      { id: 2, name: 'White Bread 700g', quantity: 3 }
    ]
  },
  {
    id: '2',
    orderNumber: '002',
    customer: 'Sarah Smith',
    total: 180.00,
    status: 'in-progress',
    createdAt: new Date(Date.now() - 30 * 60 * 1000),
    orderItems: [
      { id: 3, name: 'Milk 1L', quantity: 1 }
    ]
  },
  {
    id: '3',
    orderNumber: '003',
    customer: 'Mike Johnson',
    total: 450.75,
    status: 'ready',
    createdAt: new Date(Date.now() - 45 * 60 * 1000),
    orderItems: [
      { id: 4, name: 'Simba Chips 125g', quantity: 5 },
      { id: 5, name: 'Castle Lager 440ml', quantity: 6 }
    ]
  }
])

// Computed
const pendingCount = computed(() => queueOrders.value.filter((o: any) => o.status === 'pending').length)
const inProgressCount = computed(() => queueOrders.value.filter((o: any) => o.status === 'in-progress').length)
const readyCount = computed(() => queueOrders.value.filter((o: any) => o.status === 'ready').length)

// Helper functions
const formatTime = (date: Date) => {
  return new Date(date).toLocaleTimeString('en-ZA', { 
    hour: '2-digit', 
    minute: '2-digit' 
  })
}

const getStatusLabel = (status: string) => {
  const labels: Record<string, string> = {
    'pending': '⏳ Pending',
    'in-progress': '⚙️ In Progress',
    'ready': '✅ Ready'
  }
  return labels[status] || status
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'pending': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    'in-progress': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'ready': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400'
  }
  return classes[status] || 'bg-slate-100 text-slate-800'
}

// Actions
const updateStatus = (order: any, newStatus: string) => {
  order.status = newStatus
  const statusText = getStatusLabel(newStatus)
  alert(`✓ Order #${order.orderNumber} marked as ${statusText}`)
}

const completeOrder = (order: any) => {
  const index = queueOrders.value.findIndex((o: any) => o.id === order.id)
  if (index > -1) {
    queueOrders.value.splice(index, 1)
    alert(`✓ Order #${order.orderNumber} completed and removed from queue`)
  }
}

const cancelOrder = (order: any) => {
  if (confirm(`Cancel order #${order.orderNumber}?`)) {
    const index = queueOrders.value.findIndex((o: any) => o.id === order.id)
    if (index > -1) {
      queueOrders.value.splice(index, 1)
      alert(`✗ Order #${order.orderNumber} cancelled`)
    }
  }
}
</script>

