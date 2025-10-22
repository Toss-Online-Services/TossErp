<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              Buy Orders
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Manage and track your Buy Orders
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <NuxtLink
              to="/buying/orders/create-order"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-xl hover:from-blue-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
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
      
      <!-- Stats Cards - Now Clickable for Filtering -->
      <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4 sm:gap-6 mb-8">
        <button 
          @click="filterByStatus('')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-blue-500 border-blue-500': statusFilter === '' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Orders</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ stats.totalPOs }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl">
              <ShoppingCartIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
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
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ stats.pendingPOs }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-xl">
              <ClockIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>

        <button
          @click="filterByStatus('approved')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-blue-500 border-blue-500': statusFilter === 'approved' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Approved</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ stats.approvedPOs }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl">
              <CheckCircleIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>

        <button
          @click="filterByStatus('in-transit')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-orange-500 border-orange-500': statusFilter === 'in-transit' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">In Transit</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ stats.inTransitPOs }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-orange-500 to-red-600 rounded-xl">
              <TruckIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>

        <button
          @click="filterByStatus('delivered')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-green-500 border-green-500': statusFilter === 'delivered' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Delivered</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ stats.deliveredPOs }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
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
              class="w-full pl-11 pr-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
            />
          </div>

          <!-- Status Filter -->
          <select
            v-model="statusFilter"
            class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
          >
            <option value="">All Status</option>
            <option value="pending">⏳ Pending</option>
            <option value="approved">✅ Approved</option>
            <option value="in-transit">🚚 In Transit</option>
            <option value="delivered">📦 Delivered</option>
            <option value="cancelled">❌ Cancelled</option>
          </select>

          <!-- Supplier Filter -->
          <select
            v-model="supplierFilter"
            class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
          >
            <option value="">All Suppliers</option>
            <option v-for="supplier in suppliers" :key="supplier" :value="supplier">
              {{ supplier }}
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
            class="bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600 cursor-pointer hover:from-blue-100 hover:to-purple-100 dark:hover:from-blue-900/30 dark:hover:to-purple-900/30 transition-colors"
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-12 w-12">
                  <div class="h-12 w-12 rounded-xl bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center">
                    <span class="text-lg font-bold text-white">{{ order.customer?.charAt(0) || 'S' }}</span>
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
                <!-- Purchase Type Badge -->
                <span 
                  v-if="order.purchaseType && order.purchaseType !== 'individual'"
                  class="px-3 py-1 rounded-full text-xs font-bold flex items-center gap-1"
                  :class="getPurchaseTypeClass(order.purchaseType)"
                >
                  <component :is="getPurchaseTypeIcon(order.purchaseType)" class="w-3 h-3" />
                  {{ getPurchaseTypeLabel(order.purchaseType) }}
                </span>
                <!-- Savings Badge -->
                <span 
                  v-if="order.savingsAmount && order.savingsAmount > 0"
                  class="px-2 py-1 bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400 rounded-full text-xs font-bold"
                >
                  💰 Saved R{{ order.savingsAmount }}
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
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ formatDate(order.expectedDelivery) }}</p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Items</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ order.orderItems?.length || 0 }} items</p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Payment Terms</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">Net 30</p>
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
                  <BuyingOrderTimeline
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
                v-if="order.status === 'pending' || order.status === 'approved'"
                @click.stop="cancelOrder(order)" 
                class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-200 text-sm font-medium flex items-center transition-colors"
              >
                <XMarkIcon class="w-4 h-4 mr-1" />
                Cancel
              </button>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="filteredOrders.length === 0" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
        <div class="flex flex-col items-center justify-center">
          <div class="p-4 bg-gradient-to-br from-blue-100 to-purple-100 dark:from-blue-900/20 dark:to-purple-900/20 rounded-full mb-4">
            <ShoppingCartIcon class="w-12 h-12 text-blue-600 dark:text-blue-400" />
          </div>
          <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">No orders found</p>
          <p class="text-slate-600 dark:text-slate-400 mb-4">Start by creating your first purchase order!</p>
          <NuxtLink
            to="/buying/orders/create-order"
            class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-xl hover:from-blue-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold"
          >
            <PlusIcon class="w-5 h-5 mr-2" />
            Create Order
          </NuxtLink>
        </div>
      </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from '~/composables/useToast'
