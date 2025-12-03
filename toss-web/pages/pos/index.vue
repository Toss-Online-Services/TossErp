<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import { usePosStore } from '~/stores/pos'
import { useStockStore } from '~/stores/stock'
import { useOfflineSync } from '~/composables/useOfflineSync'
import Receipt from '~/components/pos/Receipt.vue'
import MobileCartDrawer from '~/components/pos/MobileCartDrawer.vue'

// Make this page client-only (POS doesn't need SSR)
definePageMeta({
  ssr: false
})

useHead({
  title: 'Point of Sale - TOSS'
})

const posStore = usePosStore()
const stockStore = useStockStore()
const { isOnline, pendingCount } = useOfflineSync()

// UI State
const searchQuery = ref('')
const showPaymentModal = ref(false)
const showReceipt = ref(false)
const showMobileCart = ref(false)
const showCustomerModal = ref(false)
const showDiscountModal = ref(false)
const selectedCategory = ref('all')
const selectedCartItemForDiscount = ref<string | null>(null)

// Payment State
const paymentMethod = ref<'cash' | 'card' | 'mobile'>('cash')
const cashAmount = ref(0)
const cardAmount = ref(0)
const mobileAmount = ref(0)

// Customer State
const customerSearch = ref('')
const selectedCustomer = ref<{ id: string; name: string } | null>(null)

// Discount State
const discountAmount = ref(0)

// Mock customers
const customers = ref([
  { id: '1', name: 'John Doe', phone: '+27 82 123 4567' },
  { id: '2', name: 'Jane Smith', phone: '+27 83 234 5678' },
  { id: '3', name: 'Mike Johnson', phone: '+27 84 345 6789' }
])

const filteredCustomers = computed(() => {
  if (!customerSearch.value) return customers.value
  const query = customerSearch.value.toLowerCase()
  return customers.value.filter(c => 
    c.name.toLowerCase().includes(query) || 
    c.phone.includes(query)
  )
})

// Computed
const filteredItems = computed(() => {
  if (!stockStore.items || stockStore.items.length === 0) {
    return []
  }
  
  let items = stockStore.items.filter(item => item.isActive && item.currentStock > 0)

  if (selectedCategory.value !== 'all') {
    items = items.filter(item => item.category === selectedCategory.value)
  }

  if (searchQuery.value) {
    items = stockStore.searchItems(searchQuery.value).filter(item => item.currentStock > 0)
  }

  return items
})

const categories = computed(() => {
  if (!stockStore.items || stockStore.items.length === 0) {
    return ['all']
  }
  const cats = new Set(stockStore.items.map(item => item.category))
  return ['all', ...Array.from(cats)]
})

const totalPaid = computed(() => {
  return cashAmount.value + cardAmount.value + mobileAmount.value
})

const changeAmount = computed(() => {
  return Math.max(0, totalPaid.value - posStore.cartTotal)
})

const canCompleteSale = computed(() => {
  return totalPaid.value >= posStore.cartTotal && posStore.cart && posStore.cart.length > 0
})

// Load data
onMounted(() => {
  stockStore.fetchItems()
  posStore.fetchRecentSales()
})

// Watch for barcode scanning (simulated with Enter key)
watch(searchQuery, (newQuery) => {
  if (newQuery && newQuery.length > 5 && stockStore.items && stockStore.items.length > 0) {
    // Simulate barcode scan - if it looks like a barcode, try to find item
    const item = stockStore.items.find(i => i.barcode === newQuery)
    if (item && item.currentStock > 0) {
      addItem(item)
      searchQuery.value = ''
    }
  }
})

// Methods
function addItem(item: any) {
  posStore.addToCart(item)
  // Show mobile cart briefly on mobile after adding item
  if (typeof window !== 'undefined' && window.innerWidth < 1024) {
    showMobileCart.value = true
    setTimeout(() => {
      if (posStore.cart && posStore.cart.length > 0) {
        showMobileCart.value = false
      }
    }, 1500)
  }
}

function removeItem(cartItemId: string) {
  posStore.removeFromCart(cartItemId)
}

function updateItemQuantity(cartItemId: string, quantity: number) {
  posStore.updateQuantity(cartItemId, quantity)
}

