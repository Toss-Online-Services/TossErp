<template>
  <div class="p-6 space-y-6">
    <!-- Page Header -->
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 class="text-3xl font-bold text-transparent bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 bg-clip-text">
          Purchase Orders
        </h1>
        <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
          Track and manage orders from suppliers
        </p>
      </div>
      <NuxtLink to="/retailer/orders/new">
        <MaterialButton color="primary" size="lg">
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          New Purchase Order
        </MaterialButton>
      </NuxtLink>
    </div>

    <!-- Status Filter -->
    <MaterialCard variant="elevated" class="p-6">
      <div class="flex flex-wrap gap-2">
        <button
          v-for="status in statuses"
          :key="status.value"
          @click="selectedStatus = status.value"
          :class="[
            'px-4 py-2 rounded-full text-sm font-semibold transition-all duration-200',
            selectedStatus === status.value
              ? 'bg-orange-500 text-white shadow-lg scale-105'
              : 'bg-slate-100 dark:bg-slate-800 text-slate-700 dark:text-slate-300 hover:bg-slate-200 dark:hover:bg-slate-700 hover:scale-105'
          ]"
        >
          {{ status.label }}
        </button>
      </div>
    </MaterialCard>

    <!-- Orders Table -->
    <MaterialCard variant="elevated">
      <div v-if="isLoading" class="flex flex-col items-center justify-center p-12">
        <div class="w-12 h-12 border-4 border-orange-500 border-t-transparent rounded-full animate-spin"></div>
        <p class="mt-4 text-sm font-medium text-slate-600 dark:text-slate-400">Loading orders...</p>
      </div>

      <div v-else-if="filteredOrders.length === 0" class="flex flex-col items-center justify-center p-12 text-center">
        <div class="flex items-center justify-center w-16 h-16 mb-4 rounded-full bg-slate-100 dark:bg-slate-800">
          <svg class="w-8 h-8 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
        </div>
        <h3 class="mb-2 text-lg font-semibold text-slate-900 dark:text-white">No purchase orders found</h3>
        <p class="mb-4 text-sm text-slate-600 dark:text-slate-400">
          Create your first purchase order to start ordering from suppliers
        </p>
        <NuxtLink to="/retailer/orders/new">
          <MaterialButton color="primary">
            Create Purchase Order
          </MaterialButton>
        </NuxtLink>
      </div>

      <div v-else class="overflow-x-auto">
        <MaterialDataTable
          :columns="columns"
          :data="filteredOrders"
          :sortable="true"
        >
          <template #cell-po="{ row }">
            <div class="min-w-[120px]">
              <div class="text-sm font-semibold text-slate-900 dark:text-white">{{ row.poNumber }}</div>
              <div class="text-xs text-slate-500 dark:text-slate-400">{{ formatDate(row.orderDate) }}</div>
            </div>
          </template>
          <template #cell-supplier="{ row }">
            <span class="text-sm text-slate-700 dark:text-slate-300">
              {{ row.supplierName || 'Unknown' }}
            </span>
          </template>
          <template #cell-status="{ row }">
            <UiBadge
              :variant="getStatusVariant(row.status)"
              class="font-medium"
            >
              <div class="flex items-center gap-1.5">
                <span class="w-1.5 h-1.5 rounded-full" :class="getStatusDotClass(row.status)"></span>
                {{ row.status }}
              </div>
            </UiBadge>
          </template>
          <template #cell-total="{ row }">
            <span class="text-sm font-semibold text-slate-900 dark:text-white">
              R{{ formatCurrency(row.totalAmount) }}
            </span>
          </template>
          <template #cell-actions="{ row }">
            <div class="flex items-center justify-end">
              <NuxtLink :to="`/retailer/orders/${row.id}`">
                <MaterialButton variant="text" size="sm" color="primary">
                  <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                  View Details
                </MaterialButton>
              </NuxtLink>
            </div>
          </template>
        </MaterialDataTable>
      </div>
    </MaterialCard>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'retailer',
  middleware: 'auth',
  meta: {
    roles: ['StoreOwner', 'Vendor']
  }
})

const purchaseOrdersAPI = usePurchaseOrdersAPI()
const shopId = ref(1) // TODO: Get from session
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

const columns = [
  { key: 'po', label: 'PO Number', sortable: true },
  { key: 'supplier', label: 'Supplier', sortable: true },
  { key: 'status', label: 'Status', sortable: true },
  { key: 'total', label: 'Total Amount', sortable: true },
  { key: 'actions', label: '', sortable: false }
]

const loadOrders = async () => {
  isLoading.value = true
  try {
    orders.value = await purchaseOrdersAPI.getPurchaseOrders({
      shopId: shopId.value,
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

const getStatusVariant = (status: string) => {
  const variants: Record<string, 'default' | 'success' | 'warning' | 'destructive' | 'secondary'> = {
    'Draft': 'secondary',
    'Submitted': 'warning',
    'Accepted': 'default',
    'Shipped': 'default',
    'Delivered': 'success'
  }
  return variants[status] || 'default'
}

const getStatusDotClass = (status: string) => {
  const classes: Record<string, string> = {
    'Draft': 'bg-slate-400',
    'Submitted': 'bg-yellow-500 animate-pulse',
    'Accepted': 'bg-blue-500',
    'Shipped': 'bg-purple-500 animate-pulse',
    'Delivered': 'bg-green-500'
  }
  return classes[status] || 'bg-slate-400'
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