import BuyingOrderTimeline from '~/components/buying/OrderTimeline.vue'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  ArrowDownTrayIcon,
  ShoppingCartIcon,
  ClockIcon,
  TruckIcon,
  CheckCircleIcon,
  PrinterIcon,
  XMarkIcon,
  UserGroupIcon,
  PaperAirplaneIcon
} from '@heroicons/vue/24/outline'
import { useBuyingAPI } from '~/composables/useBuyingAPI'

// Page metadata
useHead({
  title: 'Buy Orders - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage and track Buy Orders in TOSS ERP' }
  ]
})

// Composables
const router = useRouter()
const buyingAPI = useBuyingAPI()

// State
const loading = ref(true)
const searchQuery = ref('')
const statusFilter = ref('')
const supplierFilter = ref('')
const orders = ref<any[]>([])
const expandedOrders = ref<string[]>([])

// Load orders on mount
onMounted(async () => {
  await loadOrders()
})

const loadOrders = async () => {
  loading.value = true
  try {
    orders.value = await buyingAPI.getOrders()
  } catch (error) {
    console.error('Failed to load orders:', error)
  } finally {
    loading.value = false
  }
}

// Stats - computed from actual orders
const stats = computed(() => ({
  totalPOs: orders.value.length,
  pendingPOs: orders.value.filter((o: any) => o.status === 'pending').length,
  approvedPOs: orders.value.filter((o: any) => o.status === 'approved').length,
  inTransitPOs: orders.value.filter((o: any) => o.status === 'in-transit').length,
  deliveredPOs: orders.value.filter((o: any) => o.status === 'delivered' || o.status === 'received').length
}))

// Suppliers - computed from orders
const suppliers = computed(() => {
  return [...new Set(orders.value.map((o: any) => o.customer))]
})

// Computed
const filteredOrders = computed(() => {
  return orders.value.filter((order: any) => {
    const matchesSearch = !searchQuery.value || 
      order.orderNumber?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      order.customer?.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !statusFilter.value || order.status.toLowerCase() === statusFilter.value.toLowerCase()
    const matchesSupplier = !supplierFilter.value || order.customer === supplierFilter.value
    
    return matchesSearch && matchesStatus && matchesSupplier
  })
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

// Methods
const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'pending': 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400',
    'approved': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'in-transit': 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
    'delivered': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    'cancelled': 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    'aggregated': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400'
  }
  return classes[status.toLowerCase()] || 'bg-slate-100 text-slate-800'
}

const getPurchaseTypeClass = (type: string) => {
  const classes: Record<string, string> = {
    'aggregated': 'bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-400',
    'group-buy': 'bg-purple-100 dark:bg-purple-900/30 text-purple-700 dark:text-purple-400'
  }
  return classes[type] || 'bg-slate-100 text-slate-800'
}

const getPurchaseTypeLabel = (type: string) => {
  const labels: Record<string, string> = {
    'aggregated': 'Order Placed',
    'group-buy': 'Group Buy'
  }
  return labels[type] || type
}

const getPurchaseTypeIcon = (type: string) => {
  const icons: Record<string, any> = {
    'aggregated': ShoppingCartIcon,
    'group-buy': UserGroupIcon
  }
  return icons[type] || ShoppingCartIcon
}

const getStatusLabel = (status: string) => {
  const labels: Record<string, string> = {
    'pending': '⏳ Pending',
    'approved': '✅ Approved',
    'in-transit': '🚚 In Transit',
    'delivered': '📦 Delivered',
    'cancelled': '❌ Cancelled',
    'aggregated': 'Order Placed'
  }
  return labels[status.toLowerCase()] || status
}

