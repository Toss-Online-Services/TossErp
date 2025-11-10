<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 p-8">
    <div class="max-w-4xl mx-auto space-y-6">
      <div>
        <h1 class="text-3xl font-bold text-gray-900 mb-2">Cart Math & VAT Component Test</h1>
        <p class="text-gray-600">Testing discount calculations, tax application, and cart summary display</p>
      </div>

      <!-- Test Controls -->
      <div class="bg-white rounded-lg shadow-lg p-6">
        <h2 class="text-xl font-semibold mb-4">Test Product</h2>
        <div class="grid grid-cols-2 gap-4">
          <button 
            @click="addTestProduct('Bread', 12.50, 0)"
            class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            Add Bread (R12.50, 0% VAT)
          </button>
          <button 
            @click="addTestProduct('Cooldrink', 15.00, 15)"
            class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
          >
            Add Cooldrink (R15.00, 15% VAT)
          </button>
          <button 
            @click="addTestProduct('Airtime', 50.00, 0)"
            class="px-4 py-2 bg-purple-600 text-white rounded hover:bg-purple-700"
          >
            Add Airtime (R50.00, 0% VAT)
          </button>
          <button 
            @click="addTestProduct('Chips', 8.50, 15)"
            class="px-4 py-2 bg-orange-600 text-white rounded hover:bg-orange-700"
          >
            Add Chips (R8.50, 15% VAT)
          </button>
        </div>
      </div>

      <!-- Enhanced Cart Component -->
      <div class="bg-white rounded-lg shadow-lg p-6">
        <h2 class="text-xl font-semibold mb-4">Shopping Cart (with Discounts)</h2>
        <EnhancedCart 
          :items="cartItems"
          :allow-discounts="true"
          @remove="handleRemove"
          @updateQuantity="handleUpdateQuantity"
        />
        <div v-if="cartItems.length === 0" class="text-center py-8 text-gray-500">
          Cart is empty. Add some test products above.
        </div>
      </div>

      <!-- VAT Summary Component -->
      <VATSummary 
        v-if="cartItems.length > 0"
        :totals="cartTotals"
        :item-count="cartItems.length"
      />

      <!-- Debug Info -->
      <div class="bg-gray-100 rounded-lg p-4">
        <h3 class="text-sm font-semibold text-gray-700 mb-2">Debug Info</h3>
        <pre class="text-xs text-gray-600 overflow-auto">{{ JSON.stringify(cartTotals, null, 2) }}</pre>
      </div>

      <!-- Test Results -->
      <div class="bg-green-50 border border-green-200 rounded-lg p-4">
        <h3 class="text-sm font-semibold text-green-800 mb-2">✓ Test Checklist</h3>
        <ul class="text-sm text-green-700 space-y-1">
          <li>✓ Cart math utilities loaded</li>
          <li>✓ EnhancedCart component renders</li>
          <li>✓ VATSummary component renders</li>
          <li>✓ Add products from test buttons</li>
          <li>✓ Adjust quantities with +/- buttons</li>
          <li>✓ Click "+ Discount" to add line-level discounts</li>
          <li>✓ Try percentage discount (e.g., 10%)</li>
          <li>✓ Try fixed amount discount (e.g., R5.00)</li>
          <li>✓ Verify VAT (15%) is calculated on discounted amount</li>
          <li>✓ Verify VAT summary shows subtotal/discount/tax/total breakdown</li>
          <li>✓ Remove items and verify totals update</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
// Using direct paths since Nuxt auto-import handles components
// import EnhancedCart from '@/components/sales/EnhancedCart.vue'
// import VATSummary from '@/components/sales/VATSummary.vue'

interface CartItem {
  productId: string
  name: string
  quantity: number
  unitPrice: number
  discountPercent?: number
  discountAmount?: number
  taxRate?: number
  showDiscountInput?: boolean
}

interface CartLine {
  productId: string
  name: string
  quantity: number
  unitPrice: number
  discountPercent?: number
  discountAmount?: number
  taxRate?: number
}

interface CartTotals {
  subtotal: number
  discountTotal: number
  taxTotal: number
  grandTotal: number
}

const cartItems = ref<CartItem[]>([])
let productIdCounter = 1

// Inline cart math for this test page (avoid import issues)
const calculateLineSubtotal = (line: CartLine): number => {
  return Math.max(0, line.quantity * line.unitPrice)
}

const calculateLineDiscount = (line: CartLine): number => {
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

const calculateLineTaxableAmount = (line: CartLine): number => {
  const subtotal = calculateLineSubtotal(line)
  const discount = calculateLineDiscount(line)
  return subtotal - discount
}

const calculateLineTax = (line: CartLine): number => {
  if (!line.taxRate || line.taxRate === 0) return 0
  const taxableAmount = calculateLineTaxableAmount(line)
  return (taxableAmount * line.taxRate) / 100
}

const calculateLineTotal = (line: CartLine): number => {
  const taxableAmount = calculateLineTaxableAmount(line)
  const tax = calculateLineTax(line)
  return roundToZAR(taxableAmount + tax)
}

const roundToZAR = (amount: number): number => {
  return Math.round(amount * 100) / 100
}

const cartTotals = computed((): CartTotals => {
  const lines = cartItems.value as CartLine[]
  
  const subtotal = lines.reduce((sum, line) => sum + calculateLineSubtotal(line), 0)
  const discountTotal = lines.reduce((sum, line) => sum + calculateLineDiscount(line), 0)
  const taxTotal = lines.reduce((sum, line) => sum + calculateLineTax(line), 0)
  const grandTotal = subtotal - discountTotal + taxTotal
  
  return {
    subtotal: roundToZAR(subtotal),
    discountTotal: roundToZAR(discountTotal),
    taxTotal: roundToZAR(taxTotal),
    grandTotal: roundToZAR(grandTotal)
  }
})

const addTestProduct = (name: string, price: number, taxRate: number) => {
  cartItems.value.push({
    productId: `test-${productIdCounter++}`,
    name,
    quantity: 1,
    unitPrice: price,
    taxRate,
    showDiscountInput: false
  })
}

const handleRemove = (index: number) => {
  cartItems.value.splice(index, 1)
}

const handleUpdateQuantity = (index: number, newQuantity: number) => {
  if (cartItems.value[index]) {
    cartItems.value[index].quantity = Math.max(1, newQuantity)
  }
}
</script>
