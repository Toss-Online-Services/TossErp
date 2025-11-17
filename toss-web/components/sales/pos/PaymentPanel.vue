<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Input } from '~/components/ui/input'
import { Label } from '~/components/ui/label'
import { Badge } from '~/components/ui/badge'
import { Separator } from '~/components/ui/separator'
import { CreditCard, Banknote, Smartphone, Building2, Trash2 } from 'lucide-vue-next'
import type { PosPaymentEntry } from '~/types/sales'

const props = defineProps<{
  total: number
  payments: PosPaymentEntry[]
  totalPaid: number
  balance: number
  isComplete: boolean
}>()

const emit = defineEmits<{
  addPayment: [payment: Omit<PosPaymentEntry, 'id'>]
  removePayment: [paymentId: number]
  completeSale: []
}>()

const paymentMethods = [
  { value: 'cash', label: 'Cash', icon: Banknote, color: 'bg-green-500' },
  { value: 'card', label: 'Card', icon: CreditCard, color: 'bg-blue-500' },
  { value: 'mobile-money', label: 'Mobile Money', icon: Smartphone, color: 'bg-purple-500' },
  { value: 'account', label: 'Account', icon: Building2, color: 'bg-orange-500' }
] as const

const selectedMethod = ref<'cash' | 'card' | 'mobile-money' | 'bank-transfer' | 'pay-link' | 'account'>('cash')
const paymentAmount = ref(0)
const paymentReference = ref('')

const formatPrice = (price: number) => {
  return `R ${price.toFixed(2)}`
}

const quickAmounts = computed(() => {
  const remaining = props.balance
  if (remaining <= 0) return []
  
  const amounts = [10, 20, 50, 100, 200, 500]
  return amounts.filter(amt => amt <= remaining * 2)
})

const setQuickAmount = (amount: number) => {
  paymentAmount.value = amount
}

const setExactAmount = () => {
  paymentAmount.value = props.balance
}

const addPayment = () => {
  if (paymentAmount.value <= 0) return
  
  emit('addPayment', {
    method: selectedMethod.value,
    amount: paymentAmount.value,
    reference: paymentReference.value || undefined,
    receivedAt: new Date().toISOString()
  })
  
  // Reset form
  paymentAmount.value = 0
  paymentReference.value = ''
}

const getMethodIcon = (method: string) => {
  const found = paymentMethods.find(m => m.value === method)
  return found?.icon || CreditCard
}

const getMethodColor = (method: string) => {
  const found = paymentMethods.find(m => m.value === method)
  return found?.color || 'bg-gray-500'
}
</script>

<template>
  <Card class="h-full flex flex-col">
    <CardHeader class="pb-3">
      <CardTitle class="text-lg">Payment</CardTitle>
    </CardHeader>
    
    <CardContent class="flex-1 flex flex-col gap-4">
      <!-- Payment Summary -->
      <div class="p-4 rounded-lg bg-primary/10 border border-primary/20">
        <div class="space-y-2">
          <div class="flex justify-between text-sm">
            <span class="text-muted-foreground">Total:</span>
            <span class="font-semibold">{{ formatPrice(total) }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-muted-foreground">Paid:</span>
            <span class="font-semibold text-green-600">{{ formatPrice(totalPaid) }}</span>
          </div>
          <Separator />
          <div class="flex justify-between">
            <span class="font-medium">Balance:</span>
            <span class="text-lg font-bold" :class="balance > 0 ? 'text-orange-600' : 'text-green-600'">
              {{ formatPrice(balance) }}
            </span>
          </div>
        </div>
      </div>

      <!-- Existing Payments -->
      <div v-if="payments.length > 0" class="space-y-2">
        <Label class="text-xs text-muted-foreground">Applied Payments:</Label>
        <div class="space-y-2">
          <div
            v-for="payment in payments"
            :key="payment.id"
            class="flex items-center justify-between p-2 rounded-md border bg-card"
          >
            <div class="flex items-center gap-2">
              <div :class="getMethodColor(payment.method)" class="p-1.5 rounded">
                <component :is="getMethodIcon(payment.method)" class="h-4 w-4 text-white" />
              </div>
              <div>
                <p class="text-sm font-medium capitalize">{{ payment.method.replace('-', ' ') }}</p>
                <p v-if="payment.reference" class="text-xs text-muted-foreground">{{ payment.reference }}</p>
              </div>
            </div>
            <div class="flex items-center gap-2">
              <span class="font-semibold">{{ formatPrice(payment.amount) }}</span>
              <Button
                size="icon"
                variant="ghost"
                class="h-7 w-7 text-destructive hover:text-destructive"
                @click="emit('removePayment', payment.id)"
              >
                <Trash2 class="h-4 w-4" />
              </Button>
            </div>
          </div>
        </div>
      </div>

      <Separator />

      <!-- Add Payment Form -->
      <div v-if="balance > 0" class="space-y-4">
        <div class="space-y-2">
          <Label>Payment Method</Label>
          <div class="grid grid-cols-2 gap-2">
            <Button
              v-for="method in paymentMethods"
              :key="method.value"
              :variant="selectedMethod === method.value ? 'default' : 'outline'"
              class="justify-start"
              @click="selectedMethod = method.value"
            >
              <component :is="method.icon" class="h-4 w-4 mr-2" />
              {{ method.label }}
            </Button>
          </div>
        </div>

        <div class="space-y-2">
          <Label>Amount</Label>
          <Input
            v-model.number="paymentAmount"
            type="number"
            step="0.01"
            min="0"
            :max="balance * 2"
            placeholder="0.00"
          />
          <div class="flex flex-wrap gap-2">
            <Button
              v-for="amount in quickAmounts"
              :key="amount"
              size="sm"
              variant="outline"
              @click="setQuickAmount(amount)"
            >
              {{ formatPrice(amount) }}
            </Button>
            <Button size="sm" variant="outline" @click="setExactAmount">
              Exact
            </Button>
          </div>
        </div>

        <div class="space-y-2">
          <Label>Reference (Optional)</Label>
          <Input
            v-model="paymentReference"
            type="text"
            placeholder="Transaction ID, receipt number, etc."
          />
        </div>

        <Button
          class="w-full"
          :disabled="paymentAmount <= 0"
          @click="addPayment"
        >
          Add Payment
        </Button>
      </div>

      <Separator />

      <!-- Complete Sale Button -->
      <Button
        class="w-full"
        size="lg"
        :disabled="!isComplete || total === 0"
        @click="emit('completeSale')"
      >
        <CreditCard class="h-5 w-5 mr-2" />
        Complete Sale
      </Button>

      <div v-if="balance < 0" class="text-center">
        <Badge variant="secondary">
          Change: {{ formatPrice(Math.abs(balance)) }}
        </Badge>
      </div>
    </CardContent>
  </Card>
</template>
