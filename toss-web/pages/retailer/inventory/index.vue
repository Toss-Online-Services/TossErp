<template>
  <div class="p-6 space-y-6">
    <!-- Page Header -->
    <div>
      <h1 class="text-3xl font-bold text-transparent bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 bg-clip-text">
        Inventory
      </h1>
      <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
        Monitor your stock levels and receive low stock alerts
      </p>
    </div>

    <!-- Low Stock Alert -->
    <MaterialCard
      v-if="lowStockItems.length > 0"
      variant="elevated"
      class="overflow-hidden"
    >
      <div class="relative p-6 bg-gradient-to-r from-yellow-50 to-orange-50 dark:from-yellow-900/20 dark:to-orange-900/20">
        <div class="flex items-start gap-4">
          <div class="flex items-center justify-center flex-shrink-0 w-12 h-12 rounded-full bg-yellow-100 dark:bg-yellow-900/50">
            <svg class="w-6 h-6 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
          </div>
          <div class="flex-1">
            <h3 class="text-lg font-semibold text-yellow-900 dark:text-yellow-200">
              Low Stock Alert
            </h3>
            <p class="mt-1 text-sm text-yellow-800 dark:text-yellow-300">
              {{ lowStockItems.length }} product(s) are running low on stock and need attention
            </p>
          </div>
          <UiBadge variant="warning" class="text-sm px-3 py-1">
            {{ lowStockItems.length }}
          </UiBadge>
        </div>
      </div>
    </MaterialCard>

    <!-- Stock Levels Table -->
    <MaterialCard variant="elevated">
      <div v-if="isLoading" class="flex flex-col items-center justify-center p-12">
        <div class="w-12 h-12 border-4 border-orange-500 border-t-transparent rounded-full animate-spin"></div>
        <p class="mt-4 text-sm font-medium text-slate-600 dark:text-slate-400">Loading inventory...</p>
      </div>

      <div v-else class="overflow-x-auto">
        <MaterialDataTable
          :columns="columns"
          :data="products"
          :sortable="true"
        >
          <template #cell-product="{ row }">
            <div class="min-w-[200px]">
              <div class="text-sm font-semibold text-slate-900 dark:text-white">{{ row.name }}</div>
              <div class="text-xs text-slate-500 dark:text-slate-400">SKU: {{ row.sku }}</div>
            </div>
          </template>
          <template #cell-currentStock="{ row }">
            <span class="text-sm font-semibold text-slate-900 dark:text-white">
              {{ row.availableStock || 0 }}
            </span>
          </template>
          <template #cell-reorderPoint="{ row }">
            <span class="text-sm text-slate-600 dark:text-slate-400">
              {{ row.minimumStockLevel || 10 }}
            </span>
          </template>
          <template #cell-status="{ row }">
            <UiBadge
              :variant="getStockStatusVariant(row.availableStock, row.minimumStockLevel)"
            >
              {{ getStockStatus(row.availableStock, row.minimumStockLevel) }}
            </UiBadge>
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

const productsAPI = useProductsAPI()
const shopId = ref(1) // TODO: Get from session
const products = ref<any[]>([])
const lowStockItems = ref<any[]>([])
const isLoading = ref(true)

const columns = [
  { key: 'product', label: 'Product', sortable: true },
  { key: 'currentStock', label: 'Current Stock', sortable: true },
  { key: 'reorderPoint', label: 'Reorder Point', sortable: true },
  { key: 'status', label: 'Status', sortable: true }
]

const loadInventory = async () => {
  isLoading.value = true
  try {
    products.value = await productsAPI.getProducts(shopId.value)
    lowStockItems.value = await productsAPI.getLowStockItems(shopId.value, 10)
  } catch (error) {
    console.error('Failed to load inventory:', error)
  } finally {
    isLoading.value = false
  }
}

const getStockStatus = (stock: number, reorderPoint: number) => {
  if (stock === 0) return 'Out of Stock'
  if (stock <= reorderPoint) return 'Low Stock'
  return 'In Stock'
}

const getStockStatusVariant = (stock: number, reorderPoint: number) => {
  if (stock === 0) return 'destructive'
  if (stock <= reorderPoint) return 'warning'
  return 'success'
}

const getStockStatusClass = (stock: number, reorderPoint: number) => {
  if (stock === 0) return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  if (stock <= reorderPoint) return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
  return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
}

onMounted(() => {
  loadInventory()
})
</script>

