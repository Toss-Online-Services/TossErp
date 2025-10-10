<template>
  <div class="space-y-6">
    <PageHeader
      title="Purchase Orders"
      description="Manage purchase orders and supplier transactions"
    />

    <div class="flex justify-between items-center">
      <div class="flex gap-4">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search orders..."
          class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white"
        />
        <select v-model="filterStatus" class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white">
          <option value="">All Status</option>
          <option value="Draft">Draft</option>
          <option value="Submitted">Submitted</option>
          <option value="Confirmed">Confirmed</option>
          <option value="Received">Received</option>
          <option value="Cancelled">Cancelled</option>
        </select>
      </div>
      <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        New Purchase Order
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Orders</p>
            <p class="text-2xl font-bold dark:text-white">{{ orders.length }}</p>
          </div>
          <Icon name="mdi:file-document-outline" class="w-10 h-10 text-blue-500" />
        </div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Pending</p>
            <p class="text-2xl font-bold text-yellow-600">{{ orders.filter(o => o.status === 'Submitted').length }}</p>
          </div>
          <Icon name="mdi:clock-outline" class="w-10 h-10 text-yellow-500" />
        </div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Value</p>
            <p class="text-2xl font-bold text-green-600">{{ formatCurrency(totalValue) }}</p>
          </div>
          <Icon name="mdi:currency-usd" class="w-10 h-10 text-green-500" />
        </div>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">This Month</p>
            <p class="text-2xl font-bold text-purple-600">{{ formatCurrency(monthValue) }}</p>
          </div>
          <Icon name="mdi:calendar-month" class="w-10 h-10 text-purple-500" />
        </div>
      </div>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">PO Number</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Supplier</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Items</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Amount</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="order in filteredOrders" :key="order.id" class="dark:text-white hover:bg-gray-50 dark:hover:bg-gray-700">
            <td class="px-6 py-4 font-mono text-sm">{{ order.poNumber }}</td>
            <td class="px-6 py-4">
              <div>
                <p class="font-medium">{{ order.supplier }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ order.supplierContact }}</p>
              </div>
            </td>
            <td class="px-6 py-4 text-sm">{{ formatDate(order.date) }}</td>
            <td class="px-6 py-4 text-sm">{{ order.itemCount }} items</td>
            <td class="px-6 py-4 text-right font-medium">{{ formatCurrency(order.amount) }}</td>
            <td class="px-6 py-4">
              <span :class="getStatusClass(order.status)" class="px-2 py-1 text-xs rounded-full">
                {{ order.status }}
              </span>
            </td>
            <td class="px-6 py-4 text-right">
              <button class="text-blue-600 hover:text-blue-800 mr-3">View</button>
              <button class="text-green-600 hover:text-green-800 mr-3">Receive</button>
              <button class="text-red-600 hover:text-red-800">Cancel</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

definePageMeta({
  middleware: ['auth'],
  layout: 'default',
})

useHead({
  title: 'Purchase Orders - TOSS ERP',
})

const searchQuery = ref('')
const filterStatus = ref('')

const orders = ref([
  { id: 1, poNumber: 'PO-2024-001', supplier: 'ABC Suppliers Ltd', supplierContact: 'john@abc.com', date: '2024-01-15', itemCount: 5, amount: 45000, status: 'Received' },
  { id: 2, poNumber: 'PO-2024-002', supplier: 'XYZ Manufacturing', supplierContact: 'sarah@xyz.com', date: '2024-01-20', itemCount: 3, amount: 32500, status: 'Confirmed' },
  { id: 3, poNumber: 'PO-2024-003', supplier: 'Global Trade Co', supplierContact: 'mike@global.com', date: '2024-02-01', itemCount: 8, amount: 67800, status: 'Submitted' },
  { id: 4, poNumber: 'PO-2024-004', supplier: 'Tech Supplies Inc', supplierContact: 'info@tech.com', date: '2024-02-05', itemCount: 12, amount: 125000, status: 'Draft' },
  { id: 5, poNumber: 'PO-2024-005', supplier: 'Quality Parts Ltd', supplierContact: 'sales@quality.com', date: '2024-02-10', itemCount: 6, amount: 54300, status: 'Confirmed' },
])

const filteredOrders = computed(() => {
  let result = orders.value

  if (filterStatus.value) {
    result = result.filter(o => o.status === filterStatus.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(o => 
      o.poNumber.toLowerCase().includes(query) ||
      o.supplier.toLowerCase().includes(query)
    )
  }

  return result
})

const totalValue = computed(() => orders.value.reduce((sum, o) => sum + o.amount, 0))
const monthValue = computed(() => {
  const currentMonth = new Date().toISOString().slice(0, 7)
  return orders.value
    .filter(o => o.date.startsWith(currentMonth))
    .reduce((sum, o) => sum + o.amount, 0)
})

const getStatusClass = (status: string) => {
  const classes = {
    'Draft': 'bg-gray-100 text-gray-800',
    'Submitted': 'bg-yellow-100 text-yellow-800',
    'Confirmed': 'bg-blue-100 text-blue-800',
    'Received': 'bg-green-100 text-green-800',
    'Cancelled': 'bg-red-100 text-red-800',
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(amount)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-ZA')
}
</script>
