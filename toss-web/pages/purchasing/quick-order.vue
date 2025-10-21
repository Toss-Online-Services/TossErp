<template>
  <div class="min-h-screen bg-gray-50 pb-20">
    <div class="p-4 space-y-4 sm:p-6 sm:space-y-6">
      <!-- Header -->
      <div class="flex items-center gap-3 mb-6">
        <NuxtLink to="/" class="flex items-center justify-center w-10 h-10 bg-white rounded-lg border-2 border-gray-200 hover:border-blue-500 transition-colors touch-manipulation">
          <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
          </svg>
        </NuxtLink>
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Order Stock</h1>
          <p class="text-base text-gray-600">Select items for your shop</p>
        </div>
      </div>
      
      <!-- Popular Products -->
      <div class="bg-white rounded-xl border-2 border-gray-200 p-5 shadow-sm">
        <h2 class="text-xl font-bold text-gray-900 mb-4">Popular Items</h2>
        <div class="grid grid-cols-2 sm:grid-cols-3 gap-3">
          <div 
            v-for="product in products"
            :key="product.id"
            class="bg-gray-50 rounded-lg p-4 border-2 border-gray-200 hover:border-blue-500 transition-all cursor-pointer"
            @click="addToOrder(product)"
          >
            <div class="text-4xl mb-2 text-center">{{ product.emoji }}</div>
            <p class="font-bold text-base text-gray-900 text-center mb-1">{{ product.name }}</p>
            <p class="text-sm text-gray-600 text-center mb-2">{{ product.unit }}</p>
            <p class="text-lg font-bold text-blue-600 text-center">R {{ product.price }}</p>
          </div>
        </div>
      </div>
      
      <!-- Order Summary -->
      <div v-if="orderItems.length > 0" class="bg-blue-50 rounded-xl border-2 border-blue-200 p-5 shadow-sm sticky bottom-20">
        <div class="flex justify-between items-center mb-3">
          <h3 class="text-xl font-bold text-gray-900">Your Order</h3>
          <button @click="orderItems = []" class="text-red-600 font-medium text-sm">Clear All</button>
        </div>
        <div class="space-y-2 mb-3">
          <div 
            v-for="item in orderItems"
            :key="item.id"
            class="flex justify-between items-center bg-white rounded-lg p-3"
          >
            <div class="flex items-center gap-2 min-w-0 flex-1">
              <span class="text-2xl flex-shrink-0">{{ item.emoji }}</span>
              <div class="min-w-0">
                <p class="font-bold text-gray-900 text-sm">{{ item.name }}</p>
                <p class="text-xs text-gray-600">{{ item.unit }}</p>
              </div>
            </div>
            <div class="flex items-center gap-2 flex-shrink-0">
              <button @click="removeFromOrder(item.id)" class="w-8 h-8 bg-red-100 rounded-lg flex items-center justify-center touch-manipulation">
                <svg class="w-4 h-4 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4" />
                </svg>
              </button>
              <span class="font-bold text-base text-gray-900 min-w-[60px] text-right">R {{ item.price }}</span>
            </div>
          </div>
        </div>
        <div class="border-t-2 border-blue-200 pt-3 mb-4">
          <div class="flex justify-between items-center">
            <span class="text-lg font-bold text-gray-900">Total:</span>
            <span class="text-2xl font-bold text-blue-600">R {{ totalPrice }}</span>
          </div>
        </div>
        <button
          @click="placeOrder"
          class="w-full px-6 py-4 bg-blue-600 text-white rounded-xl font-bold text-lg shadow-lg hover:bg-blue-700 transition-all touch-manipulation min-h-[56px] flex items-center justify-center gap-2"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <span>Place Order ({{ orderItems.length }} items)</span>
        </button>
      </div>
      
      <!-- Empty State -->
      <div v-else class="bg-white rounded-xl border-2 border-gray-200 p-8 text-center shadow-sm">
        <div class="w-20 h-20 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
          <svg class="w-10 h-10 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
          </svg>
        </div>
        <p class="text-lg text-gray-600 font-medium">No items in your order yet</p>
        <p class="text-base text-gray-500 mt-1">Tap on items above to add them to your order</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const products = ref([
  { id: 1, name: 'White Bread', unit: '1 loaf', price: 12, emoji: '🍞' },
  { id: 2, name: 'Brown Bread', unit: '1 loaf', price: 14, emoji: '🥖' },
  { id: 3, name: 'Milk (Full Cream)', unit: '1L', price: 18, emoji: '🥛' },
  { id: 4, name: 'Coca-Cola', unit: '2L', price: 22, emoji: '🥤' },
  { id: 5, name: 'Chips', unit: '120g', price: 15, emoji: '🍟' },
  { id: 6, name: 'Sugar', unit: '2.5kg', price: 35, emoji: '🧂' },
  { id: 7, name: 'Maize Meal', unit: '10kg', price: 85, emoji: '🌽' },
  { id: 8, name: 'Rice', unit: '2kg', price: 45, emoji: '🍚' },
  { id: 9, name: 'Cooking Oil', unit: '750ml', price: 28, emoji: '🫗' },
  { id: 10, name: 'Eggs', unit: '1 dozen', price: 42, emoji: '🥚' },
  { id: 11, name: 'Soap', unit: '1 bar', price: 8, emoji: '🧼' },
  { id: 12, name: 'Airtime', unit: 'R5-R100', price: 50, emoji: '📱' },
])

const orderItems = ref([])

const totalPrice = computed(() => {
  return orderItems.value.reduce((sum, item) => sum + item.price, 0)
})

const addToOrder = (product) => {
  orderItems.value.push({ ...product })
}

const removeFromOrder = (itemId) => {
  const index = orderItems.value.findIndex((item) => item.id === itemId)
  if (index > -1) {
    orderItems.value.splice(index, 1)
  }
}

const placeOrder = () => {
  if (orderItems.value.length > 0) {
    // Store order in localStorage for confirmation page
    localStorage.setItem('toss-current-order', JSON.stringify({
      items: orderItems.value,
      total: totalPrice.value,
      orderNumber: 'ORD' + Date.now(),
      date: new Date().toISOString()
    }))
    
    // Navigate to confirmation
    router.push('/stock/order-confirmation')
  }
}
</script>

