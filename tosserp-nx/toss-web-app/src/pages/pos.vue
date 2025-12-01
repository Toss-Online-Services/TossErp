<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useIndexedDB } from '@/composables/useIndexedDB'
import { usePosSync } from '@/composables/usePosSync'
import { useNetworkStatus } from '@/composables/useNetworkStatus'

const { isOnline } = useNetworkStatus()
const {
  getCart,
  addToCart,
  updateCartItem,
  clearCart,
  searchCachedProducts
} = useIndexedDB()

const { queueSaleForSync, isSyncing, pendingSalesCount } = usePosSync()

const cart = ref<Array<{
  productId: number
  productName: string
  quantity: number
  unitPrice: number
  total: number
}>>([])

const searchQuery = ref('')
const searchResults = ref<Array<{
  id: number
  name: string
  sku: string
  price: number
  barcode?: string
}>>([])

const shopId = ref(1) // TODO: Get from auth/context
const customerId = ref<number | undefined>()
const paymentMethod = ref<'Cash' | 'Card' | 'Mobile'>('Cash')
const paymentAmount = ref(0)
const notes = ref('')

const cartTotal = computed(() => {
  return cart.value.reduce((sum, item) => sum + item.total, 0)
})

const change = computed(() => {
  if (paymentMethod.value === 'Cash') {
    return Math.max(0, paymentAmount.value - cartTotal.value)
  }
  return 0
})

const loadCart = async () => {
  cart.value = await getCart()
}

const handleSearch = async () => {
  if (!searchQuery.value.trim()) {
    searchResults.value = []
    return
  }

  // Search cached products
  const cached = await searchCachedProducts(searchQuery.value)
  searchResults.value = cached.slice(0, 10)

  // TODO: Also search API if online
  if (isOnline.value) {
    // Fetch from API and cache results
  }
}

const addProductToCart = async (product: typeof searchResults.value[0]) => {
  await addToCart({
    productId: product.id,
    productName: product.name,
    quantity: 1,
    unitPrice: product.price,
    total: product.price
  })
  await loadCart()
  searchQuery.value = ''
  searchResults.value = []
}

const updateQuantity = async (productId: number, delta: number) => {
  const item = cart.value.find(i => i.productId === productId)
  if (item) {
    const newQuantity = item.quantity + delta
    await updateCartItem(productId, newQuantity)
    await loadCart()
  }
}

const removeFromCart = async (productId: number) => {
  await updateCartItem(productId, 0)
  await loadCart()
}

const handleCheckout = async () => {
  if (cart.value.length === 0) {
    alert('Cart is empty')
    return
  }

  if (paymentMethod.value === 'Cash' && paymentAmount.value < cartTotal.value) {
    alert('Insufficient payment amount')
    return
  }

  try {
    const items = cart.value.map(item => ({
      productId: item.productId,
      quantity: item.quantity,
      unitPrice: item.unitPrice
    }))

    await queueSaleForSync(
      shopId.value,
      items,
      paymentMethod.value,
      customerId.value,
      undefined,
      notes.value
    )

    // Clear cart after queuing
    await clearCart()
    await loadCart()
    
    // Reset form
    paymentAmount.value = 0
    notes.value = ''
    customerId.value = undefined

    alert(isOnline.value ? 'Sale completed!' : 'Sale queued for sync when online')
  } catch (error) {
    console.error('Checkout error:', error)
    alert('Failed to process sale. Please try again.')
  }
}

// Watch for online status to trigger sync
watch(isOnline, async (online) => {
  if (online && !isSyncing.value) {
    const apiBaseUrl = useRuntimeConfig().public.apiBase || 'http://localhost:5000'
    const { syncQueuedSales } = usePosSync()
    await syncQueuedSales(apiBaseUrl)
  }
})

onMounted(() => {
  loadCart()
})
</script>

