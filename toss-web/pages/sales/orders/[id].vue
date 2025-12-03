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

async function handleCreateInvoice() {
  if (!order.value) return
  try {
    const invoice = await salesStore.createInvoiceFromOrder(order.value.id)
    navigateTo(`/sales/invoices/${invoice.id}`)
  } catch (error) {
    console.error('Failed to create invoice:', error)
  }
}

function handlePrint() {
  window.print()
}

async function handleSend() {
  if (!order.value) return
  try {
    // TODO: Implement send functionality (email/SMS/WhatsApp)
    // This would typically send the order to the customer
    alert(`Sending order ${order.value.orderNumber} to ${order.value.customerName}...`)
    // await salesStore.sendOrder(order.value.id)
  } catch (error) {
    console.error('Failed to send order:', error)
  }
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
      <div class="mb-6">
        <button
          @click="navigateTo('/sales/orders')"
          class="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Orders</span>
        </button>
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <h3 class="text-3xl font-bold text-gray-900 mb-2">{{ order.orderNumber }}</h3>
            <p class="text-gray-600 text-sm">Sales order details and items</p>
          </div>
          <div class="flex flex-wrap items-center gap-3">
            <span :class="['px-3 py-1 text-sm font-medium rounded-full', getStatusColor(order.status)]">
              {{ getStatusLabel(order.status) }}
            </span>
            <span :class="['px-3 py-1 text-sm font-medium rounded-full', getPaymentStatusColor(order.paymentStatus)]">
              {{ getPaymentStatusLabel(order.paymentStatus) }}
            </span>
            <button
              v-if="order.status === 'confirmed' && order.paymentStatus !== 'paid'"
              @click="handleCreateInvoice"
              class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">receipt_long</i>
              <span>Create Invoice</span>
            </button>
            <button
              @click="handlePrint"
              class="inline-flex items-center gap-2 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">print</i>
              <span>Print</span>
            </button>
            <button
              @click="handleSend"
              class="inline-flex items-center gap-2 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">send</i>
              <span>Send</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Info Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">person</i>
            Customer Information
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Customer Name</p>
              <p class="text-base font-semibold text-gray-900">{{ order.customerName }}</p>
            </div>
            <div v-if="order.quotationId">
              <p class="text-xs text-gray-500 mb-1">Quotation</p>
              <p class="text-sm text-gray-700">{{ order.quotationId }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">calendar_today</i>
            Order Dates
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Order Date</p>
              <p class="text-base font-semibold text-gray-900">{{ formatDate(order.orderDate) }}</p>
            </div>
            <div v-if="order.deliveryDate">
              <p class="text-xs text-gray-500 mb-1">Expected Delivery</p>
              <p class="text-base font-semibold text-gray-900">{{ formatDate(order.deliveryDate) }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">info</i>
            Order Summary
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Total Items</p>
              <p class="text-base font-semibold text-gray-900">{{ order.items.length }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Total Amount</p>
              <p class="text-lg font-bold text-gray-900">R {{ order.total.toFixed(2) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Items Table -->
      <div class="bg-white rounded-xl shadow-card overflow-hidden mb-6">
        <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
          <h4 class="text-lg font-semibold text-gray-900 flex items-center gap-2">
            <i class="material-symbols-rounded text-xl">shopping_cart</i>
            Order Items
          </h4>
          <span class="text-sm text-gray-500">{{ order.items.length }} item(s)</span>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Item</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Quantity</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Rate</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Discount</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Amount</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="item in order.items" :key="item.id" class="hover:bg-gray-50 transition-colors">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-gray-900">{{ item.itemName }}</div>
                  <div v-if="item.itemId" class="text-xs text-gray-500 mt-1">ID: {{ item.itemId }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-gray-900 font-medium">{{ item.quantity }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-gray-900">R {{ item.rate.toFixed(2) }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-gray-900">
                    <span v-if="item.discount > 0" class="text-orange-600">-R {{ item.discount.toFixed(2) }}</span>
                    <span v-else class="text-gray-400">-</span>
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right">
                  <span class="text-sm font-semibold text-gray-900">R {{ item.amount.toFixed(2) }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Totals and Notes -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Totals -->
        <div class="lg:col-span-2 bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">receipt</i>
            Order Summary
          </h4>
          <div class="flex justify-end">
            <div class="w-full max-w-sm space-y-3">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">Subtotal:</span>
                <span class="text-gray-900 font-medium">R {{ order.subtotal.toFixed(2) }}</span>
              </div>
              <div v-if="order.discount > 0" class="flex justify-between text-sm">
                <span class="text-gray-600">Discount:</span>
                <span class="text-orange-600 font-medium">-R {{ order.discount.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">Tax (15%):</span>
                <span class="text-gray-900 font-medium">R {{ order.tax.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-lg font-bold border-t-2 border-gray-300 pt-3 mt-3">
                <span class="text-gray-900">Total:</span>
                <span class="text-gray-900">R {{ order.total.toFixed(2) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Notes -->
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">note</i>
            Notes
          </h4>
          <div v-if="order.notes" class="text-sm text-gray-700 leading-relaxed">
            {{ order.notes }}
          </div>
          <div v-else class="text-sm text-gray-400 italic">
            No notes added
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