function openDiscountModal(cartItemId: string) {
  selectedCartItemForDiscount.value = cartItemId
  showDiscountModal.value = true
}

function applyDiscount(discount: number) {
  if (selectedCartItemForDiscount.value) {
    posStore.updateDiscount(selectedCartItemForDiscount.value, discount)
    showDiscountModal.value = false
    selectedCartItemForDiscount.value = null
  }
}

function openPayment() {
  if (!posStore.cart || posStore.cart.length === 0) return
  showPaymentModal.value = true
  cashAmount.value = Math.ceil(posStore.cartTotal)
  cardAmount.value = 0
  mobileAmount.value = 0
  paymentMethod.value = 'cash'
}

function selectCustomer(customer: { id: string; name: string }) {
  selectedCustomer.value = customer
  showCustomerModal.value = false
  customerSearch.value = ''
}

function clearCustomer() {
  selectedCustomer.value = null
}

async function completeSale() {
  try {
    if (!canCompleteSale.value) {
      alert('Insufficient payment amount')
      return
    }

    const payments = []
    if (cashAmount.value > 0) {
      payments.push({ type: 'cash' as const, amount: cashAmount.value })
    }
    if (cardAmount.value > 0) {
      payments.push({ type: 'card' as const, amount: cardAmount.value })
    }
    if (mobileAmount.value > 0) {
      payments.push({ type: 'mobile' as const, amount: mobileAmount.value })
    }

    await posStore.completeSale(
      payments,
      selectedCustomer.value?.id,
      selectedCustomer.value?.name
    )
    
    showPaymentModal.value = false
    showMobileCart.value = false
    cashAmount.value = 0
    cardAmount.value = 0
    mobileAmount.value = 0
    
    // Show receipt
    showReceipt.value = true
  } catch (error) {
    console.error('Failed to complete sale:', error)
    alert('Failed to complete sale. Please try again.')
  }
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function handlePrintReceipt() {
  window.print()
}

function closeReceipt() {
  showReceipt.value = false
  selectedCustomer.value = null
}
</script>

<template>
  <div class="min-h-screen bg-gray-100 pb-12">
    <!-- Top Bar - Material Dashboard Style -->
    <div class="sticky top-2 z-30 mx-3 mt-2 rounded-xl bg-white/80 backdrop-blur-md shadow-md px-4 py-3 mb-4">
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-4">
          <h1 class="text-xl font-bold text-gray-900">Point of Sale</h1>
          <div v-if="!isOnline" class="flex items-center gap-2 px-3 py-1 bg-orange-100 text-orange-700 rounded-full text-sm">
            <i class="material-symbols-rounded text-sm">cloud_off</i>
            <span>Offline</span>
          </div>
          <div v-if="pendingCount > 0" class="flex items-center gap-2 px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-sm">
            <i class="material-symbols-rounded text-sm animate-spin">sync</i>
            <span>{{ pendingCount }} pending</span>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <button
            @click="showCustomerModal = true"
            class="p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
            :class="{ 'bg-blue-100 text-blue-600': selectedCustomer }"
          >
            <i class="material-symbols-rounded">person</i>
          </button>
          <button
            @click="posStore.holdSale()"
            :disabled="!posStore.cart || posStore.cart.length === 0"
            class="p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors disabled:opacity-50"
          >
            <i class="material-symbols-rounded">pause</i>
          </button>
          <button
            @click="showMobileCart = !showMobileCart"
            class="lg:hidden p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors relative"
          >
            <i class="material-symbols-rounded">shopping_cart</i>
            <span
              v-if="posStore.cartItemCount && posStore.cartItemCount > 0"
              class="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center"
            >
              {{ posStore.cartItemCount }}
            </span>
          </button>
        </div>
      </div>
    </div>

    <div class="px-4">
      <div class="grid grid-cols-1 lg:grid-cols-12 gap-6">
        <!-- Left: Products (Mobile Full Width, Desktop 8 columns) -->
        <div class="lg:col-span-8">
          <!-- Search and Filters Card -->
          <div class="bg-white rounded-xl shadow-sm mb-6 p-6">
            <div class="flex flex-col md:flex-row gap-4 mb-4">
              <div class="flex-1 relative">
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="Search products or scan barcode..."
                  class="w-full px-4 py-3 pr-12 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 text-base"
                  autofocus
                >
                <i class="material-symbols-rounded absolute right-4 top-1/2 -translate-y-1/2 text-gray-400">search</i>
              </div>
              <button class="px-6 py-3 bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-lg hover:shadow-lg transition-shadow flex items-center justify-center gap-2 font-semibold">
                <i class="material-symbols-rounded">qr_code_scanner</i>
                <span class="hidden sm:inline">Scan</span>
              </button>
            </div>

            <!-- Category Tabs -->
            <div class="flex gap-2 overflow-x-auto pb-2 scrollbar-hide">
              <button
                v-for="cat in categories || ['all']"
                :key="cat"
                @click="selectedCategory = cat"
                :class="[
                  'px-4 py-2 rounded-lg whitespace-nowrap transition-all font-medium text-sm',
                  selectedCategory === cat
                    ? 'bg-gradient-to-br from-blue-500 to-blue-600 text-white shadow-md'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                ]"
              >
                {{ cat === 'all' ? 'All Categories' : cat }}
              </button>
            </div>
          </div>

          <!-- Products Grid Card -->
          <div class="bg-white rounded-xl shadow-sm p-6">
            <div v-if="stockStore.loading" class="text-center py-12">
              <i class="material-symbols-rounded text-6xl text-gray-400 mb-4 animate-spin">sync</i>
              <p class="text-gray-600">Loading products...</p>
            </div>

            <div v-else-if="filteredItems.length === 0" class="text-center py-12">
              <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">inventory_2</i>
              <p class="text-gray-600 font-medium">No products found</p>
              <p class="text-sm text-gray-500 mt-2">Try a different search or category</p>
            </div>

            <div v-else class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-3 xl:grid-cols-4 gap-4">
              <button
                v-for="item in filteredItems"
                :key="item.id"
                @click="addItem(item)"
                class="bg-white border border-gray-200 rounded-xl p-4 hover:shadow-lg hover:border-blue-300 transition-all text-left group"
              >
                <div class="aspect-square bg-gradient-to-br from-gray-100 to-gray-200 rounded-lg mb-3 flex items-center justify-center group-hover:from-blue-50 group-hover:to-blue-100 transition-colors">
                  <i class="material-symbols-rounded text-4xl text-gray-400 group-hover:text-blue-500">inventory_2</i>
                </div>
                <h3 class="font-semibold text-gray-900 mb-1 line-clamp-2 text-sm">{{ item.name }}</h3>
                <p class="text-xs text-gray-500 mb-2">{{ item.code }}</p>
                <div class="flex items-center justify-between">
                  <span class="text-base font-bold text-blue-600">{{ formatCurrency(item.sellingPrice) }}</span>
                  <span class="text-xs text-gray-500 bg-gray-100 px-2 py-1 rounded">{{ item.currentStock }} {{ item.unit }}</span>
                </div>
              </button>
            </div>
          </div>
        </div>

        <!-- Right: Cart (Hidden on mobile, shown as drawer) -->
        <div class="hidden lg:block lg:col-span-4">
          <div class="bg-white rounded-xl shadow-sm flex flex-col sticky top-24" style="max-height: calc(100vh - 8rem);">
            <!-- Cart Header -->
            <div class="p-6 border-b bg-gradient-to-br from-gray-800 to-gray-900 text-white rounded-t-xl">
              <div class="flex items-center justify-between mb-2">
                <h2 class="text-lg font-bold">Current Sale</h2>
                <button
                  v-if="posStore.cart.length > 0"
                  @click="posStore.clearCart()"
                  class="p-2 hover:bg-white/10 rounded-lg transition-colors"
                >
                  <i class="material-symbols-rounded">delete</i>
                </button>
              </div>
              <p class="text-sm text-white/80">{{ posStore.cartItemCount || 0 }} items</p>
              <div v-if="selectedCustomer" class="mt-2 text-xs text-white/70">
                <i class="material-symbols-rounded text-sm align-middle">person</i>
                {{ selectedCustomer.name }}
              </div>
            </div>

            <!-- Cart Items -->
            <div class="flex-1 overflow-y-auto p-4">
              <div v-if="!posStore.cart || posStore.cart.length === 0" class="text-center py-12">
                <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">shopping_cart</i>
                <p class="text-gray-600 font-medium">Cart is empty</p>
                <p class="text-sm text-gray-500 mt-2">Add items to start a sale</p>
              </div>

              <div v-else-if="posStore.cart && posStore.cart.length > 0" class="space-y-3">
                <div
                  v-for="item in posStore.cart"
                  :key="item.id"
                  class="bg-gray-50 rounded-xl p-4 border border-gray-200"
                >
                  <div class="flex items-start justify-between mb-3">
                    <div class="flex-1">
                      <h3 class="font-semibold text-gray-900 text-sm mb-1">{{ item.name }}</h3>
                      <p class="text-xs text-gray-600">{{ item.code }}</p>
                      <p class="text-sm font-bold text-blue-600 mt-1">{{ formatCurrency(item.price) }} each</p>
                    </div>
                    <button
                      @click="removeItem(item.id)"
                      class="text-red-600 hover:text-red-700 p-1"
                    >
                      <i class="material-symbols-rounded text-lg">close</i>
                    </button>
                  </div>

                  <div class="flex items-center justify-between mb-2">
                    <div class="flex items-center gap-2">
                      <button
                        @click="updateItemQuantity(item.id, item.quantity - 1)"
                        class="w-8 h-8 bg-white rounded-lg border border-gray-300 flex items-center justify-center hover:bg-gray-100 transition-colors"
                      >
                        <i class="material-symbols-rounded text-sm">remove</i>
                      </button>
                      <span class="w-12 text-center font-semibold text-gray-900">{{ item.quantity }}</span>
                      <button
                        @click="updateItemQuantity(item.id, item.quantity + 1)"
                        class="w-8 h-8 bg-white rounded-lg border border-gray-300 flex items-center justify-center hover:bg-gray-100 transition-colors"
                      >
                        <i class="material-symbols-rounded text-sm">add</i>
                      </button>
                    </div>
                    <span class="font-bold text-gray-900 text-lg">{{ formatCurrency(item.total) }}</span>
                  </div>

                  <div class="flex items-center justify-between pt-2 border-t border-gray-200">
                    <button
                      @click="openDiscountModal(item.id)"
                      class="text-xs text-blue-600 hover:text-blue-700 flex items-center gap-1"
                    >
                      <i class="material-symbols-rounded text-sm">discount</i>
                      <span v-if="item.discount > 0">Discount: {{ formatCurrency(item.discount) }}</span>
                      <span v-else>Add Discount</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Cart Summary -->
            <div class="border-t p-6 bg-gray-50 rounded-b-xl space-y-3">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">Subtotal</span>
                <span class="font-semibold text-gray-900">{{ formatCurrency(posStore.cartSubtotal) }}</span>
              </div>
              <div v-if="posStore.cartDiscount > 0" class="flex justify-between text-sm">
                <span class="text-gray-600">Discount</span>
                <span class="font-semibold text-red-600">-{{ formatCurrency(posStore.cartDiscount) }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">VAT (15%)</span>
                <span class="font-semibold text-gray-900">{{ formatCurrency(posStore.cartTax) }}</span>
              </div>
              <div class="flex justify-between text-xl font-bold pt-3 border-t border-gray-300">
                <span class="text-gray-900">Total</span>
                <span class="text-blue-600">{{ formatCurrency(posStore.cartTotal) }}</span>
              </div>

              <button
                @click="openPayment"
                :disabled="!posStore.cart || posStore.cart.length === 0"
                class="w-full py-4 bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-xl font-bold text-lg hover:shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
              >
                <i class="material-symbols-rounded">point_of_sale</i>
                Charge {{ formatCurrency(posStore.cartTotal) }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Mobile Cart Drawer -->
    <MobileCartDrawer
      :is-open="showMobileCart"
      @close="showMobileCart = false"
      @checkout="openPayment"
    />

    <!-- Payment Modal -->
    <div
      v-if="showPaymentModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showPaymentModal = false"
    >
      <div class="bg-white rounded-xl max-w-md w-full max-h-[90vh] overflow-y-auto shadow-2xl">
        <div class="p-6 border-b">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-2xl font-bold text-gray-900">Payment</h2>
            <button
              @click="showPaymentModal = false"
              class="p-2 text-gray-400 hover:text-gray-600 rounded-lg transition-colors"
            >
              <i class="material-symbols-rounded">close</i>
            </button>
          </div>

          <div class="bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl p-4 text-white mb-4">
            <p class="text-sm text-white/80 mb-1">Total Amount</p>
            <p class="text-3xl font-bold">{{ formatCurrency(posStore.cartTotal) }}</p>
          </div>
        </div>

        <div class="p-6 space-y-6">
          <!-- Payment Methods -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-3">Payment Method</label>
            <div class="grid grid-cols-3 gap-3">
              <button
                @click="paymentMethod = 'cash'; cashAmount = Math.ceil(posStore.cartTotal); cardAmount = 0; mobileAmount = 0"
                :class="[
                  'p-4 rounded-xl border-2 transition-all',
                  paymentMethod === 'cash'
                    ? 'border-blue-500 bg-blue-50'
                    : 'border-gray-200 hover:border-gray-300'
                ]"
              >
                <i class="material-symbols-rounded text-2xl mb-2" :class="paymentMethod === 'cash' ? 'text-blue-600' : 'text-gray-400'">payments</i>
                <p class="text-xs font-medium" :class="paymentMethod === 'cash' ? 'text-blue-600' : 'text-gray-600'">Cash</p>
              </button>
              <button
                @click="paymentMethod = 'card'; cardAmount = Math.ceil(posStore.cartTotal); cashAmount = 0; mobileAmount = 0"
                :class="[
                  'p-4 rounded-xl border-2 transition-all',
                  paymentMethod === 'card'
                    ? 'border-blue-500 bg-blue-50'
                    : 'border-gray-200 hover:border-gray-300'
                ]"
              >
                <i class="material-symbols-rounded text-2xl mb-2" :class="paymentMethod === 'card' ? 'text-blue-600' : 'text-gray-400'">credit_card</i>
                <p class="text-xs font-medium" :class="paymentMethod === 'card' ? 'text-blue-600' : 'text-gray-600'">Card</p>
              </button>
              <button
                @click="paymentMethod = 'mobile'; mobileAmount = Math.ceil(posStore.cartTotal); cashAmount = 0; cardAmount = 0"
                :class="[
                  'p-4 rounded-xl border-2 transition-all',
                  paymentMethod === 'mobile'
                    ? 'border-blue-500 bg-blue-50'
                    : 'border-gray-200 hover:border-gray-300'
                ]"
              >
                <i class="material-symbols-rounded text-2xl mb-2" :class="paymentMethod === 'mobile' ? 'text-blue-600' : 'text-gray-400'">phone_android</i>
                <p class="text-xs font-medium" :class="paymentMethod === 'mobile' ? 'text-blue-600' : 'text-gray-600'">Mobile</p>
              </button>
            </div>
          </div>

          <!-- Cash Amount -->
          <div v-if="paymentMethod === 'cash'">
            <label class="block text-sm font-medium text-gray-700 mb-2">Cash Received</label>
            <input
              v-model.number="cashAmount"
              type="number"
              step="0.01"
              class="w-full px-4 py-3 text-2xl font-bold border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
              autofocus
            >
          </div>

          <!-- Card Amount -->
          <div v-if="paymentMethod === 'card'">
            <label class="block text-sm font-medium text-gray-700 mb-2">Card Amount</label>
            <input
              v-model.number="cardAmount"
              type="number"
              step="0.01"
              class="w-full px-4 py-3 text-2xl font-bold border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
              autofocus
            >
          </div>

          <!-- Mobile Amount -->
          <div v-if="paymentMethod === 'mobile'">
            <label class="block text-sm font-medium text-gray-700 mb-2">Mobile Payment Amount</label>
            <input
              v-model.number="mobileAmount"
              type="number"
              step="0.01"
              class="w-full px-4 py-3 text-2xl font-bold border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
              autofocus
            >
          </div>

          <!-- Change Display -->
          <div v-if="changeAmount > 0" class="bg-green-50 rounded-xl p-4 border border-green-200">
            <div class="flex justify-between items-center">
              <span class="text-sm font-medium text-green-700">Change</span>
              <span class="text-2xl font-bold text-green-700">{{ formatCurrency(changeAmount) }}</span>
            </div>
          </div>

          <!-- Insufficient Payment Warning -->
          <div v-if="totalPaid < posStore.cartTotal && totalPaid > 0" class="bg-red-50 rounded-xl p-4 border border-red-200">
            <div class="flex items-center gap-2">
              <i class="material-symbols-rounded text-red-600">warning</i>
              <span class="text-sm font-medium text-red-700">
                Still need {{ formatCurrency(posStore.cartTotal - totalPaid) }}
              </span>
            </div>
          </div>

          <button
            @click="completeSale"
            :disabled="!canCompleteSale"
            class="w-full py-4 bg-gradient-to-br from-green-500 to-green-600 text-white rounded-xl font-bold text-lg hover:shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
          >
            <i class="material-symbols-rounded">check_circle</i>
            Complete Sale
          </button>
        </div>
      </div>
    </div>

    <!-- Customer Selection Modal -->
    <div
      v-if="showCustomerModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showCustomerModal = false"
    >
      <div class="bg-white rounded-xl max-w-md w-full shadow-2xl">
        <div class="p-6 border-b">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-xl font-bold text-gray-900">Select Customer</h2>
            <button
              @click="showCustomerModal = false"
              class="p-2 text-gray-400 hover:text-gray-600 rounded-lg transition-colors"
            >
              <i class="material-symbols-rounded">close</i>
            </button>
          </div>
          <input
            v-model="customerSearch"
            type="text"
            placeholder="Search customer..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
            autofocus
          >
        </div>
        <div class="max-h-96 overflow-y-auto p-4">
          <div class="space-y-2">
            <button
              v-for="customer in filteredCustomers"
              :key="customer.id"
              @click="selectCustomer(customer)"
              class="w-full p-3 text-left rounded-lg hover:bg-gray-100 transition-colors border border-gray-200"
            >
              <p class="font-semibold text-gray-900">{{ customer.name }}</p>
              <p class="text-sm text-gray-600">{{ customer.phone }}</p>
            </button>
          </div>
          <button
            v-if="selectedCustomer"
            @click="clearCustomer"
            class="w-full mt-4 p-3 text-red-600 hover:bg-red-50 rounded-lg transition-colors border border-red-200"
          >
            Clear Selection
          </button>
        </div>
      </div>
    </div>

    <!-- Discount Modal -->
    <div
      v-if="showDiscountModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click.self="showDiscountModal = false"
    >
      <div class="bg-white rounded-xl max-w-sm w-full shadow-2xl">
        <div class="p-6 border-b">
          <div class="flex items-center justify-between">
            <h2 class="text-xl font-bold text-gray-900">Apply Discount</h2>
            <button
              @click="showDiscountModal = false"
              class="p-2 text-gray-400 hover:text-gray-600 rounded-lg transition-colors"
            >
              <i class="material-symbols-rounded">close</i>
            </button>
          </div>
        </div>
        <div class="p-6">
          <input
            v-model.number="discountAmount"
            type="number"
            step="0.01"
            placeholder="Discount amount"
            class="w-full px-4 py-3 text-lg border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500 mb-4"
            autofocus
          >
          <div class="flex gap-2">
            <button
              @click="applyDiscount(discountAmount || 0)"
              class="flex-1 py-3 bg-blue-600 text-white rounded-lg font-semibold hover:bg-blue-700 transition-colors"
            >
              Apply
            </button>
            <button
              @click="showDiscountModal = false"
              class="flex-1 py-3 bg-gray-200 text-gray-700 rounded-lg font-semibold hover:bg-gray-300 transition-colors"
            >
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Receipt -->
    <Receipt
      v-if="posStore.currentSale"
      :sale="posStore.currentSale"
      :show="showReceipt"
      @close="closeReceipt"
      @print="handlePrintReceipt"
    />
  </div>
</template>

<style scoped>
.scrollbar-hide {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
.scrollbar-hide::-webkit-scrollbar {
  display: none;
}
</style>
