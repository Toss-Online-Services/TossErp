<template>
  <div class="relative min-h-screen overflow-hidden bg-gradient-to-br from-slate-50 via-green-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Mobile-First Page Container -->
    <div class="p-4 pb-20 space-y-4 sm:p-6 sm:space-y-6 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col justify-between gap-3 sm:flex-row sm:items-center sm:gap-0">
        <div>
          <h1 class="text-2xl font-bold text-transparent sm:text-3xl bg-gradient-to-r from-green-600 to-blue-600 bg-clip-text">Create Order</h1>
          <p class="mt-1 text-sm text-slate-600 dark:text-slate-400 sm:text-base">Customer orders for later fulfillment</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <Button 
            @click="showOrderQueue = true"
            class="bg-gradient-to-r from-green-600 to-blue-600 hover:from-green-700 hover:to-blue-700"
          >
            📋 Order Queue
            <Badge v-if="pendingOrders.length > 0" variant="secondary" class="ml-2 bg-white/20 text-white">{{ pendingOrders.length }}</Badge>
          </Button>
        </div>
      </div>

      <!-- Main Interface -->
      <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
        <!-- Product Selection -->
        <div class="lg:col-span-2">
          <ProductGrid
            :products="products"
            :categories="categories"
            v-model:search-query="searchQuery"
            v-model:selected-category="selectedCategory"
            @select-product="addToCart"
            @open-scanner="showBarcodeScanner = true"
          />
        </div>

        <!-- Cart and Order -->
        <div class="space-y-4">
          <CartDisplay
            :items="cartItems"
            title="Current Order"
            @clear-cart="clearCart"
            @update-quantity="updateQuantity"
            @remove-item="removeFromCart"
          >
            <template #customer-section>
              <!-- Customer Selection -->
              <div class="mb-4">
                <label class="block mb-2 text-sm font-medium text-gray-700">Customer</label>
                <select 
                  v-model="selectedCustomer"
                  class="w-full px-3 py-2 text-gray-900 bg-white border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Walk-in Customer</option>
                  <option v-for="customer in customers" :key="customer.id" :value="customer.id">
                    {{ customer.name }}
                  </option>
                </select>
              </div>
            </template>

            <template #payment-section>
              <!-- Payment Methods -->
              <div class="mb-4">
                <label class="block mb-2 text-sm font-medium text-gray-700">Payment Method</label>
                <div class="grid grid-cols-2 gap-2">
                  <button 
                    v-for="method in paymentMethods" 
                    :key="method.id"
                    @click="selectedPaymentMethod = method.id"
                    :class="[
                      'p-2 rounded-lg text-sm font-medium transition-colors',
                      selectedPaymentMethod === method.id
                        ? 'bg-blue-600 text-white'
                        : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                    ]"
                  >
                    {{ method.name }}
                  </button>
                </div>
              </div>
            </template>

            <template #action-button>
              <!-- Create Order Button -->
              <button 
                @click="createOrder"
                :disabled="cartItems.length === 0"
                class="w-full py-3 font-bold text-white transition-all duration-200 shadow-lg rounded-xl hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700"
              >
                📦 Create Order - R{{ formatCurrency(cartTotal) }}
              </button>
            </template>
          </CartDisplay>

          <!-- Quick Actions -->
          <div class="p-4 border shadow-lg bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl border-slate-200/50 dark:border-slate-700/50 sm:p-6">
            <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">Quick Actions</h3>
            <div class="space-y-3">
              <Button 
                @click="holdOrder"
                variant="default"
                class="w-full bg-gradient-to-r from-orange-600 to-amber-600 hover:from-orange-700 hover:to-amber-700"
              >
                ⏸️ Hold Order
              </Button>
              <Button 
                @click="voidOrder"
                variant="destructive"
                class="w-full bg-gradient-to-r from-red-600 to-blue-600 hover:from-red-700 hover:to-blue-700"
              >
                ❌ Void Order
              </Button>
              <Button 
                @click="showCustomerModal = true"
                variant="default"
                class="w-full bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700"
              >
                👤 Add Customer
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Customer Info Modal -->
    <CustomerInfoModal
      :show="showCustomerModal"
      :customer-info="customer"
      @close="showCustomerModal = false"
      @save="saveCustomerInfo"
    />

    <!-- Order Queue Panel -->
    <OrderQueue
      :show="showOrderQueue"
      :orders="pendingOrders"
      @close="showOrderQueue = false"
      @update-status="updateOrderStatus"
      @complete="completeOrder"
      @cancel="cancelOrder"
    />

    <!-- Barcode Scanner Component -->
    <BarcodeScanner 
      v-model="showBarcodeScanner" 
      @barcode-scanned="handleBarcodeScanned"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import ProductGrid from '~/components/sales/ProductGrid.vue'
