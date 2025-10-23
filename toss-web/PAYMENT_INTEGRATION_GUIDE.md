# Payment Link Integration Guide

## Overview
This guide explains how to integrate payment links into TOSS for Group Buying pools and Shared Delivery runs.

---

## Payment Gateway Options for South Africa

### Recommended Providers

1. **Yoco** (Recommended)
   - ✅ South African company
   - ✅ No monthly fees
   - ✅ Simple API
   - ✅ WhatsApp payment links
   - Cost: 2.95% per transaction
   - Website: https://www.yoco.com

2. **Paystack**
   - ✅ Popular in Africa
   - ✅ Good documentation
   - ✅ Subscription support
   - Cost: 2.9% + R2 per transaction
   - Website: https://paystack.com

3. **PayFast**
   - ✅ Local South African
   - ✅ Established provider
   - ✅ Subscription billing
   - Cost: 2.9% + R2 per transaction
   - Website: https://www.payfast.co.za

---

## Implementation Steps

### 1. Set Up Payment Gateway Account

```bash
# For Yoco
1. Sign up at https://www.yoco.com/za/online/
2. Verify business details
3. Get API keys from dashboard
4. Add keys to .env file
```

```env
# .env
YOCO_SECRET_KEY=sk_live_xxxxxxxxxxxxx
YOCO_PUBLIC_KEY=pk_live_xxxxxxxxxxxxx
PAYMENT_WEBHOOK_SECRET=whsec_xxxxxxxxxxxxx
```

### 2. Install Payment SDK

```bash
# For Yoco
pnpm add @yoco/sdk

# For Paystack
pnpm add paystack

# For PayFast
pnpm add payfast
```

### 3. Create Payment Composable

Create `composables/usePayments.ts`:

```typescript
import { ref } from 'vue'

export interface PaymentLinkRequest {
  amount: number
  currency: string
  description: string
  customerName: string
  customerEmail?: string
  customerPhone: string
  metadata: {
    type: 'pool' | 'run'
    id: string
    shopId: string
  }
  successUrl: string
  cancelUrl: string
}

export interface PaymentLink {
  id: string
  url: string
  amount: number
  status: 'pending' | 'paid' | 'failed' | 'expired'
  createdAt: Date
  expiresAt: Date
}

export const usePayments = () => {
  const loading = ref(false)
  const error = ref<string | null>(null)

  const createPaymentLink = async (request: PaymentLinkRequest): Promise<PaymentLink> => {
    loading.value = true
    error.value = null
    
    try {
      // Example with Yoco API
      const response = await $fetch('/api/payments/create-link', {
        method: 'POST',
        body: request
      })
      
      return response as PaymentLink
    } catch (err) {
      error.value = 'Failed to create payment link'
      throw err
    } finally {
      loading.value = false
    }
  }

  const getPaymentStatus = async (paymentId: string): Promise<string> => {
    try {
      const response = await $fetch(`/api/payments/${paymentId}/status`)
      return response.status
    } catch (err) {
      error.value = 'Failed to get payment status'
      throw err
    }
  }

  const sendPaymentLink = async (paymentLink: PaymentLink, phone: string): Promise<void> => {
    try {
      await $fetch('/api/payments/send-link', {
        method: 'POST',
        body: {
          paymentLinkId: paymentLink.id,
          phone,
          message: `Pay R${paymentLink.amount} for your purchase: ${paymentLink.url}`
        }
      })
    } catch (err) {
      error.value = 'Failed to send payment link'
      throw err
    }
  }

  return {
    loading,
    error,
    createPaymentLink,
    getPaymentStatus,
    sendPaymentLink
  }
}
```

### 4. Create API Endpoints

Create `server/api/payments/create-link.post.ts`:

