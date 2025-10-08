<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
    <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold text-gray-900">Point of Sale</h1>
          <p class="text-gray-600 mt-1 text-sm sm:text-base">Quick checkout system for Thabo's Spaza Shop</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button @click="openDrawer" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <CurrencyDollarIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Open Drawer
          </button>
          <button @click="showReports = true" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-gray-600 hover:bg-gray-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <ChartBarIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Reports
          </button>
        </div>
      </div>

      <!-- Hardware Status Indicators -->
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
        <div class="bg-white p-3 rounded-lg border border-gray-200">
          <div class="flex items-center gap-2">
            <div class="w-3 h-3 rounded-full" :class="hardwareStatus.barcodeScanner ? 'bg-green-500 animate-pulse' : 'bg-red-500'"></div>
            <span class="text-sm font-medium text-gray-900">Barcode Scanner</span>
          </div>
          <p class="text-xs text-gray-600 mt-1">{{ hardwareStatus.barcodeScanner ? 'Ready' : 'Not Connected' }}</p>
        </div>
        <div class="bg-white p-3 rounded-lg border border-gray-200">
          <div class="flex items-center gap-2">
            <div class="w-3 h-3 rounded-full" :class="hardwareStatus.cardReader ? 'bg-green-500 animate-pulse' : 'bg-red-500'"></div>
            <span class="text-sm font-medium text-gray-900">Card Reader</span>
          </div>
          <p class="text-xs text-gray-600 mt-1">{{ hardwareStatus.cardReader ? 'Ready' : 'Not Connected' }}</p>
        </div>
        <div class="bg-white p-3 rounded-lg border border-gray-200">
          <div class="flex items-center gap-2">
            <div class="w-3 h-3 rounded-full" :class="hardwareStatus.printer ? 'bg-green-500 animate-pulse' : 'bg-red-500'"></div>
            <span class="text-sm font-medium text-gray-900">Receipt Printer</span>
          </div>
          <p class="text-xs text-gray-600 mt-1">{{ hardwareStatus.printer ? 'Ready' : 'Not Connected' }}</p>
        </div>
        <div class="bg-white p-3 rounded-lg border border-gray-200">
          <div class="flex items-center gap-2">
            <div class="w-3 h-3 rounded-full" :class="hardwareStatus.cashDrawer ? 'bg-green-500 animate-pulse' : 'bg-red-500'"></div>
            <span class="text-sm font-medium text-gray-900">Cash Drawer</span>
          </div>
          <p class="text-xs text-gray-600 mt-1">{{ hardwareStatus.cashDrawer ? 'Ready' : 'Not Connected' }}</p>
        </div>
      </div>

      <!-- POS Stats Cards -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white p-4 sm:p-6 rounded-xl shadow-sm border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Today's Sales</p>
              <p class="text-lg sm:text-2xl font-bold text-gray-900">R {{ formatCurrency(todaySales) }}</p>
              <p class="text-xs sm:text-sm text-emerald-600">{{ todayTransactions }} transactions</p>
            </div>
            <div class="p-2 sm:p-3 bg-emerald-100 rounded-full">
              <CurrencyDollarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-emerald-600" />
            </div>
          </div>
        </div>

        <div class="bg-white p-4 sm:p-6 rounded-xl shadow-sm border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Current Sale</p>
              <p class="text-lg sm:text-2xl font-bold text-gray-900">R {{ formatCurrency(cartTotal) }}</p>
              <p class="text-xs sm:text-sm text-blue-600">{{ cartItems.length }} items</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 rounded-full">
              <ShoppingCartIcon class="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white p-4 sm:p-6 rounded-xl shadow-sm border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Avg. Sale</p>
              <p class="text-lg sm:text-2xl font-bold text-gray-900">R {{ formatCurrency(averageSale) }}</p>
              <p class="text-xs sm:text-sm text-yellow-600">Last hour</p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 rounded-full">
              <ChartBarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white p-4 sm:p-6 rounded-xl shadow-sm border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Cash Float</p>
              <p class="text-lg sm:text-2xl font-bold text-gray-900">R {{ formatCurrency(cashFloat) }}</p>
              <p class="text-xs sm:text-sm text-purple-600">In drawer</p>
            </div>
            <div class="p-2 sm:p-3 bg-purple-100 rounded-full">
              <BanknotesIcon class="w-4 h-4 sm:w-6 sm:h-6 text-purple-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Main POS Interface -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Product Search and Selection -->
        <div class="lg:col-span-2 space-y-4">
          <!-- Search and Scanner -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-4 sm:p-6">
            <div class="flex items-center space-x-3">
              <div class="flex-1 relative">
                <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-slate-400" />
                <input 
                  v-model="searchQuery"
                  type="text"
                  placeholder="Scan barcode or search products..."
                  class="barcode-input w-full pl-10 pr-4 py-3 rounded-lg border border-gray-300 bg-white text-gray-900 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  @keyup.enter="addFirstProductToCart"
                  ref="searchInput"
                />
              </div>
              <button 
                @click="showBarcodeScanner = true"
                class="p-3 rounded-lg transition-colors bg-blue-600 hover:bg-blue-700 text-white"
              >
                <QrCodeIcon class="w-6 h-6" />
              </button>
              <button 
                @click="requestHardwareAccess"
                class="p-3 rounded-lg transition-colors bg-purple-600 hover:bg-purple-700 text-white"
                title="Request Hardware Access"
              >
                <CogIcon class="w-6 h-6" />
              </button>
            </div>
          </div>

          <!-- Category Filters -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-4 sm:p-6">
            <div class="flex flex-wrap gap-2">
              <button 
                v-for="category in categories" 
                :key="category.id"
                @click="selectedCategory = category.id"
                :class="[
                  'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
                  selectedCategory === category.id
                    ? 'bg-blue-600 text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                ]"
              >
                {{ category.name }}
              </button>
            </div>
          </div>

          <!-- Products Grid -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-4 sm:p-6">
            <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-4">
              <button 
                v-for="product in filteredProducts" 
                :key="product.id"
                @click="addToCart(product)"
                :disabled="product.stock === 0"
                class="bg-gray-50 rounded-lg border border-gray-200 p-3 hover:border-blue-500 hover:shadow-lg transition-all text-left disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <div class="aspect-square bg-gray-100 rounded-lg mb-3 flex items-center justify-center overflow-hidden">
                  <img 
                    v-if="product.image" 
                    :src="product.image" 
                    :alt="product.name"
                    class="w-full h-full object-cover"
                  />
                  <CubeIcon v-else class="w-8 h-8 text-slate-400" />
                </div>
                <h3 class="font-medium text-gray-900 text-sm truncate mb-1">{{ product.name }}</h3>
                <p class="text-xs text-gray-500 truncate mb-2">{{ product.sku }}</p>
                <div class="flex items-center justify-between">
                  <span class="text-sm font-bold text-blue-600">R{{ product.price.toFixed(2) }}</span>
                  <span 
                    :class="[
                      'text-xs px-2 py-1 rounded-full',
                      product.stock > 10 
                        ? 'bg-green-100 text-green-700' 
                        : product.stock > 0
                          ? 'bg-yellow-100 text-yellow-700'
                          : 'bg-red-100 text-red-700'
                    ]"
                  >
                    Stock: {{ product.stock }}
                  </span>
                </div>
              </button>
            </div>
          </div>
        </div>

        <!-- Cart and Checkout -->
        <div class="space-y-4">
          <!-- Current Sale -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-4 sm:p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-gray-900">Current Sale</h3>
              <button 
                v-if="cartItems.length > 0"
                @click="clearCart"
                class="text-sm text-red-600 hover:text-red-700"
              >
                Clear All
              </button>
            </div>
            
            <div v-if="cartItems.length === 0" class="text-center py-8">
              <ShoppingCartIcon class="w-16 h-16 text-gray-300 mx-auto mb-4" />
              <p class="text-gray-500">No items in cart</p>
              <p class="text-sm text-gray-400">Scan or click products to add</p>
            </div>

            <div v-else class="space-y-3 max-h-64 overflow-y-auto">
              <div 
                v-for="item in cartItems" 
                :key="item.id"
                class="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
              >
                <div class="flex-1">
                  <h4 class="font-medium text-gray-900 text-sm">{{ item.name }}</h4>
                  <p class="text-xs text-gray-500">R{{ item.price.toFixed(2) }} each</p>
                </div>
                <div class="flex items-center space-x-2">
                  <button 
                    @click="updateQuantity(item.id, item.quantity - 1)"
                    class="w-6 h-6 rounded-full bg-gray-200 flex items-center justify-center text-gray-600 hover:bg-gray-300"
                  >
                    <MinusIcon class="w-3 h-3" />
                  </button>
                  <span class="w-8 text-center text-sm font-medium text-gray-900">{{ item.quantity }}</span>
                  <button 
                    @click="updateQuantity(item.id, item.quantity + 1)"
                    class="w-6 h-6 rounded-full bg-gray-200 flex items-center justify-center text-gray-600 hover:bg-gray-300"
                  >
                    <PlusIcon class="w-3 h-3" />
                  </button>
                  <button 
                    @click="removeFromCart(item.id)"
                    class="ml-2 text-red-500 hover:text-red-600"
                  >
                    <TrashIcon class="w-4 h-4" />
                  </button>
                </div>
              </div>
            </div>

            <!-- Cart Total -->
            <div v-if="cartItems.length > 0" class="mt-4 pt-4 border-t border-gray-200">
              <div class="flex justify-between items-center mb-4">
                <span class="text-lg font-semibold text-gray-900">Total:</span>
                <span class="text-xl font-bold text-blue-600">R{{ formatCurrency(cartTotal) }}</span>
              </div>
              
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

              <!-- Checkout Button -->
              <button 
                @click="processPayment"
                class="w-full py-3 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg font-medium transition-colors"
              >
                Process Payment - R{{ formatCurrency(cartTotal) }}
              </button>
            </div>
            </div>

          <!-- Quick Actions -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-4 sm:p-6">
            <h3 class="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h3>
            <div class="space-y-3">
              <button 
                @click="holdSale"
                class="w-full py-2 bg-orange-600 hover:bg-orange-700 text-white rounded-lg font-medium transition-colors"
              >
                Hold Sale
              </button>
              <button 
                @click="voidSale"
                class="w-full py-2 bg-red-600 hover:bg-red-700 text-white rounded-lg font-medium transition-colors"
              >
                Void Sale
              </button>
              <button 
                @click="showCustomerModal = true"
                class="w-full py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium transition-colors"
              >
                Add Customer
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Payment Success Modal -->
    <div v-if="showSuccessModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div class="bg-white rounded-xl p-6 max-w-md w-full">
      <div class="text-center">
          <div class="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <CheckIcon class="w-8 h-8 text-green-600" />
        </div>
          <h3 class="text-xl font-semibold text-gray-900 mb-2">Payment Successful!</h3>
          <p class="text-gray-600 mb-6">Transaction completed successfully</p>
          <div class="flex space-x-3">
            <button 
              @click="printReceipt"
              class="flex-1 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium transition-colors"
            >
              Print Receipt
            </button>
            <button 
              @click="emailReceipt"
              class="flex-1 py-2 bg-gray-600 hover:bg-gray-700 text-white rounded-lg font-medium transition-colors"
            >
              Email Receipt
            </button>
        </div>
      </div>
    </div>
    </div>

    <!-- Barcode Scanner Component -->
    <BarcodeScanner 
      v-model="showBarcodeScanner" 
      @barcode-scanned="handleBarcodeScanned"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import {
  MagnifyingGlassIcon,
  QrCodeIcon,
  CurrencyDollarIcon,
  ChartBarIcon,
  BanknotesIcon,
  ShoppingCartIcon,
  CubeIcon,
  PlusIcon,
  MinusIcon,
  TrashIcon,
  CheckIcon,
  CreditCardIcon,
  PrinterIcon,
  CogIcon
} from '@heroicons/vue/24/outline'
import BarcodeScanner from '~/components/pos/BarcodeScanner.vue'

