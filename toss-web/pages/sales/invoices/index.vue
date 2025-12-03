<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useSalesStore, type Invoice } from '~/stores/sales'
import { useCrmStore } from '~/stores/crm'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Invoices - TOSS'
})

const salesStore = useSalesStore()
const crmStore = useCrmStore()
const searchQuery = ref('')
const statusFilter = ref<string>('all')
const showCreateModal = ref(false)
const showEditModal = ref(false)
const selectedInvoice = ref<Invoice | null>(null)

// Computed
const filteredInvoices = computed(() => {
  let filtered = (salesStore.invoices || []).filter(inv => inv != null)

  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(inv => inv.status === statusFilter.value)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(inv =>
      (inv.invoiceNumber || '').toLowerCase().includes(query) ||
      (inv.customerName || '').toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => {
    const dateA = a.invoiceDate ? new Date(a.invoiceDate).getTime() : 0
    const dateB = b.invoiceDate ? new Date(b.invoiceDate).getTime() : 0
    return dateB - dateA
  })
})

const stats = computed(() => {
  const invoices = salesStore.invoices || []
  const now = new Date()
  const overdue = invoices.filter(inv => {
    if (!inv || !inv.dueDate) return false
    return inv.status !== 'paid' && inv.status !== 'cancelled' && new Date(inv.dueDate) < now
  }).length
  const totalReceivables = invoices
    .filter(inv => inv && inv.status !== 'paid' && inv.status !== 'cancelled')
    .reduce((sum, inv) => sum + (inv.amountDue || 0), 0)
  
  return {
    total: invoices.length,
    unpaid: invoices.filter(inv => inv && (inv.status === 'sent' || inv.status === 'partially_paid')).length,
    overdue,
    totalReceivables
  }
})

// Methods
onMounted(async () => {
  try {
    await salesStore.fetchInvoices()
    await crmStore.fetchCustomers()
  } catch (error) {
    console.error('Error loading invoices:', error)
  }
})

function handleCreate() {
  selectedInvoice.value = null
  showCreateModal.value = true
}

function handleEdit(invoice: Invoice) {
  selectedInvoice.value = invoice
  showEditModal.value = true
}

function handleView(invoice: Invoice) {
  navigateTo(`/sales/invoices/${invoice.id}`)
}

function handleInvoiceSaved() {
  showCreateModal.value = false
  showEditModal.value = false
  selectedInvoice.value = null
  salesStore.fetchInvoices()
}

function getStatusColor(status: string) {
  const colors: Record<string, string> = {
    draft: 'text-gray-600 bg-gray-100',
    sent: 'text-blue-600 bg-blue-100',
    paid: 'text-green-600 bg-green-100',
    partially_paid: 'text-orange-600 bg-orange-100',
    overdue: 'text-red-600 bg-red-100',
    cancelled: 'text-gray-600 bg-gray-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: string) {
  const labels: Record<string, string> = {
    draft: 'Draft',
    sent: 'Sent',
    paid: 'Paid',
    partially_paid: 'Partially Paid',
    overdue: 'Overdue',
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
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Invoices</h3>
      <p class="text-gray-600 text-sm">Manage sales invoices and payments</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Invoices</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.total }}</p>
          </div>
          <div class="p-3 bg-blue-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-blue-600">receipt_long</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Unpaid</p>
            <p class="mt-2 text-3xl font-bold text-orange-600">{{ stats.unpaid }}</p>
          </div>
          <div class="p-3 bg-orange-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-orange-600">schedule</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Overdue</p>
            <p class="mt-2 text-3xl font-bold text-red-600">{{ stats.overdue }}</p>
          </div>
          <div class="p-3 bg-red-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-red-600">warning</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Receivables</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">R {{ stats.totalReceivables.toFixed(2) }}</p>
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
                placeholder="Search by invoice number or customer..."
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
              <option value="paid">Paid</option>
              <option value="partially_paid">Partially Paid</option>
              <option value="overdue">Overdue</option>
              <option value="cancelled">Cancelled</option>
            </select>
          </div>
        </div>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors whitespace-nowrap"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create Invoice</span>
        </button>
      </div>
    </div>

    <!-- Invoices Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="!salesStore.invoices || salesStore.invoices.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">receipt_long</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No invoices found</h3>
        <p class="text-gray-600 mb-6">Get started by creating your first invoice</p>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create Invoice</span>
        </button>
      </div>

      <div v-else-if="filteredInvoices.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">search</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No invoices match your filters</h3>
        <p class="text-gray-600 mb-6">Try adjusting your search or filter criteria</p>
      </div>

      <table v-else class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Invoice #</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Customer</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Invoice Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Due Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Amount Due</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="invoice in filteredInvoices" :key="invoice.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ invoice.invoiceNumber }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-900">{{ invoice.customerName }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatDate(invoice.invoiceDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatDate(invoice.dueDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              R {{ invoice.total.toFixed(2) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              R {{ invoice.amountDue.toFixed(2) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(invoice.status)]">
                {{ getStatusLabel(invoice.status) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <div class="flex items-center justify-end gap-2">
                <button
                  @click="handleView(invoice)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="View"
                >
                  <i class="material-symbols-rounded text-lg">visibility</i>
                </button>
                <button
                  v-if="invoice.status === 'draft'"
                  @click="handleEdit(invoice)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="Edit"
                >
                  <i class="material-symbols-rounded text-lg">edit</i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

