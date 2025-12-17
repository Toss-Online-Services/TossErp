<template>
  <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 overflow-hidden">
    <div class="bg-gradient-to-r from-blue-600 to-purple-600 px-4 py-4 sm:py-5">
      <div class="flex items-center justify-between">
        <h3 class="text-lg font-bold text-white">{{ title }}</h3>
        <button 
          v-if="items.length > 0"
          @click="$emit('clearCart')"
          class="px-3 py-1.5 bg-white/20 hover:bg-white/30 backdrop-blur-sm rounded-lg text-sm font-medium text-white transition-colors"
        >
          Clear All
        </button>
      </div>
    </div>
    
    <div class="p-4 sm:p-6">
      <div v-if="items.length === 0" class="text-center py-8">
        <ShoppingCartIcon class="w-16 h-16 text-gray-300 mx-auto mb-4" />
        <p class="text-gray-500">No items in cart</p>
        <p class="text-sm text-gray-400">Scan or click products to add</p>
      </div>

      <div v-else class="space-y-3 max-h-64 overflow-y-auto">
        <div 
          v-for="item in items" 
          :key="item.id"
          class="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
        >
          <div class="flex-1">
            <h4 class="font-medium text-gray-900 text-sm">{{ item.name }}</h4>
            <p class="text-xs text-gray-500">R{{ item.price.toFixed(2) }} each</p>
          </div>
          <div class="flex items-center space-x-2">
            <button 
              @click="$emit('updateQuantity', item.id, item.quantity - 1)"
              class="w-6 h-6 rounded-full bg-gray-200 flex items-center justify-center text-gray-600 hover:bg-gray-300"
            >
              <MinusIcon class="w-3 h-3" />
            </button>
            <span class="w-8 text-center text-sm font-medium text-gray-900">{{ item.quantity }}</span>
            <button 
              @click="$emit('updateQuantity', item.id, item.quantity + 1)"
              class="w-6 h-6 rounded-full bg-gray-200 flex items-center justify-center text-gray-600 hover:bg-gray-300"
            >
              <PlusIcon class="w-3 h-3" />
            </button>
            <button 
              @click="$emit('removeItem', item.id)"
              class="ml-2 text-red-500 hover:text-red-600"
            >
              <TrashIcon class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      <!-- Cart Total -->
      <div v-if="items.length > 0" class="mt-4 pt-4 border-t border-gray-200">
        <div class="flex justify-between items-center mb-4">
          <span class="text-lg font-semibold text-gray-900">Total:</span>
          <span class="text-xl font-bold text-blue-600">R{{ formatCurrency(total) }}</span>
        </div>
        
        <slot name="customer-section"></slot>
        <slot name="payment-section"></slot>
        <slot name="action-button"></slot>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import {
  ShoppingCartIcon,
  PlusIcon,
  MinusIcon,
  TrashIcon
} from '@heroicons/vue/24/outline'

interface CartItem {
  id: number | string
  name: string
  price: number
  quantity: number
}

interface Props {
  items: CartItem[]
  title?: string
}

const props = withDefaults(defineProps<Props>(), {
  title: 'Current Sale'
})

defineEmits<{
  'clearCart': []
  'updateQuantity': [id: number | string, quantity: number]
  'removeItem': [id: number | string]
}>()

const total = computed(() => {
  return props.items.reduce((sum: number, item: CartItem) => sum + (item.price * item.quantity), 0)
})

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}
</script>

