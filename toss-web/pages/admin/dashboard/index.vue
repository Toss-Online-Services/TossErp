<template>
  <div class="p-6">
    <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-6">Admin Dashboard</h1>

    <div v-if="isLoading" class="text-center py-12">
      <div class="inline-block w-8 h-8 border-4 border-red-600 border-t-transparent rounded-full animate-spin"></div>
      <p class="mt-2 text-gray-600">Loading dashboard...</p>
    </div>

    <div v-else class="space-y-6">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <MaterialStatsCard
          title="Active Retailers"
          :value="stats.activeRetailers"
          icon="heroicons:shop"
          color="primary"
        />
        <MaterialStatsCard
          title="Active Suppliers"
          :value="stats.activeSuppliers"
          icon="heroicons:truck"
          color="success"
        />
        <MaterialStatsCard
          title="Active Drivers"
          :value="stats.activeDrivers"
          icon="heroicons:user-group"
          color="purple"
        />
        <MaterialStatsCard
          title="Total Sales Today"
          :value="`R${formatCurrency(stats.totalSalesToday)}`"
          icon="heroicons:currency-rand"
          color="warning"
        />
      </div>

      <!-- Orders by Status -->
      <MaterialCard class="mt-6">
        <template #title>
          Orders by Status
        </template>
        <div class="grid grid-cols-2 md:grid-cols-5 gap-4">
          <div v-for="(count, status) in stats.ordersByStatus" :key="status" class="text-center p-4">
            <p class="text-2xl font-bold text-gray-900 dark:text-gray-100">{{ count }}</p>
            <UiBadge :color="getStatusColor(status)" class="mt-1">{{ status }}</UiBadge>
          </div>
        </div>
      </MaterialCard>
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

const { get } = useApi()
const isLoading = ref(true)
const stats = ref({
  activeRetailers: 0,
  activeSuppliers: 0,
  activeDrivers: 0,
  totalSalesToday: 0,
  ordersByStatus: {
    'Draft': 0,
    'Submitted': 0,
    'Accepted': 0,
    'Shipped': 0,
    'Delivered': 0
  }
})

const loadStats = async () => {
  isLoading.value = true
  try {
    // TODO: Replace with actual API calls when backend endpoints are ready
    // For now, using mock data
    const users = await get('/api/users/list?take=1000')
    const retailers = users.filter((u: any) => u.roles?.includes('StoreOwner') || u.roles?.includes('Vendor'))
    const suppliers = users.filter((u: any) => u.roles?.includes('Supplier'))
    const drivers = users.filter((u: any) => u.roles?.includes('Driver'))

    stats.value = {
      activeRetailers: retailers.length,
      activeSuppliers: suppliers.length,
      activeDrivers: drivers.length,
      totalSalesToday: 0, // TODO: Get from sales API
      ordersByStatus: {
        'Draft': 0,
        'Submitted': 0,
        'Accepted': 0,
        'Shipped': 0,
        'Delivered': 0
      }
    }

    // TODO: Load orders by status from purchase orders API
  } catch (error) {
    console.error('Failed to load dashboard stats:', error)
  } finally {
    isLoading.value = false
  }
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

onMounted(() => {
  loadStats()
})
</script>

