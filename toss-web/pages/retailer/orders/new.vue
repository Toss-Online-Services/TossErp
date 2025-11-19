<template>
  <div class="p-6">
    <div class="mb-6">
      <NuxtLink
        to="/retailer/orders"
        class="text-blue-600 hover:text-blue-800 dark:text-blue-400"
      >
        ← Back to Purchase Orders
      </NuxtLink>
    </div>

    <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-6">
      Create Purchase Order
    </h1>

    <form @submit.prevent="handleSubmit" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 max-w-4xl">
      <div class="space-y-6">
        <!-- Supplier Selection -->
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Supplier *
          </label>
          <select
            v-model="form.supplierId"
            required
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          >
            <option value="">Select Supplier</option>
            <option v-for="supplier in suppliers" :key="supplier.id" :value="supplier.id">
              {{ supplier.name }}
            </option>
          </select>
        </div>

        <!-- Products Selection -->
        <div>
          <h2 class="text-lg font-semibold mb-4">Products</h2>
          <div class="space-y-4">
            <div v-for="(item, index) in form.items" :key="index" class="flex gap-4 items-end">
              <div class="flex-1">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Product
                </label>
                <select
                  v-model="item.productId"
                  @change="updateProductPrice(index)"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Select Product</option>
                  <option v-for="product in products" :key="product.id" :value="product.id">
                    {{ product.name }} (Stock: {{ product.availableStock }})
                  </option>
                </select>
              </div>
              <div class="w-32">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Quantity
                </label>
                <input
                  v-model.number="item.quantity"
                  type="number"
                  min="1"
                  required
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>
              <div class="w-32">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Unit Price (R)
                </label>
                <input
                  v-model.number="item.unitPrice"
                  type="number"
                  step="0.01"
                  min="0"
                  required
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                />
              </div>
              <button
                type="button"
                @click="removeItem(index)"
                class="px-4 py-2 text-red-600 hover:text-red-800"
              >
                Remove
              </button>
            </div>
            <button
              type="button"
              @click="addItem"
              class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
            >
              + Add Product
            </button>
          </div>
        </div>

        <!-- Notes -->
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            Notes
          </label>
          <textarea
            v-model="form.notes"
            rows="3"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <!-- Total -->
        <div class="pt-4 border-t">
          <div class="flex justify-between text-lg font-semibold">
            <span>Total:</span>
            <span>R{{ formatCurrency(totalAmount) }}</span>
          </div>
        </div>

        <!-- Actions -->
        <div class="flex justify-end space-x-4 pt-4 border-t">
          <NuxtLink
            to="/retailer/orders"
            class="px-6 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
          >
            Cancel
          </NuxtLink>
          <button
            type="submit"
            :disabled="isSubmitting || form.items.length === 0"
            class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
          >
            {{ isSubmitting ? 'Creating...' : 'Create Purchase Order' }}
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

const router = useRouter()
const purchaseOrdersAPI = usePurchaseOrdersAPI()
const productsAPI = useProductsAPI()
const shopId = ref(1) // TODO: Get from session

const suppliers = ref<any[]>([
  { id: 1, name: 'Supplier 1' }, // TODO: Load from API
  { id: 2, name: 'Supplier 2' }
])
const products = ref<any[]>([])
const isSubmitting = ref(false)

const form = ref({
  supplierId: null as number | null,
  items: [
    { productId: null as number | null, quantity: 1, unitPrice: 0 }
  ],
  notes: ''
})

const totalAmount = computed(() => {
  return form.value.items.reduce((sum, item) => {
    return sum + (item.quantity * item.unitPrice)
  }, 0)
})

const loadProducts = async () => {
  try {
    products.value = await productsAPI.getProducts(shopId.value)
  } catch (error) {
    console.error('Failed to load products:', error)
  }
}

const addItem = () => {
  form.value.items.push({ productId: null, quantity: 1, unitPrice: 0 })
}

const removeItem = (index: number) => {
  form.value.items.splice(index, 1)
}

const updateProductPrice = (index: number) => {
  const item = form.value.items[index]
  if (item.productId) {
    const product = products.value.find(p => p.id === item.productId)
    if (product) {
      item.unitPrice = product.basePrice || 0
    }
  }
}

const handleSubmit = async () => {
  if (!form.value.supplierId || form.value.items.length === 0) return

  isSubmitting.value = true
  try {
    const items = form.value.items
      .filter(item => item.productId && item.quantity > 0)
      .map(item => ({
        productId: item.productId!,
        quantity: item.quantity,
        unitPrice: item.unitPrice
      }))

    await purchaseOrdersAPI.createPurchaseOrder({
      shopId: shopId.value,
      supplierId: form.value.supplierId,
      items,
      notes: form.value.notes || undefined
    })

    router.push('/retailer/orders')
  } catch (error: any) {
    console.error('Failed to create purchase order:', error)
    alert(error.data?.message || 'Failed to create purchase order. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

onMounted(() => {
  loadProducts()
})
</script>

