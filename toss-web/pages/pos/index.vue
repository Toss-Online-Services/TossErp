<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { usePosStore } from '~/stores/pos'
import { useStockStore } from '~/stores/stock'
import { useOfflineSync } from '~/composables/useOfflineSync'

useHead({
  title: 'Point of Sale - TOSS'
})

const posStore = usePosStore()
const stockStore = useStockStore()
const { isOnline, pendingCount } = useOfflineSync()

const searchQuery = ref('')
const showPaymentModal = ref(false)
const cashAmount = ref(0)
const selectedCategory = ref('all')

// Computed
const filteredItems = computed(() => {
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
  const cats = new Set(stockStore.items.map(item => item.category))
  return ['all', ...Array.from(cats)]
})

const changeAmount = computed(() => {
  return Math.max(0, cashAmount.value - posStore.cartTotal)
})

// Load data
onMounted(() => {
  stockStore.fetchItems()
  posStore.fetchRecentSales()
})

// Methods
function addItem(item: any) {
  posStore.addToCart(item)
}

function removeItem(cartItemId: string) {
  posStore.removeFromCart(cartItemId)
}

function updateItemQuantity(cartItemId: string, quantity: number) {
  posStore.updateQuantity(cartItemId, quantity)
}

function openPayment() {
  if (posStore.cart.length === 0) return
  showPaymentModal.value = true
  cashAmount.value = Math.ceil(posStore.cartTotal)
}

