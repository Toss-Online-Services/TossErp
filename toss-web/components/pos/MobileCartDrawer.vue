<script setup lang="ts">
import { computed } from 'vue'
import { usePosStore } from '~/stores/pos'

interface Props {
  isOpen: boolean
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  checkout: []
}>()

const posStore = usePosStore()

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function removeItem(cartItemId: string) {
  posStore.removeFromCart(cartItemId)
}

function updateQuantity(cartItemId: string, quantity: number) {
  posStore.updateQuantity(cartItemId, quantity)
}
</script>

<template>
  <!-- Mobile Cart Drawer -->
  <div
    :class="[
      'fixed inset-0 z-50 lg:hidden transition-transform duration-300',
      isOpen ? 'translate-x-0' : 'translate-x-full'
    ]"
  >
    <!-- Backdrop -->
    <div
      v-if="isOpen"
      class="absolute inset-0 bg-black bg-opacity-50"
      @click="emit('close')"
    ></div>

    <!-- Drawer -->
    <div class="absolute right-0 top-0 bottom-0 w-full max-w-sm bg-white shadow-2xl flex flex-col">
      <!-- Header -->
      <div class="p-4 border-b bg-gradient-to-br from-gray-800 to-gray-900 text-white">
        <div class="flex items-center justify-between mb-2">
          <h2 class="text-lg font-bold">Cart</h2>
          <button
            @click="emit('close')"
            class="p-2 hover:bg-white/10 rounded-lg transition-colors"
          >
            <i class="material-symbols-rounded">close</i>
          </button>
        </div>
        <p class="text-sm text-white/80">{{ posStore.cartItemCount }} items</p>
      </div>

      <!-- Cart Items -->
      <div class="flex-1 overflow-y-auto p-4">
        <div v-if="posStore.cart.length === 0" class="text-center py-12">
          <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">shopping_cart</i>
          <p class="text-gray-600">Cart is empty</p>
        </div>

        <div v-else class="space-y-3">
          <div
            v-for="item in posStore.cart"
            :key="item.id"
            class="bg-gray-50 rounded-xl p-3"
          >
            <div class="flex items-start justify-between mb-2">
              <div class="flex-1">
                <h3 class="font-semibold text-gray-900 text-sm">{{ item.name }}</h3>
                <p class="text-xs text-gray-600">{{ item.code }}</p>
              </div>
              <button
                @click="removeItem(item.id)"
                class="text-red-600 hover:text-red-700 p-1"
              >
                <i class="material-symbols-rounded text-lg">close</i>
              </button>
            </div>

            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2">
                <button
                  @click="updateQuantity(item.id, item.quantity - 1)"
                  class="w-8 h-8 bg-white rounded-lg border border-gray-300 flex items-center justify-center hover:bg-gray-100"
                >
                  <i class="material-symbols-rounded text-sm">remove</i>
                </button>
                <span class="w-12 text-center font-semibold">{{ item.quantity }}</span>
                <button
                  @click="updateQuantity(item.id, item.quantity + 1)"
                  class="w-8 h-8 bg-white rounded-lg border border-gray-300 flex items-center justify-center hover:bg-gray-100"
                >
                  <i class="material-symbols-rounded text-sm">add</i>
                </button>
              </div>
              <span class="font-bold text-gray-900">{{ formatCurrency(item.total) }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Summary -->
      <div class="border-t p-4 bg-white space-y-3">
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
          <span class="text-gray-900">{{ formatCurrency(posStore.cartTotal) }}</span>
        </div>

        <button
          @click="emit('checkout')"
          :disabled="posStore.cart.length === 0"
          class="w-full py-4 bg-gradient-to-br from-gray-800 to-gray-900 text-white rounded-xl font-bold text-lg hover:shadow-lg transition-shadow disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Checkout {{ formatCurrency(posStore.cartTotal) }}
        </button>
      </div>
    </div>
  </div>
</template>

