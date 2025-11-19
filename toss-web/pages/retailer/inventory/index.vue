<template>
  <div class="p-6">
    <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-6">Inventory</h1>

    <!-- Low Stock Alert -->
    <div v-if="lowStockItems.length > 0" class="mb-6 p-4 bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-lg">
      <div class="flex items-center">
        <svg class="w-5 h-5 text-yellow-600 dark:text-yellow-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
        </svg>
        <p class="font-semibold text-yellow-800 dark:text-yellow-200">
          {{ lowStockItems.length }} product(s) are running low on stock
        </p>
      </div>
    </div>

    <!-- Stock Levels Table -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
      <div v-if="isLoading" class="p-8 text-center">
        <div class="inline-block w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
        <p class="mt-2 text-gray-600">Loading inventory...</p>
      </div>

      <table v-else class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Product</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">SKU</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Current Stock</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Reorder Point</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
          </tr>
        </thead>
        <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="product in products" :key="product.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-gray-100">
              {{ product.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
              {{ product.sku }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-100">
              {{ product.availableStock || 0 }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
              {{ product.minimumStockLevel || 10 }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span
                :class="[
                  'px-2 py-1 text-xs font-semibold rounded-full',
                  getStockStatusClass(product.availableStock, product.minimumStockLevel)
                ]"
              >
                {{ getStockStatus(product.availableStock, product.minimumStockLevel) }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
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

const productsAPI = useProductsAPI()
const shopId = ref(1) // TODO: Get from session
const products = ref<any[]>([])
const lowStockItems = ref<any[]>([])
const isLoading = ref(true)

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

const getStockStatusClass = (stock: number, reorderPoint: number) => {
  if (stock === 0) return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  if (stock <= reorderPoint) return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
  return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
}

onMounted(() => {
  loadInventory()
})
</script>