// Hardware status tracking
const hardwareStatus = ref({
  barcodeScanner: false,
  cardReader: false,
  printer: false,
  cashDrawer: false
})

// Barcode scanning state
let barcodeBuffer = ''
let barcodeTimeout: ReturnType<typeof setTimeout> | null = null

// Reactive data
const searchQuery = ref('')
const selectedCategory = ref('all')
const scannerActive = ref(false)
const showBarcodeScanner = ref(false)
const cartItems = ref<any[]>([])
const selectedCustomer = ref('')
const selectedPaymentMethod = ref('cash')
const showSuccessModal = ref(false)
const showCustomerModal = ref(false)
const showReports = ref(false)
const searchInput = ref<HTMLInputElement>()

// POS Stats
const todaySales = ref(18496)
const todayTransactions = ref(48)
const averageSale = ref(285)
const cashFloat = ref(2500)

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

// Customers
const customers = ref([
  { id: 1, name: 'John Doe' },
  { id: 2, name: 'Jane Smith' },
  { id: 3, name: 'Mike Johnson' }
])

// Payment methods
const paymentMethods = ref([
  { id: 'cash', name: 'Cash' },
  { id: 'card', name: 'Card' },
  { id: 'eft', name: 'EFT' },
  { id: 'account', name: 'Account' }
])

