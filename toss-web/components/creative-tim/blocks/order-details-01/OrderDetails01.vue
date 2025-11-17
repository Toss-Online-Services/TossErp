<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Separator } from '~/components/ui/separator'

type TimelineStep = {
  title: string
  status: 'Complete' | 'In Progress' | 'Pending'
  detail: string
  date: string
}

const props = withDefaults(
  defineProps<{
    timeline?: TimelineStep[]
  }>(),
  {
    timeline: () => [
      { title: 'Order Placed', status: 'Complete', detail: 'Your order was placed', date: 'April 1, 2024' },
      { title: 'Order Confirmed', status: 'Complete', detail: 'Confirmed on', date: 'April 2, 2024' },
      { title: 'Order Shipped', status: 'In Progress', detail: 'Shipped on', date: 'April 3, 2024' },
      { title: 'Order Delivered', status: 'Pending', detail: 'Expected delivery', date: 'April 6, 2024' },
    ],
  }
)
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">order-details-01</p>
      <h2 class="text-xl font-semibold">Order details with timeline</h2>
    </div>
    <Card class="border-border/70">
      <CardHeader class="flex flex-col gap-2 md:flex-row md:items-center md:justify-between">
        <div>
          <CardTitle>Order #1234</CardTitle>
          <CardDescription>Placed on April 1, 2024 · 1 item</CardDescription>
        </div>
        <Button variant="outline" size="sm">
          View invoice
        </Button>
      </CardHeader>
      <CardContent class="grid gap-6 lg:grid-cols-[2fr,1fr]">
        <div class="space-y-4">
          <div class="grid gap-3">
            <div class="rounded-xl border border-border/80 p-4">
              <p class="text-sm font-medium text-muted-foreground">Premium Suit</p>
              <p class="text-2xl font-semibold">$790</p>
              <p class="text-xs text-muted-foreground">Size M · Qty 1</p>
            </div>
            <div class="rounded-xl border border-border/80 p-4 text-sm text-muted-foreground">
              <p>Delivery Address</p>
              <p class="font-medium text-foreground">362 Ridgewood Avenue, Alaska 99669, USA</p>
            </div>
          </div>

          <Separator />

          <div class="space-y-4">
            <p class="text-sm font-semibold text-muted-foreground">Order Timeline</p>
            <div class="space-y-4">
              <div
                v-for="step in props.timeline"
                :key="step.title"
                class="flex gap-4"
              >
                <div class="relative flex flex-col items-center">
                  <span
                    class="h-3 w-3 rounded-full"
                    :class="step.status === 'Complete' ? 'bg-emerald-500' : step.status === 'In Progress' ? 'bg-amber-500' : 'bg-border'"
                  />
                  <span class="mt-1 h-full w-px bg-border" />
                </div>
                <div>
                  <p class="font-medium">{{ step.title }}</p>
                  <p class="text-sm text-muted-foreground">
                    {{ step.detail }} {{ step.date }}
                  </p>
                  <p class="text-xs uppercase tracking-wide text-muted-foreground/80">
                    {{ step.status }}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="space-y-4 rounded-xl border border-border/80 p-4 text-sm">
          <div class="flex justify-between">
            <span class="text-muted-foreground">Subtotal</span>
            <span class="font-medium">$1,780.00</span>
          </div>
          <div class="flex justify-between">
            <span class="text-muted-foreground">Shipping estimate</span>
            <span class="font-medium">$0</span>
          </div>
          <div class="flex justify-between">
            <span class="text-muted-foreground">Tax estimate</span>
            <span class="font-medium">$5</span>
          </div>
          <Separator />
          <div class="flex justify-between text-base font-semibold">
            <span>Order Total</span>
            <span>$1,785.00</span>
          </div>
          <Button class="w-full">
            Track order
          </Button>
          <p class="text-center text-xs text-muted-foreground">
            Estimated delivery: April 6, 2024
          </p>
        </div>
      </CardContent>
    </Card>
  </section>
</template>