```typescript
import { Yoco } from '@yoco/sdk'

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const config = useRuntimeConfig()
  
  const yoco = new Yoco(config.yocoSecretKey)
  
  try {
    const paymentLink = await yoco.paymentLinks.create({
      amount: body.amount * 100, // Convert to cents
      currency: body.currency || 'ZAR',
      description: body.description,
      metadata: body.metadata,
      successUrl: body.successUrl,
      cancelUrl: body.cancelUrl
    })
    
    return {
      id: paymentLink.id,
      url: paymentLink.url,
      amount: body.amount,
      status: 'pending',
      createdAt: new Date(),
      expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000) // 24 hours
    }
  } catch (error) {
    throw createError({
      statusCode: 500,
      message: 'Failed to create payment link'
    })
  }
})
```

Create `server/api/payments/webhook.post.ts`:

```typescript
export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const config = useRuntimeConfig()
  
  // Verify webhook signature
  const signature = getHeader(event, 'x-yoco-signature')
  // Implement signature verification
  
  // Handle payment events
  switch (body.type) {
    case 'payment.succeeded':
      await handlePaymentSuccess(body.data)
      break
    case 'payment.failed':
      await handlePaymentFailure(body.data)
      break
    case 'payment.refunded':
      await handlePaymentRefund(body.data)
      break
  }
  
  return { received: true }
})

async function handlePaymentSuccess(data: any) {
  // Update pool or run payment status
  const metadata = data.metadata
  
  if (metadata.type === 'pool') {
    // Update pool participant payment status
    await $fetch(`/api/pools/${metadata.id}/payment-confirmed`, {
      method: 'POST',
      body: { shopId: metadata.shopId, paymentId: data.id }
    })
  } else if (metadata.type === 'run') {
    // Update run participant payment status
    await $fetch(`/api/runs/${metadata.id}/payment-confirmed`, {
      method: 'POST',
      body: { shopId: metadata.shopId, paymentId: data.id }
    })
  }
  
  // Send WhatsApp confirmation
  await sendWhatsAppNotification(metadata.phone, 'Payment confirmed! ✅')
}
```

### 5. Integrate into Group Buying

Update `composables/useGroupBuying.ts`:

```typescript
import { usePayments } from './usePayments'

export const useGroupBuying = () => {
  const payments = usePayments()
  
  // ... existing code ...
  
  const confirmPool = async (poolId: string): Promise<void> => {
    loading.value = true
    
    try {
      // Mark pool as confirmed
      const pool = mockPools.find(p => p.id === poolId)
      if (pool) {
        pool.status = 'pending' // Pending payment
        
        // Generate payment links for all participants
        for (const participant of pool.participantList) {
          const paymentLink = await payments.createPaymentLink({
            amount: pool.unitPrice * participant.quantity,
            currency: 'ZAR',
            description: `Pool payment for ${pool.productName}`,
            customerName: participant.name,
            customerPhone: participant.phone,
            metadata: {
              type: 'pool',
              id: poolId,
              shopId: participant.id
            },
            successUrl: `${window.location.origin}/buying/group-buying?payment=success`,
            cancelUrl: `${window.location.origin}/buying/group-buying?payment=cancelled`
          })
          
          // Send payment link via WhatsApp
          await payments.sendPaymentLink(paymentLink, participant.phone)
        }
      }
    } finally {
      loading.value = false
    }
  }
  
  return {
    // ... existing exports ...
    confirmPool
  }
}
```

### 6. Add Payment Status Display

Create `components/buying/PaymentStatusBadge.vue`:

```vue
<template>
  <span 
    class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium"
    :class="statusClass"
  >
    {{ statusIcon }} {{ statusLabel }}
  </span>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  status: 'pending' | 'paid' | 'failed' | 'expired'
}>()

const statusClass = computed(() => {
  const classes = {
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    paid: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    failed: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    expired: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
  }
  return classes[props.status]
})

const statusIcon = computed(() => {
  const icons = {
    pending: '⏳',
    paid: '✅',
    failed: '❌',
    expired: '⏰'
  }
  return icons[props.status]
})

const statusLabel = computed(() => {
  const labels = {
    pending: 'Pending Payment',
    paid: 'Paid',
    failed: 'Payment Failed',
    expired: 'Payment Expired'
  }
  return labels[props.status]
})
</script>
```

