<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">Point of Sale</h1>
          <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Quick checkout system for Thabo's Spaza Shop</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button @click="openDrawer" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <CurrencyDollarIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Open Drawer
          </button>
          <button @click="showReports = true" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-slate-600 hover:bg-slate-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <ChartBarIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Reports
          </button>
        </div>
      </div>

      <!-- POS Stats Cards -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Today's Sales</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(todaySales) }}</p>
              <p class="text-xs sm:text-sm text-emerald-600">{{ todayTransactions }} transactions</p>
            </div>
            <div class="p-2 sm:p-3 bg-emerald-100 dark:bg-emerald-900 rounded-full">
              <CurrencyDollarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-emerald-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Current Sale</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(cartTotal) }}</p>
              <p class="text-xs sm:text-sm text-blue-600">{{ cartItems.length }} items</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <ShoppingCartIcon class="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Avg. Sale</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(averageSale) }}</p>
              <p class="text-xs sm:text-sm text-yellow-600">Last hour</p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <ChartBarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Cash Float</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(cashFloat) }}</p>
              <p class="text-xs sm:text-sm text-purple-600">In drawer</p>
            </div>
            <div class="p-2 sm:p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
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
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <div class="flex items-center space-x-3">
              <div class="flex-1 relative">
                <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-slate-400" />
                <input 
                  v-model="searchQuery"
                  type="text"
                  placeholder="Scan barcode or search products..."
                  class="w-full pl-10 pr-4 py-3 rounded-lg border border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  @keyup.enter="addFirstProductToCart"
                />
              </div>
              <button 
                @click="toggleScanner"
                :class="[
                  'p-3 rounded-lg transition-colors',
                  scannerActive 
                    ? 'bg-blue-600 text-white' 
                    : 'bg-slate-200 dark:bg-slate-700 text-slate-700 dark:text-slate-300 hover:bg-slate-300 dark:hover:bg-slate-600'
                ]"
              >
                <QrCodeIcon class="w-6 h-6" />
              </button>
            </div>
          </div>

          <!-- Category Filters -->
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <div class="flex flex-wrap gap-2">
              <button 
                v-for="category in categories" 
                :key="category.id"
                @click="selectedCategory = category.id"
                :class="[
                  'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
                  selectedCategory === category.id
                    ? 'bg-blue-600 text-white'
                    : 'bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300 hover:bg-slate-200 dark:hover:bg-slate-600'
                ]"
              >
                {{ category.name }}
              </button>
            </div>
          </div>

          <!-- Products Grid -->
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-4">
              <button 
                v-for="product in filteredProducts" 
                :key="product.id"
                @click="addToCart(product)"
                :disabled="product.stock === 0"
                class="bg-slate-50 dark:bg-slate-700 rounded-lg border border-slate-200 dark:border-slate-600 p-3 hover:border-blue-500 dark:hover:border-blue-500 hover:shadow-lg transition-all text-left disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <div class="aspect-square bg-slate-100 dark:bg-slate-600 rounded-lg mb-3 flex items-center justify-center overflow-hidden">
                  <img 
                    v-if="product.image" 
                    :src="product.image" 
                    :alt="product.name"
                    class="w-full h-full object-cover"
                  />
                  <CubeIcon v-else class="w-8 h-8 text-slate-400" />
                </div>
                <h3 class="font-medium text-slate-900 dark:text-white text-sm truncate mb-1">{{ product.name }}</h3>
                <p class="text-xs text-slate-500 dark:text-slate-400 truncate mb-2">{{ product.sku }}</p>
                <div class="flex items-center justify-between">
                  <span class="text-sm font-bold text-blue-600">R{{ product.price.toFixed(2) }}</span>
                  <span 
                    :class="[
                      'text-xs px-2 py-1 rounded-full',
                      product.stock > 10 
                        ? 'bg-green-100 dark:bg-green-900 text-green-700 dark:text-green-300' 
                        : product.stock > 0
                          ? 'bg-yellow-100 dark:bg-yellow-900 text-yellow-700 dark:text-yellow-300'
                          : 'bg-red-100 dark:bg-red-900 text-red-700 dark:text-red-300'
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
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Current Sale</h3>
              <button 
                v-if="cartItems.length > 0"
                @click="clearCart"
                class="text-sm text-red-600 hover:text-red-700"
              >
                Clear All
              </button>
            </div>
            
            <div v-if="cartItems.length === 0" class="text-center py-8">
              <ShoppingCartIcon class="w-16 h-16 text-slate-300 dark:text-slate-600 mx-auto mb-4" />
              <p class="text-slate-500 dark:text-slate-400">No items in cart</p>
              <p class="text-sm text-slate-400 dark:text-slate-500">Scan or click products to add</p>
            </div>

            <div v-else class="space-y-3 max-h-64 overflow-y-auto">
              <div 
                v-for="item in cartItems" 
                :key="item.id"
                class="flex items-center justify-between p-3 bg-slate-50 dark:bg-slate-700 rounded-lg"
              >
                <div class="flex-1">
                  <h4 class="font-medium text-slate-900 dark:text-white text-sm">{{ item.name }}</h4>
                  <p class="text-xs text-slate-500 dark:text-slate-400">R{{ item.price.toFixed(2) }} each</p>
                </div>
                <div class="flex items-center space-x-2">
                  <button 
                    @click="updateQuantity(item.id, item.quantity - 1)"
                    class="w-6 h-6 rounded-full bg-slate-200 dark:bg-slate-600 flex items-center justify-center text-slate-600 dark:text-slate-300 hover:bg-slate-300 dark:hover:bg-slate-500"
                  >
                    <MinusIcon class="w-3 h-3" />
                  </button>
                  <span class="w-8 text-center text-sm font-medium text-slate-900 dark:text-white">{{ item.quantity }}</span>
                  <button 
                    @click="updateQuantity(item.id, item.quantity + 1)"
                    class="w-6 h-6 rounded-full bg-slate-200 dark:bg-slate-600 flex items-center justify-center text-slate-600 dark:text-slate-300 hover:bg-slate-300 dark:hover:bg-slate-500"
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
            <div v-if="cartItems.length > 0" class="mt-4 pt-4 border-t border-slate-200 dark:border-slate-700">
              <div class="flex justify-between items-center mb-4">
                <span class="text-lg font-semibold text-slate-900 dark:text-white">Total:</span>
                <span class="text-xl font-bold text-blue-600">R{{ formatCurrency(cartTotal) }}</span>
              </div>
              
              <!-- Customer Selection -->
              <div class="mb-4">
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Customer</label>
                <select 
                  v-model="selectedCustomer"
                  class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                >
                  <option value="">Walk-in Customer</option>
                  <option v-for="customer in customers" :key="customer.id" :value="customer.id">
                    {{ customer.name }}
                  </option>
                </select>
              </div>

              <!-- Payment Methods -->
              <div class="mb-4">
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Payment Method</label>
                <div class="grid grid-cols-2 gap-2">
                  <button 
                    v-for="method in paymentMethods" 
                    :key="method.id"
                    @click="selectedPaymentMethod = method.id"
                    :class="[
                      'p-2 rounded-lg text-sm font-medium transition-colors',
                      selectedPaymentMethod === method.id
                        ? 'bg-blue-600 text-white'
                        : 'bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300 hover:bg-slate-200 dark:hover:bg-slate-600'
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
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Quick Actions</h3>
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
      <div class="bg-white dark:bg-slate-800 rounded-xl p-6 max-w-md w-full">
        <div class="text-center">
          <div class="w-16 h-16 bg-green-100 dark:bg-green-900 rounded-full flex items-center justify-center mx-auto mb-4">
            <CheckIcon class="w-8 h-8 text-green-600" />
          </div>
          <h3 class="text-xl font-semibold text-slate-900 dark:text-white mb-2">Payment Successful!</h3>
          <p class="text-slate-600 dark:text-slate-400 mb-6">Transaction completed successfully</p>
          <div class="flex space-x-3">
            <button 
              @click="printReceipt"
              class="flex-1 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium transition-colors"
            >
              Print Receipt
            </button>
            <button 
              @click="emailReceipt"
              class="flex-1 py-2 bg-slate-600 hover:bg-slate-700 text-white rounded-lg font-medium transition-colors"
            >
              Email Receipt
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
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
  CheckIcon
} from '@heroicons/vue/24/outline'

// Reactive data
const searchQuery = ref('')
const selectedCategory = ref('all')
const scannerActive = ref(false)
const cartItems = ref<any[]>([])
const selectedCustomer = ref('')
const selectedPaymentMethod = ref('cash')
const showSuccessModal = ref(false)
const showCustomerModal = ref(false)
const showReports = ref(false)

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

const removeFromCart = (productId: number) => {
  cartItems.value = cartItems.value.filter(item => item.id !== productId)
}

const updateQuantity = (productId: number, newQuantity: number) => {
  if (newQuantity <= 0) {
    removeFromCart(productId)
    return
  }

  const item = cartItems.value.find(item => item.id === productId)
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

const toggleScanner = () => {
  scannerActive.value = !scannerActive.value
  // In production, integrate with barcode scanner
}

const processPayment = () => {
  if (cartItems.value.length === 0) return
  
  // In production, integrate with payment gateway
  showSuccessModal.value = true
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

const openDrawer = () => {
  // In production, integrate with cash drawer
  alert('Cash drawer opened')
}

const printReceipt = () => {
  // In production, integrate with receipt printer
  window.print()
  closeSuccessModal()
}

const emailReceipt = () => {
  // In production, send email via API
  alert('Receipt emailed successfully')
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