<template>
  <div>
    <NuxtLink to="/supplier/orders" class="inline-flex items-center text-green-600 hover:text-green-800 dark:text-green-400 mb-6">
      <span class="mr-2">&#8592;</span> Back to Orders
    </NuxtLink>
    <div v-if="isLoading" class="text-center py-12">
      <div class="inline-block w-8 h-8 border-4 border-green-600 border-t-transparent rounded-full animate-spin"></div>
      <p class="mt-2 text-gray-600">Loading order...</p>
    </div>
    <div v-else-if="order" class="space-y-6">
      <!-- Order Header -->
      <MaterialCard variant="elevated">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h1 class="text-2xl font-bold bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 bg-clip-text text-transparent mb-2">{{ order.poNumber }}</h1>
            <p class="text-sm text-slate-600 dark:text-slate-400">From: {{ order.shopName || 'Unknown Retailer' }}</p>
            <p class="text-sm text-slate-600 dark:text-slate-400">Date: {{ formatDate(order.orderDate) }}</p>
          </div>
          <UiBadge :color="getStatusColor(order.status)" class="px-4 py-2 text-sm font-semibold">{{ order.status }}</UiBadge>
        </div>
        <!-- Order Items -->
        <div class="mt-6">
          <h2 class="text-lg font-semibold mb-4">Items</h2>
          <MaterialDataTable
            :rows="order.items"
            :columns="[
              { key: 'productName', label: 'Product' },
              { key: 'productSKU', label: 'SKU' },
              { key: 'quantity', label: 'Quantity' },
              { key: 'unitPrice', label: 'Unit Price', render: (row) => `R${formatCurrency(row.unitPrice)}` },
              { key: 'lineTotal', label: 'Total', render: (row) => `R${formatCurrency(row.lineTotal)}` }
            ]"
          />
        </div>
        <div class="mt-6 pt-6 border-t flex justify-between items-center">
          <div>
            <p class="text-sm text-slate-600 dark:text-slate-400">Subtotal</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-gray-100">R{{ formatCurrency(order.totalAmount) }}</p>
          </div>
        </div>
      </MaterialCard>
      <!-- Actions -->
      <MaterialCard v-if="order.status === 'Submitted'" variant="elevated" class="mt-6">
        <h2 class="text-lg font-semibold mb-4">Actions</h2>
        <div class="flex gap-4">
          <MaterialButton @click="handleApprove" :disabled="isProcessing" color="success">
            Accept Order
          </MaterialButton>
          <MaterialButton @click="handleReject" :disabled="isProcessing" color="danger">
            Reject Order
          </MaterialButton>
        </div>
      </MaterialCard>
      <MaterialCard v-else-if="order.status === 'Accepted'" variant="elevated" class="mt-6">
        <h2 class="text-lg font-semibold mb-4">Update Status</h2>
        <div class="flex gap-4">
          <MaterialButton @click="() => updateStatus('Shipped')" :disabled="isProcessing" color="primary">
            Mark as Shipped
          </MaterialButton>
        </div>
      </MaterialCard>
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
<template>
  <div>
    <MaterialButton to="/supplier/orders" color="success" variant="text" class="mb-6">
      ← Back to Orders
    </MaterialButton>
    <div v-if="isLoading" class="text-center py-12">
      <div class="inline-block w-8 h-8 border-4 border-green-600 border-t-transparent rounded-full animate-spin"></div>
      <p class="mt-2 text-gray-600">Loading order...</p>
    </div>
    <div v-else-if="order" class="space-y-6">
      <!-- Order Header -->
      <MaterialCard variant="elevated">
        ...existing code...
      </MaterialCard>
      <!-- Actions -->
      <MaterialCard v-if="order.status === 'Submitted'" variant="elevated" class="mt-6">
        ...existing code...
      </MaterialCard>
      <MaterialCard v-else-if="order.status === 'Accepted'" variant="elevated" class="mt-6">
        ...existing code...
      </MaterialCard>
    </div>
  </div>
</template>

