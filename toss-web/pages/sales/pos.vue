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
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-green-600 hover:bg-green-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <CurrencyDollarIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Open Drawer
          </button>
          <button @click="viewReports" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <ChartBarIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Reports
          </button>
        </div>
      </div>

      <!-- Current Sale Stats -->
      <div class="grid grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="text-center">
            <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Today's Sales</p>
            <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(todaysSales) }}</p>
            <p class="text-xs sm:text-sm text-green-600">{{ todaysTransactions }} transactions</p>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="text-center">
            <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Current Sale</p>
            <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(currentSaleTotal) }}</p>
            <p class="text-xs sm:text-sm text-blue-600">{{ currentSaleItems.length }} items</p>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="text-center">
            <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Avg. Sale</p>
            <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(avgSaleValue) }}</p>
            <p class="text-xs sm:text-sm text-purple-600">Last hour</p>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="text-center">
            <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Cash Float</p>
            <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(cashFloat) }}</p>
            <p class="text-xs sm:text-sm text-yellow-600">In drawer</p>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6">
        <!-- Product Search & Categories -->
        <div class="lg:col-span-2 space-y-4">
          <!-- Product Search -->
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <div class="flex gap-3 mb-4">
              <div class="flex-1">
                <input v-model="productSearch" @input="searchProducts" type="text" placeholder="Scan barcode or search products..." 
                       class="w-full px-3 sm:px-4 py-3 text-lg border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              </div>
              <button @click="toggleScanner" class="px-4 py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                <QrCodeIcon class="w-5 h-5" />
              </button>
            </div>

            <!-- Quick Categories -->
            <div class="flex flex-wrap gap-2 mb-4">
              <button v-for="category in productCategories" :key="category.id"
                      @click="filterByCategory(category.id)"
                      :class="[selectedCategory === category.id ? 'bg-blue-600 text-white' : 'bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300']"
                      class="px-3 py-2 rounded-lg text-sm hover:bg-blue-500 hover:text-white transition-colors">
                {{ category.name }}
              </button>
            </div>

            <!-- Product Grid -->
            <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3">
              <div v-for="product in filteredProducts" :key="product.id"
                   @click="addToSale(product)"
                   class="bg-slate-50 dark:bg-slate-700 p-3 rounded-lg cursor-pointer hover:bg-blue-50 dark:hover:bg-blue-900 transition-colors border-2 border-transparent hover:border-blue-300 dark:hover:border-blue-600">
                <div class="text-center">
                  <div class="w-12 h-12 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mx-auto mb-2">
                    <TagIcon class="w-6 h-6 text-blue-600" />
                  </div>
                  <p class="font-medium text-sm text-slate-900 dark:text-white truncate">{{ product.name }}</p>
                  <p class="text-xs text-slate-600 dark:text-slate-400">{{ product.sku }}</p>
                  <p class="text-sm font-bold text-blue-600 dark:text-blue-400 mt-1">R {{ formatCurrency(product.price) }}</p>
                  <p class="text-xs text-slate-500">Stock: {{ product.stock }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Current Sale Cart -->
        <div class="space-y-4">
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
              <div class="flex justify-between items-center">
                <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Current Sale</h3>
                <button @click="clearSale" class="text-red-600 hover:text-red-700 text-sm">
                  Clear All
                </button>
              </div>
            </div>
            
            <div class="p-4 sm:p-6">
              <div v-if="currentSaleItems.length === 0" class="text-center py-8">
                <ShoppingCartIcon class="w-12 h-12 text-slate-400 mx-auto mb-3" />
                <p class="text-slate-500 dark:text-slate-400">No items in cart</p>
                <p class="text-sm text-slate-400 dark:text-slate-500">Scan or click products to add</p>
              </div>

              <div v-else class="space-y-3 max-h-96 overflow-y-auto">
                <div v-for="(item, index) in currentSaleItems" :key="index"
                     class="flex items-center justify-between p-3 bg-slate-50 dark:bg-slate-700 rounded-lg">
                  <div class="flex-1 min-w-0">
                    <p class="font-medium text-sm text-slate-900 dark:text-white truncate">{{ item.name }}</p>
                    <p class="text-xs text-slate-600 dark:text-slate-400">R {{ formatCurrency(item.price) }} each</p>
                  </div>
                  <div class="flex items-center gap-2">
                    <button @click="decreaseQuantity(index)" class="w-6 h-6 bg-red-100 dark:bg-red-900 text-red-600 rounded-full flex items-center justify-center text-sm">
                      -
                    </button>
                    <span class="w-8 text-center text-sm font-medium text-slate-900 dark:text-white">{{ item.quantity }}</span>
                    <button @click="increaseQuantity(index)" class="w-6 h-6 bg-green-100 dark:bg-green-900 text-green-600 rounded-full flex items-center justify-center text-sm">
                      +
                    </button>
                    <button @click="removeFromSale(index)" class="w-6 h-6 bg-slate-100 dark:bg-slate-600 text-slate-600 dark:text-slate-400 rounded-full flex items-center justify-center text-sm ml-1">
                      <XMarkIcon class="w-4 h-4" />
                    </button>
                  </div>
                </div>
              </div>

              <!-- Sale Totals -->
              <div v-if="currentSaleItems.length > 0" class="mt-6 pt-4 border-t border-slate-200 dark:border-slate-700">
                <div class="space-y-2">
                  <div class="flex justify-between">
                    <span class="text-sm text-slate-600 dark:text-slate-400">Subtotal:</span>
                    <span class="text-sm font-medium text-slate-900 dark:text-white">R {{ formatCurrency(currentSaleSubtotal) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-sm text-slate-600 dark:text-slate-400">Tax (15%):</span>
                    <span class="text-sm font-medium text-slate-900 dark:text-white">R {{ formatCurrency(currentSaleTax) }}</span>
                  </div>
                  <div class="flex justify-between text-lg font-bold border-t pt-2 border-slate-200 dark:border-slate-700">
                    <span class="text-slate-900 dark:text-white">Total:</span>
                    <span class="text-blue-600 dark:text-blue-400">R {{ formatCurrency(currentSaleTotal) }}</span>
                  </div>
                </div>

                <!-- Payment Buttons -->
                <div class="grid grid-cols-1 gap-2 mt-4">
                  <button @click="processPayment('cash')" 
                          class="w-full px-4 py-3 bg-green-600 hover:bg-green-700 text-white rounded-lg font-medium">
                    <BanknotesIcon class="w-5 h-5 inline mr-2" />
                    Cash Payment
                  </button>
                  <button @click="processPayment('card')" 
                          class="w-full px-4 py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium">
                    <CreditCardIcon class="w-5 h-5 inline mr-2" />
                    Card Payment
                  </button>
                  <button @click="processPayment('split')" 
                          class="w-full px-4 py-3 bg-purple-600 hover:bg-purple-700 text-white rounded-lg font-medium">
                    <CurrencyDollarIcon class="w-5 h-5 inline mr-2" />
                    Split Payment
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
            <h4 class="text-sm font-semibold text-slate-900 dark:text-white mb-3">Quick Actions</h4>
            <div class="grid grid-cols-2 gap-2">
              <button @click="holdSale" class="px-3 py-2 bg-yellow-100 dark:bg-yellow-900 text-yellow-800 dark:text-yellow-200 rounded-lg text-sm">
                Hold Sale
              </button>
              <button @click="voidSale" class="px-3 py-2 bg-red-100 dark:bg-red-900 text-red-800 dark:text-red-200 rounded-lg text-sm">
                Void Sale
              </button>
              <button @click="applyDiscount" class="px-3 py-2 bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200 rounded-lg text-sm">
                Discount
              </button>
              <button @click="addCustomer" class="px-3 py-2 bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200 rounded-lg text-sm">
                Customer
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Transactions -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Recent Transactions</h3>
        </div>
        <div class="p-4 sm:p-6">
          <div class="space-y-3">
            <div v-for="transaction in recentTransactions" :key="transaction.id" 
                 class="flex items-center justify-between p-3 rounded-lg border border-slate-100 dark:border-slate-700">
              <div class="flex items-center space-x-3">
                <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="getPaymentMethodColor(transaction.paymentMethod)">
                  <BanknotesIcon v-if="transaction.paymentMethod === 'cash'" class="w-4 h-4 text-white" />
                  <CreditCardIcon v-else class="w-4 h-4 text-white" />
                </div>
                <div>
                  <p class="text-sm font-medium text-slate-900 dark:text-white">Transaction #{{ transaction.id }}</p>
                  <p class="text-xs text-slate-600 dark:text-slate-400">{{ transaction.items }} items • {{ formatTime(transaction.timestamp) }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-sm font-semibold text-slate-900 dark:text-white">R {{ formatCurrency(transaction.total) }}</p>
                <p class="text-xs text-slate-600 dark:text-slate-400">{{ transaction.paymentMethod }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Payment Modal -->
    <div v-if="showPaymentModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-md w-full mx-4">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Process Payment</h3>
        </div>
        <div class="p-4 sm:p-6">
          <div class="text-center mb-6">
            <p class="text-3xl font-bold text-blue-600 dark:text-blue-400">R {{ formatCurrency(currentSaleTotal) }}</p>
            <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">Total Amount</p>
          </div>

          <div v-if="paymentMethod === 'cash'" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Amount Received</label>
              <input v-model.number="amountReceived" type="number" step="0.01" placeholder="0.00"
                     class="w-full px-3 py-2 text-lg text-center border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
            </div>
            <div v-if="amountReceived >= currentSaleTotal" class="bg-green-50 dark:bg-green-900/20 p-4 rounded-lg">
              <p class="text-center text-lg font-semibold text-green-700 dark:text-green-300">
                Change: R {{ formatCurrency(amountReceived - currentSaleTotal) }}
              </p>
            </div>
          </div>

          <div class="flex justify-end space-x-3 mt-6">
            <button @click="showPaymentModal = false" type="button" 
                    class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
              Cancel
            </button>
            <button @click="completeSale" 
                    :disabled="paymentMethod === 'cash' && amountReceived < currentSaleTotal"
                    class="px-6 py-2 bg-green-600 hover:bg-green-700 disabled:bg-slate-400 text-white rounded-lg">
              Complete Sale
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  CurrencyDollarIcon,
  ChartBarIcon,
  QrCodeIcon,
  TagIcon,
  ShoppingCartIcon,
  XMarkIcon,
  BanknotesIcon,
  CreditCardIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Point of Sale - TOSS ERP',
  meta: [
    { name: 'description', content: 'Quick checkout system for Thabo\'s Spaza Shop' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// Reactive data
const productSearch = ref('')
const selectedCategory = ref('')
const showPaymentModal = ref(false)
const paymentMethod = ref('')
const amountReceived = ref(0)

// POS statistics
const todaysSales = ref(18450)
const todaysTransactions = ref(47)
const avgSaleValue = ref(285)
const cashFloat = ref(2500)

// Current sale
const currentSaleItems = ref([])

// Product categories for Thabo's Spaza Shop
const productCategories = ref([
  { id: '', name: 'All' },
  { id: 'groceries', name: 'Groceries' },
  { id: 'beverages', name: 'Beverages' },
  { id: 'snacks', name: 'Snacks' },
  { id: 'household', name: 'Household' },
  { id: 'personal', name: 'Personal Care' },
  { id: 'frozen', name: 'Frozen' }
])

// Sample products for Thabo's Spaza Shop
const products = ref([
  { id: '1', name: 'Coca Cola 2L', sku: 'CC2L001', price: 35, stock: 24, category: 'beverages', barcode: '123456789' },
  { id: '2', name: 'White Bread 700g', sku: 'WB700', price: 18, stock: 15, category: 'groceries', barcode: '234567890' },
  { id: '3', name: 'Milk 1L', sku: 'MLK1L', price: 22, stock: 12, category: 'groceries', barcode: '345678901' },
  { id: '4', name: 'Simba Chips 125g', sku: 'SC125', price: 12, stock: 30, category: 'snacks', barcode: '456789012' },
  { id: '5', name: 'Sunlight Soap 250g', sku: 'SS250', price: 15, stock: 20, category: 'household', barcode: '567890123' },
  { id: '6', name: 'Maggi 2-Minute Noodles', sku: 'MGN2M', price: 8, stock: 48, category: 'groceries', barcode: '678901234' },
  { id: '7', name: 'Castle Lager 440ml', sku: 'CL440', price: 28, stock: 18, category: 'beverages', barcode: '789012345' },
  { id: '8', name: 'Purity Baby Food', sku: 'PBF001', price: 25, stock: 10, category: 'groceries', barcode: '890123456' },
  { id: '9', name: 'Colgate Toothpaste', sku: 'CT001', price: 32, stock: 8, category: 'personal', barcode: '901234567' },
  { id: '10', name: 'Frozen Chicken 1kg', sku: 'FCK1K', price: 65, stock: 6, category: 'frozen', barcode: '012345678' }
])

// Recent transactions
const recentTransactions = ref([
  { id: '001', total: 156, items: 8, paymentMethod: 'cash', timestamp: new Date() },
  { id: '002', total: 89, items: 4, paymentMethod: 'card', timestamp: new Date(Date.now() - 15 * 60 * 1000) },
  { id: '003', total: 245, items: 12, paymentMethod: 'cash', timestamp: new Date(Date.now() - 32 * 60 * 1000) },
  { id: '004', total: 67, items: 3, paymentMethod: 'card', timestamp: new Date(Date.now() - 45 * 60 * 1000) },
  { id: '005', total: 134, items: 7, paymentMethod: 'cash', timestamp: new Date(Date.now() - 58 * 60 * 1000) }
])

// Computed
const filteredProducts = computed(() => {
  let filtered = products.value

  if (selectedCategory.value) {
    filtered = filtered.filter(product => product.category === selectedCategory.value)
  }

  if (productSearch.value) {
    const search = productSearch.value.toLowerCase()
    filtered = filtered.filter(product => 
      product.name.toLowerCase().includes(search) ||
      product.sku.toLowerCase().includes(search) ||
      product.barcode.includes(search)
    )
  }

  return filtered
})

const currentSaleSubtotal = computed(() => {
  return currentSaleItems.value.reduce((total, item) => {
    return total + (item.price * item.quantity)
  }, 0)
})

const currentSaleTax = computed(() => {
  return currentSaleSubtotal.value * 0.15
})

const currentSaleTotal = computed(() => {
  return currentSaleSubtotal.value + currentSaleTax.value
})

// Helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatTime = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

const getPaymentMethodColor = (method: string) => {
  return method === 'cash' ? 'bg-green-600' : 'bg-blue-600'
}

// Product functions
const searchProducts = () => {
  // Auto-select if exact barcode match
  const exactMatch = products.value.find(p => p.barcode === productSearch.value)
  if (exactMatch) {
    addToSale(exactMatch)
    productSearch.value = ''
  }
}

const filterByCategory = (categoryId: string) => {
  selectedCategory.value = categoryId
}

const toggleScanner = () => {
  alert('Barcode scanner feature coming soon!')
}

// Sale functions
const addToSale = (product: any) => {
  const existingItem = currentSaleItems.value.find(item => item.id === product.id)
  
  if (existingItem) {
    if (existingItem.quantity < product.stock) {
      existingItem.quantity += 1
    } else {
      alert(`Only ${product.stock} units available in stock`)
    }
  } else {
    if (product.stock > 0) {
      currentSaleItems.value.push({
        ...product,
        quantity: 1
      })
    } else {
      alert('Product out of stock')
    }
  }
}

const removeFromSale = (index: number) => {
  currentSaleItems.value.splice(index, 1)
}

const increaseQuantity = (index: number) => {
  const item = currentSaleItems.value[index]
  const product = products.value.find(p => p.id === item.id)
  
  if (item.quantity < product.stock) {
    item.quantity += 1
  } else {
    alert(`Only ${product.stock} units available in stock`)
  }
}

const decreaseQuantity = (index: number) => {
  const item = currentSaleItems.value[index]
  if (item.quantity > 1) {
    item.quantity -= 1
  } else {
    removeFromSale(index)
  }
}

const clearSale = () => {
  if (currentSaleItems.value.length > 0) {
    if (confirm('Clear all items from current sale?')) {
      currentSaleItems.value = []
    }
  }
}

// Payment functions
const processPayment = (method: string) => {
  if (currentSaleItems.value.length === 0) {
    alert('No items in cart')
    return
  }
  
  paymentMethod.value = method
  amountReceived.value = method === 'cash' ? 0 : currentSaleTotal.value
  showPaymentModal.value = true
}

const completeSale = () => {
  try {
    // Process the sale
    const transaction = {
      id: (recentTransactions.value.length + 1).toString().padStart(3, '0'),
      total: currentSaleTotal.value,
      items: currentSaleItems.value.length,
      paymentMethod: paymentMethod.value,
      timestamp: new Date()
    }

    // Update stock levels
    currentSaleItems.value.forEach(saleItem => {
      const product = products.value.find(p => p.id === saleItem.id)
      if (product) {
        product.stock -= saleItem.quantity
      }
    })

    // Add to recent transactions
    recentTransactions.value.unshift(transaction)
    
    // Update daily stats
    todaysSales.value += currentSaleTotal.value
    todaysTransactions.value += 1

    // Clear current sale
    currentSaleItems.value = []
    showPaymentModal.value = false
    amountReceived.value = 0

    // Show success message
    alert(`Sale completed successfully! Transaction #${transaction.id}`)

    // Print receipt (placeholder)
    if (confirm('Print receipt?')) {
      printReceipt(transaction)
    }

  } catch (error) {
    console.error('Error completing sale:', error)
    alert('Failed to complete sale. Please try again.')
  }
}

// Quick actions
const holdSale = () => {
  if (currentSaleItems.value.length > 0) {
    alert('Sale held successfully')
    // In a real app, you'd save this to held sales
    currentSaleItems.value = []
  }
}

const voidSale = () => {
  if (currentSaleItems.value.length > 0) {
    if (confirm('Void current sale?')) {
      currentSaleItems.value = []
    }
  }
}

const applyDiscount = () => {
  alert('Discount feature coming soon!')
}

const addCustomer = () => {
  alert('Customer lookup feature coming soon!')
}

// Utility functions
const openDrawer = () => {
  alert('Cash drawer opened')
}

const viewReports = () => {
  alert('Navigating to sales reports...')
}

const printReceipt = (transaction: any) => {
  alert(`Printing receipt for transaction #${transaction.id}`)
}
</script>
