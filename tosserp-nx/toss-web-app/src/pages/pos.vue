<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useIndexedDB } from '@/composables/useIndexedDB'
import { usePosSync } from '@/composables/usePosSync'
import { useNetworkStatus } from '@/composables/useNetworkStatus'
import { useSalesApi } from '@/composables/useSalesApi'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { Search, Plus, Minus, X, ShoppingCart, CreditCard, Wifi, WifiOff, CheckCircle2 } from 'lucide-vue-next'

const { isOnline } = useNetworkStatus()
const {
  getCart,
  addToCart,
  updateCartItem,
  clearCart,
  searchCachedProducts
} = useIndexedDB()

const { queueSaleForSync, isSyncing, pendingSalesCount } = usePosSync()
const { posCheckout, isLoading: isApiLoading } = useSalesApi()

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

const showSuccess = ref(false)
const successMessage = ref('')

const handleCheckout = async () => {
  if (cart.value.length === 0) {
    return
  }

  if (paymentMethod.value === 'Cash' && paymentAmount.value < cartTotal.value) {
    return
  }

  try {
    const items = cart.value.map(item => ({
      productId: item.productId,
      quantity: item.quantity,
      unitPrice: item.unitPrice
    }))

    // If online, try direct API call first
    if (isOnline.value) {
      try {
        const result = await posCheckout({
          shopId: shopId.value,
          customerId: customerId.value,
          paymentMethod: paymentMethod.value as any,
          notes: notes.value,
          items,
          idempotencyKey: `pos-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
        })

        // Success - clear cart
        await clearCart()
        await loadCart()
        paymentAmount.value = 0
        notes.value = ''
        customerId.value = undefined

        showSuccess.value = true
        successMessage.value = `Sale ${result.saleNumber} completed!`
        setTimeout(() => { showSuccess.value = false }, 3000)
        return
      } catch (apiError) {
        // If API fails, fall back to queue
        console.warn('Direct API call failed, queuing:', apiError)
      }
    }

    // Queue for sync (offline or API failed)
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

    showSuccess.value = true
    successMessage.value = isOnline.value ? 'Sale completed!' : 'Sale queued for sync when online'
    setTimeout(() => { showSuccess.value = false }, 3000)
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
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Point of Sale</h1>
        <p class="text-muted-foreground mt-1">Quick checkout for walk-in customers</p>
      </div>
      <div class="flex items-center gap-3">
        <div
          class="flex items-center gap-2 px-3 py-1.5 rounded-full text-xs font-medium border"
          :class="isOnline ? 'text-emerald-600 border-emerald-200 bg-emerald-50' : 'text-amber-600 border-amber-200 bg-amber-50'"
        >
          <component :is="isOnline ? Wifi : WifiOff" :size="14" />
          <span>{{ isOnline ? 'Online' : 'Offline' }}</span>
        </div>
        <div v-if="!isOnline && pendingSalesCount" class="text-sm text-amber-600 font-medium">
          {{ pendingSalesCount }} pending
        </div>
      </div>
    </div>

    <!-- Success Message -->
    <div
      v-if="showSuccess"
      class="flex items-center gap-2 p-4 bg-emerald-50 border border-emerald-200 rounded-lg text-emerald-800"
    >
      <CheckCircle2 :size="20" />
      <span class="font-medium">{{ successMessage }}</span>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Left: Product Search & Cart -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Product Search -->
        <Card>
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <Search :size="18" />
              Search Products
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="relative">
              <Search
                :size="18"
                class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
              />
              <input
                v-model="searchQuery"
                @input="handleSearch"
                type="text"
                placeholder="Search by name, SKU, or scan barcode..."
                class="w-full pl-10 pr-4 py-2.5 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            <div v-if="searchResults.length > 0" class="mt-3 space-y-1 max-h-64 overflow-y-auto">
              <button
                v-for="product in searchResults"
                :key="product.id"
                @click="addProductToCart(product)"
                class="w-full text-left px-4 py-3 hover:bg-accent rounded-lg transition-colors flex items-center justify-between border border-transparent hover:border-primary/20"
              >
                <div>
                  <div class="font-medium text-foreground">{{ product.name }}</div>
                  <div class="text-sm text-muted-foreground">{{ product.sku }}</div>
                </div>
                <div class="font-semibold text-primary">R{{ product.price.toFixed(2) }}</div>
              </button>
            </div>
          </CardContent>
        </Card>

        <!-- Cart -->
        <Card>
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <ShoppingCart :size="18" />
              Cart ({{ cart.length }})
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div v-if="cart.length === 0" class="text-center py-12 text-muted-foreground">
              <ShoppingCart :size="48" class="mx-auto mb-3 opacity-50" />
              <p>Cart is empty</p>
              <p class="text-sm mt-1">Search and add products to get started</p>
            </div>

            <div v-else class="space-y-3">
              <div
                v-for="item in cart"
                :key="item.productId"
                class="flex items-center justify-between p-3 border rounded-lg hover:bg-accent/50 transition-colors"
              >
                <div class="flex-1">
                  <div class="font-medium text-foreground">{{ item.productName }}</div>
                  <div class="text-sm text-muted-foreground">
                    R{{ item.unitPrice.toFixed(2) }} each
                  </div>
                </div>

                <div class="flex items-center gap-3">
                  <div class="flex items-center gap-2 border rounded-md">
                    <button
                      @click="updateQuantity(item.productId, -1)"
                      class="p-1.5 hover:bg-accent rounded-l transition-colors"
                      :disabled="item.quantity <= 1"
                    >
                      <Minus :size="16" />
                    </button>
                    <span class="w-12 text-center font-medium">{{ item.quantity }}</span>
                    <button
                      @click="updateQuantity(item.productId, 1)"
                      class="p-1.5 hover:bg-accent rounded-r transition-colors"
                    >
                      <Plus :size="16" />
                    </button>
                  </div>
                  <span class="w-24 text-right font-semibold text-foreground">
                    R{{ item.total.toFixed(2) }}
                  </span>
                  <button
                    @click="removeFromCart(item.productId)"
                    class="p-2 text-red-600 hover:bg-red-50 rounded transition-colors"
                    aria-label="Remove item"
                  >
                    <X :size="18" />
                  </button>
                </div>
              </div>

              <div class="mt-4 pt-4 border-t">
                <div class="flex justify-between items-center">
                  <span class="text-lg font-semibold text-foreground">Total:</span>
                  <span class="text-2xl font-bold text-primary">R{{ cartTotal.toFixed(2) }}</span>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <!-- Right: Payment -->
      <div class="lg:col-span-1">
        <Card class="sticky top-4">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <CreditCard :size="18" />
              Payment
            </CardTitle>
          </CardHeader>
          <CardContent class="space-y-4">
            <!-- Payment Method -->
            <div>
              <label class="block text-sm font-medium mb-2 text-foreground">Payment Method</label>
              <select
                v-model="paymentMethod"
                class="w-full px-4 py-2.5 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              >
                <option value="Cash">Cash</option>
                <option value="Card">Card</option>
                <option value="Mobile">Mobile</option>
              </select>
            </div>

            <!-- Cash Amount -->
            <div v-if="paymentMethod === 'Cash'">
              <label class="block text-sm font-medium mb-2 text-foreground">Amount Received</label>
              <div class="relative">
                <span class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">R</span>
                <input
                  v-model.number="paymentAmount"
                  type="number"
                  step="0.01"
                  min="0"
                  :placeholder="cartTotal.toFixed(2)"
                  class="w-full pl-8 pr-4 py-2.5 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                />
              </div>
              <div v-if="change > 0" class="mt-2 p-2 bg-emerald-50 border border-emerald-200 rounded text-emerald-700 font-semibold">
                Change: R{{ change.toFixed(2) }}
              </div>
              <div v-else-if="paymentAmount > 0 && paymentAmount < cartTotal" class="mt-2 text-sm text-red-600">
                Insufficient amount
              </div>
            </div>

            <!-- Notes -->
            <div>
              <label class="block text-sm font-medium mb-2 text-foreground">Notes (optional)</label>
              <textarea
                v-model="notes"
                rows="3"
                placeholder="Add any notes about this sale..."
                class="w-full px-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary resize-none"
              ></textarea>
            </div>

            <!-- Checkout Button -->
            <Button
              @click="handleCheckout"
              :disabled="cart.length === 0 || (paymentMethod === 'Cash' && paymentAmount < cartTotal) || isApiLoading || isSyncing"
              class="w-full py-3 text-base font-semibold"
              size="lg"
            >
              <span v-if="isApiLoading || isSyncing">Processing...</span>
              <span v-else>{{ isOnline ? 'Complete Sale' : 'Queue Sale' }}</span>
            </Button>

            <div v-if="isSyncing" class="text-sm text-primary text-center">
              Syncing queued sales...
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  </div>
</template>