### 7. Update Pool Display

In `pages/buying/group-buying/index.vue`, add payment status:

```vue
<template>
  <!-- ... existing code ... -->
  
  <div class="flex items-center gap-2">
    <span :class="getStatusBadge(pool.status)">
      {{ getStatusLabel(pool.status) }}
    </span>
    <PaymentStatusBadge 
      v-if="pool.status === 'pending' && pool.myPaymentStatus"
      :status="pool.myPaymentStatus"
    />
  </div>
  
  <!-- ... existing code ... -->
</template>
```

---

## Testing

### Test Mode Setup

```typescript
// Use test API keys for development
if (process.env.NODE_ENV === 'development') {
  YOCO_SECRET_KEY = 'sk_test_xxxxxxxxxxxxx'
  YOCO_PUBLIC_KEY = 'pk_test_xxxxxxxxxxxxx'
}
```

### Test Payment Flow

1. Create a pool
2. Join the pool
3. Confirm pool (triggers payment link generation)
4. Receive WhatsApp with payment link
5. Complete test payment
6. Verify webhook updates pool status
7. Check "Payment Confirmed" status

---

## Security Considerations

1. **Never expose secret keys in frontend code**
2. **Always verify webhook signatures**
3. **Use HTTPS in production**
4. **Implement rate limiting on payment endpoints**
5. **Log all payment events for audit trail**
6. **Handle failed payments gracefully**
7. **Set payment link expiration times**

---

## Monitoring & Analytics

### Track These Metrics

```typescript
interface PaymentMetrics {
  totalPaymentLinks: number
  successRate: number
  averageTimeToPayment: number
  failureReasons: Record<string, number>
  revenueByPool: Record<string, number>
}
```

### Dashboard Queries

```sql
-- Payment success rate
SELECT 
  COUNT(*) FILTER (WHERE status = 'paid') * 100.0 / COUNT(*) as success_rate
FROM payments
WHERE created_at > NOW() - INTERVAL '7 days';

-- Average payment time
SELECT 
  AVG(paid_at - created_at) as avg_payment_time
FROM payments
WHERE status = 'paid';
```

---

## Troubleshooting

### Common Issues

**Issue**: Payment link not received via WhatsApp
- **Solution**: Check WhatsApp Business API credentials
- **Solution**: Verify phone number format (+27...)

**Issue**: Webhook not triggering
- **Solution**: Verify webhook URL is publicly accessible
- **Solution**: Check webhook signature verification

**Issue**: Payment marked as failed but customer paid
- **Solution**: Check payment gateway dashboard
- **Solution**: Manually reconcile and update status

---

## Cost Estimation

### Per Transaction Costs

```
Pool Payment: R500
Gateway Fee (2.95%): R14.75
Net Amount: R485.25

Shared Run Payment: R300
Gateway Fee (2.95%): R8.85
Net Amount: R291.15
```

### Monthly Estimates (100 transactions)

```
Average Transaction: R400
Gateway Fees: R400 × 2.95% × 100 = R1,180/month
Revenue: R40,000
Net after fees: R38,820
```

---

## Next Steps

1. ✅ Choose payment gateway (Yoco recommended)
2. ✅ Create merchant account
3. ✅ Get API keys
4. ✅ Implement composable and API endpoints
5. ✅ Test in sandbox mode
6. ✅ Deploy webhook endpoint
7. ✅ Go live with production keys
8. ✅ Monitor payment success rates

---

## Support Resources

- **Yoco Docs**: https://developer.yoco.com
- **Paystack Docs**: https://paystack.com/docs
- **PayFast Docs**: https://www.payfast.co.za/developers

---

**Status**: Ready for Implementation
**Priority**: High (Blocks Pool Confirmation Flow)
**Estimated Time**: 2-3 days with testing

