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

    <div v-if="isLoading" class="text-center py-12">
      <div class="inline-block w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
      <p class="mt-2 text-gray-600">Loading order...</p>
    </div>

    <div v-else-if="order" class="space-y-6">
      <!-- Order Header -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">{{ order.poNumber }}</h1>
            <p class="text-gray-600 dark:text-gray-400">To: {{ order.supplierName || 'Unknown Supplier' }}</p>
            <p class="text-gray-600 dark:text-gray-400">Date: {{ formatDate(order.orderDate) }}</p>
          </div>
          <span
            :class="[
              'px-4 py-2 text-sm font-semibold rounded-full',
              getStatusClass(order.status)
            ]"
          >
            {{ order.status }}
          </span>
        </div>

        <!-- Order Items -->
        <div class="mt-6">
          <h2 class="text-lg font-semibold mb-4">Items</h2>
          <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Product</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">SKU</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Quantity</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Unit Price</th>
                <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Total</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="item in order.items" :key="item.id">
                <td class="px-4 py-3 text-sm text-gray-900 dark:text-gray-100">{{ item.productName }}</td>
                <td class="px-4 py-3 text-sm text-gray-500 dark:text-gray-400">{{ item.productSKU }}</td>
                <td class="px-4 py-3 text-sm text-gray-500 dark:text-gray-400">{{ item.quantity }}</td>
                <td class="px-4 py-3 text-sm text-gray-500 dark:text-gray-400">R{{ formatCurrency(item.unitPrice) }}</td>
                <td class="px-4 py-3 text-sm font-medium text-gray-900 dark:text-gray-100">R{{ formatCurrency(item.lineTotal) }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="mt-6 pt-6 border-t flex justify-between items-center">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Subtotal</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-gray-100">R{{ formatCurrency(order.totalAmount) }}</p>
          </div>
        </div>
      </div>

      <!-- Actions for Draft orders -->
      <div v-if="order.status === 'Draft'" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h2 class="text-lg font-semibold mb-4">Actions</h2>
        <button
          @click="submitOrder"
          :disabled="isProcessing"
          class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
        >
          Submit Order
        </button>
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

const route = useRoute()
const router = useRouter()
const purchaseOrdersAPI = usePurchaseOrdersAPI()

const orderId = computed(() => Number(route.params.id))
const order = ref<any>(null)
const isLoading = ref(true)
const isProcessing = ref(false)

const loadOrder = async () => {
  isLoading.value = true
  try {
    order.value = await purchaseOrdersAPI.getPurchaseOrderById(orderId.value)
  } catch (error) {
    console.error('Failed to load order:', error)
    router.push('/retailer/orders')
  } finally {
    isLoading.value = false
  }
}

const submitOrder = async () => {
  if (!confirm('Submit this purchase order to the supplier?')) return

  isProcessing.value = true
  try {
    await purchaseOrdersAPI.updatePurchaseOrderStatus(orderId.value, 'Submitted')
    await loadOrder()
  } catch (error) {
    console.error('Failed to submit order:', error)
    alert('Failed to submit order. Please try again.')
  } finally {
    isProcessing.value = false
  }
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-ZA')
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

onMounted(() => {
  loadOrder()
})
</script>

