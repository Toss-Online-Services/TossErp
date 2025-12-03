<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useSalesStore, type DeliveryNote } from '~/stores/sales'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Delivery Note Details - TOSS'
})

const route = useRoute()
const salesStore = useSalesStore()

const delivery = ref<DeliveryNote | null>(null)
const loading = ref(false)

const deliveryId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

async function loadDelivery() {
  loading.value = true
  try {
    await salesStore.fetchDeliveries()
    const id = deliveryId.value
    console.log('Loading delivery with ID:', id)
    
    delivery.value = salesStore.getDeliveryById(id) || null
    
    if (!delivery.value) {
      console.error('Delivery not found for ID:', id)
    }
  } catch (error) {
    console.error('Failed to load delivery:', error)
  } finally {
    loading.value = false
  }
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

function getStatusColor(status: string) {
  const colors: Record<string, string> = {
    pending: 'text-gray-600 bg-gray-100',
    in_transit: 'text-blue-600 bg-blue-100',
    delivered: 'text-green-600 bg-green-100',
    failed: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: string) {
  const labels: Record<string, string> = {
    pending: 'Pending',
    in_transit: 'In Transit',
    delivered: 'Delivered',
    failed: 'Failed'
  }
  return labels[status] || status
}

onMounted(async () => {
  await loadDelivery()
})
</script>

<template>
  <div class="py-6">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading delivery note...</p>
      </div>
    </div>

    <div v-else-if="!delivery" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">error</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Delivery note not found</h3>
        <p class="text-gray-600 mb-6">The delivery note you're looking for doesn't exist</p>
        <button
          @click="navigateTo('/sales/deliveries')"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Deliveries</span>
        </button>
      </div>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-6 flex items-center justify-between">
        <div>
          <button
            @click="navigateTo('/sales/deliveries')"
            class="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors"
          >
            <i class="material-symbols-rounded text-lg">arrow_back</i>
            <span>Back</span>
          </button>
          <h3 class="text-3xl font-bold text-gray-900 mb-2">{{ delivery.deliveryNumber }}</h3>
          <p class="text-gray-600 text-sm">Delivery note details and items</p>
        </div>
        <div class="flex items-center gap-3">
          <span :class="['px-3 py-1 text-sm font-medium rounded-full', getStatusColor(delivery.status)]">
            {{ getStatusLabel(delivery.status) }}
          </span>
        </div>
      </div>

      <!-- Info Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4">Order Information</h4>
          <div class="space-y-2">
            <div>
              <p class="text-sm text-gray-500">Order Number</p>
              <p class="text-base font-medium text-gray-900">{{ delivery.orderNumber }}</p>
            </div>
            <div>
              <p class="text-sm text-gray-500">Customer</p>
              <p class="text-base font-medium text-gray-900">{{ delivery.customerName }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4">Delivery Information</h4>
          <div class="space-y-2">
            <div>
              <p class="text-sm text-gray-500">Delivery Date</p>
              <p class="text-base font-medium text-gray-900">{{ formatDate(delivery.deliveryDate) }}</p>
            </div>
            <div v-if="delivery.driverName">
              <p class="text-sm text-gray-500">Driver</p>
              <p class="text-base font-medium text-gray-900">{{ delivery.driverName }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Items Table -->
      <div class="bg-white rounded-xl shadow-card overflow-hidden mb-6">
        <div class="px-6 py-4 border-b border-gray-200">
          <h4 class="text-lg font-semibold text-gray-900">Delivery Items</h4>
        </div>
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Item</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Quantity</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Rate</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Amount</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="item in delivery.items" :key="item.id" class="hover:bg-gray-50">
              <td class="px-6 py-4">
                <div class="text-sm font-medium text-gray-900">{{ item.itemName }}</div>
              </td>
              <td class="px-6 py-4 text-sm text-gray-900">
                {{ item.quantity }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-900">
                R {{ item.rate.toFixed(2) }}
              </td>
              <td class="px-6 py-4 text-sm font-medium text-gray-900 text-right">
                R {{ item.amount.toFixed(2) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Notes -->
      <div v-if="delivery.notes" class="bg-white rounded-xl shadow-card p-6">
        <h4 class="text-sm font-medium text-gray-600 mb-2">Notes</h4>
        <p class="text-sm text-gray-900">{{ delivery.notes }}</p>
      </div>
    </div>
  </div>
</template>

