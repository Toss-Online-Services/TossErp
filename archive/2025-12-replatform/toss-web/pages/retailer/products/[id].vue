<template>
  <div class="p-6">
    <div class="mb-6">
      <NuxtLink
        to="/retailer/products"
        class="text-blue-600 hover:text-blue-800 dark:text-blue-400"
      >
        ← Back to Products
      </NuxtLink>
    </div>

    <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-6">
      {{ isEdit ? 'Edit Product' : 'Add New Product' }}
    </h1>

    <form @submit.prevent="handleSubmit" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 max-w-2xl">
      <div class="space-y-6">
        <!-- Basic Information -->
        <div>
          <h2 class="text-lg font-semibold mb-4">Basic Information</h2>
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Product Name *
              </label>
              <input
                v-model="form.name"
                type="text"
                required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  SKU *
                </label>
                <input
                  v-model="form.sku"
                  type="text"
                  required
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Barcode
                </label>
                <input
                  v-model="form.barcode"
                  type="text"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Description
              </label>
              <textarea
                v-model="form.description"
                rows="3"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Category
              </label>
              <select
                v-model="form.categoryId"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              >
                <option :value="null">Uncategorized</option>
                <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                  {{ cat.name }}
                </option>
              </select>
            </div>
          </div>
        </div>

        <!-- Pricing -->
        <div>
          <h2 class="text-lg font-semibold mb-4">Pricing</h2>
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Base Price (R) *
              </label>
              <input
                v-model.number="form.basePrice"
                type="number"
                step="0.01"
                min="0"
                required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Cost Price (R)
              </label>
              <input
                v-model.number="form.costPrice"
                type="number"
                step="0.01"
                min="0"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
        </div>

        <!-- Inventory -->
        <div>
          <h2 class="text-lg font-semibold mb-4">Inventory</h2>
          <div class="grid grid-cols-3 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Unit
              </label>
              <input
                v-model="form.unit"
                type="text"
                placeholder="e.g., kg, pcs"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Min Stock Level
              </label>
              <input
                v-model.number="form.minimumStockLevel"
                type="number"
                min="0"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Reorder Quantity
              </label>
              <input
                v-model.number="form.reorderQuantity"
                type="number"
                min="0"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
        </div>

        <!-- Tax Settings -->
        <div>
          <label class="flex items-center">
            <input
              v-model="form.isTaxable"
              type="checkbox"
              class="mr-2"
            />
            <span class="text-sm font-medium text-gray-700 dark:text-gray-300">
              Product is taxable (15% VAT)
            </span>
          </label>
        </div>

        <!-- Actions -->
        <div class="flex justify-end space-x-4 pt-4 border-t">
          <NuxtLink
            to="/retailer/products"
            class="px-6 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
          >
            Cancel
          </NuxtLink>
          <button
            type="submit"
            :disabled="isSubmitting"
            class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
          >
            {{ isSubmitting ? 'Saving...' : (isEdit ? 'Update' : 'Create') }}
          </button>
        </div>
      </div>
    </form>
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

const route = useRoute()
const router = useRouter()
const productsAPI = useProductsAPI()

const isEdit = computed(() => route.params.id !== 'new')
const productId = computed(() => isEdit.value ? Number(route.params.id) : null)

const categories = ref<any[]>([])
const isSubmitting = ref(false)

const form = ref({
  name: '',
  sku: '',
  barcode: '',
  description: '',
  categoryId: null as number | null,
  basePrice: 0,
  costPrice: null as number | null,
  unit: '',
  minimumStockLevel: 10,
  reorderQuantity: null as number | null,
  isTaxable: true
})

const loadCategories = async () => {
  try {
    categories.value = await productsAPI.getCategories(1) // TODO: Get shopId from session
  } catch (error) {
    console.error('Failed to load categories:', error)
  }
}

const loadProduct = async () => {
  if (!productId.value) return

  try {
    const product = await productsAPI.getProductById(productId.value)
    form.value = {
      name: product.name || '',
      sku: product.sku || '',
      barcode: product.barcode || '',
      description: product.description || '',
      categoryId: product.categoryId || null,
      basePrice: product.basePrice || 0,
      costPrice: product.costPrice || null,
      unit: product.unit || '',
      minimumStockLevel: product.minimumStockLevel || 10,
      reorderQuantity: product.reorderQuantity || null,
      isTaxable: product.isTaxable ?? true
    }
  } catch (error) {
    console.error('Failed to load product:', error)
    router.push('/retailer/products')
  }
}

const handleSubmit = async () => {
  isSubmitting.value = true
  try {
    if (isEdit.value && productId.value) {
      await productsAPI.updateProduct(productId.value, form.value)
    } else {
      await productsAPI.createProduct(form.value)
    }
    router.push('/retailer/products')
  } catch (error: any) {
    console.error('Failed to save product:', error)
    alert(error.data?.message || 'Failed to save product. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

onMounted(async () => {
  await loadCategories()
  if (isEdit.value) {
    await loadProduct()
  }
})
</script>