const formatDate = (date: Date) => {
  return date.toLocaleDateString('en-US', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

// Action handlers

const printOrder = (order: any) => {
  // Create a print-friendly version
  const printWindow = window.open('', '_blank')
  if (!printWindow) return

  const orderData = localStorage.getItem('toss-orders')
  const orders = orderData ? JSON.parse(orderData) : []
  const fullOrder = orders.find((o: any) => o.id === order.id || o.orderNumber === order.number)

  printWindow.document.write(`
    <!DOCTYPE html>
    <html>
    <head>
      <title>Purchase Order - ${order.number}</title>
      <style>
        body {
          font-family: Arial, sans-serif;
          max-width: 800px;
          margin: 40px auto;
          padding: 20px;
        }
        .header {
          text-align: center;
          border-bottom: 2px solid #333;
          padding-bottom: 20px;
          margin-bottom: 30px;
        }
        .header h1 {
          color: #2563eb;
          margin: 0;
        }
        .order-info {
          display: grid;
          grid-template-columns: 1fr 1fr;
          gap: 20px;
          margin-bottom: 30px;
        }
        .info-block {
          border: 1px solid #ddd;
          padding: 15px;
          border-radius: 8px;
        }
        .info-block label {
          font-weight: bold;
          color: #666;
          display: block;
          margin-bottom: 5px;
        }
        table {
          width: 100%;
          border-collapse: collapse;
          margin: 20px 0;
        }
        th, td {
          border: 1px solid #ddd;
          padding: 12px;
          text-align: left;
        }
        th {
          background-color: #f3f4f6;
          font-weight: bold;
        }
        .totals {
          margin-top: 20px;
          text-align: right;
        }
        .totals div {
          margin: 5px 0;
        }
        .total-amount {
          font-size: 24px;
          font-weight: bold;
          color: #2563eb;
          margin-top: 10px;
        }
        @media print {
          button { display: none; }
        }
      </style>
    </head>
    <body>
      <div class="header">
        <h1>TOSS ERP</h1>
        <h2>Purchase Order</h2>
        <p>${order.number}</p>
      </div>
      
      <div class="order-info">
        <div class="info-block">
          <label>Order Number:</label>
          <p>${order.number}</p>
          <label>Supplier:</label>
          <p>${order.supplier}</p>
          <label>Status:</label>
          <p>${order.status}</p>
        </div>
        <div class="info-block">
          <label>Order Date:</label>
          <p>${new Date(order.orderDate).toLocaleDateString()}</p>
          <label>Expected Delivery:</label>
          <p>${new Date(order.expectedDelivery).toLocaleDateString()}</p>
          <label>Payment Terms:</label>
          <p>${order.paymentTerms}</p>
        </div>
      </div>

      <h3>Order Items</h3>
      <table>
        <thead>
          <tr>
            <th>Item</th>
            <th>SKU</th>
            <th style="text-align: right;">Qty</th>
            <th style="text-align: right;">Unit Price</th>
            <th style="text-align: right;">Total</th>
          </tr>
        </thead>
        <tbody>
          ${fullOrder?.items?.map((item: any) => `
            <tr>
              <td>${item.name}</td>
              <td>${item.sku}</td>
              <td style="text-align: right;">${item.quantity}</td>
              <td style="text-align: right;">R${item.price.toFixed(2)}</td>
              <td style="text-align: right;">R${(item.price * item.quantity).toFixed(2)}</td>
            </tr>
          `).join('') || `
            <tr>
              <td colspan="5" style="text-align: center;">No items available</td>
            </tr>
          `}
        </tbody>
      </table>

      <div class="totals">
        <div><strong>Subtotal:</strong> R${(order.totalAmount - (order.totalAmount > 500 ? 0 : 50)).toFixed(2)}</div>
        <div><strong>Delivery Fee:</strong> R${(order.totalAmount > 500 ? 0 : 50).toFixed(2)}</div>
        <div class="total-amount"><strong>Total:</strong> R${order.totalAmount.toFixed(2)}</div>
      </div>

      <button onclick="window.print()" style="margin-top: 20px; padding: 10px 20px; background: #2563eb; color: white; border: none; border-radius: 5px; cursor: pointer; font-size: 16px;">
        Print Order
      </button>
    </body>
    </html>
  `)
  printWindow.document.close()
}

const sendOrder = (order: any) => {
  const toast = useToast()
  
  // In production, this would send via WhatsApp, Email, or SMS
  const message = `Purchase Order ${order.orderNumber} for ${order.customer}\nTotal: R${order.total.toFixed(2)}\nStatus: ${getStatusLabel(order.status)}`
  
  // Mock sending functionality
  toast.success(`📤 Order details sent to ${order.customer}`, '✓ Order Sent', 3000)
  
  // In real implementation, could open WhatsApp or email:
  // const whatsappUrl = `https://wa.me/?text=${encodeURIComponent(message)}`
  // window.open(whatsappUrl, '_blank')
}

const cancelOrder = async (order: any) => {
  const toast = useToast()
  
  if (confirm(`Are you sure you want to cancel order ${order.orderNumber}?`)) {
    try {
      await buyingAPI.cancelOrder(order.id)
      await loadOrders()
      toast.warning(`Order ${order.orderNumber} has been cancelled`, 'Order Cancelled', 3000)
    } catch (error) {
      console.error('Failed to cancel order:', error)
      toast.error('Failed to cancel order', 'Error')
    }
  }
}

const exportOrders = () => {
  alert('Exporting orders to CSV/Excel')
}
</script>

