<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useSalesStore, type SalesOrder } from '~/stores/sales'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Sales Order Details - TOSS'
})

const route = useRoute()
const salesStore = useSalesStore()

const order = ref<SalesOrder | null>(null)
const loading = ref(false)

const orderId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

async function loadOrder() {
  loading.value = true
  try {
    await salesStore.fetchOrders()
    const id = orderId.value
    console.log('Loading order with ID:', id)
    
    order.value = salesStore.getOrderById(id) || null
    
    if (!order.value) {
      console.error('Order not found for ID:', id)
    }
  } catch (error) {
    console.error('Failed to load order:', error)
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
    draft: 'text-gray-600 bg-gray-100',
    confirmed: 'text-blue-600 bg-blue-100',
    partially_delivered: 'text-orange-600 bg-orange-100',
    delivered: 'text-green-600 bg-green-100',
    cancelled: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: string) {
  const labels: Record<string, string> = {
    draft: 'Draft',
    confirmed: 'Confirmed',
    partially_delivered: 'Partially Delivered',
    delivered: 'Delivered',
    cancelled: 'Cancelled'
  }
  return labels[status] || status
}

function getPaymentStatusColor(status: string) {
  const colors: Record<string, string> = {
    unpaid: 'text-red-600 bg-red-100',
    partially_paid: 'text-orange-600 bg-orange-100',
    paid: 'text-green-600 bg-green-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getPaymentStatusLabel(status: string) {
  const labels: Record<string, string> = {
    unpaid: 'Unpaid',
    partially_paid: 'Partially Paid',
    paid: 'Paid'
  }
  return labels[status] || status
}

onMounted(async () => {
  await loadOrder()
})
</script>

<template>
  <div class="py-6">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading order...</p>
      </div>
    </div>

    <div v-else-if="!order" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">error</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Order not found</h3>
        <p class="text-gray-600 mb-6">The order you're looking for doesn't exist</p>
        <button
          @click="navigateTo('/sales/orders')"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Orders</span>
        </button>
      </div>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-6 flex items-center justify-between">
        <div>
          <button
            @click="navigateTo('/sales/orders')"
            class="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors"
          >
            <i class="material-symbols-rounded text-lg">arrow_back</i>
            <span>Back</span>
          </button>
          <h3 class="text-3xl font-bold text-gray-900 mb-2">{{ order.orderNumber }}</h3>
          <p class="text-gray-600 text-sm">Sales order details and items</p>
        </div>
        <div class="flex items-center gap-3">
          <span :class="['px-3 py-1 text-sm font-medium rounded-full', getStatusColor(order.status)]">
            {{ getStatusLabel(order.status) }}
          </span>
          <span :class="['px-3 py-1 text-sm font-medium rounded-full', getPaymentStatusColor(order.paymentStatus)]">
            {{ getPaymentStatusLabel(order.paymentStatus) }}
          </span>
        </div>
      </div>

      <!-- Info Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4">Customer Information</h4>
          <div class="space-y-2">
            <div>
              <p class="text-sm text-gray-500">Customer</p>
              <p class="text-base font-medium text-gray-900">{{ order.customerName }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4">Order Dates</h4>
          <div class="space-y-2">
            <div>
              <p class="text-sm text-gray-500">Order Date</p>
              <p class="text-base font-medium text-gray-900">{{ formatDate(order.orderDate) }}</p>
            </div>
            <div v-if="order.deliveryDate">
              <p class="text-sm text-gray-500">Delivery Date</p>
              <p class="text-base font-medium text-gray-900">{{ formatDate(order.deliveryDate) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Items Table -->
      <div class="bg-white rounded-xl shadow-card overflow-hidden mb-6">
        <div class="px-6 py-4 border-b border-gray-200">
          <h4 class="text-lg font-semibold text-gray-900">Order Items</h4>
        </div>
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Item</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Quantity</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Rate</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Discount</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Amount</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="item in order.items" :key="item.id" class="hover:bg-gray-50">
              <td class="px-6 py-4">
                <div class="text-sm font-medium text-gray-900">{{ item.itemName }}</div>
              </td>
              <td class="px-6 py-4 text-sm text-gray-900">
                {{ item.quantity }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-900">
                R {{ item.rate.toFixed(2) }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-900">
                R {{ item.discount.toFixed(2) }}
              </td>
              <td class="px-6 py-4 text-sm font-medium text-gray-900 text-right">
                R {{ item.amount.toFixed(2) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Totals -->
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex justify-end">
          <div class="w-64 space-y-2">
            <div class="flex justify-between text-sm">
              <span class="text-gray-600">Subtotal:</span>
              <span class="text-gray-900">R {{ order.subtotal.toFixed(2) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600">Discount:</span>
              <span class="text-gray-900">R {{ order.discount.toFixed(2) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600">Tax (15%):</span>
              <span class="text-gray-900">R {{ order.tax.toFixed(2) }}</span>
            </div>
            <div class="flex justify-between text-lg font-bold border-t border-gray-200 pt-2">
              <span class="text-gray-900">Total:</span>
              <span class="text-gray-900">R {{ order.total.toFixed(2) }}</span>
            </div>
          </div>
        </div>
        <div v-if="order.notes" class="mt-6 pt-6 border-t border-gray-200">
          <h4 class="text-sm font-medium text-gray-600 mb-2">Notes</h4>
          <p class="text-sm text-gray-900">{{ order.notes }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

