<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">Sales Orders</h1>
          <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Track and manage customer orders for Thabo's Spaza Shop</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button @click="showNewOrderModal = true" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <PlusIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            New Order
          </button>
          <ExportButton
            :data="filteredOrders"
            filename="sales_orders"
            title="Sales Orders Report"
            data-type="sales"
            @export-start="() => {}"
            @export-complete="(format) => showNotification(`Orders exported as ${format.toUpperCase()}`, 'success')"
            @export-error="(error) => showNotification(error, 'error')"
          />
        </div>
      </div>

      <!-- Order Stats -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Total Orders</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ totalOrders }}</p>
              <p class="text-xs sm:text-sm text-blue-600">{{ newOrders }} this week</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <ShoppingBagIcon class="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Pending</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ pendingOrders }}</p>
              <p class="text-xs sm:text-sm text-yellow-600">{{ urgentOrders }} urgent</p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <ClockIcon class="w-4 h-4 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Delivered</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ deliveredOrders }}</p>
              <p class="text-xs sm:text-sm text-green-600">{{ deliveryRate }}% rate</p>
            </div>
            <div class="p-2 sm:p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <TruckIcon class="w-4 h-4 sm:w-6 sm:h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Order Value</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(totalOrderValue) }}</p>
              <p class="text-xs sm:text-sm text-purple-600">R {{ formatCurrency(avgOrderValue) }} avg</p>
            </div>
            <div class="p-2 sm:p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <CurrencyDollarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-purple-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
        <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
          <div class="flex-1">
            <input v-model="searchQuery" type="text" placeholder="Search orders..." 
                   class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
          </div>
          <div class="flex gap-2 sm:gap-3">
            <select v-model="statusFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Status</option>
              <option value="pending">Pending</option>
              <option value="confirmed">Confirmed</option>
              <option value="preparing">Preparing</option>
              <option value="ready">Ready</option>
              <option value="delivered">Delivered</option>
              <option value="cancelled">Cancelled</option>
            </select>
            <select v-model="priorityFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Priority</option>
              <option value="urgent">Urgent</option>
              <option value="high">High</option>
              <option value="normal">Normal</option>
              <option value="low">Low</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Orders List -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Recent Orders</h3>
        </div>
        <div class="p-4 sm:p-6">
          <div class="space-y-3 sm:space-y-4">
            <div v-for="order in filteredOrders" :key="order.id" 
                 class="flex items-center justify-between p-4 rounded-lg border border-slate-100 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
              <div class="flex items-center space-x-3 flex-1 min-w-0">
                <div class="w-10 h-10 rounded-full flex items-center justify-center" :class="getStatusColor(order.status)">
                  <ShoppingBagIcon class="w-5 h-5 text-white" />
                </div>
                <div class="flex-1 min-w-0">
                  <div class="flex items-center gap-2">
                    <p class="text-sm font-medium text-slate-900 dark:text-white truncate">{{ order.orderNumber }}</p>
                    <span class="inline-flex px-2 py-1 text-xs rounded-full" :class="getStatusBadge(order.status)">
                      {{ order.status }}
                    </span>
                    <span v-if="order.priority !== 'normal'" class="inline-flex px-2 py-1 text-xs rounded-full" :class="getPriorityBadge(order.priority)">
                      {{ order.priority }}
                    </span>
                  </div>
                  <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">{{ order.customer }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-500">{{ order.items }} items • {{ formatDate(order.orderDate) }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-sm font-semibold text-slate-900 dark:text-white">R {{ formatCurrency(order.total) }}</p>
                <div class="flex gap-1 mt-1">
                  <button @click="viewOrder(order)" class="p-1 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900 rounded">
                    <EyeIcon class="w-4 h-4" />
                  </button>
                  <button @click="updateOrderStatus(order)" class="p-1 text-green-600 hover:bg-green-50 dark:hover:bg-green-900 rounded">
                    <ArrowPathIcon class="w-4 h-4" />
                  </button>
                  <button @click="printOrder(order)" class="p-1 text-purple-600 hover:bg-purple-50 dark:hover:bg-purple-900 rounded">
                    <PrinterIcon class="w-4 h-4" />
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- New Order Modal -->
    <div v-if="showNewOrderModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Create New Order</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createOrder">
            <div class="space-y-4">
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Order Number</label>
                  <input v-model="newOrder.orderNumber" type="text" required readonly
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-slate-50 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Priority</label>
                  <select v-model="newOrder.priority" 
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                    <option value="normal">Normal</option>
                    <option value="high">High</option>
                    <option value="urgent">Urgent</option>
                  </select>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Customer Information</label>
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <input v-model="newOrder.customerName" placeholder="Customer Name" required
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                  <input v-model="newOrder.customerPhone" type="tel" placeholder="Phone Number"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Delivery Address</label>
                <textarea v-model="newOrder.deliveryAddress" rows="2" placeholder="Enter delivery address..."
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Order Items</label>
                <div class="space-y-2">
                  <div v-for="(item, index) in newOrder.items" :key="index" class="flex gap-2 items-end">
                    <div class="flex-1">
                      <input v-model="item.name" placeholder="Item name" required
                             class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-20">
                      <input v-model.number="item.quantity" type="number" placeholder="Qty" min="1" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-24">
                      <input v-model.number="item.price" type="number" step="0.01" placeholder="Price" min="0" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <button type="button" @click="removeOrderItem(index)" class="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded-lg">
                      <XMarkIcon class="w-4 h-4" />
                    </button>
                  </div>
                </div>
                <button type="button" @click="addOrderItem" class="mt-2 text-sm text-blue-600 hover:text-blue-700">
                  + Add Item
                </button>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Special Instructions</label>
                <textarea v-model="newOrder.notes" rows="2" placeholder="Any special instructions..."
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <div class="bg-blue-50 dark:bg-blue-900/20 p-4 rounded-lg">
                <div class="flex justify-between items-center">
                  <span class="text-lg font-semibold text-slate-900 dark:text-white">Order Total:</span>
                  <span class="text-xl font-bold text-blue-600 dark:text-blue-400">R {{ formatCurrency(calculateOrderTotal()) }}</span>
                </div>
              </div>
            </div>

            <div class="flex justify-end space-x-3 mt-6">
              <button @click="showNewOrderModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                Create Order
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import ExportButton from '~/components/common/ExportButton.vue'
import { 
  ShoppingBagIcon,
  PlusIcon,
  ArrowDownTrayIcon,
  ClockIcon,
  TruckIcon,
  CurrencyDollarIcon,
  EyeIcon,
  ArrowPathIcon,
  PrinterIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Sales Orders - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage customer orders for Thabo\'s Spaza Shop' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// Reactive data
const showNewOrderModal = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')
const priorityFilter = ref('')

// Order statistics
const totalOrders = ref(156)
const newOrders = ref(23)
const pendingOrders = ref(18)
const urgentOrders = ref(4)
const deliveredOrders = ref(138)
const deliveryRate = ref(89)
const totalOrderValue = ref(347850)
const avgOrderValue = ref(2230)

// Sample orders data for Thabo's Spaza Shop
const orders = ref([
  {
    id: '1',
    orderNumber: 'ORD-2025-001',
    customer: 'Nomsa Community Kitchen',
    customerPhone: '+27 82 456 7890',
    total: 4850,
    items: 8,
    status: 'preparing',
    priority: 'urgent',
    orderDate: new Date(),
    deliveryAddress: '123 Community Street, Soweto',
    notes: 'Needed for lunch service - please deliver before 11 AM'
  },
  {
    id: '2',
    orderNumber: 'ORD-2025-002', 
    customer: 'Sipho Auto Repair',
    customerPhone: '+27 73 123 4567',
    total: 1250,
    items: 5,
    status: 'ready',
    priority: 'normal',
    orderDate: new Date(Date.now() - 2 * 60 * 60 * 1000),
    deliveryAddress: '456 Garage Road, Alexandra',
    notes: 'Customer will collect in person'
  },
  {
    id: '3',
    orderNumber: 'ORD-2025-003',
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
    orderNumber: 'ORD-2025-004',
    customer: 'Mandla Construction',
    customerPhone: '+27 76 345 6789',
    total: 12500,
    items: 15,
    status: 'confirmed',
    priority: 'high',
    orderDate: new Date(Date.now() - 3 * 60 * 60 * 1000),
    deliveryAddress: 'Construction Site, 321 Building Ave, Orange Farm',
    notes: 'Large order for construction workers - arrange truck delivery'
  },
  {
    id: '5',
    orderNumber: 'ORD-2025-005',
    customer: 'Grace Catering Services',
    customerPhone: '+27 82 567 8901',
    total: 3200,
    items: 12,
    status: 'pending',
    priority: 'urgent',
    orderDate: new Date(Date.now() - 30 * 60 * 1000),
    deliveryAddress: '654 Event Hall, Tembisa',
    notes: 'Wedding catering supplies - time sensitive'
  }
])

// Form data
const newOrder = ref({
  orderNumber: generateOrderNumber(),
  customerName: '',
  customerPhone: '',
  deliveryAddress: '',
  priority: 'normal',
  items: [
    { name: '', quantity: 1, price: 0 }
  ],
  notes: ''
})

// Computed
const filteredOrders = computed(() => {
  let filtered = orders.value

  if (searchQuery.value) {
    filtered = filtered.filter(order => 
      order.customer.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      order.orderNumber.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  if (statusFilter.value) {
    filtered = filtered.filter(order => order.status === statusFilter.value)
  }

  if (priorityFilter.value) {
    filtered = filtered.filter(order => order.priority === priorityFilter.value)
  }

  return filtered
})

// Helper functions
function generateOrderNumber() {
  const date = new Date()
  const year = date.getFullYear()
  const nextNumber = (orders.value.length + 1).toString().padStart(3, '0')
  return `ORD-${year}-${nextNumber}`
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    hour: '2-digit',
    minute: '2-digit',
    day: 'numeric',
    month: 'short'
  }).format(date)
}

const getStatusColor = (status: string) => {
  const colors = {
    pending: 'bg-yellow-600',
    confirmed: 'bg-blue-600',
    preparing: 'bg-purple-600',
    ready: 'bg-green-600',
    delivered: 'bg-emerald-600',
    cancelled: 'bg-red-600'
  }
  return colors[status as keyof typeof colors] || 'bg-slate-600'
}

const getStatusBadge = (status: string) => {
  const badges = {
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    confirmed: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    preparing: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    ready: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    delivered: 'bg-emerald-100 text-emerald-800 dark:bg-emerald-900 dark:text-emerald-200',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return badges[status as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

const getPriorityBadge = (priority: string) => {
  const badges = {
    urgent: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    high: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    normal: 'bg-slate-100 text-slate-800 dark:bg-slate-900 dark:text-slate-200',
    low: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
  return badges[priority as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

// Order form functions
const addOrderItem = () => {
  newOrder.value.items.push({ name: '', quantity: 1, price: 0 })
}

const removeOrderItem = (index: number) => {
  if (newOrder.value.items.length > 1) {
    newOrder.value.items.splice(index, 1)
  }
}

const calculateOrderTotal = () => {
  return newOrder.value.items.reduce((total, item) => {
    return total + (item.quantity * item.price)
  }, 0)
}

// Actions
const createOrder = async () => {
  try {
    const order = {
      id: Date.now().toString(),
      orderNumber: newOrder.value.orderNumber,
      customer: newOrder.value.customerName,
      customerPhone: newOrder.value.customerPhone,
      total: calculateOrderTotal(),
      items: newOrder.value.items.length,
      status: 'pending',
      priority: newOrder.value.priority,
      orderDate: new Date(),
      deliveryAddress: newOrder.value.deliveryAddress,
      notes: newOrder.value.notes
    }

    orders.value.unshift(order)
    totalOrders.value += 1
    pendingOrders.value += 1
    
    showNewOrderModal.value = false
    
    // Reset form
    newOrder.value = {
      orderNumber: generateOrderNumber(),
      customerName: '',
      customerPhone: '',
      deliveryAddress: '',
      priority: 'normal',
      items: [{ name: '', quantity: 1, price: 0 }],
      notes: ''
    }
    
    alert('Order created successfully!')
  } catch (error) {
    console.error('Error creating order:', error)
    alert('Failed to create order. Please try again.')
  }
}

const viewOrder = (order: any) => {
  alert(`Viewing order ${order.orderNumber} for ${order.customer}`)
}

const updateOrderStatus = (order: any) => {
  const statuses = ['pending', 'confirmed', 'preparing', 'ready', 'delivered']
  const currentIndex = statuses.indexOf(order.status)
  if (currentIndex < statuses.length - 1) {
    order.status = statuses[currentIndex + 1]
    alert(`Order ${order.orderNumber} status updated to ${order.status}`)
  }
}

const printOrder = (order: any) => {
  alert(`Printing order ${order.orderNumber}`)
}

const showNotification = (message: string, type: 'success' | 'error') => {
  if (type === 'success') {
    alert(`✅ ${message}`)
  } else {
    alert(`❌ ${message}`)
  }
}
</script>
