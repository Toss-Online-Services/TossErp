<template>
  <div class="p-6">
    <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-6">All Orders</h1>

    <!-- Status Filter -->
    <div class="mb-6">
      <div class="flex space-x-2">
        <MaterialButton
          v-for="status in statuses"
          :key="status.value"
          :color="selectedStatus === status.value ? 'primary' : 'default'"
          @click="selectedStatus = status.value"
        >
          {{ status.label }}
        </MaterialButton>
      </div>
    </div>

    <!-- Orders Table -->
    <MaterialDataTable
      :loading="isLoading"
      :rows="filteredOrders"
      :columns="[
        { key: 'poNumber', label: 'PO Number', render: (row) => row.poNumber },
        { key: 'shopName', label: 'Retailer', render: (row) => row.shopName || 'Unknown' },
        { key: 'supplierName', label: 'Supplier', render: (row) => row.supplierName || 'Unknown' },
        { key: 'orderDate', label: 'Date', render: (row) => formatDate(row.orderDate) },
        { key: 'status', label: 'Status', render: (row) => h(UiBadge, { color: getStatusColor(row.status), class: 'text-xs font-semibold' }, () => row.status) },
        { key: 'totalAmount', label: 'Total', render: (row) => `R${formatCurrency(row.totalAmount)}` },
        { key: 'actions', label: 'Actions', align: 'right', render: (row) => h(NuxtLink, { to: `/admin/orders/${row.id}`, class: 'text-primary' }, () => 'View') }
      ]"
      class="bg-white dark:bg-gray-800 rounded-lg shadow"
      empty-message="No orders found."
    />
  <script setup lang="ts">
  // eslint-disable-next-line @typescript-eslint/no-undef, no-undef
  const getStatusColor = (status: string) => {
    const colors: Record<string, string> = {
      'Draft': 'default',
      'Submitted': 'warning',
      'Accepted': 'primary',
      'Shipped': 'purple',
      'Delivered': 'success'
    }
    return colors[status] || 'default'
  }
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'auth',
  meta: {
    roles: ['Administrator']
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
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-ZA')
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'Draft': 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200',
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

