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
              to="/sales/create-order"
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
      
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Orders</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ totalOrders }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-blue-600 rounded-xl">
              <ShoppingBagIcon class="w-8 h-8 text-white" />
            </div>
        </div>
      </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Pending</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ pendingOrders }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-orange-500 to-red-600 rounded-xl">
              <ClockIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Preparing</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ preparingOrders }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-xl">
              <SparklesIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Delivered</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ deliveredOrders }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <CheckCircleIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Value</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">R{{ (totalOrderValue / 1000).toFixed(1) }}K</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl">
              <CurrencyDollarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
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
              <option value="pending">Pending</option>
              <option value="confirmed">Confirmed</option>
              <option value="preparing">Preparing</option>
              <option value="ready">Ready</option>
              <option value="delivered">Delivered</option>
              <option value="cancelled">Cancelled</option>
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
          <!-- Order Header -->
          <div class="bg-gradient-to-r from-green-50 to-blue-50 dark:from-green-900/20 dark:to-blue-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600">
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
                  {{ order.status }}
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
          </div>
        </div>
      </div>
    </div>

          <!-- Order Details -->
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
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ order.items }} items</p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Payment Terms</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">
                  {{ order.deliveryAddress ? 'COD' : 'Cash' }}
                </p>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex items-center justify-between pt-4 border-t border-slate-200 dark:border-slate-700">
              <div class="flex space-x-3">
                <button 
                  @click="viewOrder(order)" 
                  class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-200 text-sm font-medium flex items-center"
                >
                  <EyeIcon class="w-4 h-4 mr-1" />
                  View
                </button>
                <button 
                  @click="printOrder(order)" 
                  class="text-purple-600 hover:text-purple-800 dark:text-purple-400 dark:hover:text-purple-200 text-sm font-medium flex items-center"
                >
                  <PrinterIcon class="w-4 h-4 mr-1" />
                  Print
                </button>
                <button 
                  v-if="order.deliveryAddress"
                  @click="trackOrder(order)" 
                  class="text-indigo-600 hover:text-indigo-800 dark:text-indigo-400 dark:hover:text-indigo-200 text-sm font-medium flex items-center"
                >
                  <TruckIcon class="w-4 h-4 mr-1" />
                  Track
                </button>
              </div>
              <button 
                v-if="order.status === 'pending' || order.status === 'confirmed'"
                @click="cancelOrder(order)" 
                class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-200 text-sm font-medium flex items-center"
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
            to="/sales/create-order"
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
  ShoppingBagIcon,
  PlusIcon,
  ArrowDownTrayIcon,
  ClockIcon,
  TruckIcon,
  CurrencyDollarIcon,
  EyeIcon,
  PrinterIcon,
  XMarkIcon,
  CheckCircleIcon,
  SparklesIcon,
  MagnifyingGlassIcon
} from '@heroicons/vue/24/outline'

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

// Reactive data
const searchQuery = ref('')
const statusFilter = ref('')
const customerFilter = ref('')

// Order statistics
const totalOrders = ref(156)
const pendingOrders = ref(18)
const preparingOrders = ref(12)
const deliveredOrders = ref(126)
const totalOrderValue = ref(347850)

// Sample orders data
const orders = ref([
  {
    id: '1',
    orderNumber: 'SO-2025-001',
    customer: 'Nomsa Community Kitchen',
    customerPhone: '+27 82 456 7890',
    total: 4850,
    items: 8,
    status: 'preparing',
    priority: 'urgent',
    orderDate: new Date(),
    expectedDelivery: new Date(Date.now() + 2 * 60 * 60 * 1000),
    deliveryAddress: '123 Community Street, Soweto',
    notes: 'Needed for lunch service - please deliver before 11 AM'
  },
  {
    id: '2',
    orderNumber: 'SO-2025-002', 
    customer: 'Sipho Auto Repair',
    customerPhone: '+27 73 123 4567',
    total: 1250,
    items: 5,
    status: 'ready',
    priority: 'normal',
    orderDate: new Date(Date.now() - 2 * 60 * 60 * 1000),
    expectedDelivery: new Date(Date.now() + 1 * 60 * 60 * 1000),
    deliveryAddress: '456 Garage Road, Alexandra',
    notes: 'Customer will collect in person'
  },
  {
    id: '3',
    orderNumber: 'SO-2025-003',
    customer: 'Lerato Hair Studio',
    customerPhone: '+27 84 789 0123', 
    total: 890,
    items: 3,
    status: 'delivered',
    priority: 'normal',
    orderDate: new Date(Date.now() - 24 * 60 * 60 * 1000),
    deliveryAddress: '789 Beauty Lane, Diepsloot',
    notes: 'Weekly hair product delivery'
  },
  {
    id: '4',
    orderNumber: 'SO-2025-004',
    customer: 'Mandla Construction',
    customerPhone: '+27 76 345 6789',
    total: 12500,
    items: 15,
    status: 'confirmed',
    priority: 'high',
    orderDate: new Date(Date.now() - 3 * 60 * 60 * 1000),
    expectedDelivery: new Date(Date.now() + 5 * 60 * 60 * 1000),
    deliveryAddress: 'Construction Site, 321 Building Ave, Orange Farm',
    notes: 'Large order for construction workers - arrange truck delivery'
  },
  {
    id: '5',
    orderNumber: 'SO-2025-005',
    customer: 'Grace Catering Services',
    customerPhone: '+27 82 567 8901',
    total: 3200,
    items: 12,
    status: 'pending',
    priority: 'urgent',
    orderDate: new Date(Date.now() - 30 * 60 * 1000),
    expectedDelivery: new Date(Date.now() + 4 * 60 * 60 * 1000),
    deliveryAddress: '654 Event Hall, Tembisa',
    notes: 'Wedding catering supplies - time sensitive'
  }
])

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

const getStatusClass = (status: string) => {
  const classes = {
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    confirmed: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    preparing: 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
    ready: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    delivered: 'bg-emerald-100 text-emerald-800 dark:bg-emerald-900/30 dark:text-emerald-400',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
  return classes[status as keyof typeof classes] || 'bg-slate-100 text-slate-800'
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
const viewOrder = (order: any) => {
  alert(`Viewing order ${order.orderNumber} for ${order.customer}`)
}

const printOrder = (order: any) => {
  alert(`Printing order ${order.orderNumber}`)
}

const trackOrder = (order: any) => {
  alert(`Tracking delivery for order ${order.orderNumber}`)
}

const cancelOrder = (order: any) => {
  if (confirm(`Are you sure you want to cancel order ${order.orderNumber}?`)) {
    order.status = 'cancelled'
    alert(`Order ${order.orderNumber} has been cancelled`)
  }
}

const exportOrders = () => {
  alert('Exporting orders...')
}
</script>
