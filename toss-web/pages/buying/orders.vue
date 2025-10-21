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
              to="/buying/create-order"
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
      
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Orders</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.totalPOs }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl">
              <ShoppingCartIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Pending</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.pendingPOs }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-orange-500 to-red-600 rounded-xl">
              <ClockIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">In Transit</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.inTransitPOs }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-xl">
              <TruckIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Delivered</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.deliveredPOs }}</p>
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
              <p class="text-3xl font-bold text-slate-900 dark:text-white">R{{ stats.totalValue }}K</p>
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
              class="w-full pl-11 pr-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
            />
          </div>

          <!-- Status Filter -->
          <select
            v-model="statusFilter"
            class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
          >
              <option value="">All Status</option>
            <option value="pending">Pending</option>
            <option value="approved">Approved</option>
            <option value="in-transit">In Transit</option>
              <option value="delivered">Delivered</option>
              <option value="cancelled">Cancelled</option>
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

      <!-- Loading State -->
      <div v-if="loading" class="space-y-4">
        <div v-for="n in 3" :key="n" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 animate-pulse">
          <div class="flex items-center justify-between mb-4">
            <div class="flex-1">
              <div class="h-6 bg-slate-200 dark:bg-slate-700 rounded w-32 mb-2"></div>
              <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-48"></div>
            </div>
            <div class="h-8 w-24 bg-slate-200 dark:bg-slate-700 rounded-full"></div>
          </div>
        </div>
      </div>

      <!-- Orders List -->
      <div v-else-if="filteredOrders.length > 0" class="space-y-4">
        <div v-for="order in filteredOrders" :key="order.id" 
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden hover:shadow-xl transition-all duration-300"
        >
          <!-- Order Header -->
          <div class="bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600">
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-12 w-12">
                  <div class="h-12 w-12 rounded-xl bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center">
                    <span class="text-lg font-bold text-white">{{ order.supplier.charAt(0) }}</span>
                  </div>
                </div>
                <div>
                  <h3 class="text-lg font-bold text-slate-900 dark:text-white">{{ order.number }}</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400">{{ order.supplier }}</p>
                </div>
              </div>
              <div class="flex items-center space-x-3">
                <span 
                  class="px-3 py-1 rounded-full text-sm font-medium"
                  :class="getStatusClass(order.status)"
                >
                  {{ order.status }}
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
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">R{{ order.totalAmount.toLocaleString() }}</p>
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
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ formatDate(order.expectedDelivery) }}</p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Items</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ order.itemCount }} items</p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Payment Terms</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ order.paymentTerms }}</p>
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
                  v-if="order.status === 'pending'"
                  @click="approveOrder(order)" 
                  class="text-green-600 hover:text-green-800 dark:text-green-400 dark:hover:text-green-200 text-sm font-medium flex items-center"
                >
                  <CheckCircleIcon class="w-4 h-4 mr-1" />
                  Approve
                </button>
                <button 
                  @click="printOrder(order)" 
                  class="text-purple-600 hover:text-purple-800 dark:text-purple-400 dark:hover:text-purple-200 text-sm font-medium flex items-center"
                >
                  <PrinterIcon class="w-4 h-4 mr-1" />
                  Print
                </button>
                <button 
                  @click="trackOrder(order)" 
                  class="text-indigo-600 hover:text-indigo-800 dark:text-indigo-400 dark:hover:text-indigo-200 text-sm font-medium flex items-center"
                >
                  <TruckIcon class="w-4 h-4 mr-1" />
                  Track
                </button>
              </div>
              <button 
                v-if="order.status === 'pending' || order.status === 'approved'"
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
      <div v-else-if="!loading" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
          <div class="flex flex-col items-center justify-center">
            <div class="p-4 bg-gradient-to-br from-blue-100 to-purple-100 dark:from-blue-900/20 dark:to-purple-900/20 rounded-full mb-4">
              <ShoppingCartIcon class="w-12 h-12 text-blue-600 dark:text-blue-400" />
            </div>
            <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">No orders found</p>
            <p class="text-slate-600 dark:text-slate-400 mb-4">Start by creating your first purchase order!</p>
            <NuxtLink
              to="/buying/create-order"
              class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-xl hover:from-blue-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Create Order
            </NuxtLink>
          </div>
        </div>
    </div>

    <!-- Order Details Modal -->
    <OrderDetailsModal
      :show="showOrderDetails"
      :order="selectedOrder"
      @close="closeOrderDetails"
      @print="printOrder"
      @track="trackOrder"
    />

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from '~/composables/useToast'
import OrderDetailsModal from '~/components/buying/OrderDetailsModal.vue'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  ArrowDownTrayIcon,
  ShoppingCartIcon,
  ClockIcon,
  TruckIcon,
  CheckCircleIcon,
  CurrencyDollarIcon,
  EyeIcon,
  PrinterIcon,
  XMarkIcon,
  BoltIcon,
  UserGroupIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Buy Orders - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage and track Buy Orders in TOSS ERP' }
  ]
})

// Composables
const router = useRouter()

// State
const loading = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')
const supplierFilter = ref('')

// Stats
const stats = ref({
  totalPOs: 0,
  pendingPOs: 0,
  inTransitPOs: 0,
  deliveredPOs: 0,
  totalValue: 0
})

// Suppliers list
const suppliers = ref([
  'ABC Suppliers',
  'XYZ Wholesalers',
  'Quality Foods Ltd',
  'Tech Solutions Inc',
  'Multiple Suppliers'
])

