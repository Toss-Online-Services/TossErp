<template>
  <div class="p-6">
    <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-6">My Deliveries</h1>

    <!-- Status Filter -->
    <div class="mb-6">
      <div class="flex space-x-2">
        <button
          v-for="status in statuses"
          :key="status.value"
          @click="selectedStatus = status.value"
          :class="[
            'px-4 py-2 rounded-lg text-sm font-medium',
            selectedStatus === status.value
              ? 'bg-purple-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          ]"
        >
          {{ status.label }}
        </button>
      </div>
    </div>

    <!-- Deliveries List -->
    <div class="space-y-4">
      <div v-if="isLoading" class="text-center py-12">
        <div class="inline-block w-8 h-8 border-4 border-purple-600 border-t-transparent rounded-full animate-spin"></div>
        <p class="mt-2 text-gray-600">Loading deliveries...</p>
      </div>

      <div v-else-if="filteredDeliveries.length === 0" class="text-center py-12 text-gray-500">
        No deliveries found.
      </div>

      <div
        v-for="delivery in filteredDeliveries"
        :key="delivery.id"
        class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"
      >
        <div class="flex justify-between items-start mb-4">
          <div>
            <h3 class="text-lg font-semibold text-gray-900 dark:text-gray-100">{{ delivery.poNumber }}</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">
              From: {{ delivery.supplierName || 'Supplier' }} → To: {{ delivery.shopName || 'Retailer' }}
            </p>
            <p class="text-sm text-gray-600 dark:text-gray-400">
              Date: {{ formatDate(delivery.orderDate) }}
            </p>
          </div>
          <span
            :class="[
              'px-3 py-1 text-xs font-semibold rounded-full',
              getStatusClass(delivery.status)
            ]"
          >
            {{ delivery.status }}
          </span>
        </div>

        <div class="mb-4">
          <p class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Items:</p>
          <ul class="list-disc list-inside text-sm text-gray-600 dark:text-gray-400">
            <li v-for="item in delivery.items?.slice(0, 3)" :key="item.id">
              {{ item.productName }} ({{ item.quantity }})
            </li>
            <li v-if="delivery.items?.length > 3" class="text-gray-500">
              +{{ delivery.items.length - 3 }} more items
            </li>
          </ul>
        </div>

        <div class="flex justify-between items-center">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Total: <span class="font-semibold text-gray-900 dark:text-gray-100">R{{ formatCurrency(delivery.totalAmount) }}</span></p>
          </div>
          <NuxtLink
            :to="`/driver/deliveries/${delivery.id}`"
            class="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700"
          >
            View Details
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'driver',
  middleware: 'auth',
  meta: {
    roles: ['Driver']
  }
})

const deliveriesAPI = useDeliveriesAPI()
const userStore = useUserStore()
const driverId = ref(1) // TODO: Get from user session
const deliveries = ref<any[]>([])
const isLoading = ref(true)
const selectedStatus = ref<string | null>(null)

const statuses = [
  { value: null, label: 'All' },
  { value: 'Accepted', label: 'Accepted' },
  { value: 'PickedUp', label: 'Picked Up' },
  { value: 'Delivered', label: 'Delivered' }
]

const loadDeliveries = async () => {
  isLoading.value = true
  try {
    deliveries.value = await deliveriesAPI.getDeliveries(
      driverId.value,
      selectedStatus.value || undefined
    )
  } catch (error) {
    console.error('Failed to load deliveries:', error)
  } finally {
    isLoading.value = false
  }
}

const filteredDeliveries = computed(() => {
  return deliveries.value
})

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-ZA')
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'Accepted': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'PickedUp': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    'Delivered': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

watch(selectedStatus, () => {
  loadDeliveries()
})

onMounted(() => {
  loadDeliveries()
})
</script>

