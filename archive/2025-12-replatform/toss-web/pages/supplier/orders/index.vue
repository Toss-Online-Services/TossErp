<template>
  <div class="p-6">
    <MaterialCard variant="elevated" class="mb-6">
      <h1 class="text-2xl font-bold bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 bg-clip-text text-transparent mb-2">Incoming Purchase Orders</h1>
      <p class="text-sm text-slate-600 dark:text-slate-400">Manage and track all incoming orders from retailers</p>
    </MaterialCard>

    <!-- Status Filter -->
    <MaterialCard variant="outlined" class="mb-6">
      <div class="flex flex-wrap gap-2">
        <UiBadge
          v-for="status in statuses"
          :key="status.value"
          :color="selectedStatus === status.value ? 'success' : 'default'"
          @click="selectedStatus = status.value"
          class="cursor-pointer px-4 py-2 text-sm font-medium"
        >
          {{ status.label }}
        </UiBadge>
      </div>
    </MaterialCard>

    <!-- Orders Table -->
    <MaterialCard variant="elevated">
      <div v-if="isLoading" class="p-8 text-center">
        <div class="inline-block w-8 h-8 border-4 border-green-600 border-t-transparent rounded-full animate-spin"></div>
        <p class="mt-2 text-gray-600">Loading orders...</p>
      </div>

      <div v-else-if="filteredOrders.length === 0" class="p-8 text-center text-gray-500">
        No purchase orders found.
      </div>

      <MaterialDataTable
        v-else
        :rows="filteredOrders"
        :columns="[
          { key: 'poNumber', label: 'PO Number', class: 'font-medium' },
          { key: 'shopName', label: 'Retailer', class: '' },
          { key: 'orderDate', label: 'Date', class: '', render: (row) => formatDate(row.orderDate) },
          { key: 'status', label: 'Status', class: '', render: (row) => h(UiBadge, { color: getStatusColor(row.status) }, () => row.status) },
          { key: 'totalAmount', label: 'Total', class: 'font-medium', render: (row) => `R${formatCurrency(row.totalAmount)}` },
          { key: 'actions', label: '', class: 'text-right', render: (row) => h(NuxtLink, { to: `/supplier/orders/${row.id}`, class: 'text-green-600 hover:text-green-900 dark:text-green-400' }, () => 'View') }
        ]"
      />
    </MaterialCard>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'supplier',
  middleware: 'auth',
  meta: {
    roles: ['Supplier']
  }
})

const purchaseOrdersAPI = usePurchaseOrdersAPI()
const supplierId = ref(1) // TODO: Get from session
const orders = ref<any[]>([])
const isLoading = ref(true)
const selectedStatus = ref<string | null>(null)

const statuses = [
  { value: null, label: 'All' },
  { value: 'Submitted', label: 'Pending' },
  { value: 'Accepted', label: 'Accepted' },
  { value: 'Shipped', label: 'Shipped' },
  { value: 'Delivered', label: 'Delivered' }
]

const loadOrders = async () => {
  isLoading.value = true
  try {
    // TODO: Filter by supplierId when backend supports it
    orders.value = await purchaseOrdersAPI.getPurchaseOrders({
      status: selectedStatus.value || undefined
    })
  } catch (error) {
    console.error('Failed to load purchase orders:', error)
  } finally {
    isLoading.value = false
  }
}

const filteredOrders = computed(() => {
  return orders.value
})

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-ZA')
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'Submitted': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'Accepted': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'Shipped': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    'Delivered': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

watch(selectedStatus, () => {
  loadOrders()
})

onMounted(() => {
  loadOrders()
})
</script>

