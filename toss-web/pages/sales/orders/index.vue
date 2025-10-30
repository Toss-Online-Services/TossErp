<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-green-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-green-600 to-blue-600 bg-clip-text text-transparent">
              Sales Orders
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Manage and track your Sales Orders
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <NuxtLink
              to="/sales/orders/create-order"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-xl hover:from-green-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Create Order
            </NuxtLink>
        </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- Stats Cards - Now Clickable (Queue Status) -->
      <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4 sm:gap-6 mb-8">
        <button 
          @click="filterByStatus('')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-blue-500 border-blue-500': statusFilter === '' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Orders</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ totalOrders }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl">
              <ShoppingBagIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
        </div>
      </button>

        <button
          @click="filterByStatus('pending')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-yellow-500 border-yellow-500': statusFilter === 'pending' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Pending</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ pendingOrders }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-xl">
              <ClockIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>

        <button
          @click="filterByStatus('in-progress')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-blue-500 border-blue-500': statusFilter === 'in-progress' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">In Progress</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ inProgressOrders }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl">
              <SparklesIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>

        <button
          @click="filterByStatus('ready')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-green-500 border-green-500': statusFilter === 'ready' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Ready</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ readyOrders }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <CheckCircleIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>

        <button
          @click="filterByStatus('completed')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-emerald-500 border-emerald-500': statusFilter === 'completed' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Completed</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ completedOrders }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-emerald-500 to-teal-600 rounded-xl">
              <TruckIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <!-- Search -->
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
              <MagnifyingGlassIcon class="h-5 w-5 text-slate-400" />
            </div>
            <input
              type="text"
              v-model="searchQuery"
              placeholder="Search orders..."
              class="w-full pl-11 pr-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
            />
          </div>

          <!-- Status Filter -->
          <select
            v-model="statusFilter"
            class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
          >
              <option value="">All Status</option>
              <option value="pending">⏳ Pending</option>
              <option value="in-progress">⚙️ In Progress</option>
              <option value="ready">✅ Ready</option>
              <option value="completed">📦 Completed</option>
              <option value="cancelled">❌ Cancelled</option>
            </select>

          <!-- Customer Filter -->
          <select
            v-model="customerFilter"
            class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
          >
            <option value="">All Customers</option>
            <option v-for="customer in customers" :key="customer" :value="customer">
              {{ customer }}
            </option>
            </select>

          <!-- Actions -->
          <div class="flex space-x-2">
            <button
              @click="exportOrders"
              class="flex-1 inline-flex items-center justify-center px-4 py-2.5 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600 hover:border-slate-400 dark:hover:border-slate-500 transition-all duration-200"
            >
              <ArrowDownTrayIcon class="w-4 h-4 mr-2" />
              Export
            </button>
          </div>
        </div>
      </div>

      <!-- Orders List -->
      <div class="space-y-4">
            <div v-for="order in filteredOrders" :key="order.id" 
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden hover:shadow-xl transition-all duration-300"
        >
          <!-- Order Header - Clickable to expand/collapse -->
          <div 
            @click="toggleOrderExpansion(order.id)"
            class="bg-gradient-to-r from-green-50 to-blue-50 dark:from-green-900/20 dark:to-blue-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600 cursor-pointer hover:from-green-100 hover:to-blue-100 dark:hover:from-green-900/30 dark:hover:to-blue-900/30 transition-colors"
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-12 w-12">
                  <div class="h-12 w-12 rounded-xl bg-gradient-to-br from-green-500 to-blue-600 flex items-center justify-center">
                    <span class="text-lg font-bold text-white">{{ order.customer.charAt(0) }}</span>
                  </div>
                </div>
                <div>
                  <h3 class="text-lg font-bold text-slate-900 dark:text-white">{{ order.orderNumber }}</h3>
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
                <span 
                  v-if="order.priority !== 'normal'"
                  class="px-2 py-1 rounded-full text-xs font-bold"
                  :class="getPriorityClass(order.priority)"
                >
                  {{ order.priority.toUpperCase() }}
                </span>
              <div class="text-right">
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">R{{ order.total.toLocaleString() }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                    {{ expandedOrders.includes(order.id) ? '▲ Click to collapse' : '▼ Click to expand' }}
                  </p>
          </div>
        </div>
      </div>
    </div>

          <!-- Order Details - Always Visible -->
          <div class="px-6 py-4">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-4">
                <div>
                <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Order Date</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ formatDate(order.orderDate) }}</p>
                </div>
                <div>
                <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Expected Delivery</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">
                  {{ order.deliveryAddress ? formatDate(order.expectedDelivery || order.orderDate) : 'Walk-in' }}
                </p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Items</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ order.orderItems?.length || 0 }} items</p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Payment Terms</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">
                  {{ order.deliveryAddress ? 'COD' : 'Cash' }}
                </p>
              </div>
              </div>

            <!-- Order Items Grid - Visible when expanded -->
            <transition 
              enter-active-class="transition-all duration-300 ease-out"
              enter-from-class="max-h-0 opacity-0"
              enter-to-class="max-h-[1000px] opacity-100"
              leave-active-class="transition-all duration-200 ease-in"
              leave-from-class="max-h-[1000px] opacity-100"
              leave-to-class="max-h-0 opacity-0"
            >
              <div v-if="expandedOrders.includes(order.id)" class="overflow-hidden mb-4">
                <div class="border-t border-slate-200 dark:border-slate-700 pt-4">
                  <h4 class="text-sm font-bold text-slate-900 dark:text-white mb-3">📦 Order Items</h4>
                  <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
                    <div 
                      v-for="item in order.orderItems" 
                      :key="item.id"
                      class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-3 border border-slate-200 dark:border-slate-600"
                    >
                      <div class="flex items-start justify-between mb-2">
                    <div class="flex-1">
                          <h5 class="font-semibold text-slate-900 dark:text-white text-sm">{{ item.name }}</h5>
                          <p class="text-xs text-slate-500 dark:text-slate-400">SKU: {{ item.sku }}</p>
                        </div>
                      </div>
                      <div class="flex items-center justify-between mt-2">
                        <span class="text-sm font-bold text-blue-600 dark:text-blue-400">
                          {{ item.quantity }}x @ R{{ item.price.toFixed(2) }}
                        </span>
                        <span 
                          :class="[
                            'text-xs px-2 py-1 rounded-full font-medium',
                            item.stock > 10 
                              ? 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400' 
                              : item.stock > 0
                                ? 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400'
                                : 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400'
                          ]"
                        >
                          Stock: {{ item.stock }}
                        </span>
                    </div>
                      <div class="mt-2 pt-2 border-t border-slate-200 dark:border-slate-600">
                        <p class="text-sm font-bold text-slate-900 dark:text-white">
                          Total: R{{ (item.quantity * item.price).toFixed(2) }}
                        </p>
                    </div>
                    </div>
                  </div>
                </div>
              </div>
            </transition>

            <!-- Expandable Timeline Section -->
            <transition 
              enter-active-class="transition-all duration-300 ease-out"
              enter-from-class="max-h-0 opacity-0"
              enter-to-class="max-h-[1000px] opacity-100"
              leave-active-class="transition-all duration-200 ease-in"
              leave-from-class="max-h-[1000px] opacity-100"
              leave-to-class="max-h-0 opacity-0"
            >
              <div v-if="expandedOrders.includes(order.id)" class="overflow-hidden">
                <div class="border-t border-slate-200 dark:border-slate-700 pt-6 mb-4">
                  <SalesOrderTimeline
                    :order-number="order.orderNumber"
                    :status="order.status"
                    :order-date="order.orderDate"
                    :expected-delivery="order.expectedDelivery"
                  />
                </div>
              </div>
            </transition>

            <!-- Actions -->
            <div class="flex items-center justify-between pt-4 border-t border-slate-200 dark:border-slate-700">
              <div class="flex space-x-3">
                <button 
                  @click.stop="printOrder(order)" 
                  class="text-purple-600 hover:text-purple-800 dark:text-purple-400 dark:hover:text-purple-200 text-sm font-medium flex items-center transition-colors"
                >
                  <PrinterIcon class="w-4 h-4 mr-1" />
                  Print
                </button>
                <button 
                  @click.stop="sendOrder(order)" 
                  class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-200 text-sm font-medium flex items-center transition-colors"
                >
                  <PaperAirplaneIcon class="w-4 h-4 mr-1" />
                  Send
                </button>
              </div>
              <button 
                v-if="order.status === 'pending' || order.status === 'in-progress'"
                @click.stop="cancelOrder(order)" 
                class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-200 text-sm font-medium flex items-center transition-colors"
              >
                <XMarkIcon class="w-4 h-4 mr-1" />
                Cancel
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="filteredOrders.length === 0" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
        <div class="flex flex-col items-center justify-center">
          <div class="p-4 bg-gradient-to-br from-green-100 to-blue-100 dark:from-green-900/20 dark:to-blue-900/20 rounded-full mb-4">
            <ShoppingBagIcon class="w-12 h-12 text-green-600 dark:text-green-400" />
          </div>
          <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">No orders found</p>
          <p class="text-slate-600 dark:text-slate-400 mb-4">Start by creating your first sales order!</p>
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
import { ref, computed, onMounted } from 'vue'
import { 
  ShoppingBagIcon,
  PlusIcon,
  ArrowDownTrayIcon,
  ClockIcon,
  TruckIcon,
  CurrencyDollarIcon,
  PrinterIcon,
  XMarkIcon,
  CheckCircleIcon,
  SparklesIcon,
  MagnifyingGlassIcon,
  PaperAirplaneIcon
} from '@heroicons/vue/24/outline'
import SalesOrderTimeline from '~/components/sales/OrderTimeline.vue'
import { useSalesAPI } from '~/composables/useSalesAPI'
import { getErrorNotification, logError } from '~/utils/errorHandler'

