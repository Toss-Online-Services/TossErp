<template>
  <div class="p-6">
    <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-6">Dashboard</h1>

    <div v-if="isLoading" class="text-center py-12">
      <div class="inline-block w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
      <p class="mt-2 text-gray-600">Loading dashboard...</p>
    </div>

    <div v-else class="space-y-6">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Today's Sales</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">R{{ formatCurrency(stats.todaySales) }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <svg class="w-6 h-6 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Products</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">{{ stats.totalProducts }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <svg class="w-6 h-6 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Low Stock Items</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">{{ stats.lowStockItems }}</p>
            </div>
            <div class="p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <svg class="w-6 h-6 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Pending Orders</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">{{ stats.pendingOrders }}</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <svg class="w-6 h-6 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h2 class="text-lg font-semibold mb-4">Quick Actions</h2>
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
          <NuxtLink
            to="/sales/pos"
            class="p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg hover:bg-blue-100 dark:hover:bg-blue-900/30 text-center"
          >
            <svg class="w-8 h-8 mx-auto mb-2 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
            <p class="text-sm font-medium text-gray-900 dark:text-gray-100">POS</p>
          </NuxtLink>
          <NuxtLink
            to="/retailer/products/new"
            class="p-4 bg-green-50 dark:bg-green-900/20 rounded-lg hover:bg-green-100 dark:hover:bg-green-900/30 text-center"
          >
            <svg class="w-8 h-8 mx-auto mb-2 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
            </svg>
            <p class="text-sm font-medium text-gray-900 dark:text-gray-100">Add Product</p>
          </NuxtLink>
          <NuxtLink
            to="/retailer/orders/new"
            class="p-4 bg-purple-50 dark:bg-purple-900/20 rounded-lg hover:bg-purple-100 dark:hover:bg-purple-900/30 text-center"
          >
            <svg class="w-8 h-8 mx-auto mb-2 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
            </svg>
            <p class="text-sm font-medium text-gray-900 dark:text-gray-100">New Order</p>
          </NuxtLink>
          <NuxtLink
            to="/retailer/inventory"
            class="p-4 bg-yellow-50 dark:bg-yellow-900/20 rounded-lg hover:bg-yellow-100 dark:hover:bg-yellow-900/30 text-center"
          >
            <svg class="w-8 h-8 mx-auto mb-2 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
            </svg>
            <p class="text-sm font-medium text-gray-900 dark:text-gray-100">Inventory</p>
          </NuxtLink>
        </div>
      </div>
    </div>
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

const salesAPI = useSalesAPI()
const productsAPI = useProductsAPI()
const purchaseOrdersAPI = usePurchaseOrdersAPI()
const shopId = ref(1) // TODO: Get from session
const isLoading = ref(true)
const stats = ref({
  todaySales: 0,
  totalProducts: 0,
  lowStockItems: 0,
  pendingOrders: 0
})

const loadStats = async () => {
  isLoading.value = true
  try {
    // Load today's sales
    const salesSummary = await salesAPI.getDailySummary(shopId.value)
    stats.value.todaySales = salesSummary?.totalSales || 0

    // Load products count
    const products = await productsAPI.getProducts(shopId.value)
    stats.value.totalProducts = products?.length || 0

    // Load low stock items
    const lowStock = await productsAPI.getLowStockItems(shopId.value, 10)
    stats.value.lowStockItems = lowStock?.length || 0

    // Load pending orders
    const orders = await purchaseOrdersAPI.getPurchaseOrders({
      shopId: shopId.value,
      status: 'Submitted'
    })
    stats.value.pendingOrders = orders?.length || 0
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

