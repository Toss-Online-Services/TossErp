<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useSalesStore, type Quotation, type QuotationStatus } from '~/stores/sales'
import { useCrmStore } from '~/stores/crm'
import { useStockStore } from '~/stores/stock'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Quotations - TOSS'
})

const salesStore = useSalesStore()
const crmStore = useCrmStore()
const stockStore = useStockStore()
const searchQuery = ref('')
const statusFilter = ref<QuotationStatus | 'all'>('all')
const showCreateModal = ref(false)
const showEditModal = ref(false)
const selectedQuotation = ref<Quotation | null>(null)

// Computed
const filteredQuotations = computed(() => {
  let filtered = salesStore.quotations

  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(q => q.status === statusFilter.value)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(q =>
      q.quotationNumber.toLowerCase().includes(query) ||
      q.customerName.toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
})

const stats = computed(() => {
  const quotes = salesStore.quotations
  return {
    total: quotes.length,
    pending: quotes.filter(q => q.status === 'sent').length,
    accepted: quotes.filter(q => q.status === 'accepted').length,
    totalValue: quotes.reduce((sum, q) => sum + q.total, 0)
  }
})

// Methods
onMounted(async () => {
  await salesStore.fetchQuotations()
  await crmStore.fetchCustomers()
  await stockStore.fetchItems()
})

function handleCreate() {
  selectedQuotation.value = null
  showCreateModal.value = true
}

function handleEdit(quotation: Quotation) {
  selectedQuotation.value = quotation
  showEditModal.value = true
}

function handleView(quotation: Quotation) {
  navigateTo(`/sales/quotations/${quotation.id}`)
}

function handleConvertToOrder(quotation: Quotation) {
  if (confirm('Convert this quotation to a sales order?')) {
    salesStore.convertQuotationToOrder(quotation.id)
    salesStore.fetchQuotations()
    salesStore.fetchOrders()
  }
}

function handleQuotationSaved() {
  showCreateModal.value = false
  showEditModal.value = false
  selectedQuotation.value = null
  salesStore.fetchQuotations()
}

function getStatusColor(status: QuotationStatus) {
  const colors: Record<QuotationStatus, string> = {
    draft: 'text-gray-600 bg-gray-100',
    sent: 'text-blue-600 bg-blue-100',
    accepted: 'text-green-600 bg-green-100',
    rejected: 'text-red-600 bg-red-100',
    expired: 'text-orange-600 bg-orange-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: QuotationStatus) {
  const labels: Record<QuotationStatus, string> = {
    draft: 'Draft',
    sent: 'Sent',
    accepted: 'Accepted',
    rejected: 'Rejected',
    expired: 'Expired'
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
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Quotations</h3>
      <p class="text-gray-600 text-sm">Create and manage sales quotations</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Quotations</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.total }}</p>
          </div>
          <div class="p-3 bg-blue-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-blue-600">description</i>
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
            <p class="text-sm font-medium text-gray-600">Accepted</p>
            <p class="mt-2 text-3xl font-bold text-green-600">{{ stats.accepted }}</p>
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
          <div class="p-3 bg-purple-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-purple-600">payments</i>
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
                placeholder="Search by quotation number or customer..."
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
              <option value="sent">Sent</option>
              <option value="accepted">Accepted</option>
              <option value="rejected">Rejected</option>
              <option value="expired">Expired</option>
            </select>
          </div>
        </div>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors whitespace-nowrap"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create Quotation</span>
        </button>
      </div>
    </div>

    <!-- Quotations Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="salesStore.quotations.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">description</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No quotations found</h3>
        <p class="text-gray-600 mb-6">Get started by creating your first quotation</p>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create Quotation</span>
        </button>
      </div>

      <div v-else-if="filteredQuotations.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">search</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No quotations match your filters</h3>
        <p class="text-gray-600 mb-6">Try adjusting your search or filter criteria</p>
      </div>

      <table v-else class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Quotation #</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Customer</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Valid Until</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Items</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="quotation in filteredQuotations" :key="quotation.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ quotation.quotationNumber }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-900">{{ quotation.customerName }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatDate(quotation.date) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatDate(quotation.validUntil) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ quotation.items.length }} item{{ quotation.items.length !== 1 ? 's' : '' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              R {{ quotation.total.toFixed(2) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(quotation.status)]">
                {{ getStatusLabel(quotation.status) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <div class="flex items-center justify-end gap-2">
                <button
                  @click="handleView(quotation)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="View"
                >
                  <i class="material-symbols-rounded text-lg">visibility</i>
                </button>
                <button
                  v-if="quotation.status === 'draft'"
                  @click="handleEdit(quotation)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="Edit"
                >
                  <i class="material-symbols-rounded text-lg">edit</i>
                </button>
                <button
                  v-if="quotation.status === 'accepted'"
                  @click="handleConvertToOrder(quotation)"
                  class="p-2 text-green-600 hover:text-green-900 hover:bg-green-100 rounded-lg transition-colors"
                  title="Convert to Order"
                >
                  <i class="material-symbols-rounded text-lg">shopping_bag</i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