import CartDisplay from '~/components/sales/CartDisplay.vue'
import CustomerInfoModal from '~/components/sales/CustomerInfoModal.vue'
import OrderQueue from '~/components/sales/OrderQueue.vue'
import BarcodeScanner from '~/components/pos/BarcodeScanner.vue'
import { useSalesAPI } from '~/composables/useSalesAPI'
import { getErrorNotification, logError } from '~/utils/errorHandler'
// Button and Badge are auto-imported in Nuxt 4 with shadcn-vue

// Page metadata
useHead({
  title: 'Create Order - TOSS ERP',
  meta: [
    { name: 'description', content: 'Create customer orders for later fulfillment' }
  ]
})

definePageMeta({
  layout: 'default'
})

// API
const salesAPI = useSalesAPI()

// State
const searchQuery = ref('')
const selectedCategory = ref('all')
const showBarcodeScanner = ref(false)
const showCustomerModal = ref(false)
const showOrderQueue = ref(false)
const cartItems = ref<any[]>([])
const pendingOrders = ref<any[]>([])
const selectedCustomer = ref('')
const selectedPaymentMethod = ref('Cash')
const products = ref<any[]>([])
const customers = ref<any[]>([])

// Customer info for modal
const customer = ref({
  name: '',
  phone: '',
  notes: ''
})

// Load data on mount
onMounted(async () => {
  await loadProducts()
  await loadCustomers()
  await loadPendingOrders()
})

const loadProducts = async () => {
  try {
    products.value = await salesAPI.getProducts()
  } catch (error) {
    logError(error, 'load_data', 'Failed to load products')
    showNotification(getErrorNotification(error, 'product_search'), 'error')
  }
}

const loadCustomers = async () => {
  try {
    customers.value = await salesAPI.getCustomers()
  } catch (error) {
    logError(error, 'load_data', 'Failed to load customers')
    showNotification(getErrorNotification(error, 'customer_lookup'), 'error')
  }
}

const loadPendingOrders = async () => {
  try {
    const shopId = 1 // TODO: Get from session/auth
    const queueData = await salesAPI.getQueueOrders(shopId)
    
    // Transform backend data to match frontend expectations
    pendingOrders.value = queueData.map((order: any) => ({
      id: order.id,
      orderNumber: order.saleNumber,
      customer: order.customerName || 'Walk-in Customer',
      customerPhone: order.customerPhone,
      status: order.status.toLowerCase(),
      total: order.total,
      orderItems: order.items || [],
      createdAt: new Date(order.saleDate),
      notes: order.customerNotes,
      expectedCompletion: order.expectedCompletionTime ? new Date(order.expectedCompletionTime) : null,
      queuePosition: order.queuePosition
    }))
  } catch (error) {
    logError(error, 'load_data', 'Failed to load pending orders')
    // Don't show notification for background loading
  }
}

// Payment methods
const paymentMethods = ref([
  { id: 'Cash', name: 'Cash' },
  { id: 'Card', name: 'Card' },
  { id: 'MobileMoney', name: 'Mobile Money' },
  { id: 'BankTransfer', name: 'Bank Transfer' },
  { id: 'PayLink', name: 'Pay Link' }
])

// Categories
const categories = ref([
  { id: 'all', name: 'All' },
  { id: 'groceries', name: 'Groceries' },
  { id: 'beverages', name: 'Beverages' },
  { id: 'snacks', name: 'Snacks' },
  { id: 'household', name: 'Household' },
  { id: 'personal', name: 'Personal Care' },
  { id: 'frozen', name: 'Frozen' }
])

// Computed
const cartTotal = computed(() => {
  return cartItems.value.reduce((total: number, item: any) => total + (item.price * item.quantity), 0)
})

// Methods
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const addToCart = (product: any) => {
  if (product.stock === 0) return

  const existingItem = cartItems.value.find((item: any) => item.id === product.id)
  if (existingItem) {
    existingItem.quantity += 1
  } else {
    cartItems.value.push({
      id: product.id,
      name: product.name,
      price: product.price,
      quantity: 1
    })
  }
}

const removeFromCart = (productId: number | string) => {
  cartItems.value = cartItems.value.filter((item: any) => item.id != productId)
}

const updateQuantity = (productId: number | string, newQuantity: number) => {
  if (newQuantity <= 0) {
    removeFromCart(productId)
    return
  }

  const item = cartItems.value.find((item: any) => item.id == productId)
  if (item) {
    item.quantity = newQuantity
  }
}