// Mock orders data
const orders = ref([
  {
    id: 1,
    number: 'PO-2025-001',
    supplier: 'ABC Suppliers',
    status: 'in-transit',
    orderDate: new Date('2025-01-15'),
    expectedDelivery: new Date('2025-01-25'),
    totalAmount: 15500,
    itemCount: 12,
    paymentTerms: 'Net 30'
  },
  {
    id: 2,
    number: 'PO-2025-002',
    supplier: 'XYZ Wholesalers',
    status: 'pending',
    orderDate: new Date('2025-01-18'),
    expectedDelivery: new Date('2025-01-28'),
    totalAmount: 8200,
    itemCount: 6,
    paymentTerms: 'Net 15'
  },
  {
    id: 3,
    number: 'PO-2025-003',
    supplier: 'Quality Foods Ltd',
    status: 'delivered',
    orderDate: new Date('2025-01-10'),
    expectedDelivery: new Date('2025-01-20'),
    totalAmount: 23400,
    itemCount: 18,
    paymentTerms: 'Net 30'
  },
  {
    id: 4,
    number: 'PO-2025-004',
    supplier: 'Tech Solutions Inc',
    status: 'approved',
    orderDate: new Date('2025-01-20'),
    expectedDelivery: new Date('2025-02-01'),
    totalAmount: 45000,
    itemCount: 3,
    paymentTerms: 'Net 60'
  }
])

// Load orders from localStorage
  const loadOrders = () => {
    const savedOrders = localStorage.getItem('toss-orders')
    if (savedOrders) {
      const parsedOrders = JSON.parse(savedOrders)
      // Map the saved orders to match the expected format
      orders.value = parsedOrders.map((order: Record<string, any>) => ({
        id: order.id || order.orderNumber,
        number: order.orderNumber,
        supplier: order.supplier || 'Multiple Suppliers',
        status: order.status?.toLowerCase() || 'pending',
        orderDate: new Date(order.date),
        expectedDelivery: new Date(order.expectedDelivery || Date.now() + 2 * 24 * 60 * 60 * 1000),
        totalAmount: order.total,
        itemCount: order.items?.length || 0,
        paymentTerms: 'Net 30'
      }))

      // Update stats based on loaded orders
      updateStats()
    }
  }

  // Update stats based on orders
  const updateStats = () => {
    stats.value.totalPOs = orders.value.length
    stats.value.pendingPOs = orders.value.filter((o: Record<string, any>) => o.status === 'pending').length
    stats.value.inTransitPOs = orders.value.filter((o: Record<string, any>) => o.status === 'in-transit').length
    stats.value.deliveredPOs = orders.value.filter((o: Record<string, any>) => o.status === 'delivered').length
    // Calculate total value in thousands and round to 2 decimal places
    const totalInThousands = orders.value.reduce((sum: number, o: Record<string, any>) => sum + (o.totalAmount / 1000), 0)
    stats.value.totalValue = Math.round(totalInThousands * 100) / 100
  }

// Computed
const filteredOrders = computed(() => {
  return orders.value.filter((order: Record<string, any>) => {
    const matchesSearch = !searchQuery.value || 
      order.number.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      order.supplier.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !statusFilter.value || order.status === statusFilter.value
    const matchesSupplier = !supplierFilter.value || order.supplier === supplierFilter.value
    
    return matchesSearch && matchesStatus && matchesSupplier
  })
})

onMounted(() => {
  loadOrders()
})

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

const formatDate = (date: Date) => {
  return date.toLocaleDateString('en-US', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

// Modal state
const showOrderDetails = ref(false)
const selectedOrder = ref<Record<string, any> | null>(null)

const viewOrder = (order: Record<string, any>) => {
  console.log('View Order clicked:', order)
  selectedOrder.value = order
  showOrderDetails.value = true
  console.log('Modal should be visible:', showOrderDetails.value)
}

const closeOrderDetails = () => {
  showOrderDetails.value = false
  selectedOrder.value = null
}

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

const trackOrder = (order: any) => {
  // Navigate to track orders page with order number as query param
  router.push({
    path: '/buying/track-orders',
    query: { order: order.number }
  })
}

const approveOrder = (order: Record<string, any>) => {
  const toast = useToast()
  
  // Update order status
  const orderData = localStorage.getItem('toss-orders')
  if (orderData) {
    const allOrders = JSON.parse(orderData)
    const orderIndex = allOrders.findIndex((o: Record<string, any>) => o.id === order.id || o.orderNumber === order.number)
    
    if (orderIndex > -1) {
      allOrders[orderIndex].status = 'approved'
      localStorage.setItem('toss-orders', JSON.stringify(allOrders))
      
      // Reload orders
      loadOrders()
      
      toast.success(`Order ${order.number} approved successfully!`, '✅ Order Approved', 3000)
    }
  }
}

const cancelOrder = (order: Record<string, any>) => {
  const toast = useToast()
  
  if (confirm(`Are you sure you want to cancel order ${order.number}?`)) {
    const orderData = localStorage.getItem('toss-orders')
    if (orderData) {
      const allOrders = JSON.parse(orderData)
      const orderIndex = allOrders.findIndex((o: Record<string, any>) => o.id === order.id || o.orderNumber === order.number)
      
      if (orderIndex > -1) {
        allOrders[orderIndex].status = 'cancelled'
        localStorage.setItem('toss-orders', JSON.stringify(allOrders))
        
        // Reload orders
        loadOrders()
        
        toast.warning(`Order ${order.number} has been cancelled`, 'Order Cancelled', 3000)
      }
    }
  }
}

const exportOrders = () => {
  alert('Exporting orders to CSV/Excel')
}
</script>