// Page metadata
useHead({
  title: 'Sales Orders - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage customer orders' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// API
const salesAPI = useSalesAPI()

// Reactive data
const searchQuery = ref('')
const statusFilter = ref('')
const customerFilter = ref('')
const expandedOrders = ref<string[]>([])
const orders = ref<any[]>([])
const loading = ref(false)

// Load orders on mount
onMounted(async () => {
  await loadOrders()
})

const loadOrders = async () => {
  loading.value = true
  try {
    const shopId = 1 // TODO: Get from session/auth
    const backendOrders = await salesAPI.getOrders({ shopId })
    
    // Transform backend data to match frontend expectations
    orders.value = backendOrders.map((order: any) => {
      // Map backend OrderStatus enum to frontend status strings
      let status = 'pending'
      if (order.orderStatus === 'Processing' || order.orderStatus === 'Processing') {
        status = 'in-progress'
      } else if (order.orderStatus === 'Complete' || order.orderStatus === 'Completed') {
        status = 'completed'
      } else if (order.orderStatus === 'Cancelled') {
        status = 'cancelled'
      } else {
        status = order.orderStatus?.toLowerCase() || 'pending'
      }
      
      return {
        id: order.id,
        orderNumber: order.orderNumber,
        customer: order.customerName || 'Unknown',
        customerId: order.customerId,
        status: status,
        orderStatus: order.orderStatus,
        shippingStatus: order.shippingStatus,
        paymentStatus: order.paymentStatus,
        total: order.orderTotal || 0,
        orderTotal: order.orderTotal,
        orderDate: new Date(order.orderDate),
        orderItems: [], // TODO: Fetch order items separately if needed
        itemCount: order.itemCount || 0,
        priority: 'normal', // Default priority
        deliveryAddress: null, // TODO: Fetch delivery address if needed
        expectedDelivery: null, // TODO: Calculate expected delivery
        customerPhone: null // TODO: Fetch customer phone if needed
      }
    })
  } catch (error) {
    logError(error, 'load_data', 'Failed to load orders')
    // Show user-friendly notification for critical data loading failure
    const notification = document.createElement('div')
    notification.textContent = '⚠️ Unable to load orders. Please refresh the page.'
    notification.className = 'fixed top-20 right-4 bg-red-600 text-white px-4 py-2 rounded-lg shadow-lg z-50'
    document.body.appendChild(notification)
    setTimeout(() => notification.remove(), 5000)
  } finally {
    loading.value = false
  }
}

// Order statistics (computed from actual orders)
const totalOrders = computed(() => orders.value.length)
const pendingOrders = computed(() => orders.value.filter((o: any) => o.status === 'pending').length)
const inProgressOrders = computed(() => orders.value.filter((o: any) => o.status === 'in-progress' || o.status === 'processing').length)
const readyOrders = computed(() => orders.value.filter((o: any) => o.status === 'ready' || o.status === 'readytoship').length)
const completedOrders = computed(() => orders.value.filter((o: any) => o.status === 'completed' || o.status === 'complete').length)
const totalOrderValue = computed(() => orders.value.reduce((sum: number, o: any) => sum + o.total, 0))

// Computed
const customers = computed(() => {
  return [...new Set(orders.value.map((o: any) => o.customer))]
})

const filteredOrders = computed(() => {
  let filtered = orders.value

  if (searchQuery.value) {
    filtered = filtered.filter((order: any) => 
      order.customer.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      order.orderNumber.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  if (statusFilter.value) {
    filtered = filtered.filter((order: any) => order.status === statusFilter.value)
  }

  if (customerFilter.value) {
    filtered = filtered.filter((order: any) => order.customer === customerFilter.value)
  }

  return filtered
})

// Filter functions
const filterByStatus = (status: string) => {
  statusFilter.value = status
}

const toggleOrderExpansion = (orderId: string) => {
  const index = expandedOrders.value.indexOf(orderId)
  if (index > -1) {
    expandedOrders.value.splice(index, 1)
  } else {
    expandedOrders.value.push(orderId)
  }
}

// Helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  }).format(date)
}

const getStatusLabel = (status: string) => {
  const labels: Record<string, string> = {
    'pending': '⏳ Pending',
    'in-progress': '⚙️ In Progress',
    'ready': '✅ Ready',
    'completed': '📦 Completed',
    'cancelled': '❌ Cancelled'
  }
  return labels[status] || status
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'pending': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    'in-progress': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'ready': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    'completed': 'bg-emerald-100 text-emerald-800 dark:bg-emerald-900/30 dark:text-emerald-400',
    'cancelled': 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
  return classes[status] || 'bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400'
}

const getPriorityClass = (priority: string) => {
  const classes = {
    urgent: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    high: 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400',
    normal: 'bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400',
  }
  return classes[priority as keyof typeof classes] || 'bg-slate-100 text-slate-800'
}

// Actions
const printOrder = (order: any) => {
  const printWindow = window.open('', '_blank')
  if (printWindow) {
    printWindow.document.write(`
      <html>
        <head>
          <title>Order ${order.orderNumber}</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 20px; }
            h1 { color: #2563eb; }
            table { width: 100%; border-collapse: collapse; margin-top: 20px; }
            th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
            th { background-color: #f3f4f6; }
          </style>
        </head>
        <body>
          <h1>Order ${order.orderNumber}</h1>
          <p><strong>Customer:</strong> ${order.customer}</p>
          <p><strong>Date:</strong> ${formatDate(order.orderDate)}</p>
          <p><strong>Status:</strong> ${getStatusLabel(order.status)}</p>
          <h2>Items:</h2>
          <table>
            <tr><th>Product</th><th>SKU</th><th>Qty</th><th>Price</th><th>Total</th></tr>
            ${order.orderItems.map((item: any) => `
              <tr>
                <td>${item.name}</td>
                <td>${item.sku}</td>
                <td>${item.quantity}</td>
                <td>R${item.price.toFixed(2)}</td>
                <td>R${(item.quantity * item.price).toFixed(2)}</td>
              </tr>
            `).join('')}
          </table>
          <p style="margin-top: 20px;"><strong>Total: R${order.total.toFixed(2)}</strong></p>
        </body>
      </html>
    `)
    printWindow.document.close()
    printWindow.print()
  }
}

const sendOrder = (order: any) => {
  // In production, this would send via WhatsApp, Email, or SMS
  const message = `Order ${order.orderNumber} for ${order.customer}\nTotal: R${order.total.toFixed(2)}\nStatus: ${getStatusLabel(order.status)}`
  
  if (order.customerPhone) {
    // WhatsApp share
    const whatsappUrl = `https://wa.me/${order.customerPhone.replace(/\D/g, '')}?text=${encodeURIComponent(message)}`
    window.open(whatsappUrl, '_blank')
  } else {
    alert(`Order details:\n\n${message}\n\nNote: Add customer phone number to send via WhatsApp`)
  }
}

const cancelOrder = async (order: any) => {
  if (confirm(`Are you sure you want to cancel order ${order.orderNumber}?`)) {
    try {
      await salesAPI.cancelOrder(order.id)
      await loadOrders() // Reload to reflect changes
      alert(`✓ Order ${order.orderNumber} has been cancelled`)
    } catch (error) {
      logError(error, 'save_data', 'Failed to cancel order')
      alert(getErrorNotification(error, 'save_data').replace('⚠️ ', ''))
    }
  }
}

const exportOrders = () => {
  alert('📥 Exporting orders to CSV...')
}
</script>
