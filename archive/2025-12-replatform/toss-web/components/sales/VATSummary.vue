<template>
  <div class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-4 space-y-2">
    <h3 class="text-sm font-semibold text-blue-900 dark:text-blue-100 mb-3">Cart Summary</h3>
    
    <!-- Subtotal -->
    <div class="flex items-center justify-between text-sm">
      <span class="text-gray-700 dark:text-gray-300">Subtotal</span>
      <span class="font-medium text-gray-900 dark:text-gray-100">R{{ formatAmount(totals.subtotal) }}</span>
    </div>

    <!-- Discount Total -->
    <div v-if="totals.discountTotal > 0" class="flex items-center justify-between text-sm">
      <span class="text-orange-700 dark:text-orange-400">Total Discounts</span>
      <span class="font-medium text-orange-700 dark:text-orange-400">-R{{ formatAmount(totals.discountTotal) }}</span>
    </div>

    <!-- Amount After Discount -->
    <div v-if="totals.discountTotal > 0" class="flex items-center justify-between text-sm pt-1 border-t border-blue-100 dark:border-blue-800">
      <span class="text-gray-600 dark:text-gray-400">After Discounts</span>
      <span class="font-medium text-gray-700 dark:text-gray-300">R{{ formatAmount(totals.subtotal - totals.discountTotal) }}</span>
    </div>

    <!-- Tax Total -->
    <div v-if="totals.taxTotal > 0" class="flex items-center justify-between text-sm">
      <span class="text-gray-700 dark:text-gray-300">VAT (15%)</span>
      <span class="font-medium text-gray-900 dark:text-gray-100">R{{ formatAmount(totals.taxTotal) }}</span>
    </div>

    <!-- Grand Total -->
    <div class="flex items-center justify-between pt-3 mt-2 border-t-2 border-blue-300 dark:border-blue-700">
      <span class="text-lg font-bold text-blue-900 dark:text-blue-100">Total</span>
      <span class="text-2xl font-bold text-blue-600 dark:text-blue-400">R{{ formatAmount(totals.grandTotal) }}</span>
    </div>

    <!-- Item Count -->
    <div class="text-xs text-center text-gray-500 dark:text-gray-400 pt-2">
      {{ itemCount }} {{ itemCount === 1 ? 'item' : 'items' }} in cart
    </div>
  </div>
</template>

<script setup lang="ts">
interface CartTotals {
  subtotal: number
  discountTotal: number
  taxTotal: number
  grandTotal: number
}

interface Props {
  totals: CartTotals
  itemCount: number
}

defineProps<Props>()

const formatAmount = (amount: number): string => {
  return amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ' ')
}
</script>