<template>
  <div class="min-h-screen bg-gray-50 p-4">
    <div class="max-w-7xl mx-auto">
      <!-- Header -->
      <div class="mb-4 flex items-center justify-between">
        <h1 class="text-2xl font-bold">Point of Sale</h1>
        <div class="flex items-center gap-4">
          <div class="flex items-center gap-2">
            <div
              :class="[
                'w-3 h-3 rounded-full',
                isOnline ? 'bg-green-500' : 'bg-red-500'
              ]"
            ></div>
            <span class="text-sm">
              {{ isOnline ? 'Online' : 'Offline' }}
            </span>
          </div>
          <div v-if="!isOnline" class="text-sm text-orange-600">
            {{ pendingSalesCount }} pending
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4">
        <!-- Left: Product Search & Cart -->
        <div class="lg:col-span-2 space-y-4">
          <!-- Product Search -->
          <div class="bg-white rounded-lg shadow p-4">
            <input
              v-model="searchQuery"
              @input="handleSearch"
              type="text"
              placeholder="Search products by name, SKU, or barcode..."
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            
            <div v-if="searchResults.length > 0" class="mt-2 space-y-1">
              <button
                v-for="product in searchResults"
                :key="product.id"
                @click="addProductToCart(product)"
                class="w-full text-left px-4 py-2 hover:bg-gray-100 rounded flex items-center justify-between"
              >
                <div>
                  <div class="font-medium">{{ product.name }}</div>
                  <div class="text-sm text-gray-500">{{ product.sku }}</div>
                </div>
                <div class="font-semibold">R{{ product.price.toFixed(2) }}</div>
              </button>
            </div>
          </div>

          <!-- Cart -->
          <div class="bg-white rounded-lg shadow p-4">
            <h2 class="text-lg font-semibold mb-4">Cart</h2>
            
            <div v-if="cart.length === 0" class="text-center py-8 text-gray-500">
              Cart is empty
            </div>

            <div v-else class="space-y-2">
              <div
                v-for="item in cart"
                :key="item.productId"
                class="flex items-center justify-between p-3 border-b"
              >
                <div class="flex-1">
                  <div class="font-medium">{{ item.productName }}</div>
                  <div class="text-sm text-gray-500">
                    R{{ item.unitPrice.toFixed(2) }} each
                  </div>
                </div>
                
                <div class="flex items-center gap-3">
                  <button
                    @click="updateQuantity(item.productId, -1)"
                    class="px-3 py-1 bg-gray-200 rounded hover:bg-gray-300"
                  >
                    -
                  </button>
                  <span class="w-12 text-center">{{ item.quantity }}</span>
                  <button
                    @click="updateQuantity(item.productId, 1)"
                    class="px-3 py-1 bg-gray-200 rounded hover:bg-gray-300"
                  >
                    +
                  </button>
                  <span class="w-24 text-right font-semibold">
                    R{{ item.total.toFixed(2) }}
                  </span>
                  <button
                    @click="removeFromCart(item.productId)"
                    class="px-2 py-1 text-red-600 hover:bg-red-50 rounded"
                  >
                    ×
                  </button>
                </div>
              </div>

              <div class="mt-4 pt-4 border-t">
                <div class="flex justify-between text-lg font-bold">
                  <span>Total:</span>
                  <span>R{{ cartTotal.toFixed(2) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Right: Payment -->
        <div class="lg:col-span-1">
          <div class="bg-white rounded-lg shadow p-4 space-y-4 sticky top-4">
            <h2 class="text-lg font-semibold">Payment</h2>

            <!-- Payment Method -->
            <div>
              <label class="block text-sm font-medium mb-2">Payment Method</label>
              <select
                v-model="paymentMethod"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="Cash">Cash</option>
                <option value="Card">Card</option>
                <option value="Mobile">Mobile</option>
              </select>
            </div>

            <!-- Cash Amount -->
            <div v-if="paymentMethod === 'Cash'">
              <label class="block text-sm font-medium mb-2">Amount Received</label>
              <input
                v-model.number="paymentAmount"
                type="number"
                step="0.01"
                min="0"
                :placeholder="`R${cartTotal.toFixed(2)}`"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <div v-if="change > 0" class="mt-2 text-green-600 font-semibold">
                Change: R{{ change.toFixed(2) }}
              </div>
            </div>

            <!-- Notes -->
            <div>
              <label class="block text-sm font-medium mb-2">Notes (optional)</label>
              <textarea
                v-model="notes"
                rows="3"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              ></textarea>
            </div>

            <!-- Checkout Button -->
            <button
              @click="handleCheckout"
              :disabled="cart.length === 0 || (paymentMethod === 'Cash' && paymentAmount < cartTotal)"
              class="w-full py-3 bg-blue-600 text-white rounded-lg font-semibold hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed"
            >
              {{ isOnline ? 'Complete Sale' : 'Queue Sale' }}
            </button>

            <div v-if="isSyncing" class="text-sm text-blue-600 text-center">
              Syncing...
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

