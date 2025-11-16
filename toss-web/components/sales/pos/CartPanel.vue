<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Input } from '~/components/ui/input'
import { Trash2, Minus, Plus, Tag } from 'lucide-vue-next'
import { Separator } from '~/components/ui/separator'
import type { CartItem } from '~/composables/usePosMock'

const props = defineProps<{
  items: CartItem[]
  subtotal: number
  totalTax: number
  totalDiscount: number
  total: number
}>()

const emit = defineEmits<{
  updateQuantity: [productId: number, quantity: number]
  removeItem: [productId: number]
  applyDiscount: [productId: number, discount: number, type: 'fixed' | 'percentage']
}>()

const formatPrice = (price: number) => {
  return `R ${price.toFixed(2)}`
}

const calculateLineTotal = (item: CartItem) => {
  const subtotal = item.quantity * item.unitPrice
  const discount = item.discountType === 'percentage' 
    ? (subtotal * item.discount) / 100 
    : item.discount
  const afterDiscount = subtotal - discount
  const tax = item.isTaxable ? (afterDiscount * item.taxRate) / 100 : 0
  return afterDiscount + tax
}

const showDiscountDialog = ref(false)
const selectedItem = ref<CartItem | null>(null)
const discountAmount = ref(0)
const discountType = ref<'fixed' | 'percentage'>('fixed')

const openDiscountDialog = (item: CartItem) => {
  selectedItem.value = item
  discountAmount.value = item.discount
  discountType.value = item.discountType
  showDiscountDialog.value = true
}

const applyItemDiscount = () => {
  if (selectedItem.value) {
    emit('applyDiscount', selectedItem.value.productId, discountAmount.value, discountType.value)
    showDiscountDialog.value = false
  }
}
</script>

<template>
  <Card class="h-full flex flex-col">
    <CardHeader class="pb-3">
      <CardTitle class="text-lg">Cart ({{ items.length }} items)</CardTitle>
    </CardHeader>
    
    <CardContent class="flex-1 flex flex-col p-0">
      <div class="flex-1 px-4 overflow-y-auto max-h-[500px]">
        <div v-if="items.length === 0" class="flex flex-col items-center justify-center py-12 text-muted-foreground">
          <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mb-4 opacity-50">
            <circle cx="8" cy="21" r="1"/>
            <circle cx="19" cy="21" r="1"/>
            <path d="M2.05 2.05h2l2.66 12.42a2 2 0 0 0 2 1.58h9.78a2 2 0 0 0 1.95-1.57l1.65-7.43H5.12"/>
          </svg>
          <p>Cart is empty</p>
          <p class="text-sm">Add products to get started</p>
        </div>

        <div v-else class="space-y-3 pb-4">
          <div
            v-for="item in items"
            :key="item.productId"
            class="flex items-start gap-3 p-3 rounded-lg border bg-card hover:bg-accent/50 transition-colors"
          >
            <div class="flex-1 min-w-0">
              <h4 class="font-medium text-sm mb-1 truncate">{{ item.productName }}</h4>
              <p class="text-xs text-muted-foreground mb-2">{{ item.sku }}</p>
              
              <div class="flex items-center gap-2">
                <div class="flex items-center border rounded-md">
                  <Button
                    size="icon"
                    variant="ghost"
                    class="h-7 w-7"
                    @click="emit('updateQuantity', item.productId, item.quantity - 1)"
                  >
                    <Minus class="h-3 w-3" />
                  </Button>
                  <Input
                    :model-value="item.quantity"
                    type="number"
                    min="1"
                    class="h-7 w-12 border-0 text-center p-0"
                    @update:model-value="(val) => emit('updateQuantity', item.productId, Number(val))"
                  />
                  <Button
                    size="icon"
                    variant="ghost"
                    class="h-7 w-7"
                    @click="emit('updateQuantity', item.productId, item.quantity + 1)"
                  >
                    <Plus class="h-3 w-3" />
                  </Button>
                </div>
                
                <span class="text-sm text-muted-foreground">× {{ formatPrice(item.unitPrice) }}</span>
              </div>

              <div v-if="item.discount > 0" class="mt-2 flex items-center gap-1 text-xs text-green-600">
                <Tag class="h-3 w-3" />
                <span>
                  Discount: {{ item.discountType === 'percentage' ? `${item.discount}%` : formatPrice(item.discount) }}
                </span>
              </div>
            </div>

            <div class="text-right space-y-2">
              <div class="font-semibold">{{ formatPrice(calculateLineTotal(item)) }}</div>
              <div class="flex gap-1">
                <Button
                  size="icon"
                  variant="ghost"
                  class="h-7 w-7"
                  @click="openDiscountDialog(item)"
                  title="Apply Discount"
                >
                  <Tag class="h-4 w-4" />
                </Button>
                <Button
                  size="icon"
                  variant="ghost"
                  class="h-7 w-7 text-destructive hover:text-destructive"
                  @click="emit('removeItem', item.productId)"
                  title="Remove Item"
                >
                  <Trash2 class="h-4 w-4" />
                </Button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <Separator />
      
      <div class="p-4 space-y-2 bg-muted/30">
        <div class="flex justify-between text-sm">
          <span class="text-muted-foreground">Subtotal:</span>
          <span>{{ formatPrice(subtotal) }}</span>
        </div>
        <div v-if="totalDiscount > 0" class="flex justify-between text-sm text-green-600">
          <span>Discount:</span>
          <span>-{{ formatPrice(totalDiscount) }}</span>
        </div>
        <div class="flex justify-between text-sm">
          <span class="text-muted-foreground">VAT (15%):</span>
          <span>{{ formatPrice(totalTax) }}</span>
        </div>
        <Separator />
        <div class="flex justify-between text-lg font-bold">
          <span>Total:</span>
          <span class="text-primary">{{ formatPrice(total) }}</span>
        </div>
      </div>
    </CardContent>
  </Card>
</template>
