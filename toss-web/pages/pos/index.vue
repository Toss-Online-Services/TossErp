<script setup lang="ts">
import { ref } from 'vue'
import { Search, ShoppingCart, CreditCard } from 'lucide-vue-next'
import AppCard from '~/components/common/AppCard.vue'

useHead({
  title: 'POS - TOSS Admin',
  meta: [{ name: 'description', content: 'Point of Sale for TOSS Admin' }]
})

definePageMeta({
  layout: 'dashboard'
})

const searchQuery = ref('')
const cart = ref([
  { id: 1, name: 'White Bread', price: 12.50, quantity: 2 },
  { id: 2, name: 'Fresh Milk 1L', price: 18.90, quantity: 1 }
])

import { computed } from 'vue'

const total = computed(() => {
  return cart.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
})

const products = ref([
  { id: 1, name: 'White Bread', price: 12.50, stock: 45 },
  { id: 2, name: 'Fresh Milk 1L', price: 18.90, stock: 32 },
  { id: 3, name: 'Sugar 2.5kg', price: 45.00, stock: 28 },
  { id: 4, name: 'Cooking Oil 750ml', price: 35.50, stock: 15 }
])

const filteredProducts = computed(() => {
  if (!searchQuery.value) return products.value
  return products.value.filter(p => 
    p.name.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})
</script>

<template>
  <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
    <!-- Product Search & Selection -->
    <div class="lg:col-span-2 space-y-4">
      <AppCard>
        <div class="relative mb-4">
          <Search :size="18" class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search products..."
            class="w-full pl-10 pr-4 py-2 border border-border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>
        <div class="grid grid-cols-2 md:grid-cols-3 gap-3">
          <button
            v-for="product in filteredProducts"
            :key="product.id"
            class="p-4 border border-border rounded-lg hover:bg-muted transition-colors text-left"
          >
            <p class="font-medium text-foreground">{{ product.name }}</p>
            <p class="text-sm text-muted-foreground mt-1">R{{ product.price.toFixed(2) }}</p>
            <p class="text-xs text-muted-foreground mt-1">Stock: {{ product.stock }}</p>
          </button>
        </div>
      </AppCard>
    </div>

    <!-- Cart & Payment Panel -->
    <div class="space-y-4">
      <!-- Cart -->
      <AppCard title="Cart">
        <div v-if="cart.length === 0" class="text-center py-8 text-muted-foreground text-sm">
          Cart is empty
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="item in cart"
            :key="item.id"
            class="flex items-center justify-between p-3 bg-muted rounded-lg"
          >
            <div>
              <p class="text-sm font-medium text-foreground">{{ item.name }}</p>
              <p class="text-xs text-muted-foreground">R{{ item.price.toFixed(2) }} × {{ item.quantity }}</p>
            </div>
            <p class="text-sm font-medium text-foreground">R{{ (item.price * item.quantity).toFixed(2) }}</p>
          </div>
          <div class="border-t border-border pt-3 mt-3">
            <div class="flex items-center justify-between mb-2">
              <span class="text-sm font-medium text-foreground">Total</span>
              <span class="text-lg font-bold text-foreground">R{{ total.toFixed(2) }}</span>
            </div>
          </div>
        </div>
      </AppCard>

      <!-- Payment Panel -->
      <AppCard title="Payment">
        <div class="space-y-3">
          <button class="w-full px-4 py-3 bg-primary text-primary-foreground rounded-lg font-medium hover:opacity-90 transition-opacity">
            Cash Payment
          </button>
          <button class="w-full px-4 py-3 border border-border rounded-lg font-medium hover:bg-muted transition-colors">
            Card Payment
          </button>
          <button class="w-full px-4 py-3 border border-border rounded-lg font-medium hover:bg-muted transition-colors">
            Mobile Payment
          </button>
        </div>
      </AppCard>
    </div>
  </div>
</template>

