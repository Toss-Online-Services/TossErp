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
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Retailers</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">{{ stats.activeRetailers }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <svg class="w-6 h-6 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Suppliers</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">{{ stats.activeSuppliers }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <svg class="w-6 h-6 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Drivers</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">{{ stats.activeDrivers }}</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <svg class="w-6 h-6 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Sales Today</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">R{{ formatCurrency(stats.totalSalesToday) }}</p>
            </div>
            <div class="p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <svg class="w-6 h-6 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Orders by Status -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h2 class="text-lg font-semibold mb-4">Orders by Status</h2>
        <div class="grid grid-cols-2 md:grid-cols-5 gap-4">
          <div v-for="(count, status) in stats.ordersByStatus" :key="status" class="text-center p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <p class="text-2xl font-bold text-gray-900 dark:text-gray-100">{{ count }}</p>
            <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">{{ status }}</p>
          </div>
        </div>
      </div>
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

