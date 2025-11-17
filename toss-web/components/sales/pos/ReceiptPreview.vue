<script setup lang="ts">
import type { PosSale } from '~/types/sales'

const props = defineProps<{
  sale: PosSale | null
}>()

const formatPrice = (amount: number) => {
  return `R ${Number(amount || 0).toFixed(2)}`
}

const formatDate = (date?: string) => {
  if (!date) return ''
  return new Date(date).toLocaleString()
}
</script>

<template>
  <div v-if="sale" class="receipt-paper text-sm text-foreground">
    <div class="text-center mb-4">
      <p class="font-semibold tracking-wide">TOSS POS</p>
      <p class="text-xs text-muted-foreground">Cashier Portal</p>
      <p class="text-xs text-muted-foreground">{{ formatDate(sale.createdAt) }}</p>
    </div>

    <div class="mb-3 space-y-1 text-xs">
      <p><span class="text-muted-foreground">Reference:</span> {{ sale.reference }}</p>
      <p v-if="sale.customerName">
        <span class="text-muted-foreground">Customer:</span> {{ sale.customerName }}
      </p>
    </div>

    <div class="border-t border-b border-border py-2 mb-3 space-y-2">
      <div
        v-for="item in sale.items"
        :key="item.productId || item.productName"
        class="flex justify-between text-xs"
      >
        <div class="flex-1 pr-2">
          <p class="font-medium">{{ item.productName }}</p>
          <p class="text-muted-foreground">
            {{ item.quantity }} × {{ formatPrice(item.unitPrice || item.rate) }}
          </p>
        </div>
        <p class="font-semibold">{{ formatPrice(item.total ?? item.quantity * (item.unitPrice || item.rate)) }}</p>
      </div>
    </div>

    <div class="space-y-1 text-xs">
      <div class="flex justify-between">
        <span class="text-muted-foreground">Subtotal</span>
        <span>{{ formatPrice(sale.subtotal) }}</span>
      </div>
      <div class="flex justify-between">
        <span class="text-muted-foreground">Discount</span>
        <span>-{{ formatPrice(sale.discount || 0) }}</span>
      </div>
      <div class="flex justify-between">
        <span class="text-muted-foreground">Tax</span>
        <span>{{ formatPrice(sale.tax || 0) }}</span>
      </div>
      <div class="flex justify-between text-base font-bold border-t border-border pt-1 mt-1">
        <span>Total</span>
        <span>{{ formatPrice(sale.total) }}</span>
      </div>
    </div>

    <div class="mt-4">
      <p class="text-xs font-semibold mb-1">Payments</p>
      <div v-if="sale.payments?.length" class="space-y-1 text-xs">
        <div
          v-for="(payment, index) in sale.payments"
          :key="payment.reference || `${payment.method || payment.mode}-${index}`"
          class="flex justify-between"
        >
          <span class="capitalize">
            {{ (payment.method || payment.mode || 'cash').replace('-', ' ') }}
          </span>
          <span>{{ formatPrice(payment.amount) }}</span>
        </div>
      </div>
      <p v-else class="text-muted-foreground text-xs">No payments recorded.</p>
    </div>

    <p class="text-center text-xs text-muted-foreground mt-4">
      Thank you for supporting township SMMEs.
    </p>
  </div>

  <div v-else class="text-xs text-muted-foreground">
    No completed sale yet. Complete a sale to preview the receipt.
  </div>
</template>

