<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useBuyingStore, type PurchaseOrder, type PurchaseOrderStatus } from '~/stores/buying'
import { useStockStore } from '~/stores/stock'
import PurchaseOrderModal from '~/components/buying/PurchaseOrderModal.vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Purchase Orders - TOSS'
})

const buyingStore = useBuyingStore()
const stockStore = useStockStore()
const searchQuery = ref('')
const statusFilter = ref<PurchaseOrderStatus | 'all'>('all')
const showCreateModal = ref(false)
const showEditModal = ref(false)
const selectedPO = ref<PurchaseOrder | null>(null)

// Computed
const filteredPurchaseOrders = computed(() => {
  let filtered = buyingStore.purchaseOrders

  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(po => po.status === statusFilter.value)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(po =>
      po.poNumber.toLowerCase().includes(query) ||
      po.supplierName.toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime())
})

const stats = computed(() => {
  const pos = buyingStore.purchaseOrders
  return {
    total: pos.length,
    draft: pos.filter(po => po.status === 'draft').length,
    pending: pos.filter(po => po.status === 'submitted' || po.status === 'approved').length,
    totalValue: pos.reduce((sum, po) => sum + po.total, 0)
  }
})

// Methods
onMounted(async () => {
  await buyingStore.fetchPurchaseOrders()
  await stockStore.fetchItems()
})

function handleCreate() {
  selectedPO.value = null
  showCreateModal.value = true
}

function handleEdit(po: PurchaseOrder) {
  selectedPO.value = po
  showEditModal.value = true
}

function handleView(po: PurchaseOrder) {
  navigateTo(`/buying/purchase-orders/${po.id}`)
}

function handleSubmit(po: PurchaseOrder) {
  if (confirm('Are you sure you want to submit this purchase order?')) {
    buyingStore.submitPurchaseOrder(po.id)
  }
}

function handlePOSaved() {
  showCreateModal.value = false
  showEditModal.value = false
  selectedPO.value = null
  buyingStore.fetchPurchaseOrders()
}

function getStatusColor(status: PurchaseOrderStatus) {
  const colors: Record<PurchaseOrderStatus, string> = {
    draft: 'text-gray-600 bg-gray-100',
    submitted: 'text-blue-600 bg-blue-100',
    approved: 'text-green-600 bg-green-100',
    partially_received: 'text-orange-600 bg-orange-100',
    completed: 'text-green-600 bg-green-100',
    cancelled: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: PurchaseOrderStatus) {
  const labels: Record<PurchaseOrderStatus, string> = {
    draft: 'Draft',
    submitted: 'Submitted',
    approved: 'Approved',
    partially_received: 'Partially Received',
    completed: 'Completed',
    cancelled: 'Cancelled'
  }
  return labels[status] || status
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
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
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Purchase Orders</h3>
      <p class="text-gray-600 text-sm">Create and manage purchase orders from suppliers</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Orders</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.total }}</p>
          </div>
          <div class="p-3 bg-blue-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-blue-600">shopping_cart</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Draft</p>
            <p class="mt-2 text-3xl font-bold text-gray-600">{{ stats.draft }}</p>
          </div>
          <div class="p-3 bg-gray-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-gray-600">edit</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Pending</p>
            <p class="mt-2 text-3xl font-bold text-orange-600">{{ stats.pending }}</p>
          </div>
          <div class="p-3 bg-orange-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-orange-600">schedule</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Value</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">R {{ stats.totalValue.toFixed(2) }}</p>
          </div>
          <div class="p-3 bg-green-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-green-600">attach_money</i>
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
                placeholder="Search by PO number or supplier..."
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
              <option value="approved">Approved</option>
              <option value="partially_received">Partially Received</option>
              <option value="completed">Completed</option>
              <option value="cancelled">Cancelled</option>
            </select>
          </div>
        </div>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors whitespace-nowrap"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create PO</span>
        </button>
      </div>
    </div>

    <!-- Purchase Orders Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="buyingStore.purchaseOrders.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">shopping_cart</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No purchase orders found</h3>
        <p class="text-gray-600 mb-6">Get started by creating your first purchase order</p>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create PO</span>
        </button>
      </div>

      <div v-else-if="filteredPurchaseOrders.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">search</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No purchase orders match your filters</h3>
        <p class="text-gray-600 mb-6">Try adjusting your search or filter criteria</p>
      </div>

      <table v-else class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">PO Number</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Supplier</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Order Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Expected Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Items</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="po in filteredPurchaseOrders" :key="po.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ po.poNumber }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-900">{{ po.supplierName }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatDate(po.orderDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatDate(po.expectedDeliveryDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ po.items.length }} item{{ po.items.length !== 1 ? 's' : '' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              R {{ po.total.toFixed(2) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(po.status)]">
                {{ getStatusLabel(po.status) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <div class="flex items-center justify-end gap-2">
                <button
                  @click="handleView(po)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="View"
                >
                  <i class="material-symbols-rounded text-lg">visibility</i>
                </button>
                <button
                  v-if="po.status === 'draft'"
                  @click="handleEdit(po)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="Edit"
                >
                  <i class="material-symbols-rounded text-lg">edit</i>
                </button>
                <button
                  v-if="po.status === 'draft'"
                  @click="handleSubmit(po)"
                  class="p-2 text-blue-600 hover:text-blue-900 hover:bg-blue-100 rounded-lg transition-colors"
                  title="Submit"
                >
                  <i class="material-symbols-rounded text-lg">send</i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modals -->
    <PurchaseOrderModal
      :show="showCreateModal"
      :purchase-order="null"
      @close="showCreateModal = false"
      @saved="handlePOSaved"
    />
    <PurchaseOrderModal
      :show="showEditModal"
      :purchase-order="selectedPO"
      @close="showEditModal = false; selectedPO = null"
      @saved="handlePOSaved"
    />
  </div>
</template>

