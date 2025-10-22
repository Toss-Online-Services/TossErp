<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800 relative overflow-hidden">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
    <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">Point of Sale</h1>
          <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Quick checkout system for Thabo's Spaza Shop</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button @click="toggleFullscreen" 
                  :class="[
                    'flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 text-white rounded-xl shadow-lg hover:shadow-xl transition-all duration-200 text-sm sm:text-base font-semibold',
                    isFullscreen ? 'bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700' : 'bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700'
                  ]"
                  :title="isFullscreen ? 'Exit Fullscreen (F11)' : 'Enter Fullscreen (F11)'">
            <component :is="isFullscreen ? ArrowsPointingInIcon : ArrowsPointingOutIcon" class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            {{ isFullscreen ? 'Exit' : 'Fullscreen' }}
          </button>
          <button @click="openDrawer" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700 text-white rounded-xl shadow-lg hover:shadow-xl transition-all duration-200 text-sm sm:text-base font-semibold">
            <CurrencyDollarIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Open Drawer
          </button>
          <button @click="showReports = true" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-gradient-to-r from-slate-600 to-gray-600 hover:from-slate-700 hover:to-gray-700 text-white rounded-xl shadow-lg hover:shadow-xl transition-all duration-200 text-sm sm:text-base font-semibold">
            <ChartBarIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Reports
          </button>
        </div>
      </div>


      <!-- Compact Status Bar with Hardware & Stats -->
      <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-3">
        <div class="flex items-center justify-between flex-wrap gap-3">
          <!-- Hardware Status -->
          <div class="flex items-center gap-4">
            <div class="flex items-center gap-1.5">
              <div class="w-2 h-2 rounded-full" :class="hardwareStatus.barcodeScanner ? 'bg-green-500' : 'bg-red-500'"></div>
              <span class="text-xs text-gray-600">Scanner</span>
            </div>
            <div class="flex items-center gap-1.5">
              <div class="w-2 h-2 rounded-full" :class="hardwareStatus.cardReader ? 'bg-green-500' : 'bg-red-500'"></div>
              <span class="text-xs text-gray-600">Card</span>
            </div>
            <div class="flex items-center gap-1.5">
              <div class="w-2 h-2 rounded-full" :class="hardwareStatus.printer ? 'bg-green-500' : 'bg-red-500'"></div>
              <span class="text-xs text-gray-600">Printer</span>
            </div>
            <div class="flex items-center gap-1.5">
              <div class="w-2 h-2 rounded-full" :class="hardwareStatus.cashDrawer ? 'bg-green-500' : 'bg-red-500'"></div>
              <span class="text-xs text-gray-600">Drawer</span>
            </div>
          </div>
          
          <!-- Quick Stats -->
          <div class="flex items-center gap-4 text-xs">
            <div class="flex items-center gap-1.5">
              <span class="text-gray-600">Today:</span>
              <span class="font-semibold text-emerald-600">R{{ formatCurrency(todaySales) }}</span>
            </div>
            <div class="flex items-center gap-1.5">
              <span class="text-gray-600">Cart:</span>
              <span class="font-semibold text-blue-600">R{{ formatCurrency(cartTotal) }}</span>
            </div>
            <div class="flex items-center gap-1.5">
              <span class="text-gray-600">Float:</span>
              <span class="font-semibold text-purple-600">R{{ formatCurrency(cashFloat) }}</span>
            </div>
          </div>
          
          <button @click="showStatsDetails = !showStatsDetails" 
                  class="flex items-center gap-1 px-3 py-1.5 text-xs font-medium text-blue-600 hover:text-blue-700 bg-blue-50 hover:bg-blue-100 rounded-lg transition-colors">
            <component :is="showStatsDetails ? 'ChevronUpIcon' : 'ChevronDownIcon'" class="w-3 h-3" />
            {{ showStatsDetails ? 'Hide' : 'Details' }}
          </button>
        </div>
        
        <!-- Expandable Details -->
        <div v-if="showStatsDetails" class="grid grid-cols-2 lg:grid-cols-4 gap-3 mt-3 pt-3 border-t border-gray-200">
          <div class="p-2">
            <p class="text-xs text-gray-600">Today's Sales</p>
            <p class="text-lg font-bold text-gray-900">R {{ formatCurrency(todaySales) }}</p>
            <p class="text-xs text-emerald-600">{{ todayTransactions }} transactions</p>
          </div>
          <div class="p-2">
            <p class="text-xs text-gray-600">Current Sale</p>
            <p class="text-lg font-bold text-gray-900">R {{ formatCurrency(cartTotal) }}</p>
            <p class="text-xs text-blue-600">{{ cartItems.length }} items</p>
          </div>
          <div class="p-2">
            <p class="text-xs text-gray-600">Avg. Sale</p>
            <p class="text-lg font-bold text-gray-900">R {{ formatCurrency(averageSale) }}</p>
            <p class="text-xs text-yellow-600">Last hour</p>
          </div>
          <div class="p-2">
            <p class="text-xs text-gray-600">Cash Float</p>
            <p class="text-lg font-bold text-gray-900">R {{ formatCurrency(cashFloat) }}</p>
            <p class="text-xs text-purple-600">In drawer</p>
          </div>
        </div>
      </div>

      <!-- Main POS Interface -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Product Search and Selection -->
        <div class="lg:col-span-2 space-y-4">
          <!-- Search and Scanner -->
          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
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
                class="p-3 rounded-xl transition-all duration-200 bg-gradient-to-r from-blue-600 to-purple-600 hover:from-blue-700 hover:to-purple-700 text-white shadow-lg hover:shadow-xl"
              >
                <QrCodeIcon class="w-6 h-6" />
              </button>
              <button 
                @click="requestHardwareAccess"
                class="p-3 rounded-xl transition-all duration-200 bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700 text-white shadow-lg hover:shadow-xl"
                title="Request Hardware Access"
              >
                <CogIcon class="w-6 h-6" />
              </button>
            </div>
          </div>

          <!-- Category Filters -->
          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
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
          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
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
          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 overflow-hidden">
            <div class="bg-gradient-to-r from-blue-600 to-purple-600 px-4 py-4 sm:py-5">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-bold text-white">Current Sale</h3>
                <button 
                  v-if="cartItems.length > 0"
                  @click="clearCart"
                  class="px-3 py-1.5 bg-white/20 hover:bg-white/30 backdrop-blur-sm rounded-lg text-sm font-medium text-white transition-colors"
                >
                  Clear All
                </button>
              </div>
            </div>
            <div class="p-4 sm:p-6">
            
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
                :disabled="cartItems.length === 0"
                class="w-full py-3 rounded-xl font-bold shadow-lg hover:shadow-xl transition-all duration-200 text-white disabled:opacity-50 disabled:cursor-not-allowed bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700"
              >
                💰 Process Payment - R{{ formatCurrency(cartTotal) }}
              </button>
            </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
            <h3 class="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h3>
            <div class="space-y-3">
              <button 
                @click="holdSale"
                class="w-full py-2.5 bg-gradient-to-r from-orange-600 to-amber-600 hover:from-orange-700 hover:to-amber-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
              >
                ⏸️ Hold Sale
              </button>
              <button 
                @click="voidSale"
                class="w-full py-2.5 bg-gradient-to-r from-red-600 to-pink-600 hover:from-red-700 hover:to-pink-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
              >
                ❌ Void Sale
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
  CogIcon,
  ArrowsPointingOutIcon,
  ArrowsPointingInIcon,
  ChevronUpIcon,
  ChevronDownIcon
} from '@heroicons/vue/24/outline'
import BarcodeScanner from '~/components/pos/BarcodeScanner.vue'
import { useSalesAPI } from '~/composables/useSalesAPI'

