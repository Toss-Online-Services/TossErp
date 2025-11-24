<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">All Orders</h1>
      <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">View and manage purchase orders across the platform</p>
    </div>

    <!-- Status Filter -->
    <MaterialCard variant="elevated" class="p-4">
      <div class="flex flex-wrap gap-2">
        <button
          v-for="status in statuses"
          :key="status.value || 'all'"
          @click="selectedStatus = status.value"
          :class="[
            'px-4 py-2 text-sm font-medium rounded-lg transition-colors',
            selectedStatus === status.value
              ? 'bg-indigo-600 text-white shadow-lg'
              : 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'
          ]"
        >
          {{ status.label }}
        </button>
      </div>
    </MaterialCard>

    <!-- Orders Table -->
    <MaterialCard variant="elevated" class="overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 dark:bg-gray-700/50">
            <tr>
              <th class="text-left py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">PO Number</th>
              <th class="text-left py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Retailer</th>
              <th class="text-left py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Supplier</th>
              <th class="text-left py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Date</th>
              <th class="text-left py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Status</th>
              <th class="text-right py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Total</th>
              <th class="text-right py-4 px-6 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-if="isLoading">
              <td colspan="7" class="py-12 text-center">
                <div class="flex items-center justify-center">
                  <div class="w-8 h-8 border-4 border-indigo-500 border-t-transparent rounded-full animate-spin" />
                  <span class="ml-3 text-sm text-gray-600 dark:text-gray-400">Loading orders...</span>
                </div>
              </td>
            </tr>
            <tr
              v-else-if="filteredOrders.length === 0"
              class="hover:bg-gray-50 dark:hover:bg-gray-700/50"
            >
              <td colspan="7" class="py-12 text-center text-gray-500 dark:text-gray-400">
                No orders found
              </td>
            </tr>
            <tr
              v-for="order in filteredOrders"
              :key="order.id"
              class="hover:bg-gray-50 dark:hover:bg-gray-700/50 transition-colors"
            >
              <td class="py-4 px-6">
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ order.poNumber || order.id }}</span>
              </td>
              <td class="py-4 px-6 text-sm text-gray-900 dark:text-white">{{ order.shopName || 'Unknown' }}</td>
              <td class="py-4 px-6 text-sm text-gray-900 dark:text-white">{{ order.supplierName || 'Unknown' }}</td>
              <td class="py-4 px-6 text-sm text-gray-900 dark:text-white">{{ formatDate(order.orderDate) }}</td>
              <td class="py-4 px-6">
                <span
                  :class="[
                    'px-2 py-1 text-xs font-semibold rounded-full',
                    getStatusClass(order.status)
                  ]"
                >
                  {{ order.status }}
                </span>
              </td>
              <td class="py-4 px-6 text-right text-sm font-medium text-gray-900 dark:text-white">
                {{ formatCurrency(order.totalAmount) }}
              </td>
              <td class="py-4 px-6 text-right">
                <NuxtLink
                  :to="`/admin/orders/${order.id}`"
                  class="text-sm text-indigo-600 dark:text-indigo-400 hover:text-indigo-700 dark:hover:text-indigo-300 font-medium"
                >
                  View →
                </NuxtLink>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </MaterialCard>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default',
  middleware: 'auth',
  meta: {
    roles: ['Administrator'],
    role: 'admin',
    title: 'Orders',
    subtitle: 'Monitor and manage purchase orders'
  }
})

const purchaseOrdersAPI = usePurchaseOrdersAPI()
const orders = ref<any[]>([])
const isLoading = ref(true)
const selectedStatus = ref<string | null>(null)

const statuses = [
  { value: null, label: 'All' },
  { value: 'Draft', label: 'Draft' },
  { value: 'Submitted', label: 'Submitted' },
  { value: 'Accepted', label: 'Accepted' },
  { value: 'Shipped', label: 'Shipped' },
  { value: 'Delivered', label: 'Delivered' }
]

const loadOrders = async () => {
  isLoading.value = true
  try {
    orders.value = await purchaseOrdersAPI.getPurchaseOrders({
      status: selectedStatus.value || undefined
    })
  } catch (error) {
    console.error('Failed to load orders:', error)
  } finally {
    isLoading.value = false
  }
}

const filteredOrders = computed(() => {
  return orders.value
})

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'Draft': 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200',
    'Submitted': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'Accepted': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'Shipped': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    'Delivered': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  }
  return classes[status] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200'
}

watch(selectedStatus, () => {
  loadOrders()
})

onMounted(() => {
  loadOrders()
})
</script>
