<template>
  <div class="p-6">
    <div class="mb-6">
      <NuxtLink
        to="/driver/deliveries"
        class="text-purple-600 hover:text-purple-800 dark:text-purple-400"
      >
        ← Back to Deliveries
      </NuxtLink>
    </div>

    <div v-if="isLoading" class="text-center py-12">
      <div class="inline-block w-8 h-8 border-4 border-purple-600 border-t-transparent rounded-full animate-spin"></div>
      <p class="mt-2 text-gray-600">Loading delivery...</p>
    </div>

    <div v-else-if="delivery" class="space-y-6">
      <!-- Delivery Header -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">{{ delivery.poNumber }}</h1>
            <p class="text-gray-600 dark:text-gray-400 mt-2">
              <span class="font-semibold">From:</span> {{ delivery.supplierName || 'Supplier' }}<br>
              <span class="font-semibold">To:</span> {{ delivery.shopName || 'Retailer' }}
            </p>
            <p class="text-gray-600 dark:text-gray-400 mt-2">
              Date: {{ formatDate(delivery.orderDate) }}
            </p>
          </div>
          <span
            :class="[
              'px-4 py-2 text-sm font-semibold rounded-full',
              getStatusClass(delivery.status)
            ]"
          >
            {{ delivery.status }}
          </span>
        </div>

        <!-- Contact Information -->
        <div class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
          <h3 class="font-semibold mb-2">Contact Information</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">
            Supplier: {{ delivery.supplierPhone || 'N/A' }}<br>
            Retailer: {{ delivery.shopPhone || 'N/A' }}
          </p>
        </div>

        <!-- Delivery Items -->
        <div class="mt-6">
          <h2 class="text-lg font-semibold mb-4">Items to Deliver</h2>
          <div class="space-y-2">
            <div
              v-for="item in delivery.items"
              :key="item.id"
              class="flex justify-between items-center p-3 bg-gray-50 dark:bg-gray-700 rounded"
            >
              <div>
                <p class="font-medium text-gray-900 dark:text-gray-100">{{ item.productName }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400">SKU: {{ item.productSKU }}</p>
              </div>
              <div class="text-right">
                <p class="font-medium text-gray-900 dark:text-gray-100">Qty: {{ item.quantity }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400">R{{ formatCurrency(item.lineTotal) }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="mt-6 pt-6 border-t flex justify-between items-center">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Amount</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-gray-100">R{{ formatCurrency(delivery.totalAmount) }}</p>
          </div>
        </div>
      </div>

      <!-- Status Actions -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h2 class="text-lg font-semibold mb-4">Update Status</h2>
        
        <div v-if="delivery.status === 'Accepted'" class="space-y-4">
          <button
            @click="markPickedUp"
            :disabled="isProcessing"
            class="w-full px-6 py-3 bg-purple-600 text-white rounded-lg hover:bg-purple-700 disabled:opacity-50"
          >
            Mark as Picked Up
          </button>
        </div>

        <div v-else-if="delivery.status === 'PickedUp'" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Delivery Notes (optional)
            </label>
            <textarea
              v-model="deliveryNotes"
              rows="3"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 mb-4"
              placeholder="Any notes about the delivery..."
            />
          </div>
          <div class="flex items-center mb-4">
            <input
              v-model="deliveryConfirmed"
              type="checkbox"
              class="mr-2"
            />
            <label class="text-sm font-medium text-gray-700 dark:text-gray-300">
              Delivery confirmed by recipient
            </label>
          </div>
          <button
            @click="markDelivered"
            :disabled="isProcessing"
            class="w-full px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50"
          >
            Mark as Delivered
          </button>
        </div>

        <div v-else-if="delivery.status === 'Delivered'" class="p-4 bg-green-50 dark:bg-green-900/20 rounded-lg">
          <p class="text-green-800 dark:text-green-200 font-semibold">✓ Delivery completed</p>
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

const route = useRoute()
const router = useRouter()
const deliveriesAPI = useDeliveriesAPI()

const deliveryId = computed(() => Number(route.params.id))
const delivery = ref<any>(null)
const isLoading = ref(true)
const isProcessing = ref(false)
const deliveryNotes = ref('')
const deliveryConfirmed = ref(true)

const loadDelivery = async () => {
  isLoading.value = true
  try {
    delivery.value = await deliveriesAPI.getDeliveryById(deliveryId.value)
  } catch (error) {
    console.error('Failed to load delivery:', error)
    router.push('/driver/deliveries')
  } finally {
    isLoading.value = false
  }
}

const markPickedUp = async () => {
  if (!confirm('Mark this delivery as picked up?')) return

  isProcessing.value = true
  try {
    await deliveriesAPI.markPickedUp(deliveryId.value)
    await loadDelivery()
  } catch (error) {
    console.error('Failed to mark as picked up:', error)
    alert('Failed to update status. Please try again.')
  } finally {
    isProcessing.value = false
  }
}

const markDelivered = async () => {
  if (!confirm('Mark this delivery as completed?')) return

  isProcessing.value = true
  try {
    await deliveriesAPI.markDelivered(
      deliveryId.value,
      deliveryNotes.value || undefined,
      deliveryConfirmed.value
    )
    await loadDelivery()
  } catch (error) {
    console.error('Failed to mark as delivered:', error)
    alert('Failed to update status. Please try again.')
  } finally {
    isProcessing.value = false
  }
}

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

onMounted(() => {
  loadDelivery()
})
</script>