const clearCart = () => {
  cartItems.value = []
}

const handleBarcodeScanned = (barcode: string) => {
  const product = products.value.find((p: any) => 
    p.sku.toLowerCase() === barcode.toLowerCase() ||
    barcode.toLowerCase().includes(p.sku.toLowerCase())
  )
  
  if (product && product.stock > 0) {
    addToCart(product)
    showNotification(`✓ Added ${product.name} to cart`)
  } else {
    showNotification(`✗ Product not found: ${barcode}`, 'error')
  }
}

const saveCustomerInfo = (customerInfo: any) => {
  customer.value = { ...customerInfo }
  showCustomerModal.value = false
  showNotification('✓ Customer info saved')
}

const createOrder = async () => {
  if (cartItems.value.length === 0) return

  const customerName = selectedCustomer.value 
    ? customers.value.find((c: any) => c.id == selectedCustomer.value)?.name || customer.value.name || 'Walk-in Customer'
    : customer.value.name || 'Walk-in Customer'

  try {
    // Use createQueueOrder for queue-based orders
    await salesAPI.createQueueOrder({
      shopId: 1, // TODO: Get from session
      customerName: customerName,
      customerPhone: customer.value.phone || '',
      customerNotes: customer.value.notes || '',
      items: cartItems.value.map((item: any) => ({
        productId: item.id,
        quantity: item.quantity,
        unitPrice: item.price
      })),
      paymentType: selectedPaymentMethod.value as any,
      totalAmount: cartTotal.value,
      estimatedPreparationMinutes: 15 // Default 15 minutes
    })

    // Reload pending orders
    await loadPendingOrders()
    
    // Clear form
    clearCart()
    selectedCustomer.value = ''
    selectedPaymentMethod.value = 'Cash'
    customer.value = { name: '', phone: '', notes: '' }
    
    showNotification(`✓ Order created for ${customerName}`)
  } catch (error) {
    logError(error, 'order_creation', 'Failed to create order')
    showNotification(getErrorNotification(error, 'order_creation'), 'error')
  }
}

const holdOrder = () => {
  if (cartItems.value.length === 0) return
  
  // In production, save to held orders
  showNotification('✓ Order held successfully')
  clearCart()
}

const voidOrder = () => {
  if (cartItems.value.length === 0) return
  
  if (confirm('Are you sure you want to void this order?')) {
    clearCart()
    showNotification('✗ Order voided')
  }
}

const updateOrderStatus = async (orderId: string, newStatus: string) => {
  try {
    // Map frontend status to backend SaleStatus enum values
    const statusMap: Record<string, string> = {
      'pending': 'Pending',
      'in-progress': 'InProgress',
      'ready': 'Ready'
    }
    const backendStatus = statusMap[newStatus] || newStatus
    await salesAPI.updateQueueOrderStatus(Number(orderId), backendStatus as any)
    await loadPendingOrders()
    const statusText = newStatus === 'pending' ? 'Pending' : newStatus === 'in-progress' ? 'In Progress' : 'Ready'
    showNotification(`✓ Order marked as ${statusText}`)
  } catch (error) {
    logError(error, 'save_data', 'Failed to update order status')
    showNotification(getErrorNotification(error, 'save_data'), 'error')
  }
}

const completeOrder = async (orderId: string) => {
  try {
    await salesAPI.completeQueueOrder(Number(orderId))
    await loadPendingOrders()
    showNotification(`✓ Order completed`)
  } catch (error) {
    logError(error, 'save_data', 'Failed to complete order')
    showNotification(getErrorNotification(error, 'save_data'), 'error')
  }
}

const cancelOrder = async (orderId: string) => {
  if (confirm('Cancel this order?')) {
    try {
      await salesAPI.voidSale(Number(orderId), 'Cancelled by user')
      await loadPendingOrders()
      showNotification(`✗ Order cancelled`)
    } catch (error) {
      logError(error, 'save_data', 'Failed to cancel order')
      showNotification(getErrorNotification(error, 'save_data'), 'error')
    }
  }
}

const showNotification = (message: string, type: 'success' | 'error' = 'success') => {
  const notification = document.createElement('div')
  notification.textContent = type === 'success' ? `✓ ${message}` : `✗ ${message}`
  notification.className = `fixed top-20 right-4 ${type === 'success' ? 'bg-green-600' : 'bg-red-600'} text-white px-4 py-2 rounded-lg shadow-lg z-50 animate-fade-in`
  document.body.appendChild(notification)
  setTimeout(() => notification.remove(), 2000)
}
</script>

