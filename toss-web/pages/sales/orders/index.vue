<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useSalesStore, type SalesOrder } from '~/stores/sales'
import { useCrmStore } from '~/stores/crm'
import { useStockStore } from '~/stores/stock'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Sales Orders - TOSS'
})

const salesStore = useSalesStore()
const crmStore = useCrmStore()
const stockStore = useStockStore()
const searchQuery = ref('')
const statusFilter = ref<string>('all')
const showCreateModal = ref(false)
const showEditModal = ref(false)
const selectedOrder = ref<SalesOrder | null>(null)

// Computed
const filteredOrders = computed(() => {
  let filtered = salesStore.orders

  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(o => o.status === statusFilter.value)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(o =>
      o.orderNumber.toLowerCase().includes(query) ||
      o.customerName.toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime())
})

const stats = computed(() => {
  const orders = salesStore.orders
  return {
    total: orders.length,
    active: orders.filter(o => o.status !== 'cancelled' && o.status !== 'delivered').length,
    delivered: orders.filter(o => o.status === 'delivered').length,
    totalValue: orders.reduce((sum, o) => sum + o.total, 0)
  }
})

// Methods
onMounted(async () => {
  await salesStore.fetchOrders()
  await crmStore.fetchCustomers()
  await stockStore.fetchItems()
})

function handleCreate() {
  selectedOrder.value = null
  showCreateModal.value = true
}

function handleEdit(order: SalesOrder) {
  selectedOrder.value = order
  showEditModal.value = true
}

function handleView(order: SalesOrder) {
  navigateTo(`/sales/orders/${order.id}`)
}

function handleCreateInvoice(order: SalesOrder) {
  if (confirm('Create invoice from this order?')) {
    salesStore.createInvoiceFromOrder(order.id)
    salesStore.fetchOrders()
    salesStore.fetchInvoices()
  }
}

function handleOrderSaved() {
  showCreateModal.value = false
  showEditModal.value = false
  selectedOrder.value = null
  salesStore.fetchOrders()
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
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Sales Orders</h3>
      <p class="text-gray-600 text-sm">Manage customer orders and deliveries</p>
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
            <i class="material-symbols-rounded text-2xl text-blue-600">shopping_bag</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Active</p>
            <p class="mt-2 text-3xl font-bold text-orange-600">{{ stats.active }}</p>
          </div>
          <div class="p-3 bg-orange-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-orange-600">pending_actions</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Delivered</p>
            <p class="mt-2 text-3xl font-bold text-green-600">{{ stats.delivered }}</p>
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
                placeholder="Search by order number or customer..."
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
              <option value="confirmed">Confirmed</option>
              <option value="partially_delivered">Partially Delivered</option>
              <option value="delivered">Delivered</option>
              <option value="cancelled">Cancelled</option>
            </select>
          </div>
        </div>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors whitespace-nowrap"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create Order</span>
        </button>
      </div>
    </div>

    <!-- Orders Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="salesStore.orders.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">shopping_bag</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No orders found</h3>
        <p class="text-gray-600 mb-6">Start by creating your first sales order</p>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create Order</span>
        </button>
      </div>

      <div v-else-if="filteredOrders.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">search</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No orders match your filters</h3>
        <p class="text-gray-600 mb-6">Try adjusting your search or filter criteria</p>
      </div>

      <table v-else class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Order #</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Customer</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Order Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Delivery Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Items</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="order in filteredOrders" :key="order.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ order.orderNumber }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-900">{{ order.customerName }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatDate(order.orderDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatDate(order.deliveryDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ order.items.length }} item{{ order.items.length !== 1 ? 's' : '' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              R {{ order.total.toFixed(2) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(order.status)]">
                {{ getStatusLabel(order.status) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <div class="flex items-center justify-end gap-2">
                <button
                  @click="handleView(order)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="View"
                >
                  <i class="material-symbols-rounded text-lg">visibility</i>
                </button>
                <button
                  v-if="order.status === 'draft'"
                  @click="handleEdit(order)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="Edit"
                >
                  <i class="material-symbols-rounded text-lg">edit</i>
                </button>
                <button
                  v-if="order.status === 'confirmed' || order.status === 'partially_delivered'"
                  @click="handleCreateInvoice(order)"
                  class="p-2 text-green-600 hover:text-green-900 hover:bg-green-100 rounded-lg transition-colors"
                  title="Create Invoice"
                >
                  <i class="material-symbols-rounded text-lg">receipt_long</i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