// API
const salesAPI = useSalesAPI()

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
const isFullscreen = ref(false)
const showStatsDetails = ref(false)
const products = ref<any[]>([])
const customers = ref<any[]>([])

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

// Load data on mount
const loadData = async () => {
  try {
    products.value = await salesAPI.getProducts()
    customers.value = await salesAPI.getCustomers()
  } catch (error) {
    console.error('Failed to load POS data:', error)
  }
}

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
    filtered = filtered.filter((p: any) => p.category === selectedCategory.value)
  }

  if (searchQuery.value) {
    filtered = filtered.filter((p: any) => 
      p.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      p.sku.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  return filtered
})

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
  await loadData()
  await initializeHardware()
  setupBarcodeScanning()
  
  // Add fullscreen event listeners
  document.addEventListener('fullscreenchange', handleFullscreenChange)
  document.addEventListener('keydown', handleKeyboardShortcut)
})

onUnmounted(() => {
  cleanupHardware()
  
  // Remove fullscreen event listeners
  document.removeEventListener('fullscreenchange', handleFullscreenChange)
  document.removeEventListener('keydown', handleKeyboardShortcut)
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
  const product = products.value.find((p: any) => 
    p.sku.toLowerCase() === barcode.toLowerCase() ||
    barcode.includes(p.id.toString())
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
  
  try {
    if (selectedPaymentMethod.value === 'card' && hardwareStatus.value.cardReader) {
      showNotification('Processing card payment...')
      await new Promise(resolve => setTimeout(resolve, 2000))
    }

    // Create sale order via API
    const customerName = selectedCustomer.value 
      ? customers.value.find((c: any) => c.id == selectedCustomer.value)?.name || 'Walk-in Customer'
      : 'Walk-in Customer'

    await salesAPI.createOrder({
      customer: customerName,
      orderItems: cartItems.value.map((item: any) => ({
        id: item.id,
        name: item.name,
        sku: item.sku || `SKU-${item.id}`,
        quantity: item.quantity,
        price: item.price,
        stock: item.stock || 0
      })),
      total: cartTotal.value,
      status: 'completed',
      paymentMethod: selectedPaymentMethod.value
    })

    showSuccessModal.value = true
  } catch (error) {
    console.error('Payment processing failed:', error)
    showNotification('Payment failed', 'error')
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
        items: cartItems.value.map((item: any) => ({
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
        commands.push(...Array.from(encoder.encode(`${item.name}\n`)))
        commands.push(...Array.from(encoder.encode(`${item.quantity} x R${item.price.toFixed(2)} = R${item.total.toFixed(2)}\n`)))
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

// Fullscreen functionality
const toggleFullscreen = async () => {
  try {
    if (!document.fullscreenElement) {
      // Enter fullscreen
      await document.documentElement.requestFullscreen()
      isFullscreen.value = true
      showStatsDetails.value = false // Collapse stats in fullscreen
      
      // Close sidebar/burger menu
      const sidebar = document.querySelector('[data-sidebar]')
      if (sidebar) {
        sidebar.classList.remove('open')
      }
      
      showNotification('✓ Entered fullscreen mode. Press F11 or ESC to exit')
    } else {
      // Exit fullscreen
      await document.exitFullscreen()
      isFullscreen.value = false
      showNotification('✓ Exited fullscreen mode')
    }
  } catch (error) {
    console.error('Fullscreen error:', error)
    showNotification('✗ Fullscreen not supported or blocked', 'error')
  }
}

// Handle fullscreen changes (F11, ESC, etc.)
const handleFullscreenChange = () => {
  isFullscreen.value = !!document.fullscreenElement
}

// Keyboard shortcut for fullscreen (F11)
const handleKeyboardShortcut = (event: KeyboardEvent) => {
  if (event.key === 'F11') {
    event.preventDefault()
    toggleFullscreen()
  }
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