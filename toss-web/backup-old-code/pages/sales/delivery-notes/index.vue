<template>
  <div class="container mx-auto px-4 py-8">
    <!-- Header -->
    <div class="mb-8">
      <h1 class="text-3xl font-bold mb-2">{{ t('sales.deliveryNotes.title') }}</h1>
      <p class="text-gray-600">{{ t('sales.deliveryNotes.subtitle') }}</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-600 text-sm">{{ t('sales.deliveryNotes.scheduled') }}</p>
            <p class="text-3xl font-bold text-blue-600">{{ stats.scheduled }}</p>
          </div>
          <Icon name="mdi:calendar-clock" size="40" class="text-blue-600" />
        </div>
      </div>

      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-600 text-sm">{{ t('sales.deliveryNotes.inTransit') }}</p>
            <p class="text-3xl font-bold text-yellow-600">{{ stats.inTransit }}</p>
          </div>
          <Icon name="mdi:truck-delivery" size="40" class="text-yellow-600" />
        </div>
      </div>

      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-600 text-sm">{{ t('sales.deliveryNotes.delivered') }}</p>
            <p class="text-3xl font-bold text-green-600">{{ stats.delivered }}</p>
          </div>
          <Icon name="mdi:check-circle" size="40" class="text-green-600" />
        </div>
      </div>

      <div class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-600 text-sm">{{ t('sales.deliveryNotes.failed') }}</p>
            <p class="text-3xl font-bold text-red-600">{{ stats.failed }}</p>
          </div>
          <Icon name="mdi:alert-circle" size="40" class="text-red-600" />
        </div>
      </div>
    </div>

    <!-- Filters and Actions -->
    <div class="bg-white rounded-lg shadow-sm p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4 items-end">
        <!-- Search -->
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700 mb-2">
            {{ t('common.search') }}
          </label>
          <div class="relative">
            <Icon name="mdi:magnify" class="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
            <input
              v-model="searchQuery"
              type="text"
              :placeholder="t('sales.deliveryNotes.searchPlaceholder')"
              class="w-full pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        </div>

        <!-- Status Filter -->
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700 mb-2">
            {{ t('common.status') }}
          </label>
          <select
            v-model="filterStatus"
            class="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
          >
            <option value="">{{ t('sales.deliveryNotes.allStatus') }}</option>
            <option value="scheduled">{{ t('sales.deliveryNotes.scheduled') }}</option>
            <option value="in-transit">{{ t('sales.deliveryNotes.inTransit') }}</option>
            <option value="delivered">{{ t('sales.deliveryNotes.delivered') }}</option>
            <option value="failed">{{ t('sales.deliveryNotes.failed') }}</option>
          </select>
        </div>

        <!-- Date Range -->
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700 mb-2">
            {{ t('sales.deliveryNotes.deliveryDate') }}
          </label>
          <input
            v-model="filterDate"
            type="date"
            class="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <!-- New Delivery Button -->
        <NuxtLink
          to="/sales/delivery-notes/create"
          class="btn btn-primary flex items-center gap-2 whitespace-nowrap"
        >
          <Icon name="mdi:plus" />
          {{ t('sales.deliveryNotes.newDelivery') }}
        </NuxtLink>
      </div>
    </div>

    <!-- Delivery Notes Table -->
    <div class="bg-white rounded-lg shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ t('sales.deliveryNotes.deliveryNo') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ t('sales.deliveryNotes.orderRef') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ t('sales.quotations.customer') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ t('sales.deliveryNotes.deliveryDate') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ t('sales.deliveryNotes.driver') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ t('sales.deliveryNotes.location') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ t('common.status') }}
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                {{ t('common.actions') }}
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="delivery in filteredDeliveries" :key="delivery.id" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="font-medium text-blue-600">{{ delivery.deliveryNo }}</span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <NuxtLink :to="`/sales/orders/${delivery.orderId}`" class="text-blue-600 hover:underline">
                  {{ delivery.orderRef }}
                </NuxtLink>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center">
                  <Icon name="mdi:account" class="mr-2 text-gray-400" />
                  {{ delivery.customerName }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm">
                  <div>{{ formatDate(delivery.deliveryDate) }}</div>
                  <div class="text-gray-500">{{ delivery.scheduledTime }}</div>
                </div>
              </td>
              <td class="px-6 py-4">
                <div v-if="delivery.driver" class="flex items-center">
                  <Icon name="mdi:account-circle" class="mr-2 text-gray-400" />
                  <span>{{ delivery.driver }}</span>
                </div>
                <span v-else class="text-gray-400">-</span>
              </td>
              <td class="px-6 py-4">
                <div class="text-sm">
                  <div class="flex items-center">
                    <Icon name="mdi:map-marker" class="mr-1 text-gray-400" />
                    {{ delivery.location }}
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="getStatusClass(delivery.status)" class="px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full">
                  {{ t(`sales.deliveryNotes.${delivery.status}`) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end gap-2">
                  <button
                    @click="viewDelivery(delivery.id)"
                    class="text-blue-600 hover:text-blue-900"
                    :title="t('common.view')"
                  >
                    <Icon name="mdi:eye" size="20" />
                  </button>
                  
                  <button
                    v-if="delivery.status === 'scheduled'"
                    @click="startDelivery(delivery.id)"
                    class="text-green-600 hover:text-green-900"
                    :title="t('sales.deliveryNotes.startDelivery')"
                  >
                    <Icon name="mdi:truck-fast" size="20" />
                  </button>
                  
                  <button
                    @click="printPackingSlip(delivery.id)"
                    class="text-purple-600 hover:text-purple-900"
                    :title="t('sales.deliveryNotes.printPackingSlip')"
                  >
                    <Icon name="mdi:printer" size="20" />
                  </button>
                  
                  <button
                    v-if="delivery.status === 'delivered' && delivery.proofOfDelivery"
                    @click="viewProof(delivery.id)"
                    class="text-indigo-600 hover:text-indigo-900"
                    :title="t('sales.deliveryNotes.viewProof')"
                  >
                    <Icon name="mdi:file-check" size="20" />
                  </button>
                </div>
              </td>
            </tr>
            
            <tr v-if="filteredDeliveries.length === 0">
              <td colspan="8" class="px-6 py-12 text-center text-gray-500">
                <Icon name="mdi:truck-delivery-outline" size="48" class="mx-auto mb-4 text-gray-300" />
                <p>{{ t('sales.deliveryNotes.noDeliveries') }}</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Map View Toggle (Future Enhancement) -->
    <div class="mt-6 flex justify-center">
      <button
        @click="toggleMapView"
        class="btn btn-secondary flex items-center gap-2"
      >
        <Icon :name="showMap ? 'mdi:format-list-bulleted' : 'mdi:map'" />
        {{ showMap ? t('sales.deliveryNotes.listView') : t('sales.deliveryNotes.mapView') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
const { t } = useI18n()

// State
const searchQuery = ref('')
const filterStatus = ref('')
const filterDate = ref('')
const showMap = ref(false)

// Stats
const stats = ref({
  scheduled: 12,
  inTransit: 8,
  delivered: 45,
  failed: 2
})

// Mock data - Replace with API
const deliveries = ref([
  {
    id: 1,
    deliveryNo: 'DN-2025-001',
    orderRef: 'SO-2025-015',
    orderId: 15,
    customerName: 'Mama Dlamini Spaza',
    deliveryDate: '2025-01-10',
    scheduledTime: '10:00 - 12:00',
    driver: 'Thabo Ndlovu',
    location: 'Soweto, Johannesburg',
    status: 'in-transit',
    proofOfDelivery: null
  },
  {
    id: 2,
    deliveryNo: 'DN-2025-002',
    orderRef: 'SO-2025-016',
    orderId: 16,
    customerName: 'Sibusiso Butchery',
    deliveryDate: '2025-01-10',
    scheduledTime: '14:00 - 16:00',
    driver: 'Sipho Khumalo',
    location: 'Alexandra, Johannesburg',
    status: 'scheduled',
    proofOfDelivery: null
  },
  {
    id: 3,
    deliveryNo: 'DN-2025-003',
    orderRef: 'SO-2025-014',
    orderId: 14,
    customerName: 'Thandi Hair Salon',
    deliveryDate: '2025-01-09',
    scheduledTime: '09:00 - 11:00',
    driver: 'Mandla Zulu',
    location: 'Khayelitsha, Cape Town',
    status: 'delivered',
    proofOfDelivery: {
      signature: 'base64...',
      photo: 'base64...',
      timestamp: '2025-01-09T10:45:00',
      location: { lat: -33.9677, lng: 18.6713 }
    }
  }
])

// Computed
const filteredDeliveries = computed(() => {
  return deliveries.value.filter(delivery => {
    const matchesSearch = !searchQuery.value ||
      delivery.deliveryNo.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      delivery.orderRef.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      delivery.customerName.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !filterStatus.value || delivery.status === filterStatus.value
    const matchesDate = !filterDate.value || delivery.deliveryDate === filterDate.value
    
    return matchesSearch && matchesStatus && matchesDate
  })
})

// Methods
const getStatusClass = (status: string) => {
  const classes = {
    'scheduled': 'bg-blue-100 text-blue-800',
    'in-transit': 'bg-yellow-100 text-yellow-800',
    'delivered': 'bg-green-100 text-green-800',
    'failed': 'bg-red-100 text-red-800'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-ZA', { 
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  })
}

const viewDelivery = (id: number) => {
  navigateTo(`/sales/delivery-notes/${id}`)
}

const startDelivery = (id: number) => {
  // Update delivery status to in-transit
  const delivery = deliveries.value.find(d => d.id === id)
  if (delivery) {
    delivery.status = 'in-transit'
    // API call would go here
    alert(t('sales.deliveryNotes.deliveryStarted'))
  }
}

const printPackingSlip = (id: number) => {
  // Generate and print packing slip
  console.log('Printing packing slip for delivery:', id)
  // Open print dialog or download PDF
}

const viewProof = (id: number) => {
  navigateTo(`/sales/delivery-notes/${id}?tab=proof`)
}

const toggleMapView = () => {
  showMap.value = !showMap.value
  if (showMap.value) {
    // Initialize map view
    alert(t('sales.deliveryNotes.mapViewComingSoon'))
  }
}

// Page meta
definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

// SEO
useHead({
  title: t('sales.deliveryNotes.title')
})
</script>

<style scoped>
.btn {
  @apply px-4 py-2 rounded-lg font-medium transition-colors;
}

.btn-primary {
  @apply bg-blue-600 text-white hover:bg-blue-700;
}

.btn-secondary {
  @apply bg-gray-200 text-gray-800 hover:bg-gray-300;
}
</style>
