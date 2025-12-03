<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useBuyingStore, type GoodsReceipt, type GoodsReceiptStatus } from '~/stores/buying'
import GoodsReceiptModal from '~/components/buying/GoodsReceiptModal.vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Goods Receipts - TOSS'
})

const buyingStore = useBuyingStore()
const searchQuery = ref('')
const statusFilter = ref<GoodsReceiptStatus | 'all'>('all')
const showCreateModal = ref(false)

// Computed
const filteredReceipts = computed(() => {
  let filtered = buyingStore.goodsReceipts

  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(gr => gr.status === statusFilter.value)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(gr =>
      gr.receiptNumber.toLowerCase().includes(query) ||
      gr.supplierName.toLowerCase().includes(query) ||
      gr.purchaseOrderNumber?.toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => new Date(b.receiptDate).getTime() - new Date(a.receiptDate).getTime())
})

const stats = computed(() => {
  const receipts = buyingStore.goodsReceipts
  return {
    total: receipts.length,
    submitted: receipts.filter(gr => gr.status === 'submitted').length,
    totalValue: receipts.reduce((sum, gr) => sum + gr.total, 0)
  }
})

// Methods
onMounted(async () => {
  await buyingStore.fetchGoodsReceipts()
  await buyingStore.fetchPurchaseOrders()
})

function handleCreate() {
  showCreateModal.value = true
}

function handleView(gr: GoodsReceipt) {
  navigateTo(`/buying/receipts/${gr.id}`)
}

function handleReceiptSaved() {
  showCreateModal.value = false
  buyingStore.fetchGoodsReceipts()
}

function getStatusColor(status: GoodsReceiptStatus) {
  const colors: Record<GoodsReceiptStatus, string> = {
    draft: 'text-gray-600 bg-gray-100',
    submitted: 'text-green-600 bg-green-100',
    cancelled: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: GoodsReceiptStatus) {
  const labels: Record<GoodsReceiptStatus, string> = {
    draft: 'Draft',
    submitted: 'Submitted',
    cancelled: 'Cancelled'
  }
  return labels[status] || status
}

function formatDate(date: Date) {
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}
</script>

<template>
  <div class="py-6">
    <div class="mb-8">
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Goods Receipts</h3>
      <p class="text-gray-600 text-sm">Record received goods and update stock levels</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Receipts</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.total }}</p>
          </div>
          <div class="p-3 bg-blue-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-blue-600">inventory</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Submitted</p>
            <p class="mt-2 text-3xl font-bold text-green-600">{{ stats.submitted }}</p>
          </div>
          <div class="p-3 bg-green-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-green-600">check_circle</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Value</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">R {{ stats.totalValue.toFixed(2) }}</p>
          </div>
          <div class="p-3 bg-orange-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-orange-600">attach_money</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Actions -->
    <div class="bg-white rounded-xl shadow-card p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4 items-center justify-between">
        <div class="flex-1 w-full flex gap-4">
          <div class="flex-1">
            <div class="relative">
              <i class="material-symbols-rounded absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">search</i>
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search by receipt number, supplier, or PO number..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
              />
            </div>
          </div>
          <div class="md:w-48">
            <select
              v-model="statusFilter"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent bg-white"
            >
              <option value="all">All Status</option>
              <option value="draft">Draft</option>
              <option value="submitted">Submitted</option>
              <option value="cancelled">Cancelled</option>
            </select>
          </div>
        </div>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors whitespace-nowrap"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create Receipt</span>
        </button>
      </div>
    </div>

    <!-- Goods Receipts Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="buyingStore.goodsReceipts.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">inventory</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No goods receipts found</h3>
        <p class="text-gray-600 mb-6">Get started by creating your first goods receipt</p>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create Receipt</span>
        </button>
      </div>

      <div v-else-if="filteredReceipts.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">search</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No receipts match your filters</h3>
        <p class="text-gray-600 mb-6">Try adjusting your search or filter criteria</p>
      </div>

      <table v-else class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Receipt Number</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Supplier</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">PO Number</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Receipt Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Items</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="gr in filteredReceipts" :key="gr.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ gr.receiptNumber }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-900">{{ gr.supplierName }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ gr.purchaseOrderNumber || '-' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatDate(gr.receiptDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ gr.items.length }} item{{ gr.items.length !== 1 ? 's' : '' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              R {{ gr.total.toFixed(2) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(gr.status)]">
                {{ getStatusLabel(gr.status) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <div class="flex items-center justify-end gap-2">
                <button
                  @click="handleView(gr)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="View"
                >
                  <i class="material-symbols-rounded text-lg">visibility</i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modals -->
    <GoodsReceiptModal
      :show="showCreateModal"
      :goods-receipt="null"
      @close="showCreateModal = false"
      @saved="handleReceiptSaved"
    />
  </div>
</template>
