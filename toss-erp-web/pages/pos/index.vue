<script setup lang="ts">
import { useOfflineQueue } from '~/composables/useOfflineQueue'

const { data: products } = await useFetch('/api/stock/items')
const cart = ref<any[]>([])
const queue = useOfflineQueue()

const addToCart = (product: any) => {
  cart.value.push({ ...product, qty: 1 })
}

const total = computed(() =>
  cart.value.reduce((sum, line) => sum + (line.price || 0) * (line.qty || 1), 0)
)

const checkout = () => {
  queue.enqueue('pos-sale', {
    lines: cart.value,
    total: total.value,
    timestamp: Date.now()
  })
  cart.value = []
  alert('Sale queued. Will sync when online.')
}
</script>

<template>
  <div class="grid gap-4 lg:grid-cols-3">
    <div class="lg:col-span-2 space-y-3">
      <div class="flex items-center justify-between">
        <div>
          <p class="text-sm text-muted-foreground">POS</p>
          <h1 class="text-xl font-semibold">Products</h1>
        </div>
      </div>
      <div class="grid gap-3 sm:grid-cols-2 xl:grid-cols-3">
        <div
          v-for="product in products || []"
          :key="product.id"
          class="card-surface p-3 space-y-2"
        >
          <p class="font-semibold">{{ product.name }}</p>
          <p class="text-sm text-muted-foreground">R {{ product.price }}</p>
          <p class="text-xs text-muted-foreground">Stock: {{ product.qty }}</p>
          <button
            class="w-full rounded-lg bg-primary text-primary-foreground px-3 py-2 text-sm font-medium hover:opacity-90"
            @click="addToCart(product)"
          >
            Add
          </button>
        </div>
      </div>
    </div>

    <div class="card-surface p-4 space-y-3">
      <h3 class="text-lg font-semibold">Cart</h3>
      <div v-if="cart.length === 0" class="text-sm text-muted-foreground">No items yet</div>
      <div v-else class="space-y-2">
        <div
          v-for="line in cart"
          :key="line.id + line.qty"
          class="flex items-center justify-between text-sm"
        >
          <span class="font-medium">{{ line.name }}</span>
          <span>R {{ (line.price || 0) * (line.qty || 1) }}</span>
        </div>
      </div>
      <div class="flex items-center justify-between text-base font-semibold pt-2 border-t border-border/60">
        <span>Total</span>
        <span>R {{ total }}</span>
      </div>
      <button
        class="w-full rounded-lg bg-secondary text-secondary-foreground px-3 py-2 text-sm font-medium hover:opacity-90"
        :disabled="cart.length === 0"
        @click="checkout"
      >
        Checkout (offline capable)
      </button>
      <p class="text-xs text-muted-foreground">
        Transactions are queued locally and will sync when online.
      </p>
    </div>
  </div>
</template>