async function completeSale() {
  try {
    if (cashAmount.value < posStore.cartTotal) {
      alert('Insufficient payment amount')
      return
    }

    const payments = [
      {
        type: 'cash' as const,
        amount: cashAmount.value
      }
    ]

    await posStore.completeSale(payments)
    
    showPaymentModal.value = false
    cashAmount.value = 0
    
    // Show receipt or print
    alert(`Sale completed! Invoice: ${posStore.currentSale?.invoiceNumber}`)
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
</script>

<template>
  <div class="h-screen flex flex-col bg-gray-100">
    <!-- Top Bar -->
    <div class="bg-white shadow-sm px-4 py-3 flex items-center justify-between">
      <div class="flex items-center gap-4">
        <h1 class="text-xl font-bold text-gray-900">Point of Sale</h1>
        <div v-if="!isOnline" class="flex items-center gap-2 px-3 py-1 bg-orange-100 text-orange-700 rounded-full text-sm">
          <i class="material-symbols-rounded text-sm">cloud_off</i>
          <span>Offline Mode</span>
        </div>
        <div v-if="pendingCount > 0" class="flex items-center gap-2 px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-sm">
          <i class="material-symbols-rounded text-sm">sync</i>
          <span>{{ pendingCount }} pending</span>
        </div>
      </div>
      <div class="flex items-center gap-2">
        <button
          @click="posStore.holdSale()"
          class="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors"
        >
          <i class="material-symbols-rounded">pause</i>
        </button>
        <button class="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors">
          <i class="material-symbols-rounded">receipt_long</i>
        </button>
      </div>
    </div>

    <div class="flex-1 flex overflow-hidden">
      <!-- Left: Products -->
      <div class="flex-1 flex flex-col overflow-hidden">
        <!-- Search and Filters -->
        <div class="bg-white p-4 border-b">
          <div class="flex gap-4 mb-4">
            <div class="flex-1 relative">
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search products (name, code, barcode)..."
                class="w-full px-4 py-3 pr-12 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500 text-lg"
                autofocus
              >
              <i class="material-symbols-rounded absolute right-4 top-1/2 -translate-y-1/2 text-gray-400">search</i>
            </div>
            <button class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 flex items-center gap-2">
              <i class="material-symbols-rounded">qr_code_scanner</i>
              <span>Scan</span>
            </button>
          </div>

          <!-- Category Tabs -->
          <div class="flex gap-2 overflow-x-auto pb-2">
            <button
              v-for="cat in categories"
              :key="cat"
              @click="selectedCategory = cat"
              :class="[
                'px-4 py-2 rounded-lg whitespace-nowrap transition-colors',
                selectedCategory === cat
                  ? 'bg-blue-600 text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              {{ cat === 'all' ? 'All' : cat }}
            </button>
          </div>
        </div>

        <!-- Products Grid -->
        <div class="flex-1 overflow-y-auto p-4">
          <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-4">
            <button
              v-for="item in filteredItems"
              :key="item.id"
              @click="addItem(item)"
              class="bg-white rounded-lg p-4 hover:shadow-lg transition-shadow text-left"
            >
              <div class="aspect-square bg-gray-100 rounded-lg mb-3 flex items-center justify-center">
                <i class="material-symbols-rounded text-4xl text-gray-400">inventory_2</i>
              </div>
              <h3 class="font-semibold text-gray-900 mb-1 line-clamp-2">{{ item.name }}</h3>
              <p class="text-sm text-gray-600 mb-2">{{ item.code }}</p>
              <div class="flex items-center justify-between">
                <span class="text-lg font-bold text-blue-600">{{ formatCurrency(item.sellingPrice) }}</span>
                <span class="text-xs text-gray-500">{{ item.currentStock }} {{ item.unit }}</span>
              </div>
            </button>
          </div>

          <!-- Empty State -->
          <div v-if="filteredItems.length === 0" class="text-center py-12">
            <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">inventory_2</i>
            <p class="text-gray-600">No products found</p>
          </div>
        </div>
      </div>

      <!-- Right: Cart -->
      <div class="w-96 bg-white border-l flex flex-col">
        <!-- Cart Header -->
        <div class="p-4 border-b">
          <div class="flex items-center justify-between mb-2">
            <h2 class="text-lg font-bold text-gray-900">Current Sale</h2>
            <button
              v-if="posStore.cart.length > 0"
              @click="posStore.clearCart()"
              class="text-red-600 hover:text-red-700"
            >
              <i class="material-symbols-rounded">delete</i>
            </button>
          </div>
          <p class="text-sm text-gray-600">{{ posStore.cartItemCount }} items</p>
        </div>

        <!-- Cart Items -->
        <div class="flex-1 overflow-y-auto p-4">
          <div v-if="posStore.cart.length === 0" class="text-center py-12">
            <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">shopping_cart</i>
            <p class="text-gray-600">Cart is empty</p>
            <p class="text-sm text-gray-500 mt-2">Add items to start a sale</p>
          </div>

          <div v-else class="space-y-3">
            <div
              v-for="item in posStore.cart"
              :key="item.id"
              class="bg-gray-50 rounded-lg p-3"
            >
              <div class="flex items-start justify-between mb-2">
                <div class="flex-1">
                  <h3 class="font-semibold text-gray-900 text-sm">{{ item.name }}</h3>
                  <p class="text-xs text-gray-600">{{ item.code }}</p>
                </div>
                <button
                  @click="removeItem(item.id)"
                  class="text-red-600 hover:text-red-700"
                >
                  <i class="material-symbols-rounded text-lg">close</i>
                </button>
              </div>

              <div class="flex items-center justify-between">
                <div class="flex items-center gap-2">
                  <button
                    @click="updateItemQuantity(item.id, item.quantity - 1)"
                    class="w-8 h-8 bg-white rounded border border-gray-300 flex items-center justify-center hover:bg-gray-100"
                  >
                    <i class="material-symbols-rounded text-sm">remove</i>
                  </button>
                  <span class="w-12 text-center font-semibold">{{ item.quantity }}</span>
                  <button
                    @click="updateItemQuantity(item.id, item.quantity + 1)"
                    class="w-8 h-8 bg-white rounded border border-gray-300 flex items-center justify-center hover:bg-100"
                  >
                    <i class="material-symbols-rounded text-sm">add</i>
                  </button>
                </div>
                <span class="font-bold text-gray-900">{{ formatCurrency(item.total) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Cart Summary -->
        <div class="border-t p-4 space-y-3">
          <div class="flex justify-between text-sm">
            <span class="text-gray-600">Subtotal</span>
            <span class="font-semibold">{{ formatCurrency(posStore.cartSubtotal) }}</span>
          </div>
          <div v-if="posStore.cartDiscount > 0" class="flex justify-between text-sm">
            <span class="text-gray-600">Discount</span>
            <span class="font-semibold text-red-600">-{{ formatCurrency(posStore.cartDiscount) }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-600">VAT (15%)</span>
            <span class="font-semibold">{{ formatCurrency(posStore.cartTax) }}</span>
          </div>
          <div class="flex justify-between text-lg font-bold pt-3 border-t">
            <span>Total</span>
            <span class="text-blue-600">{{ formatCurrency(posStore.cartTotal) }}</span>
          </div>

          <button
            @click="openPayment"
            :disabled="posStore.cart.length === 0"
            class="w-full py-4 bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-lg font-bold text-lg hover:shadow-lg transition-shadow disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Charge {{ formatCurrency(posStore.cartTotal) }}
          </button>
        </div>
      </div>
    </div>

    <!-- Payment Modal -->
    <div
      v-if="showPaymentModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
    >
      <div class="bg-white rounded-xl max-w-md w-full p-6">
        <div class="flex items-center justify-between mb-6">
          <h2 class="text-2xl font-bold text-gray-900">Payment</h2>
          <button
            @click="showPaymentModal = false"
            class="text-gray-400 hover:text-gray-600"
          >
            <i class="material-symbols-rounded">close</i>
          </button>
        </div>

        <div class="space-y-6">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Total Amount</label>
            <div class="text-3xl font-bold text-blue-600">{{ formatCurrency(posStore.cartTotal) }}</div>
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Cash Received</label>
            <input
              v-model.number="cashAmount"
              type="number"
              step="0.01"
              class="w-full px-4 py-3 text-2xl font-bold border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
              autofocus
            >
          </div>

          <div v-if="cashAmount >= posStore.cartTotal" class="bg-green-50 rounded-lg p-4">
            <div class="flex justify-between items-center">
              <span class="text-sm font-medium text-green-700">Change</span>
              <span class="text-2xl font-bold text-green-700">{{ formatCurrency(changeAmount) }}</span>
            </div>
          </div>

          <button
            @click="completeSale"
            :disabled="cashAmount < posStore.cartTotal"
            class="w-full py-4 bg-gradient-to-br from-green-500 to-green-600 text-white rounded-lg font-bold text-lg hover:shadow-lg transition-shadow disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Complete Sale
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

