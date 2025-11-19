<template>
  <div class="p-6">
    <div class="mb-6">
      <NuxtLink
        to="/supplier/orders"
        class="text-green-600 hover:text-green-800 dark:text-green-400"
      >
        ← Back to Orders
      </NuxtLink>
    </div>

    <div v-if="isLoading" class="text-center py-12">
      <div class="inline-block w-8 h-8 border-4 border-green-600 border-t-transparent rounded-full animate-spin"></div>
      <p class="mt-2 text-gray-600">Loading order...</p>
    </div>

    <div v-else-if="order" class="space-y-6">
      <!-- Order Header -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">{{ order.poNumber }}</h1>
            <p class="text-gray-600 dark:text-gray-400">From: {{ order.shopName || 'Unknown Retailer' }}</p>
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

      <!-- Actions -->
      <div v-if="order.status === 'Submitted'" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h2 class="text-lg font-semibold mb-4">Actions</h2>
        <div class="flex space-x-4">
          <button
            @click="handleApprove"
            :disabled="isProcessing"
            class="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50"
          >
            Accept Order
          </button>
          <button
            @click="handleReject"
            :disabled="isProcessing"
            class="px-6 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 disabled:opacity-50"
          >
            Reject Order
          </button>
        </div>
      </div>

      <div v-else-if="order.status === 'Accepted'" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h2 class="text-lg font-semibold mb-4">Update Status</h2>
        <div class="flex space-x-4">
          <button
            @click="updateStatus('Shipped')"
            :disabled="isProcessing"
            class="px-6 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 disabled:opacity-50"
          >
            Mark as Shipped
          </button>
        </div>
      </div>
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
    router.push('/supplier/orders')
  } finally {
    isLoading.value = false
  }
}

const handleApprove = async () => {
  if (!confirm('Are you sure you want to accept this order?')) return

  isProcessing.value = true
  try {
    await purchaseOrdersAPI.approvePurchaseOrder(orderId.value)
    await loadOrder()
  } catch (error) {
    console.error('Failed to approve order:', error)
    alert('Failed to approve order. Please try again.')
  } finally {
    isProcessing.value = false
  }
}

const handleReject = async () => {
  const reason = prompt('Please provide a reason for rejection:')
  if (!reason) return

  isProcessing.value = true
  try {
    await purchaseOrdersAPI.updatePurchaseOrderStatus(orderId.value, 'Rejected', reason)
    await loadOrder()
  } catch (error) {
    console.error('Failed to reject order:', error)
    alert('Failed to reject order. Please try again.')
  } finally {
    isProcessing.value = false
  }
}

const updateStatus = async (status: string) => {
  isProcessing.value = true
  try {
    await purchaseOrdersAPI.updatePurchaseOrderStatus(orderId.value, status)
    await loadOrder()
  } catch (error) {
    console.error('Failed to update status:', error)
    alert('Failed to update status. Please try again.')
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
    'Submitted': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'Accepted': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'Shipped': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    'Delivered': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'Rejected': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

onMounted(() => {
  loadOrder()
})
</script>

