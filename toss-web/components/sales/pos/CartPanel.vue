<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card' 
import { Button } from '~/components/ui/button'
import { Input } from '~/components/ui/input'
import { Label } from '~/components/ui/label'
import { Trash2, Minus, Plus, Tag, ShoppingCart } from 'lucide-vue-next'
import { Separator } from '~/components/ui/separator'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '~/components/ui/dialog'
import { Badge } from '~/components/ui/badge'
import type { CartItem } from '~/composables/usePosMock'

defineProps<{
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
  <Card class="flex overflow-hidden flex-col h-full shadow-lg">
    <CardHeader class="flex-none pb-4 border-b">
      <div class="flex justify-between items-center">
        <div class="flex gap-2 items-center">
          <ShoppingCart class="w-5 h-5 text-primary" />
          <CardTitle class="text-xl font-semibold">Shopping Cart</CardTitle>
        </div>
        <Badge variant="secondary" class="text-xs">{{ items.length }} {{ items.length === 1 ? 'item' : 'items' }}</Badge>
      </div>
    </CardHeader>

    <div class="flex overflow-hidden flex-col flex-1 min-h-0">
      <!-- Cart Items Section -->
      <div class="overflow-y-auto flex-1 min-h-0">
        <div class="p-6">
          <!-- Empty State -->
          <div v-if="items.length === 0" class="flex flex-col justify-center items-center py-16 text-center">
            <div class="p-6 mb-4 rounded-full bg-muted">
              <ShoppingCart class="w-12 h-12 text-muted-foreground" />
            </div>
            <h3 class="mb-2 text-lg font-semibold">Your cart is empty</h3>
            <p class="max-w-sm text-sm text-muted-foreground">
              Looks like you haven't added any items to your cart yet. Start shopping to fill it up!
            </p>
          </div>

          <!-- Cart Items List -->
          <div v-else class="space-y-4">
            <div
              v-for="item in items"
              :key="item.productId"
              class="flex relative gap-4 p-4 rounded-xl border transition-all duration-200 group bg-card hover:shadow-md"
            >
              <!-- Product Image Placeholder -->
              <div class="flex-shrink-0">
                <div class="flex justify-center items-center w-20 h-20 bg-gradient-to-br rounded-lg border from-primary/20 to-primary/5">
                  <ShoppingCart class="w-8 h-8 text-primary/40" />
                </div>
              </div>

              <!-- Product Details -->
              <div class="flex-1 min-w-0">
                <div class="flex gap-4 justify-between items-start mb-2">
                  <div class="flex-1 min-w-0">
                    <h4 class="mb-1 text-base font-semibold truncate transition-colors group-hover:text-primary">
                      {{ item.productName }}
                    </h4>
                    <p class="mb-2 text-xs text-muted-foreground">SKU: {{ item.sku }}</p>
                  </div>
                  <Button
                    size="icon"
                    variant="ghost"
                    class="w-8 h-8 opacity-0 transition-opacity text-destructive hover:text-destructive hover:bg-destructive/10 group-hover:opacity-100"
                    @click="emit('removeItem', item.productId)"
                    title="Remove Item"
                  >
                    <Trash2 class="w-4 h-4" />
                  </Button>
                </div>

                <!-- Quantity Controls -->
                <div class="flex gap-4 items-center mb-3">
                  <div class="flex overflow-hidden items-center rounded-lg border">
                    <Button
                      size="icon"
                      variant="ghost"
                      class="w-8 h-8 rounded-none"
                      @click="emit('updateQuantity', item.productId, item.quantity - 1)"
                    >
                      <Minus class="w-3.5 h-3.5" />
                    </Button>
                    <Input
                      :model-value="item.quantity"
                      type="number"
                      min="1"
                      class="p-0 w-16 h-8 text-center border-0 focus-visible:ring-0"
                      @update:model-value="(val) => emit('updateQuantity', item.productId, Number(val))"
                    />
                    <Button
                      size="icon"
                      variant="ghost"
                      class="w-8 h-8 rounded-none"
                      @click="emit('updateQuantity', item.productId, item.quantity + 1)"
                    >
                      <Plus class="w-3.5 h-3.5" />
                    </Button>
                  </div>
                  <div class="text-sm text-muted-foreground">
                    @ {{ formatPrice(item.unitPrice) }} each
                  </div>
                </div>

                <!-- Discount Badge -->
                <div v-if="item.discount > 0" class="flex gap-2 items-center">
                  <Badge variant="outline" class="text-xs text-green-600 bg-green-50 border-green-600/20 dark:bg-green-950/20">
                    <Tag class="mr-1 w-3 h-3" />
                    {{ item.discountType === 'percentage' ? `${item.discount}% off` : `${formatPrice(item.discount)} off` }}
                  </Badge>
                  <Button
                    size="sm"
                    variant="ghost"
                    class="h-6 text-xs"
                    @click="openDiscountDialog(item)"
                  >
                    Edit Discount
                  </Button>
                </div>
              </div>

              <!-- Line Total -->
              <div class="flex-shrink-0 text-right">
                <div class="mb-1 text-lg font-bold text-primary">
                  {{ formatPrice(calculateLineTotal(item)) }}
                </div>
                <div v-if="item.quantity > 1" class="text-xs text-muted-foreground">
                  {{ item.quantity }} × {{ formatPrice(item.unitPrice) }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <Separator />

      <!-- Order Summary Section -->
      <div class="flex-none p-6 space-y-4 bg-gradient-to-br border-t from-muted/50 to-muted/30">
        <div class="space-y-3">
          <div class="flex justify-between text-sm">
            <span class="text-muted-foreground">Subtotal</span>
            <span class="font-medium">{{ formatPrice(subtotal) }}</span>
          </div>
          <div v-if="totalDiscount > 0" class="flex justify-between text-sm">
            <span class="text-green-600">Discount</span>
            <span class="font-medium text-green-600">-{{ formatPrice(totalDiscount) }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-muted-foreground">VAT (15%)</span>
            <span class="font-medium">{{ formatPrice(totalTax) }}</span>
          </div>
        </div>
        
        <Separator />
        
        <div class="flex justify-between items-center pt-2">
          <span class="text-lg font-semibold">Total</span>
          <span class="text-2xl font-bold text-primary">{{ formatPrice(total) }}</span>
        </div>
      </div>
    </div>

    <!-- Discount Dialog -->
    <Dialog v-model:open="showDiscountDialog">
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Apply Discount</DialogTitle>
          <DialogDescription>
            Set a discount for {{ selectedItem?.productName }}
          </DialogDescription>
        </DialogHeader>
        <div class="py-4 space-y-4">
          <div class="space-y-2">
            <Label>Discount Type</Label>
            <div class="flex gap-2">
              <Button
                :variant="discountType === 'fixed' ? 'default' : 'outline'"
                @click="discountType = 'fixed'"
              >
                Fixed Amount
              </Button>
              <Button
                :variant="discountType === 'percentage' ? 'default' : 'outline'"
                @click="discountType = 'percentage'"
              >
                Percentage
              </Button>
            </div>
          </div>
          <div class="space-y-2">
            <Label>Discount Amount</Label>
            <Input
              v-model.number="discountAmount"
              type="number"
              :min="0"
              :max="discountType === 'percentage' ? 100 : undefined"
            />
          </div>
          <div class="flex gap-2 justify-end">
            <Button variant="outline" @click="showDiscountDialog = false">
              Cancel
            </Button>
            <Button @click="applyItemDiscount">
              Apply Discount
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  </Card>
</template>
