<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">Point of Sale - Hardware Enabled</h1>
          <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Quick checkout with barcode scanner & card reader support</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button @click="handleOpenDrawer" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <CurrencyDollarIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Open Drawer
          </button>
          <button @click="handlePrintReceipt" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <PrinterIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Print
          </button>
        </div>
      </div>

      <!-- Hardware Status Indicators -->
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
        <div class="bg-white dark:bg-slate-800 p-3 rounded-lg border border-slate-200 dark:border-slate-700">
          <div class="flex items-center gap-2">
            <div class="w-3 h-3 rounded-full" :class="barcodeScannerConnected ? 'bg-green-500' : 'bg-red-500'"></div>
            <span class="text-sm font-medium text-slate-900 dark:text-white">Barcode Scanner</span>
          </div>
          <p class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ barcodeScannerConnected ? 'Connected' : 'Disconnected' }}</p>
        </div>
        <div class="bg-white dark:bg-slate-800 p-3 rounded-lg border border-slate-200 dark:border-slate-700">
          <div class="flex items-center gap-2">
            <div class="w-3 h-3 rounded-full" :class="cardReaderConnected ? 'bg-green-500' : 'bg-red-500'"></div>
            <span class="text-sm font-medium text-slate-900 dark:text-white">Card Reader</span>
          </div>
          <p class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ cardReaderConnected ? 'Connected' : 'Disconnected' }}</p>
        </div>
        <div class="bg-white dark:bg-slate-800 p-3 rounded-lg border border-slate-200 dark:border-slate-700">
          <div class="flex items-center gap-2">
            <div class="w-3 h-3 rounded-full" :class="receiptPrinterConnected ? 'bg-green-500' : 'bg-red-500'"></div>
            <span class="text-sm font-medium text-slate-900 dark:text-white">Receipt Printer</span>
          </div>
          <p class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ receiptPrinterConnected ? 'Connected' : 'Disconnected' }}</p>
        </div>
        <div class="bg-white dark:bg-slate-800 p-3 rounded-lg border border-slate-200 dark:border-slate-700">
          <div class="flex items-center gap-2">
            <div class="w-3 h-3 rounded-full" :class="cashDrawerConnected ? 'bg-green-500' : 'bg-red-500'"></div>
            <span class="text-sm font-medium text-slate-900 dark:text-white">Cash Drawer</span>
          </div>
          <p class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ cashDrawerConnected ? 'Connected' : 'Disconnected' }}</p>
        </div>
      </div>

      <!-- Main POS Interface -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6">
        <!-- Product Search & Grid -->
        <div class="lg:col-span-2 space-y-4">
          <!-- Search Bar -->
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <div class="flex gap-3">
              <div class="flex-1 relative">
                <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-slate-400" />
                <input 
                  v-model="searchQuery"
                  type="text"
                  placeholder="Scan barcode or search products..."
                  class="barcode-input w-full pl-10 pr-4 py-3 rounded-lg border border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500"
                  ref="searchInput"
                />
              </div>
              <button @click="requestHardware" 
                      class="px-4 py-3 bg-purple-600 hover:bg-purple-700 text-white rounded-lg transition-colors">
                <CogIcon class="w-5 h-5" />
              </button>
            </div>
            <div class="mt-3 text-xs text-slate-500 dark:text-slate-400">
              <QrCodeIcon class="w-4 h-4 inline mr-1" />
              Barcode scanner ready - scan any product barcode
            </div>
          </div>

          <!-- Product Grid -->
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Products</h3>
            <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3">
              <div 
                v-for="product in filteredProducts" 
                :key="product.id"
                @click="addToCart(product)"
                class="p-3 border border-slate-200 dark:border-slate-600 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700 cursor-pointer transition-colors"
              >
                <div class="aspect-square bg-slate-100 dark:bg-slate-600 rounded-lg mb-2 flex items-center justify-center">
                  <CubeIcon class="w-8 h-8 text-slate-400" />
                </div>
                <h4 class="font-medium text-slate-900 dark:text-white text-sm truncate">{{ product.name }}</h4>
                <p class="text-slate-600 dark:text-slate-400 text-xs">R {{ formatCurrency(product.price) }}</p>
                <p class="text-slate-500 dark:text-slate-500 text-xs">Stock: {{ product.stock }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Cart & Checkout -->
        <div class="space-y-4">
          <!-- Cart -->
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Cart ({{ cartItems.length }})</h3>
              <button @click="clearCart" class="text-red-600 hover:text-red-700 text-sm">Clear</button>
            </div>
            
            <div v-if="cartItems.length === 0" class="text-center py-8">
              <ShoppingCartIcon class="w-12 h-12 text-slate-300 dark:text-slate-600 mx-auto mb-2" />
              <p class="text-slate-500 dark:text-slate-400 text-sm">Cart is empty</p>
            </div>

            <div v-else class="space-y-3 max-h-64 overflow-y-auto">
              <div 
                v-for="item in cartItems" 
                :key="item.id"
                class="flex items-center justify-between p-3 bg-slate-50 dark:bg-slate-700 rounded-lg"
              >
                <div class="flex-1 min-w-0">
                  <h4 class="font-medium text-slate-900 dark:text-white text-sm truncate">{{ item.name }}</h4>
                  <p class="text-slate-600 dark:text-slate-400 text-xs">R {{ formatCurrency(item.price) }} each</p>
                </div>
                <div class="flex items-center gap-2">
                  <button @click="updateQuantity(item.id, item.quantity - 1)" 
                          class="w-6 h-6 bg-slate-200 dark:bg-slate-600 rounded-full flex items-center justify-center">
                    <MinusIcon class="w-3 h-3" />
                  </button>
                  <span class="text-sm font-medium w-8 text-center">{{ item.quantity }}</span>
                  <button @click="updateQuantity(item.id, item.quantity + 1)" 
                          class="w-6 h-6 bg-slate-200 dark:bg-slate-600 rounded-full flex items-center justify-center">
                    <PlusIcon class="w-3 h-3" />
                  </button>
                </div>
              </div>
            </div>

            <!-- Total -->
            <div v-if="cartItems.length > 0" class="mt-4 pt-4 border-t border-slate-200 dark:border-slate-700">
              <div class="flex justify-between items-center">
                <span class="text-lg font-semibold text-slate-900 dark:text-white">Total:</span>
                <span class="text-2xl font-bold text-blue-600">R {{ formatCurrency(cartTotal) }}</span>
              </div>
            </div>
          </div>

          <!-- Payment Methods -->
          <div v-if="cartItems.length > 0" class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Payment</h3>
            <div class="space-y-3">
              <button @click="processPayment('cash')" 
                      class="w-full p-3 bg-green-600 hover:bg-green-700 text-white rounded-lg transition-colors flex items-center justify-center gap-2">
                <BanknotesIcon class="w-5 h-5" />
                Cash Payment
              </button>
              <button @click="processPayment('card')" 
                      :disabled="!cardReaderConnected"
                      class="w-full p-3 bg-blue-600 hover:bg-blue-700 disabled:bg-slate-400 text-white rounded-lg transition-colors flex items-center justify-center gap-2">
                <CreditCardIcon class="w-5 h-5" />
                Card Payment {{ !cardReaderConnected ? '(No Reader)' : '' }}
              </button>
              <button @click="processPayment('mobile')" 
                      class="w-full p-3 bg-purple-600 hover:bg-purple-700 text-white rounded-lg transition-colors flex items-center justify-center gap-2">
                <DevicePhoneMobileIcon class="w-5 h-5" />
                Mobile Money
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Payment Processing Modal -->
    <div v-if="showPaymentModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl p-6 max-w-md w-full mx-4">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Processing Payment</h3>
        <div class="text-center">
          <div class="w-16 h-16 bg-blue-100 dark:bg-blue-900 rounded-full mx-auto mb-4 flex items-center justify-center">
            <div class="w-8 h-8 border-2 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
          </div>
          <p class="text-slate-600 dark:text-slate-400 mb-4">{{ paymentMessage }}</p>
          <button @click="cancelPayment" 
                  class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded-lg">
            Cancel
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { 
  CurrencyDollarIcon,
  PrinterIcon,
  MagnifyingGlassIcon,
  QrCodeIcon,
  CogIcon,
  ShoppingCartIcon,
  PlusIcon,
  MinusIcon,
  BanknotesIcon,
  CreditCardIcon,
  DevicePhoneMobileIcon,
  CubeIcon
} from '@heroicons/vue/24/outline'

// Page metadata
definePageMeta({
  layout: 'default',
  title: 'POS Hardware - TOSS ERP'
})

useHead({
  title: 'POS Hardware - TOSS ERP',
  meta: [{ name: 'description', content: 'Hardware-enabled POS system' }]
})

// Use POS Hardware composable
const {
  barcodeScannerConnected,
  cardReaderConnected,
  receiptPrinterConnected,
  cashDrawerConnected,
  initializeHardware,
  setupBarcodeListener,
  processCardPayment,
  printReceipt,
  openCashDrawer,
  requestBarcodeScanner,
  requestCardReader,
  requestReceiptPrinter,
  cleanup
} = usePOSHardware()

// State
const searchQuery = ref('')
const cartItems = ref<Array<{ id: string; name: string; price: number; quantity: number; barcode: string }>>([])
const showPaymentModal = ref(false)
const paymentMessage = ref('')
const searchInput = ref<HTMLInputElement>()

// Sample products
const products = ref([
  { id: '1', name: 'Coca Cola 2L', price: 35, stock: 50, barcode: '6001001234567' },
  { id: '2', name: 'White Bread 700g', price: 18, stock: 25, barcode: '6001001234568' },
  { id: '3', name: 'Milk 1L', price: 22, stock: 30, barcode: '6001001234569' },
  { id: '4', name: 'Maggi Noodles', price: 8, stock: 100, barcode: '6001001234570' },
  { id: '5', name: 'Sunlight Soap', price: 15, stock: 40, barcode: '6001001234571' },
  { id: '6', name: 'Cooking Oil 750ml', price: 45, stock: 20, barcode: '6001001234572' },
  { id: '7', name: 'Rice 5kg', price: 85, stock: 15, barcode: '6001001234573' },
  { id: '8', name: 'Sugar 2kg', price: 35, stock: 25, barcode: '6001001234574' }
])

// Computed
const filteredProducts = computed(() => {
  if (!searchQuery.value) return products.value
  return products.value.filter(p => 
    p.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    p.barcode.includes(searchQuery.value)
  )
})

const cartTotal = computed(() => {
  return cartItems.value.reduce((total, item) => total + (item.price * item.quantity), 0)
})

// Lifecycle
let cleanupBarcode: (() => void) | null = null

onMounted(async () => {
  await initializeHardware()
  
  // Setup barcode listener
  cleanupBarcode = setupBarcodeListener((barcode) => {
    handleBarcodeScanned(barcode)
  })
  
  // Focus search input
  searchInput.value?.focus()
})

onUnmounted(() => {
  if (cleanupBarcode) {
    cleanupBarcode()
  }
  cleanup()
})

// Functions
const handleBarcodeScanned = (barcode: string) => {
  const product = products.value.find(p => p.barcode === barcode)
  if (product) {
    addToCart(product)
    // Clear search
    searchQuery.value = ''
    // Show feedback
    showNotification(`Added ${product.name} to cart`)
  } else {
    showNotification(`Product not found: ${barcode}`, 'error')
  }
}

const addToCart = (product: any) => {
  const existingItem = cartItems.value.find(item => item.id === product.id)
  if (existingItem) {
    existingItem.quantity += 1
  } else {
    cartItems.value.push({
      id: product.id,
      name: product.name,
      price: product.price,
      quantity: 1,
      barcode: product.barcode
    })
  }
}

const updateQuantity = (itemId: string, newQuantity: number) => {
  if (newQuantity <= 0) {
    cartItems.value = cartItems.value.filter(item => item.id !== itemId)
  } else {
    const item = cartItems.value.find(item => item.id === itemId)
    if (item) {
      item.quantity = newQuantity
    }
  }
}

const clearCart = () => {
  if (confirm('Clear all items from cart?')) {
    cartItems.value = []
  }
}

const processPayment = async (method: string) => {
  if (cartItems.value.length === 0) return

  showPaymentModal.value = true
  paymentMessage.value = `Processing ${method} payment...`

  try {
    if (method === 'card') {
      paymentMessage.value = 'Please insert or tap your card...'
      await processCardPayment(cartTotal.value)
    } else {
      // Simulate other payment methods
      await new Promise(resolve => setTimeout(resolve, 2000))
    }

    paymentMessage.value = 'Payment successful!'
    await new Promise(resolve => setTimeout(resolve, 1000))

    // Complete transaction
    await completeTransaction(method)
  } catch (error) {
    console.error('Payment failed:', error)
    showNotification('Payment failed. Please try again.', 'error')
    showPaymentModal.value = false
  }
}

const completeTransaction = async (method: string) => {
  // Print receipt
  await handlePrintReceipt()
  
  // Clear cart
  cartItems.value = []
  
  showPaymentModal.value = false
  showNotification('Transaction completed successfully!')
}

const cancelPayment = () => {
  showPaymentModal.value = false
}

const handleOpenDrawer = async () => {
  try {
    await openCashDrawer()
    showNotification('Cash drawer opened')
  } catch (error) {
    console.error('Failed to open drawer:', error)
    showNotification('Failed to open cash drawer', 'error')
  }
}

const handlePrintReceipt = async () => {
  try {
    const receiptData = {
      storeName: "THABO'S SPAZA SHOP",
      storeAddress: '123 Main Street, Soweto',
      storePhone: '+27 11 123 4567',
      receiptNumber: `RCP-${Date.now()}`,
      date: new Date().toLocaleString('en-ZA'),
      cashier: 'Thabo',
      customer: 'Walk-in Customer',
      items: cartItems.value.map(item => ({
        name: item.name,
        quantity: item.quantity,
        price: item.price,
        total: item.price * item.quantity
      })),
      total: cartTotal.value,
      paymentMethod: 'Cash/Card/Mobile'
    }
    
    await printReceipt(receiptData)
    showNotification('Receipt printed successfully')
  } catch (error) {
    console.error('Failed to print receipt:', error)
    showNotification('Failed to print receipt', 'error')
  }
}

const requestHardware = async () => {
  try {
    await requestBarcodeScanner()
    await requestCardReader()
    await requestReceiptPrinter()
    await initializeHardware()
    showNotification('Hardware devices requested')
  } catch (error) {
    console.error('Hardware request failed:', error)
  }
}

const showNotification = (message: string, type: 'success' | 'error' = 'success') => {
  // Simple alert for now - can be replaced with toast notification
  if (type === 'error') {
    alert(`❌ ${message}`)
  } else {
    // Show brief notification
    const notification = document.createElement('div')
    notification.textContent = `✓ ${message}`
    notification.className = 'fixed top-4 right-4 bg-green-600 text-white px-4 py-2 rounded-lg shadow-lg z-50'
    document.body.appendChild(notification)
    setTimeout(() => {
      notification.remove()
    }, 2000)
  }
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}
</script>
