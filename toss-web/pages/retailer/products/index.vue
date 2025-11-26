<template>
  <div class="p-6 space-y-6">
    <!-- Page Header -->
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 class="text-3xl font-bold text-transparent bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 bg-clip-text">
          Products
        </h1>
        <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
          Manage your product catalog
        </p>
      </div>
      <NuxtLink to="/retailer/products/new">
        <MaterialButton color="primary" size="lg">
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          Add Product
        </MaterialButton>
      </NuxtLink>
    </div>

    <!-- Search and Filters -->
    <MaterialCard variant="elevated" class="p-6">
      <div class="flex flex-col gap-4 sm:flex-row">
        <div class="flex-1">
          <MaterialInput
            v-model="searchQuery"
            type="search"
            placeholder="Search products by name or SKU..."
            variant="outlined"
          >
            <template #leftIcon>
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </template>
          </MaterialInput>
        </div>
        <div class="sm:w-64">
          <UiSelect v-model="selectedCategory">
            <UiSelectTrigger>
              <UiSelectValue placeholder="All Categories" />
            </UiSelectTrigger>
            <UiSelectContent>
              <UiSelectItem value="">All Categories</UiSelectItem>
              <UiSelectItem v-for="cat in categories" :key="cat.id" :value="String(cat.id)">
                {{ cat.name }}
              </UiSelectItem>
            </UiSelectContent>
          </UiSelect>
        </div>
      </div>
    </MaterialCard>

    <!-- Products Table -->
    <MaterialCard variant="elevated">
      <div v-if="isLoading" class="flex flex-col items-center justify-center p-12">
        <div class="w-12 h-12 border-4 border-orange-500 border-t-transparent rounded-full animate-spin"></div>
        <p class="mt-4 text-sm font-medium text-slate-600 dark:text-slate-400">Loading products...</p>
      </div>

      <div v-else-if="filteredProducts.length === 0" class="flex flex-col items-center justify-center p-12 text-center">
        <div class="flex items-center justify-center w-16 h-16 mb-4 rounded-full bg-slate-100 dark:bg-slate-800">
          <svg class="w-8 h-8 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
          </svg>
        </div>
        <h3 class="mb-2 text-lg font-semibold text-slate-900 dark:text-white">No products found</h3>
        <p class="mb-4 text-sm text-slate-600 dark:text-slate-400">
          Get started by adding your first product
        </p>
        <NuxtLink to="/retailer/products/new">
          <MaterialButton color="primary">
            Add Product
          </MaterialButton>
        </NuxtLink>
      </div>

      <div v-else class="overflow-x-auto">
        <MaterialDataTable
          :columns="columns"
          :data="filteredProducts"
          :sortable="true"
        >
          <template #cell-name="{ row }">
            <div class="min-w-[200px]">
              <div class="text-sm font-semibold text-slate-900 dark:text-white">{{ row.name }}</div>
              <div v-if="row.description" class="text-xs text-slate-500 dark:text-slate-400 line-clamp-1">{{ row.description }}</div>
            </div>
          </template>
          <template #cell-stock="{ row }">
            <UiBadge
              :variant="row.availableStock > 10 ? 'success' : row.availableStock > 0 ? 'warning' : 'destructive'"
            >
              {{ row.availableStock }} units
            </UiBadge>
          </template>
          <template #cell-price="{ row }">
            <span class="font-semibold text-slate-900 dark:text-white">
              R{{ formatCurrency(row.basePrice) }}
            </span>
          </template>
          <template #cell-actions="{ row }">
            <div class="flex items-center justify-end gap-2">
              <NuxtLink :to="`/retailer/products/${row.id}`">
                <MaterialButton variant="text" size="sm" color="primary">
                  <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                  </svg>
                  Edit
                </MaterialButton>
              </NuxtLink>
              <MaterialButton @click="confirmDelete(row)" variant="text" size="sm" color="error">
                <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
                Delete
              </MaterialButton>
            </div>
          </template>
        </MaterialDataTable>
      </div>
    </MaterialCard>

    <!-- Delete Confirmation Modal -->
    <MaterialModal
      :show="showDeleteModal"
      @close="showDeleteModal = false"
      title="Delete Product"
      max-width="md"
    >
      <template #default>
        <div class="flex items-start gap-4">
          <div class="flex items-center justify-center flex-shrink-0 w-12 h-12 rounded-full bg-red-100 dark:bg-red-900/30">
            <svg class="w-6 h-6 text-red-600 dark:text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
          </div>
          <div class="flex-1">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Delete "{{ productToDelete?.name }}"?</h3>
            <p class="mt-2 text-sm text-slate-600 dark:text-slate-400">
              This action cannot be undone. The product will be permanently removed from your catalog.
            </p>
          </div>
        </div>
      </template>
      <template #footer>
        <div class="flex justify-end gap-3">
          <MaterialButton @click="showDeleteModal = false" variant="outlined">
            Cancel
          </MaterialButton>
          <MaterialButton @click="handleDelete" color="error">
            Delete Product
          </MaterialButton>
        </div>
      </template>
    </MaterialModal>
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
const categories = ref<any[]>([])
const searchQuery = ref('')
const selectedCategory = ref('')
const isLoading = ref(true)
const showDeleteModal = ref(false)
const productToDelete = ref<any>(null)

const loadProducts = async () => {
  isLoading.value = true
  try {
    products.value = await productsAPI.getProducts(shopId.value)
    categories.value = await productsAPI.getCategories(shopId.value)
  } catch (error) {
    console.error('Failed to load products:', error)
  } finally {
    isLoading.value = false
  }
}

const columns = [
  { key: 'name', label: 'Product', sortable: true },
  { key: 'sku', label: 'SKU', sortable: true },
  { key: 'categoryName', label: 'Category', sortable: true },
  { key: 'price', label: 'Price', sortable: true },
  { key: 'stock', label: 'Stock', sortable: true },
  { key: 'actions', label: '', sortable: false }
]

const filteredProducts = computed(() => {
  let filtered = products.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(p =>
      p.name?.toLowerCase().includes(query) ||
      p.sku?.toLowerCase().includes(query)
    )
  }

  if (selectedCategory.value) {
    filtered = filtered.filter(p => p.categoryId === Number(selectedCategory.value))
  }

  return filtered
})

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const confirmDelete = (product: any) => {
  productToDelete.value = product
  showDeleteModal.value = true
}

const handleDelete = async () => {
  if (!productToDelete.value) return

  try {
    await productsAPI.deleteProduct(productToDelete.value.id)
    await loadProducts()
    showDeleteModal.value = false
    productToDelete.value = null
  } catch (error) {
    console.error('Failed to delete product:', error)
    alert('Failed to delete product. Please try again.')
  }
}

onMounted(() => {
  loadProducts()
})
</script>

