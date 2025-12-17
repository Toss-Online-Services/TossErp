<template>
  <div class="space-y-3">
    <!-- Cart Items -->
    <div 
      v-for="(item, index) in items" 
      :key="item.productId"
      class="p-3 bg-gray-50 dark:bg-slate-700/50 rounded-lg space-y-2"
    >
      <!-- Item Header -->
      <div class="flex items-start justify-between">
        <div class="flex-1 min-w-0">
          <h4 class="font-medium text-gray-900 dark:text-gray-100 text-sm truncate">{{ item.name }}</h4>
          <p class="text-xs text-gray-500 dark:text-gray-400">R{{ formatPrice(item.unitPrice) }} each</p>
        </div>
        <button 
          @click="$emit('remove', index)"
          class="ml-2 text-red-500 hover:text-red-600 flex-shrink-0"
          title="Remove item"
        >
          <TrashIcon class="w-4 h-4" />
        </button>
      </div>

      <!-- Quantity Controls -->
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-2">
          <button 
            @click="$emit('updateQuantity', index, item.quantity - 1)"
            class="w-7 h-7 rounded-full bg-gray-200 dark:bg-slate-600 flex items-center justify-center text-gray-600 dark:text-gray-300 hover:bg-gray-300 dark:hover:bg-slate-500 transition-colors"
            :disabled="item.quantity <= 1"
          >
            <MinusIcon class="w-3 h-3" />
          </button>
          <span class="w-10 text-center text-sm font-medium text-gray-900 dark:text-gray-100">{{ item.quantity }}</span>
          <button 
            @click="$emit('updateQuantity', index, item.quantity + 1)"
            class="w-7 h-7 rounded-full bg-gray-200 dark:bg-slate-600 flex items-center justify-center text-gray-600 dark:text-gray-300 hover:bg-gray-300 dark:hover:bg-slate-500 transition-colors"
          >
            <PlusIcon class="w-3 h-3" />
          </button>
        </div>

        <!-- Line Subtotal -->
        <span class="text-sm font-semibold text-gray-700 dark:text-gray-300">
          R{{ formatPrice(calculateLineSubtotal(item)) }}
        </span>
      </div>

      <!-- Discount Controls (if enabled) -->
      <div v-if="allowDiscounts" class="flex items-center gap-2">
        <button
          @click="toggleDiscountMode(index)"
          class="text-xs px-2 py-1 rounded bg-orange-100 dark:bg-orange-900/30 text-orange-700 dark:text-orange-400 hover:bg-orange-200 dark:hover:bg-orange-900/50 transition-colors"
        >
          {{ item.discountPercent || item.discountAmount ? '✓ Discount' : '+ Discount' }}
        </button>
        
        <div v-if="item.showDiscountInput" class="flex items-center gap-1 flex-1">
          <input
            v-model.number="item.discountPercent"
            type="number"
            min="0"
            max="100"
            step="1"
            placeholder="%"
            class="w-16 px-2 py-1 text-xs border border-gray-300 dark:border-slate-600 rounded bg-white dark:bg-slate-700 text-gray-900 dark:text-gray-100"
            @input="clearFixedDiscount(index)"
          />
          <span class="text-xs text-gray-500">%</span>
          <span class="text-xs text-gray-400 mx-1">or</span>
          <span class="text-xs text-gray-500">R</span>
          <input
            v-model.number="item.discountAmount"
            type="number"
            min="0"
            step="0.01"
            placeholder="0.00"
            class="w-20 px-2 py-1 text-xs border border-gray-300 dark:border-slate-600 rounded bg-white dark:bg-slate-700 text-gray-900 dark:text-gray-100"
            @input="clearPercentDiscount(index)"
          />
        </div>
      </div>

      <!-- Tax Badge -->
      <div v-if="item.taxRate && item.taxRate > 0" class="flex items-center justify-between text-xs">
        <span class="text-gray-500 dark:text-gray-400">VAT ({{ item.taxRate }}%)</span>
        <span class="text-gray-600 dark:text-gray-300">R{{ formatPrice(calculateLineTax(item)) }}</span>
      </div>

      <!-- Line Total (after discount + tax) -->
      <div v-if="item.discountPercent || item.discountAmount || (item.taxRate && item.taxRate > 0)" class="flex items-center justify-between pt-2 border-t border-gray-200 dark:border-slate-600">
        <span class="text-xs font-semibold text-gray-700 dark:text-gray-300">Line Total</span>
        <span class="text-sm font-bold text-blue-600 dark:text-blue-400">R{{ formatPrice(calculateLineTotal(item)) }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { PlusIcon, MinusIcon, TrashIcon } from '@heroicons/vue/24/outline'

// Extended cart line type with UI state
interface EnhancedCartLine {
  productId: string
  name: string
  quantity: number
  unitPrice: number
  discountPercent?: number
  discountAmount?: number
  taxRate?: number
  showDiscountInput?: boolean
}

interface Props {
  items: EnhancedCartLine[]
  allowDiscounts?: boolean
}

const props = defineProps<Props>()

const emit = defineEmits<{
  remove: [index: number]
  updateQuantity: [index: number, quantity: number]
  updateDiscount: [index: number, discount: Partial<Pick<EnhancedCartLine, 'discountPercent' | 'discountAmount'>>]
}>()

// Import cart math utilities
const calculateLineSubtotal = (line: EnhancedCartLine): number => {
  return Math.max(0, line.quantity * line.unitPrice)
}

const calculateLineDiscount = (line: EnhancedCartLine): number => {
  const subtotal = calculateLineSubtotal(line)
  if (line.discountAmount !== undefined) {
    return Math.min(line.discountAmount, subtotal)
  }
  if (line.discountPercent !== undefined) {
    const percent = Math.min(line.discountPercent, 100)
    return (subtotal * percent) / 100
  }
  return 0
}

const calculateLineTaxableAmount = (line: EnhancedCartLine): number => {
  const subtotal = calculateLineSubtotal(line)
  const discount = calculateLineDiscount(line)
  return subtotal - discount
}

const calculateLineTax = (line: EnhancedCartLine): number => {
  if (!line.taxRate || line.taxRate === 0) return 0
  const taxableAmount = calculateLineTaxableAmount(line)
  return (taxableAmount * line.taxRate) / 100
}

const calculateLineTotal = (line: EnhancedCartLine): number => {
  const taxableAmount = calculateLineTaxableAmount(line)
  const tax = calculateLineTax(line)
  return roundToZAR(taxableAmount + tax)
}

const roundToZAR = (amount: number): number => {
  return Math.round(amount * 100) / 100
}

const formatCurrency = (amount: number): string => {
  return amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ' ')
}

const formatPrice = (amount: number) => {
  return formatCurrency(amount)
}

const toggleDiscountMode = (index: number) => {
  const item = props.items[index]
  if (item) {
    item.showDiscountInput = !item.showDiscountInput
  }
}

const clearPercentDiscount = (index: number) => {
  const item = props.items[index]
  if (item && item.discountAmount && item.discountAmount > 0) {
    item.discountPercent = undefined
  }
}

const clearFixedDiscount = (index: number) => {
  const item = props.items[index]
  if (item && item.discountPercent && item.discountPercent > 0) {
    item.discountAmount = undefined
  }
}
</script>
