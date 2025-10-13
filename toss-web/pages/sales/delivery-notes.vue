<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">Delivery Notes</h1>
          <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Track order fulfillment and deliveries for Thabo's Spaza Shop</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button @click="showNewDeliveryModal = true" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <PlusIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            New Delivery
          </button>
          <button @click="exportDeliveries('csv')" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-slate-600 hover:bg-slate-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <ArrowDownTrayIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Export
          </button>
        </div>
      </div>

      <!-- Delivery Stats -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Total Deliveries</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ totalDeliveries }}</p>
              <p class="text-xs sm:text-sm text-blue-600">{{ todayDeliveries }} today</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <TruckIcon class="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">In Transit</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ inTransit }}</p>
              <p class="text-xs sm:text-sm text-yellow-600">{{ pendingPickup }} pending</p>
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
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ delivered }}</p>
              <p class="text-xs sm:text-sm text-green-600">{{ deliveryRate }}% on time</p>
            </div>
            <div class="p-2 sm:p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <CheckCircleIcon class="w-4 h-4 sm:w-6 sm:h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Avg Delivery Time</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ avgDeliveryTime }}h</p>
              <p class="text-xs sm:text-sm text-purple-600">Last 30 days</p>
            </div>
            <div class="p-2 sm:p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <MapPinIcon class="w-4 h-4 sm:w-6 sm:h-6 text-purple-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
        <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
          <div class="flex-1">
            <input v-model="searchQuery" type="text" placeholder="Search deliveries..." 
                   class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
          </div>
          <div class="flex gap-2 sm:gap-3">
            <select v-model="statusFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Status</option>
              <option value="draft">Draft</option>
              <option value="ready">Ready for Pickup</option>
              <option value="in-transit">In Transit</option>
              <option value="delivered">Delivered</option>
              <option value="cancelled">Cancelled</option>
            </select>
            <select v-model="dateFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Time</option>
              <option value="today">Today</option>
              <option value="week">This Week</option>
              <option value="month">This Month</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Deliveries List -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Recent Deliveries</h3>
        </div>
        <div class="p-4 sm:p-6">
          <div class="space-y-3 sm:space-y-4">
            <div v-for="delivery in filteredDeliveries" :key="delivery.id" 
                 class="flex items-center justify-between p-4 rounded-lg border border-slate-100 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
              <div class="flex items-center space-x-3 flex-1 min-w-0">
                <div class="w-10 h-10 rounded-full flex items-center justify-center" :class="getStatusColor(delivery.status)">
                  <TruckIcon class="w-5 h-5 text-white" />
                </div>
                <div class="flex-1 min-w-0">
                  <div class="flex items-center gap-2">
                    <p class="text-sm font-medium text-slate-900 dark:text-white truncate">{{ delivery.deliveryNumber }}</p>
                    <span class="inline-flex px-2 py-1 text-xs rounded-full" :class="getStatusBadge(delivery.status)">
                      {{ delivery.status }}
                    </span>
                  </div>
                  <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">{{ delivery.customer }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-500">{{ delivery.address }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-sm font-semibold text-slate-900 dark:text-white">{{ delivery.items }} items</p>
                <p class="text-xs text-slate-500 dark:text-slate-500">{{ formatDate(delivery.deliveryDate) }}</p>
                <div class="flex gap-1 mt-1">
                  <button @click="viewDelivery(delivery)" class="p-1 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900 rounded">
                    <EyeIcon class="w-4 h-4" />
                  </button>
                  <button @click="printDeliveryNote(delivery)" class="p-1 text-purple-600 hover:bg-purple-50 dark:hover:bg-purple-900 rounded">
                    <PrinterIcon class="w-4 h-4" />
                  </button>
                  <button @click="trackDelivery(delivery)" class="p-1 text-green-600 hover:bg-green-50 dark:hover:bg-green-900 rounded">
                    <MapPinIcon class="w-4 h-4" />
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- New Delivery Modal -->
    <div v-if="showNewDeliveryModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Create Delivery Note</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createDelivery">
            <div class="space-y-4">
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Delivery Number</label>
                  <input v-model="newDelivery.deliveryNumber" type="text" required readonly
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-slate-50 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Delivery Date</label>
                  <input v-model="newDelivery.deliveryDate" type="date" required
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Customer Information</label>
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <input v-model="newDelivery.customerName" placeholder="Customer Name" required
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                  <input v-model="newDelivery.customerPhone" type="tel" placeholder="Phone Number"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Delivery Address</label>
                <textarea v-model="newDelivery.deliveryAddress" rows="2" placeholder="Enter delivery address..." required
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Items</label>
                <div class="space-y-2">
                  <div v-for="(item, index) in newDelivery.items" :key="index" class="flex gap-2 items-end">
                    <div class="flex-1">
                      <input v-model="item.name" placeholder="Item name" required
                             class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-20">
                      <input v-model.number="item.quantity" type="number" placeholder="Qty" min="1" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <button type="button" @click="removeDeliveryItem(index)" class="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded-lg">
                      <XMarkIcon class="w-4 h-4" />
                    </button>
                  </div>
                </div>
                <button type="button" @click="addDeliveryItem" class="mt-2 text-sm text-blue-600 hover:text-blue-700">
                  + Add Item
                </button>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Delivery Instructions</label>
                <textarea v-model="newDelivery.notes" rows="2" placeholder="Special delivery instructions..."
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>
            </div>

            <div class="flex justify-end space-x-3 mt-6">
              <button @click="showNewDeliveryModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                Create Delivery Note
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
import { 
  TruckIcon,
  PlusIcon,
  ArrowDownTrayIcon,
  ClockIcon,
  CheckCircleIcon,
  MapPinIcon,
  EyeIcon,
  PrinterIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Delivery Notes - TOSS ERP',
  meta: [
    { name: 'description', content: 'Track order fulfillment and deliveries' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// Reactive data
const showNewDeliveryModal = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')
const dateFilter = ref('')

// Delivery statistics
const totalDeliveries = ref(234)
const todayDeliveries = ref(12)
const inTransit = ref(18)
const pendingPickup = ref(5)
const delivered = ref(216)
const deliveryRate = ref(94)
const avgDeliveryTime = ref(4.5)

// Sample deliveries data
const deliveries = ref([
  {
    id: '1',
    deliveryNumber: 'DN-2025-001',
    customer: 'Nomsa Community Kitchen',
    customerPhone: '+27 82 456 7890',
    address: '123 Community Street, Soweto, 1818',
    items: 8,
    status: 'in-transit',
    deliveryDate: new Date(),
    notes: 'Deliver before 11 AM'
  },
  {
    id: '2',
    deliveryNumber: 'DN-2025-002',
    customer: 'Sipho Auto Repair',
    customerPhone: '+27 73 123 4567',
    address: '456 Garage Road, Alexandra, 2090',
    items: 5,
    status: 'delivered',
    deliveryDate: new Date(Date.now() - 2 * 60 * 60 * 1000),
    notes: 'Customer will collect'
  },
  {
    id: '3',
    deliveryNumber: 'DN-2025-003',
    customer: 'Lerato Hair Studio',
    customerPhone: '+27 84 789 0123',
    address: '789 Beauty Lane, Diepsloot, 2189',
    items: 3,
    status: 'ready',
    deliveryDate: new Date(),
    notes: 'Call on arrival'
  }
])

// Form data
const newDelivery = ref({
  deliveryNumber: generateDeliveryNumber(),
  customerName: '',
  customerPhone: '',
  deliveryAddress: '',
  deliveryDate: new Date().toISOString().split('T')[0],
  items: [
    { name: '', quantity: 1 }
  ],
  notes: ''
})

// Computed
const filteredDeliveries = computed(() => {
  let filtered = deliveries.value

  if (searchQuery.value) {
    filtered = filtered.filter(delivery => 
      delivery.customer.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      delivery.deliveryNumber.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  if (statusFilter.value) {
    filtered = filtered.filter(delivery => delivery.status === statusFilter.value)
  }

  return filtered
})

// Helper functions
function generateDeliveryNumber() {
  const date = new Date()
  const year = date.getFullYear()
  const nextNumber = (deliveries.value.length + 1).toString().padStart(3, '0')
  return `DN-${year}-${nextNumber}`
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
    draft: 'bg-slate-600',
    ready: 'bg-yellow-600',
    'in-transit': 'bg-blue-600',
    delivered: 'bg-green-600',
    cancelled: 'bg-red-600'
  }
  return colors[status as keyof typeof colors] || 'bg-slate-600'
}

const getStatusBadge = (status: string) => {
  const badges = {
    draft: 'bg-slate-100 text-slate-800 dark:bg-slate-900 dark:text-slate-200',
    ready: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'in-transit': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    delivered: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return badges[status as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

// Delivery form functions
const addDeliveryItem = () => {
  newDelivery.value.items.push({ name: '', quantity: 1 })
}

const removeDeliveryItem = (index: number) => {
  if (newDelivery.value.items.length > 1) {
    newDelivery.value.items.splice(index, 1)
  }
}

// Actions
const createDelivery = async () => {
  try {
    const delivery = {
      id: Date.now().toString(),
      deliveryNumber: newDelivery.value.deliveryNumber,
      customer: newDelivery.value.customerName,
      customerPhone: newDelivery.value.customerPhone,
      address: newDelivery.value.deliveryAddress,
      items: newDelivery.value.items.length,
      status: 'draft',
      deliveryDate: new Date(newDelivery.value.deliveryDate),
      notes: newDelivery.value.notes
    }

    deliveries.value.unshift(delivery)
    totalDeliveries.value += 1
    
    showNewDeliveryModal.value = false
    
    // Reset form
    newDelivery.value = {
      deliveryNumber: generateDeliveryNumber(),
      customerName: '',
      customerPhone: '',
      deliveryAddress: '',
      deliveryDate: new Date().toISOString().split('T')[0],
      items: [{ name: '', quantity: 1 }],
      notes: ''
    }
    
    alert('Delivery note created successfully!')
  } catch (error) {
    console.error('Error creating delivery:', error)
    alert('Failed to create delivery note. Please try again.')
  }
}

const viewDelivery = (delivery: any) => {
  alert(`Viewing delivery ${delivery.deliveryNumber} for ${delivery.customer}`)
}

const printDeliveryNote = (delivery: any) => {
  alert(`Printing delivery note ${delivery.deliveryNumber}`)
}

const trackDelivery = (delivery: any) => {
  alert(`Tracking delivery ${delivery.deliveryNumber}`)
}

const exportDeliveries = (format: string) => {
  alert(`Exporting deliveries as ${format.toUpperCase()}...`)
}
</script>


