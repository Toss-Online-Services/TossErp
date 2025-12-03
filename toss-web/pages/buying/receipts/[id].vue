<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useBuyingStore, type GoodsReceipt } from '~/stores/buying'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Goods Receipt Details - TOSS'
})

const route = useRoute()
const buyingStore = useBuyingStore()

const receipt = ref<GoodsReceipt | null>(null)
const loading = ref(false)

const receiptId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

async function loadReceipt() {
  loading.value = true
  try {
    await buyingStore.fetchGoodsReceipts()
    const id = receiptId.value
    console.log('Loading goods receipt with ID:', id)
    
    receipt.value = buyingStore.getGoodsReceiptById(id) || null
    
    if (!receipt.value) {
      console.error('Goods receipt not found for ID:', id)
    }
  } catch (error) {
    console.error('Failed to load goods receipt:', error)
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
    submitted: 'text-green-600 bg-green-100',
    cancelled: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: string) {
  const labels: Record<string, string> = {
    draft: 'Draft',
    submitted: 'Submitted',
    cancelled: 'Cancelled'
  }
  return labels[status] || status
}

function handlePrint() {
  window.print()
}

async function handleSend() {
  if (!receipt.value) return
  try {
    // TODO: Implement send functionality (email/SMS/WhatsApp)
    alert(`Sending goods receipt ${receipt.value.receiptNumber} to ${receipt.value.supplierName}...`)
    // await buyingStore.sendGoodsReceipt(receipt.value.id)
  } catch (error) {
    console.error('Failed to send goods receipt:', error)
  }
}

onMounted(async () => {
  await loadReceipt()
})
</script>

<template>
  <div class="py-6">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading goods receipt...</p>
      </div>
    </div>

    <div v-else-if="!receipt" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">error</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Goods receipt not found</h3>
        <p class="text-gray-600 mb-6">The goods receipt you're looking for doesn't exist</p>
        <button
          @click="navigateTo('/buying/receipts')"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Goods Receipts</span>
        </button>
      </div>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-6">
        <button
          @click="navigateTo('/buying/receipts')"
          class="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Goods Receipts</span>
        </button>
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <h3 class="text-3xl font-bold text-gray-900 mb-2">{{ receipt.receiptNumber }}</h3>
            <p class="text-gray-600 text-sm">Goods receipt details and items</p>
          </div>
          <div class="flex flex-wrap items-center gap-3">
            <span :class="['px-3 py-1 text-sm font-medium rounded-full', getStatusColor(receipt.status)]">
              {{ getStatusLabel(receipt.status) }}
            </span>
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
            <i class="material-symbols-rounded text-lg">store</i>
            Supplier Information
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Supplier Name</p>
              <p class="text-base font-semibold text-gray-900">{{ receipt.supplierName }}</p>
            </div>
            <div v-if="receipt.purchaseOrderNumber">
              <p class="text-xs text-gray-500 mb-1">Purchase Order</p>
              <p class="text-sm text-gray-700">{{ receipt.purchaseOrderNumber }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">calendar_today</i>
            Receipt Dates
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Receipt Date</p>
              <p class="text-base font-semibold text-gray-900">{{ formatDate(receipt.receiptDate) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Warehouse</p>
              <p class="text-base font-semibold text-gray-900">{{ receipt.warehouse }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">info</i>
            Receipt Summary
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Total Items</p>
              <p class="text-base font-semibold text-gray-900">{{ receipt.items.length }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Total Amount</p>
              <p class="text-lg font-bold text-gray-900">R {{ receipt.total.toFixed(2) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Items Table -->
      <div class="bg-white rounded-xl shadow-card overflow-hidden mb-6">
        <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
          <h4 class="text-lg font-semibold text-gray-900 flex items-center gap-2">
            <i class="material-symbols-rounded text-xl">inventory</i>
            Received Items
          </h4>
          <span class="text-sm text-gray-500">{{ receipt.items.length }} item(s)</span>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Item</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Ordered</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Received</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Rejected</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Rate</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Amount</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="item in receipt.items" :key="item.id" class="hover:bg-gray-50 transition-colors">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-gray-900">{{ item.itemName }}</div>
                  <div v-if="item.itemCode" class="text-xs text-gray-500 mt-1">Code: {{ item.itemCode }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-gray-900 font-medium">{{ item.orderedQuantity }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-green-600 font-medium">{{ item.receivedQuantity }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span v-if="item.rejectedQuantity > 0" class="text-sm text-red-600 font-medium">{{ item.rejectedQuantity }}</span>
                  <span v-else class="text-sm text-gray-400">-</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-gray-900">R {{ item.rate.toFixed(2) }}</span>
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
            Receipt Summary
          </h4>
          <div class="flex justify-end">
            <div class="w-full max-w-sm space-y-3">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">Subtotal:</span>
                <span class="text-gray-900 font-medium">R {{ receipt.subtotal.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">Tax (15%):</span>
                <span class="text-gray-900 font-medium">R {{ receipt.taxAmount.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-lg font-bold border-t-2 border-gray-300 pt-3 mt-3">
                <span class="text-gray-900">Total:</span>
                <span class="text-gray-900">R {{ receipt.total.toFixed(2) }}</span>
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
          <div v-if="receipt.notes" class="text-sm text-gray-700 leading-relaxed">
            {{ receipt.notes }}
          </div>
          <div v-else class="text-sm text-gray-400 italic">
            No notes added
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

