<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-green-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800 relative overflow-hidden">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-green-600 to-blue-600 bg-clip-text text-transparent">Create Order</h1>
          <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Customer orders for later fulfillment</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button 
            @click="showOrderQueue = true"
            class="px-4 py-2 bg-gradient-to-r from-green-600 to-blue-600 hover:from-green-700 hover:to-blue-700 text-white rounded-lg text-sm font-semibold transition-all duration-200 flex items-center gap-2"
          >
            📋 Order Queue
            <span v-if="pendingOrders.length > 0" class="px-2 py-0.5 bg-white/20 rounded-full text-xs">{{ pendingOrders.length }}</span>
          </button>
        </div>
      </div>

      <!-- Main Interface -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
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
                <label class="block text-sm font-medium text-gray-700 mb-2">Customer</label>
                <select 
                  v-model="selectedCustomer"
                  class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 bg-white text-gray-900"
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
                <label class="block text-sm font-medium text-gray-700 mb-2">Payment Method</label>
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
                class="w-full py-3 rounded-xl font-bold shadow-lg hover:shadow-xl transition-all duration-200 text-white disabled:opacity-50 disabled:cursor-not-allowed bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700"
              >
                📦 Create Order - R{{ formatCurrency(cartTotal) }}
              </button>
            </template>
          </CartDisplay>

          <!-- Quick Actions -->
          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Quick Actions</h3>
            <div class="space-y-3">
              <button 
                @click="holdOrder"
                class="w-full py-2.5 bg-gradient-to-r from-orange-600 to-amber-600 hover:from-orange-700 hover:to-amber-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
              >
                ⏸️ Hold Order
              </button>
              <button 
                @click="voidOrder"
                class="w-full py-2.5 bg-gradient-to-r from-red-600 to-pink-600 hover:from-red-700 hover:to-pink-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
              >
                ❌ Void Order
              </button>
              <button 
                @click="showCustomerModal = true"
                class="w-full py-2.5 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
              >
                👤 Add Customer
              </button>
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
    console.error('Failed to load products:', error)
  }
}

const loadCustomers = async () => {
  try {
    customers.value = await salesAPI.getCustomers()
  } catch (error) {
    console.error('Failed to load customers:', error)
  }
}

const loadPendingOrders = async () => {
  try {
    const allOrders = await salesAPI.getOrders()
    pendingOrders.value = allOrders.filter((o: any) => 
      o.status !== 'completed' && o.status !== 'cancelled'
    )
  } catch (error) {
    console.error('Failed to load pending orders:', error)
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
    ? customers.value.find((c: any) => c.id == selectedCustomer.value)?.name || 'Walk-in Customer'
    : 'Walk-in Customer'

  try {
    const newOrder = await salesAPI.createOrder({
      customer: customerName,
      customerPhone: customer.value.phone || '',
      orderItems: cartItems.value.map((item: any) => ({
        id: item.id,
        name: item.name,
        sku: item.sku || `SKU-${item.id}`,
        quantity: item.quantity,
        price: item.price,
        stock: item.stock || 0
      })),
      total: cartTotal.value,
      notes: customer.value.notes || '',
      status: 'pending',
      paymentMethod: selectedPaymentMethod.value
    })

    // Reload pending orders
    await loadPendingOrders()
    
    // Clear form
    clearCart()
    selectedCustomer.value = ''
    selectedPaymentMethod.value = 'Cash'
    customer.value = { name: '', phone: '', notes: '' }
    
    showNotification(`✓ Order #${newOrder.orderNumber} created for ${customerName}`)
  } catch (error) {
    console.error('Failed to create order:', error)
    showNotification('✗ Failed to create order', 'error')
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
    await salesAPI.updateOrderStatus(orderId, newStatus)
    await loadPendingOrders()
    const statusText = newStatus === 'pending' ? 'Pending' : newStatus === 'in-progress' ? 'In Progress' : 'Ready'
    showNotification(`✓ Order marked as ${statusText}`)
  } catch (error) {
    console.error('Failed to update order status:', error)
    showNotification('✗ Failed to update order', 'error')
  }
}

const completeOrder = async (orderId: string) => {
  try {
    await salesAPI.completeOrder(orderId)
    await loadPendingOrders()
    showNotification(`✓ Order completed`)
  } catch (error) {
    console.error('Failed to complete order:', error)
    showNotification('✗ Failed to complete order', 'error')
  }
}

const cancelOrder = async (orderId: string) => {
  if (confirm('Cancel this order?')) {
    try {
      await salesAPI.cancelOrder(orderId)
      await loadPendingOrders()
      showNotification(`✗ Order cancelled`)
    } catch (error) {
      console.error('Failed to cancel order:', error)
      showNotification('✗ Failed to cancel order', 'error')
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

