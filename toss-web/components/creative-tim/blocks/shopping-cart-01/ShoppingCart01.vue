<script setup lang="ts">
import { computed } from 'vue'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Separator } from '~/components/ui/separator'
import { Badge } from '~/components/ui/badge'

type CartItem = {
  id: string
  name: string
  subtitle: string
  price: number
  quantity: number
  color?: string
  size?: string
  image?: string
}

const props = withDefaults(
  defineProps<{
    items?: CartItem[]
    currency?: string
  }>(),
  {
    items: () => [
      {
        id: 'SKU-9281',
        name: 'MidnightShield Jacket',
        subtitle: 'Water-resistant utility jacket',
        price: 1490,
        quantity: 1,
        color: 'Charcoal',
        size: 'L',
        image: 'https://images.unsplash.com/photo-1503341455253-b2e723bb3dbb?auto=format&fit=crop&w=400&q=80',
      },
      {
        id: 'SKU-5340',
        name: 'Soweto Denim',
        subtitle: 'Selvedge straight fit jeans',
        price: 950,
        quantity: 2,
        color: 'Midnight',
        size: '32',
        image: 'https://images.unsplash.com/photo-1503341455253-b2e723bb3dbb?auto=format&fit=crop&w=400&q=60',
      },
      {
        id: 'SKU-7032',
        name: 'Kasbah Knit',
        subtitle: 'Merino wool crew neck',
        price: 780,
        quantity: 1,
        color: 'Sandstone',
        size: 'M',
        image: 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=400&q=80',
      },
    ],
    currency: 'R',
  }
)

const subtotal = computed(() => props.items.reduce((sum, item) => sum + item.price * item.quantity, 0))
const tax = computed(() => subtotal.value * 0.15)
const shipping = computed(() => (subtotal.value > 2000 ? 0 : 160))
const total = computed(() => subtotal.value + tax.value + shipping.value)

const formatCurrency = (value: number) => `${props.currency} ${value.toFixed(2)}`
</script>

<template>
  <section class="grid gap-6 lg:grid-cols-[2fr,1fr]">
    <Card class="border-border/60">
      <CardHeader class="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <CardTitle class="text-xl font-semibold">Shopping Cart</CardTitle>
          <CardDescription>Review items before checkout</CardDescription>
        </div>
        <Badge variant="secondary" class="w-fit">
          {{ props.items.length }} items
        </Badge>
      </CardHeader>

      <CardContent class="space-y-4">
        <div
          v-for="item in props.items"
          :key="item.id"
          class="flex flex-col gap-4 rounded-xl border border-border/70 p-4 md:flex-row md:items-center"
        >
          <div class="aspect-square h-24 w-24 overflow-hidden rounded-lg bg-muted">
            <img
              v-if="item.image"
              :src="item.image"
              alt=""
              class="h-full w-full object-cover"
            >
          </div>

          <div class="flex-1 space-y-1">
            <p class="text-base font-semibold">{{ item.name }}</p>
            <p class="text-sm text-muted-foreground">
              {{ item.subtitle }}
            </p>
            <p class="text-xs text-muted-foreground">
              {{ item.color }} · Size {{ item.size }} · {{ item.id }}
            </p>
          </div>

          <div class="flex flex-col items-start gap-2 text-sm text-muted-foreground md:items-end">
            <p>Qty {{ item.quantity }}</p>
            <p class="text-base font-semibold text-foreground">
              {{ formatCurrency(item.price * item.quantity) }}
            </p>
            <Button size="sm" variant="ghost" class="h-8 px-2 text-xs">
              Remove
            </Button>
          </div>
        </div>
      </CardContent>

      <CardFooter class="flex flex-col gap-3 border-t border-border/70 pt-4 md:flex-row md:items-center md:justify-between">
        <p class="text-sm text-muted-foreground">
          Free alterations & collection in 2 hours at your nearest pickup hub.
        </p>
        <div class="flex gap-2">
          <Button variant="outline">
            Continue Shopping
          </Button>
          <Button>
            Update Cart
          </Button>
        </div>
      </CardFooter>
    </Card>

    <Card class="border-primary/40 bg-primary/5">
      <CardHeader>
        <CardTitle>Order Summary</CardTitle>
        <CardDescription>Complete your checkout</CardDescription>
      </CardHeader>
      <CardContent class="space-y-4">
        <div class="space-y-2 text-sm">
          <div class="flex justify-between">
            <span class="text-muted-foreground">Subtotal</span>
            <span class="font-medium">{{ formatCurrency(subtotal) }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-muted-foreground">VAT (15%)</span>
            <span class="font-medium">{{ formatCurrency(tax) }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-muted-foreground">Shipping</span>
            <span class="font-medium">
              {{ shipping === 0 ? 'Free' : formatCurrency(shipping) }}
            </span>
          </div>
        </div>
        <Separator />
        <div class="flex items-center justify-between text-base font-semibold">
          <span>Total</span>
          <span>{{ formatCurrency(total) }}</span>
        </div>
        <Button class="w-full" size="lg">
          Proceed to Checkout
        </Button>
        <p class="text-center text-xs text-muted-foreground">
          Secure checkout powered by TOSS Payments Suite.
        </p>
      </CardContent>
    </Card>
  </section>
</template>

