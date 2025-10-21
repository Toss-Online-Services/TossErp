<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-green-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800 relative overflow-hidden">
    <!-- Transaction Type Watermark -->
    <div class="fixed top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 pointer-events-none z-0 select-none">
      <div class="text-[20rem] font-black text-green-500/5 dark:text-green-400/5 rotate-[-15deg] whitespace-nowrap">
        📦 ORDER
      </div>
    </div>

    <!-- Transaction Type Badge (Top Right) -->
    <div class="fixed top-4 right-4 z-50 pointer-events-none">
      <div class="flex items-center gap-2 px-6 py-3 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-2xl shadow-2xl border-4 border-white dark:border-slate-800">
        <span class="text-3xl">📦</span>
        <div>
          <div class="text-xs font-medium opacity-90">Transaction Type</div>
          <div class="text-lg font-black tracking-wider">ORDER</div>
        </div>
      </div>
    </div>

    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6 relative z-10">
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
            title="New Order"
            @clear-cart="clearCart"
            @update-quantity="updateQuantity"
            @remove-item="removeFromCart"
          >
            <template #customer-section>
              <!-- Customer Info Prompt/Display -->
              <div class="mb-4">
                <div v-if="!customer.name" 
                  class="p-3 bg-gradient-to-r from-yellow-50 to-orange-50 border-2 border-yellow-300 rounded-xl cursor-pointer hover:shadow-md transition-all"
                  @click="showCustomerModal = true"
                >
                  <div class="flex items-center gap-2">
                    <svg class="w-5 h-5 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                    </svg>
                    <span class="text-sm font-semibold text-yellow-800">Click to add customer info</span>
                  </div>
                </div>
                <div v-else 
                  class="p-3 bg-gradient-to-r from-green-50 to-blue-50 border-2 border-green-300 rounded-xl"
                >
                  <div class="flex items-center justify-between">
                    <div>
                      <p class="text-sm font-bold text-green-900">{{ customer.name }}</p>
                      <p v-if="customer.phone" class="text-xs text-green-700">{{ customer.phone }}</p>
                    </div>
                    <button 
                      @click="showCustomerModal = true"
                      class="text-xs text-blue-600 hover:text-blue-800 font-medium"
                    >
                      Edit
                    </button>
                  </div>
                </div>
              </div>
            </template>

            <template #action-button>
              <!-- Create Order Button -->
              <button 
                @click="createOrder"
                :disabled="cartItems.length === 0"
                class="w-full py-3 rounded-xl font-bold shadow-lg hover:shadow-xl transition-all duration-200 text-white disabled:opacity-50 disabled:cursor-not-allowed bg-gradient-to-r from-green-600 to-blue-600 hover:from-green-700 hover:to-blue-700"
              >
                📦 Create Order
              </button>
            </template>
          </CartDisplay>

          <!-- Quick Actions -->
          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Quick Actions</h3>
            <div class="space-y-3">
              <button 
                @click="showCustomerModal = true"
                class="w-full py-2.5 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
              >
                👤 Add/Edit Customer
              </button>
              <button 
                @click="clearCart"
                :disabled="cartItems.length === 0"
                class="w-full py-2.5 bg-gradient-to-r from-red-600 to-pink-600 hover:from-red-700 hover:to-pink-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                ❌ Clear Order
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
import { ref, computed } from 'vue'
import ProductGrid from '~/components/sales/ProductGrid.vue'
import CartDisplay from '~/components/sales/CartDisplay.vue'
import CustomerInfoModal from '~/components/sales/CustomerInfoModal.vue'
import OrderQueue from '~/components/sales/OrderQueue.vue'
import BarcodeScanner from '~/components/pos/BarcodeScanner.vue'

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

// State
const searchQuery = ref('')
const selectedCategory = ref('all')
const showBarcodeScanner = ref(false)
const showCustomerModal = ref(false)
const showOrderQueue = ref(false)
const cartItems = ref<any[]>([])
const pendingOrders = ref<any[]>([])
let orderCounter = 1

// Customer info
const customer = ref({
  name: '',
  phone: '',
  notes: ''
})

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

// Sample products
const products = ref([
  { id: 1, name: 'Coca Cola 2L', sku: 'CC2L001', price: 35, stock: 24, category: 'beverages', image: null },
  { id: 2, name: 'White Bread 700g', sku: 'WB700', price: 18, stock: 14, category: 'groceries', image: null },
  { id: 3, name: 'Milk 1L', sku: 'MLK1L', price: 22, stock: 11, category: 'groceries', image: null },
  { id: 4, name: 'Simba Chips 125g', sku: 'SC125', price: 12, stock: 30, category: 'snacks', image: null },
  { id: 5, name: 'Sunlight Soap 250g', sku: 'SS250', price: 15, stock: 8, category: 'household', image: null },
  { id: 6, name: 'Maggi 2-Minute Noodles', sku: 'MGN2M', price: 8, stock: 45, category: 'groceries', image: null },
  { id: 7, name: 'Castle Lager 440ml', sku: 'CL440', price: 25, stock: 0, category: 'beverages', image: null },
  { id: 8, name: 'Purity Baby Food', sku: 'PBF001', price: 45, stock: 12, category: 'groceries', image: null }
])

// Computed
const cartTotal = computed(() => {
  return cartItems.value.reduce((total: number, item: any) => total + (item.price * item.quantity), 0)
})

// Methods
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

const createOrder = () => {
  if (cartItems.value.length === 0) return
  
  if (!customer.value.name.trim()) {
    showCustomerModal.value = true
    showNotification('Please add customer information', 'error')
    return
  }

  const order = {
    id: Date.now().toString(),
    orderNumber: String(orderCounter++).padStart(3, '0'),
    customerName: customer.value.name,
    customerPhone: customer.value.phone,
    items: [...cartItems.value],
    total: cartTotal.value,
    notes: customer.value.notes,
    status: 'pending',
    createdAt: new Date()
  }

  pendingOrders.value.unshift(order)
  
  // Clear form
  clearCart()
  customer.value = { name: '', phone: '', notes: '' }
  
  showNotification(`✓ Order #${order.orderNumber} created for ${order.customerName}`)
  showOrderQueue.value = true
}

const updateOrderStatus = (orderId: string, newStatus: string) => {
  const order = pendingOrders.value.find((o: any) => o.id === orderId)
  if (order) {
    order.status = newStatus
    const statusText = newStatus === 'pending' ? 'Pending' : newStatus === 'in-progress' ? 'In Progress' : 'Ready'
    showNotification(`✓ Order #${order.orderNumber} marked as ${statusText}`)
  }
}

const completeOrder = (orderId: string) => {
  const order = pendingOrders.value.find((o: any) => o.id === orderId)
  if (order) {
    // Remove from pending orders
    pendingOrders.value = pendingOrders.value.filter((o: any) => o.id !== orderId)
    showNotification(`✓ Order #${order.orderNumber} completed`)
  }
}

const cancelOrder = (orderId: string) => {
  if (confirm('Cancel this order?')) {
    const order = pendingOrders.value.find((o: any) => o.id === orderId)
    if (order) {
      pendingOrders.value = pendingOrders.value.filter((o: any) => o.id !== orderId)
      showNotification(`✗ Order #${order.orderNumber} cancelled`)
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

