<template>
  <div class="p-6">
    <MaterialCard variant="elevated" class="mb-6">
      <h1 class="text-2xl font-bold bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 bg-clip-text text-transparent mb-2">Supplier Dashboard</h1>
      <p class="text-sm text-slate-600 dark:text-slate-400">Overview of your orders and quick actions</p>
    </MaterialCard>

    <div v-if="isLoading" class="text-center py-12">
      <div class="inline-block w-8 h-8 border-4 border-green-600 border-t-transparent rounded-full animate-spin"></div>
      <p class="mt-2 text-gray-600">Loading dashboard...</p>
    </div>

    <div v-else class="space-y-6">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <MaterialStatsCard
          title="Pending Orders"
          :value="stats.pendingOrders"
          icon="clock"
          color="warning"
        />
        <MaterialStatsCard
          title="Accepted Orders"
          :value="stats.acceptedOrders"
          icon="check-circle"
          color="info"
        />
        <MaterialStatsCard
          title="Shipped Orders"
          :value="stats.shippedOrders"
          icon="truck"
          color="primary"
        />
      </div>

      <!-- Quick Actions -->
      <MaterialCard variant="elevated" class="mt-6">
        <h2 class="text-lg font-semibold mb-4">Quick Actions</h2>
        <MaterialButton
          to="/supplier/orders"
          color="success"
          class="w-full"
        >
          <template #icon>
            <Icon name="heroicons:clipboard-document-list" class="w-5 h-5 mr-2" />
          </template>
          View All Orders
        </MaterialButton>
      </MaterialCard>
    </div>
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
const isLoading = ref(true)
const stats = ref({
  pendingOrders: 0,
  acceptedOrders: 0,
  shippedOrders: 0
})

const loadStats = async () => {
  isLoading.value = true
  try {
    // TODO: Filter by supplierId when backend supports it
    const allOrders = await purchaseOrdersAPI.getPurchaseOrders({})
    
    stats.value.pendingOrders = allOrders.filter((o: any) => o.status === 'Submitted').length
    stats.value.acceptedOrders = allOrders.filter((o: any) => o.status === 'Accepted').length
    stats.value.shippedOrders = allOrders.filter((o: any) => o.status === 'Shipped').length
  } catch (error) {
    console.error('Failed to load dashboard stats:', error)
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadStats()
})
</script>