// Computed properties
const filteredProducts = computed(() => {
  let filtered = products.value

  if (selectedCategory.value !== 'all') {
    filtered = filtered.filter(p => p.category === selectedCategory.value)
  }

  if (searchQuery.value) {
    filtered = filtered.filter(p => 
      p.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      p.sku.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  return filtered
})

const cartTotal = computed(() => {
  return cartItems.value.reduce((total, item) => total + (item.price * item.quantity), 0)
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

  const existingItem = cartItems.value.find(item => item.id === product.id)
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
  cartItems.value = cartItems.value.filter(item => item.id != productId)
}

const updateQuantity = (productId: number | string, newQuantity: number) => {
  if (newQuantity <= 0) {
    removeFromCart(productId)
    return
  }

  const item = cartItems.value.find(item => item.id == productId)
  if (item) {
    item.quantity = newQuantity
  }
}

const clearCart = () => {
  cartItems.value = []
  selectedCustomer.value = ''
  selectedPaymentMethod.value = 'cash'
}

const addFirstProductToCart = () => {
  if (filteredProducts.value.length > 0) {
    addToCart(filteredProducts.value[0])
    searchQuery.value = ''
  }
}

// Initialize hardware on mount
onMounted(async () => {
  await initializeHardware()
  setupBarcodeScanning()
})

onUnmounted(() => {
  cleanupHardware()
})

// Hardware initialization
const initializeHardware = async () => {
  try {
    // Check for barcode scanner (keyboard wedge is always available)
    hardwareStatus.value.barcodeScanner = true
    
    // Check for card reader
    if ('hid' in navigator) {
      const devices = await (navigator as any).hid.getDevices()
      hardwareStatus.value.cardReader = devices.length > 0
    }
    
    // Check for receipt printer
    if ('serial' in navigator) {
      const ports = await (navigator as any).serial.getPorts()
      hardwareStatus.value.printer = ports.length > 0
    }
    
    // Cash drawer usually connected to printer
    hardwareStatus.value.cashDrawer = hardwareStatus.value.printer
  } catch (error) {
    console.error('Hardware initialization failed:', error)
  }
}

// Barcode scanning setup
const setupBarcodeScanning = () => {
  document.addEventListener('keypress', handleBarcodeInput)
}

const handleBarcodeInput = (event: KeyboardEvent) => {
  // Clear timeout on each keypress
  if (barcodeTimeout) {
    clearTimeout(barcodeTimeout)
  }

  // Add character to buffer
  if (event.key.length === 1) {
    barcodeBuffer += event.key
  }

  // Process barcode on Enter key (scanners send Enter after barcode)
  if (event.key === 'Enter' && barcodeBuffer.length >= 8) {
    processBarcode(barcodeBuffer)
    barcodeBuffer = ''
    event.preventDefault()
    return
  }

  // Reset buffer after 100ms of inactivity (scanners are fast)
  barcodeTimeout = setTimeout(() => {
    barcodeBuffer = ''
  }, 100)
}

const processBarcode = (barcode: string) => {
  // Find product by SKU or barcode
  const product = products.value.find(p => 
    p.sku.toLowerCase() === barcode.toLowerCase() ||
    barcode.includes(p.id)
  )
  
  if (product && product.stock > 0) {
    addToCart(product)
    showNotification(`Added ${product.name} to cart`)
  } else {
    showNotification('Product not found or out of stock', 'error')
  }
}

const showNotification = (message: string, type: 'success' | 'error' = 'success') => {
  const notification = document.createElement('div')
  notification.textContent = type === 'success' ? `✓ ${message}` : `✗ ${message}`
  notification.className = `fixed top-20 right-4 ${type === 'success' ? 'bg-green-600' : 'bg-red-600'} text-white px-4 py-2 rounded-lg shadow-lg z-50 animate-fade-in`
  document.body.appendChild(notification)
  setTimeout(() => notification.remove(), 2000)
}

const cleanupHardware = () => {
  document.removeEventListener('keypress', handleBarcodeInput)
  if (barcodeTimeout) {
    clearTimeout(barcodeTimeout)
  }
}

const toggleScanner = () => {
  scannerActive.value = !scannerActive.value
  if (scannerActive.value) {
    showNotification('Barcode scanner activated - ready to scan')
  }
}

const handleBarcodeScanned = (barcode: string) => {
  // Find product by barcode or SKU
  const product = products.value.find(p => 
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

const requestHardwareAccess = async () => {
  try {
    // Request serial port access (for receipt printer)
    if ('serial' in navigator) {
      await (navigator as any).serial.requestPort()
      hardwareStatus.value.printer = true
      hardwareStatus.value.cashDrawer = true
      showNotification('✓ Printer and cash drawer connected')
    }
    
    // Request HID access (for card reader)
    if ('hid' in navigator) {
      await (navigator as any).hid.requestDevice({ filters: [] })
      hardwareStatus.value.cardReader = true
      showNotification('✓ Card reader connected')
    }
    
    // Re-initialize hardware
    await initializeHardware()
  } catch (error) {
    console.error('Hardware request cancelled or failed:', error)
  }
}

const processPayment = async () => {
  if (cartItems.value.length === 0) return
  
  if (selectedPaymentMethod.value === 'card' && hardwareStatus.value.cardReader) {
    // Process card payment
    try {
      showNotification('Processing card payment...')
      await new Promise(resolve => setTimeout(resolve, 2000))
      showSuccessModal.value = true
    } catch (error) {
      showNotification('Card payment failed', 'error')
    }
  } else {
    // Cash or other payment
    showSuccessModal.value = true
  }
}

const holdSale = () => {
  if (cartItems.value.length === 0) return
  
  // In production, save to held transactions
  alert('Sale held successfully')
  clearCart()
}

const voidSale = () => {
  if (cartItems.value.length === 0) return
  
  if (confirm('Are you sure you want to void this sale?')) {
    clearCart()
  }
}

const openDrawer = async () => {
  try {
    if (hardwareStatus.value.cashDrawer) {
      // Send ESC/POS command to open drawer via printer
      if ('serial' in navigator) {
        const ports = await (navigator as any).serial.getPorts()
        if (ports.length > 0) {
          const printer = ports[0]
          await printer.open({ baudRate: 9600 })
          
          // ESC p m t1 t2 - Open cash drawer command
          const command = new Uint8Array([0x1B, 0x70, 0x00, 0x19, 0xFA])
          
          const writer = printer.writable.getWriter()
          await writer.write(command)
          writer.releaseLock()
          
          await printer.close()
          showNotification('Cash drawer opened')
          return
        }
      }
    }
    
    // Fallback notification
    showNotification('Cash drawer opened (simulated)')
  } catch (error) {
    console.error('Failed to open drawer:', error)
    showNotification('Failed to open cash drawer', 'error')
  }
}

const printReceipt = async () => {
  try {
    if (hardwareStatus.value.printer) {
      // Generate and print receipt
      const receiptData = {
        storeName: "THABO'S SPAZA SHOP",
        storeAddress: '123 Main Street, Soweto',
        storePhone: '+27 11 123 4567',
        receiptNumber: `RCP-${Date.now()}`,
        date: new Date().toLocaleString('en-ZA'),
        cashier: 'Thabo',
        customer: selectedCustomer.value || 'Walk-in Customer',
        items: cartItems.value.map(item => ({
          name: item.name,
          quantity: item.quantity,
          price: item.price,
          total: item.price * item.quantity
        })),
        total: cartTotal.value,
        paymentMethod: selectedPaymentMethod.value
      }
      
      await printESCPOSReceipt(receiptData)
      showNotification('Receipt printed successfully')
    } else {
      // Fallback to browser print
      window.print()
    }
    
    closeSuccessModal()
  } catch (error) {
    console.error('Print failed:', error)
    window.print() // Fallback
    closeSuccessModal()
  }
}

const printESCPOSReceipt = async (receiptData: any) => {
  if ('serial' in navigator) {
    const ports = await (navigator as any).serial.getPorts()
    if (ports.length > 0) {
      const printer = ports[0]
      await printer.open({ baudRate: 9600 })
      
      const encoder = new TextEncoder()
      const ESC = 0x1B
      const GS = 0x1D
      
      let commands: number[] = []
      
      // Initialize
      commands.push(ESC, 0x40)
      
      // Center align
      commands.push(ESC, 0x61, 0x01)
      
      // Store name (bold)
      commands.push(ESC, 0x45, 0x01)
      commands.push(...encoder.encode(receiptData.storeName + '\n'))
      commands.push(ESC, 0x45, 0x00)
      
      // Store info
      commands.push(...encoder.encode(receiptData.storeAddress + '\n'))
      commands.push(...encoder.encode(receiptData.storePhone + '\n\n'))
      
      // Left align
      commands.push(ESC, 0x61, 0x00)
      
      // Receipt details
      commands.push(...encoder.encode(`Receipt: ${receiptData.receiptNumber}\n`))
      commands.push(...encoder.encode(`Date: ${receiptData.date}\n`))
      commands.push(...encoder.encode(`Cashier: ${receiptData.cashier}\n\n`))
      commands.push(...encoder.encode('--------------------------------\n'))
      
      // Items
      receiptData.items.forEach((item: any) => {
        commands.push(...encoder.encode(`${item.name}\n`))
        commands.push(...encoder.encode(`${item.quantity} x R${item.price.toFixed(2)} = R${item.total.toFixed(2)}\n`))
      })
      
      commands.push(...encoder.encode('--------------------------------\n'))
      
      // Total (bold)
      commands.push(ESC, 0x45, 0x01)
      commands.push(...encoder.encode(`TOTAL: R${receiptData.total.toFixed(2)}\n`))
      commands.push(ESC, 0x45, 0x00)
      
      commands.push(...encoder.encode(`Payment: ${receiptData.paymentMethod}\n\n`))
      
      // Footer
      commands.push(ESC, 0x61, 0x01)
      commands.push(...encoder.encode('Thank you!\n\n\n'))
      
      // Cut paper
      commands.push(GS, 0x56, 0x00)
      
      const writer = printer.writable.getWriter()
      await writer.write(new Uint8Array(commands))
      writer.releaseLock()
      
      await printer.close()
    }
  }
}

const emailReceipt = () => {
  // In production, send email via API
  showNotification('Receipt emailed successfully')
  closeSuccessModal()
}

const closeSuccessModal = () => {
  showSuccessModal.value = false
  clearCart()
}

// Page metadata
definePageMeta({
  layout: 'default',
  title: 'Point of Sale - TOSS ERP'
})

// Meta
useHead({
  title: 'Point of Sale - TOSS ERP',
  meta: [
    { name: 'description', content: 'Quick checkout system for retail businesses' }
  ]
})
</script>