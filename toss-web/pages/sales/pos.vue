<script setup lang="ts">
import { computed, ref } from 'vue'

// @ts-ignore -- Nuxt auto-injects definePageMeta
definePageMeta({
  layout: 'dashboard',
  middleware: 'auth',
})

const searchQuery = ref('')
const cart = ref([
  {
    id: 1,
    name: 'Bread - White Loaf',
    price: 12.50,
    quantity: 2,
    total: 25.00,
  },
  {
    id: 2,
    name: 'Milk - 2L Full Cream',
    price: 25.99,
    quantity: 1,
    total: 25.99,
  },
])

const subtotal = computed(() => 
  cart.value.reduce((sum, item) => sum + item.total, 0)
)

const tax = computed(() => subtotal.value * 0.15)
const total = computed(() => subtotal.value + tax.value)

const removeFromCart = (id: number) => {
  cart.value = cart.value.filter(item => item.id !== id)
}

const updateQuantity = (id: number, quantity: number) => {
  const item = cart.value.find(item => item.id === id)
  if (item) {
    item.quantity = quantity
    item.total = item.price * quantity
  }
}

const completeSale = () => {
  // TODO: Implement sale completion logic
  console.log('Completing sale...')
}
</script>

<template>
  <div class="flex-1 space-y-4 p-4 md:p-8 pt-6">
    <div class="flex items-center justify-between">
      <h2 class="text-3xl font-bold tracking-tight">
        Point of Sale
      </h2>
      <div class="flex items-center space-x-2">
        <Button variant="outline">
          <Icon name="mdi:history" class="mr-2 h-4 w-4" />
          Recent Sales
        </Button>
      </div>
    </div>

    <div class="grid gap-4 md:grid-cols-3">
      <!-- Product Search & Selection -->
      <Card class="md:col-span-2">
        <CardHeader>
          <CardTitle>Products</CardTitle>
          <CardDescription>Search and add products to cart</CardDescription>
        </CardHeader>
        <CardContent class="space-y-4">
          <div class="flex gap-2">
            <Input
              v-model="searchQuery"
              placeholder="Search products by name or barcode..."
              class="flex-1"
            />
            <Button variant="outline" size="icon">
              <Icon name="mdi:barcode-scan" class="h-4 w-4" />
            </Button>
          </div>

          <!-- Product Grid - Placeholder -->
          <div class="grid grid-cols-2 md:grid-cols-3 gap-4 pt-4">
            <Card
              v-for="i in 6"
              :key="i"
              class="cursor-pointer hover:border-primary transition-colors"
            >
              <CardContent class="p-4">
                <div class="aspect-square bg-muted rounded-md mb-2" />
                <p class="font-medium text-sm">Product {{ i }}</p>
                <p class="text-sm text-muted-foreground">R 25.00</p>
              </CardContent>
            </Card>
          </div>
        </CardContent>
      </Card>

      <!-- Cart & Checkout -->
      <div class="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Current Sale</CardTitle>
            <CardDescription>{{ cart.length }} items</CardDescription>
          </CardHeader>
          <CardContent class="space-y-4">
            <!-- Cart Items -->
            <div class="space-y-2 max-h-[300px] overflow-y-auto">
              <div
                v-for="item in cart"
                :key="item.id"
                class="flex items-start justify-between p-3 border rounded-lg"
              >
                <div class="flex-1">
                  <p class="font-medium text-sm">{{ item.name }}</p>
                  <p class="text-xs text-muted-foreground">R {{ item.price.toFixed(2) }}</p>
                  <div class="flex items-center gap-2 mt-2">
                    <Button
                      size="icon"
                      variant="outline"
                      class="h-6 w-6"
                      @click="updateQuantity(item.id, Math.max(1, item.quantity - 1))"
                    >
                      -
                    </Button>
                    <span class="text-sm font-medium w-8 text-center">{{ item.quantity }}</span>
                    <Button
                      size="icon"
                      variant="outline"
                      class="h-6 w-6"
                      @click="updateQuantity(item.id, item.quantity + 1)"
                    >
                      +
                    </Button>
                  </div>
                </div>
                <div class="text-right">
                  <p class="font-medium text-sm">R {{ item.total.toFixed(2) }}</p>
                  <Button
                    size="icon"
                    variant="ghost"
                    class="h-6 w-6 mt-1"
                    @click="removeFromCart(item.id)"
                  >
                    <Icon name="mdi:close" class="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </div>

            <Separator />

            <!-- Totals -->
            <div class="space-y-2">
              <div class="flex justify-between text-sm">
                <span class="text-muted-foreground">Subtotal</span>
                <span>R {{ subtotal.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-muted-foreground">VAT (15%)</span>
                <span>R {{ tax.toFixed(2) }}</span>
              </div>
              <Separator />
              <div class="flex justify-between text-lg font-bold">
                <span>Total</span>
                <span>R {{ total.toFixed(2) }}</span>
              </div>
            </div>

            <!-- Payment Methods -->
            <div class="space-y-2">
              <Label>Payment Method</Label>
              <div class="grid grid-cols-2 gap-2">
                <Button variant="outline" class="h-auto py-3">
                  <div class="flex flex-col items-center gap-1">
                    <Icon name="mdi:cash" class="h-5 w-5" />
                    <span class="text-xs">Cash</span>
                  </div>
                </Button>
                <Button variant="outline" class="h-auto py-3">
                  <div class="flex flex-col items-center gap-1">
                    <Icon name="mdi:credit-card" class="h-5 w-5" />
                    <span class="text-xs">Card</span>
                  </div>
                </Button>
              </div>
            </div>

            <!-- Action Buttons -->
            <div class="space-y-2">
              <Button class="w-full" size="lg" @click="completeSale">
                <Icon name="mdi:check" class="mr-2 h-5 w-5" />
                Complete Sale
              </Button>
              <Button variant="outline" class="w-full">
                <Icon name="mdi:cancel" class="mr-2 h-4 w-4" />
                Clear Cart
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  </div>
</template>
