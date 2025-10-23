# Group Buying & Shared Logistics - Developer Guide

Quick reference for developers working with TOSS collective procurement features.

---

## 🚀 Quick Start

### Import Types
```typescript
import type { Pool, PoolParticipant, CreatePoolDto } from '~/types/group-buying'
import type { SharedRun, DeliveryStop } from '~/types/logistics'
```

### Use Stores
```typescript
const { activePools, createPool, joinPool } = useGroupBuying()
const { activeRuns, createRun, addStopToRun } = useSharedDelivery()
```

### Use Composables
```typescript
const { 
  calculateFillPercentage,
  getTimeRemaining,
  canJoinPool 
} = useGroupBuying()

const {
  calculateDeliverySavings,
  getStopETA,
  canJoinRun
} = useSharedDelivery()
```

---

## 📦 Group Buying

### Creating a Pool

```vue
<script setup>
const { createPool } = useGroupBuying()

const newPool = await createPool({
  sku: 'BREAD-001',
  itemId: 'item-1',
  title: 'White Bread Bulk Order',
  description: 'Fresh white bread for the week',
  targetQuantity: 100,
  minParticipants: 3,
  maxParticipants: 10,
  deadline: new Date(Date.now() + 48 * 60 * 60 * 1000),
  area: 'soweto-north',
  currentPrice: 12.50,
  poolPrice: 10.00,
  splitRule: 'by-units',
  supplierId: 'supplier-1',
  supplierName: 'Albany Bread'
})
</script>
```

### Joining a Pool

```typescript
await joinPool({
  poolId: 'pool-123',
  shopId: 'shop-456',
  shopName: 'My Spaza Shop',
  quantityCommitted: 20
})
```

### Checking Pool Status

```vue
<template>
  <div>
    <!-- Progress bar -->
    <div class="progress">
      <div :style="{ width: calculateFillPercentage(pool) + '%' }"></div>
    </div>
    
    <!-- Time remaining -->
    <p>{{ getTimeRemaining(pool) }}</p>
    
    <!-- Join button -->
    <button 
      v-if="canJoinPool(pool, shopId).canJoin"
      @click="joinPool(...)"
    >
      Join Pool
    </button>
    <p v-else class="error">
      {{ canJoinPool(pool, shopId).reason }}
    </p>
    
    <!-- Status badge -->
    <span :class="getPoolStatusBadge(pool).color">
      {{ getPoolStatusBadge(pool).icon }} {{ getPoolStatusBadge(pool).label }}
    </span>
  </div>
</template>
```

### Calculating Savings

```typescript
const savings = calculateParticipantSavings(pool, quantityCommitted)

console.log(`Regular: ${formatCurrency(savings.regularCost)}`)
console.log(`Pool: ${formatCurrency(savings.poolCost)}`)
console.log(`Savings: ${formatCurrency(savings.savings)} (${savings.savingsPercentage}%)`)
```

---

## 🚚 Shared Logistics

### Creating a Delivery Run

```typescript
const { createRun } = useSharedDelivery()

const run = await createRun({
  pickupLocation: {
    address: 'JHB Fresh Produce Market',
    coordinates: { lat: -26.2418, lng: 28.0473 },
    zone: 'city-center'
  },
  scheduledDate: new Date(Date.now() + 24 * 60 * 60 * 1000),
  pickupWindow: {
    start: new Date(Date.now() + 24 * 60 * 60 * 1000),
    end: new Date(Date.now() + 26 * 60 * 60 * 1000)
  },
  vehicleType: 'bakkie',
  maxCapacity: 500,
  maxWeight: 500,
  baseFee: 150,
  feePerStop: 30,
  feeSplitRule: 'by-stops',
  driverId: 'driver-1',
  stops: [/* initial stops */]
})
```

### Adding a Stop

```typescript
await addStopToRun(runId, {
  shopId: 'shop-789',
  location: {
    address: '123 Vilakazi Street, Soweto',
    coordinates: { lat: -26.2477, lng: 27.9089 },
    zone: 'soweto-north'
  },
  orderId: 'pool-123',
  orderType: 'pool-participation',
  items: [
    {
      itemId: 'item-1',
      itemName: 'White Bread',
      sku: 'BREAD-001',
      quantity: 20,
      weight: 10,
      volume: 0.08
    }
  ],
  weight: 10,
  volume: 0.08,
  contactName: 'Thabo',
  contactPhone: '+27821234567'
})
```

### Capturing Proof of Delivery

```typescript
await capturePOD({
  stopId: 'stop-456',
  type: 'pin',
  value: '1234',
  latitude: -26.2477,
  longitude: 27.9089,
  notes: 'Delivered to owner'
})

// Or with photo
await capturePOD({
  stopId: 'stop-456',
  type: 'photo',
  value: 'data:image/jpeg;base64,...',
  latitude: -26.2477,
  longitude: 27.9089
})
```

### Tracking Delivery

```vue
<script setup>
const { startTracking, stopTracking, driverLocation } = useSharedDelivery()
const { getStopETA, isStopNearby } = useSharedDelivery()

// Start tracking
const eventSource = await startTracking(runId)

onUnmounted(() => {
  stopTracking()
  eventSource.close()
})
</script>

<template>
  <div>
    <!-- Driver location -->
    <div v-if="driverLocation">
      Driver at: {{ driverLocation.lat }}, {{ driverLocation.lng }}
    </div>
    
    <!-- Stop ETA -->
    <p v-for="stop in myDeliveries" :key="stop.id">
      ETA: {{ getStopETA(stop) }}
      <span v-if="isStopNearby(stop)">🔔 Driver nearby!</span>
    </p>
  </div>
</template>
```

---

## 💡 Common Patterns

### Pool + Delivery Integration

```typescript
// 1. Create pool
const pool = await createPool({...})

// 2. Shops join
await joinPool({...})
await joinPool({...})

// 3. Confirm when target reached
if (hasReachedTarget(pool)) {
  await confirmPool(pool.id)
  // This creates PO and payment links
}

// 4. After PO confirmed, create shared run
const run = await createRun({
  linkedOrderIds: [pool.id],
  stops: pool.participants.map(p => ({
    shopId: p.shopId,
    orderId: pool.id,
    // ... other stop details
  }))
})

// 5. Driver delivers and captures POD
for (const stop of run.dropList) {
  await capturePOD({
    stopId: stop.id,
    type: 'pin',
    value: stop.deliveryPIN
  })
}

// 6. Mark pool as fulfilled
// (happens automatically when all PODs captured)
```

### AI Suggestions

```typescript
const { getPoolSuggestion } = useGroupBuying()
const { getDeliverySuggestion } = useSharedDelivery()

// For low stock items
const lowStockItems = ['BREAD-001', 'MILK-001']
const suggestion = getPoolSuggestion(pool, shopId, lowStockItems)

if (suggestion.priority === 'high') {
  // Show prominent notification
  showNotification({
    title: 'Urgent: Join Pool',
    message: suggestion.message,
    action: suggestion.action
  })
}

// For delivery
const deliverySuggestion = getDeliverySuggestion(
  orderZone,
  orderWeight,
  availableRuns
)

if (deliverySuggestion.action === 'join-existing') {
  showOption({
    title: 'Save on Delivery',
    message: deliverySuggestion.message,
    savings: deliverySuggestion.estimatedSavings,
    run: deliverySuggestion.run
  })
}
```

---

## 🔧 Utility Functions

### Route Optimization

```typescript
import { 
  calculateDistance,
  optimizeRoute,
  validateRoute 
} from '~/server/utils/route-optimizer'

// Calculate distance between two points
const distance = calculateDistance(location1, location2)
console.log(`Distance: ${distance.toFixed(2)} km`)

// Optimize delivery route
const optimizedStops = optimizeRoute(pickupLocation, stops)

// Validate route feasibility
const validation = validateRoute(stops, vehicleCapacity, maxDistance)
if (!validation.isValid) {
  console.error('Route issues:', validation.issues)
}
```

### WhatsApp Notifications

```typescript
import { 
  sendPoolInvite,
  sendDeliveryUpdate 
} from '~/server/utils/whatsapp'

// Send pool invite
await sendPoolInvite(pool, '+27821234567', 'Join us for great savings!')

// Send delivery update
await sendDeliveryUpdate(stop, 'out-for-delivery')
```

### Payment Links

```typescript
import { 
  generatePayLink,
  processPaymentWebhook 
} from '~/server/utils/payments'

// Generate payment link
const paymentUrl = await generatePayLink(
  participant,
  participant.costShare + participant.deliveryFeeShare,
  `Pool payment for ${pool.title}`
)

// Handle webhook
const result = await processPaymentWebhook(webhookPayload)
if (result.status === 'paid') {
  // Update participant status
  participant.paymentStatus = 'paid'
}
```

---

## 🎨 UI Components

### Pool Card

```vue
<template>
  <div class="pool-card">
    <!-- Header -->
    <div class="header">
      <h3>{{ pool.title }}</h3>
      <span :class="getPoolStatusBadge(pool).color">
        {{ getPoolStatusBadge(pool).label }}
      </span>
    </div>
    
    <!-- Progress -->
    <div class="progress-section">
      <div class="progress-bar">
        <div 
          class="progress-fill"
          :style="{ width: calculateFillPercentage(pool) + '%' }"
        ></div>
      </div>
      <p>{{ pool.currentCommitment }}/{{ pool.targetQuantity }} units</p>
    </div>
    
    <!-- Savings -->
    <div class="savings">
      <p class="highlight">Save {{ pool.savingsPercentage }}%</p>
      <p class="price">
        <span class="original">R{{ pool.currentPrice }}</span>
        →
        <span class="pool">R{{ pool.poolPrice }}</span>
      </p>
    </div>
    
    <!-- Deadline -->
    <p class="deadline">⏰ {{ getTimeRemaining(pool) }}</p>
    
    <!-- Participants -->
    <div class="participants">
      <span v-for="p in pool.participants" :key="p.id" class="avatar">
        {{ p.shopName.charAt(0) }}
      </span>
      <span class="count">{{ pool.participantCount }} shops</span>
    </div>
    
    <!-- Actions -->
    <button 
      v-if="canJoinPool(pool, shopId).canJoin"
      @click="handleJoin"
      class="btn-primary"
    >
      Join Pool
    </button>
    <button 
      v-else-if="isParticipant"
      @click="viewDetails"
      class="btn-secondary"
    >
      View Details
    </button>
  </div>
</template>
```

### Delivery Stop Card

```vue
<template>
  <div class="stop-card">
    <!-- Header -->
    <div class="header">
      <h4>{{ stop.shopName }}</h4>
      <span :class="getStopStatusBadge(stop).color">
        {{ getStopStatusBadge(stop).icon }} {{ getStopStatusBadge(stop).label }}
      </span>
    </div>
    
    <!-- Location -->
    <p class="location">📍 {{ formatAddress(stop.location) }}</p>
    
    <!-- Sequence -->
    <p class="sequence">Stop #{{ stop.sequenceNumber }}</p>
    
    <!-- ETA -->
    <div class="eta">
      <p>ETA: {{ getStopETA(stop) }}</p>
      <p v-if="isStopNearby(stop)" class="nearby">
        🔔 Driver nearby!
      </p>
    </div>
    
    <!-- Items -->
    <div class="items">
      <p v-for="item in stop.items" :key="item.itemId">
        {{ item.quantity }}x {{ item.itemName }}
      </p>
    </div>
    
    <!-- Fee -->
    <div class="fee">
      <p>Your share: {{ formatCurrency(stop.feeShare) }}</p>
      <p class="explanation">{{ getFeeShareExplanation(stop, run) }}</p>
    </div>
    
    <!-- POD -->
    <div v-if="stop.proofOfDelivery" class="pod">
      <p>✅ Delivered {{ formatTime(stop.deliveredAt) }}</p>
      <img 
        v-if="stop.proofOfDelivery.type === 'photo'" 
        :src="stop.proofOfDelivery.value"
        class="pod-photo"
      />
    </div>
  </div>
</template>
```

---

## 🧪 Testing

### Unit Tests

```typescript
import { describe, it, expect } from 'vitest'
import { calculateFillPercentage, hasReachedTarget } from '~/composables/useGroupBuying'

describe('useGroupBuying', () => {
  it('calculates fill percentage correctly', () => {
    const pool = {
      targetQuantity: 100,
      currentCommitment: 75
    }
    expect(calculateFillPercentage(pool)).toBe(75)
  })
  
  it('checks if target reached', () => {
    const pool = {
      targetQuantity: 100,
      currentCommitment: 100
    }
    expect(hasReachedTarget(pool)).toBe(true)
  })
})
```

### E2E Tests

```typescript
import { test, expect } from '@playwright/test'

test('user can create and join pool', async ({ page }) => {
  await page.goto('/group-buying')
  
  // Create pool
  await page.click('text=New Pool')
  await page.fill('input[name="title"]', 'Test Pool')
  await page.fill('input[name="targetQuantity"]', '100')
  await page.click('button[type="submit"]')
  
  // Verify pool created
  await expect(page.locator('text=Test Pool')).toBeVisible()
  
  // Join pool
  await page.click('text=Join Pool')
  await page.fill('input[name="quantity"]', '20')
  await page.click('text=Confirm Join')
  
  // Verify joined
  await expect(page.locator('text=You joined')).toBeVisible()
})
```

---

## 📚 API Reference

See `IMPLEMENTATION_SUMMARY_GROUP_BUYING.md` for complete API documentation.

### Key Endpoints

```
GET    /api/pools
POST   /api/pools
GET    /api/pools/:id
PATCH  /api/pools/:id/join
PATCH  /api/pools/:id/confirm
GET    /api/pools/:id/savings

GET    /api/runs
POST   /api/runs
GET    /api/runs/:id
POST   /api/runs/:id/stops
POST   /api/runs/stops/:id/pod
GET    /api/runs/:id/cost-breakdown
```

---

## 🐛 Common Issues

### Pool not updating after join

**Problem:** Store state not refreshing.

**Solution:** Call `fetchPool(poolId)` after join action.

```typescript
await joinPool({...})
await fetchPool(poolId) // Refresh pool data
```

### Route optimization slow

**Problem:** Large number of stops (>20).

**Solution:** Use batching or limit stops per run.

```typescript
const MAX_STOPS = 10
if (stops.length > MAX_STOPS) {
  // Split into multiple runs
  const runs = chunk(stops, MAX_STOPS).map(batch =>
    createRun({ stops: batch })
  )
}
```

### Payment webhook not received

**Problem:** Webhook URL not accessible or signature invalid.

**Solution:** Check webhook URL and verify signature.

```typescript
// In webhook handler
if (!verifyPaymentWebhook(payload, signature)) {
  throw createError({ statusCode: 401, message: 'Invalid signature' })
}
```

---

## 📖 Further Reading

- [TOSS PRD Enhanced](../TOSS_PRD_ENHANCED.md)
- [Implementation Summary](../IMPLEMENTATION_SUMMARY_GROUP_BUYING.md)
- [Nuxt 3 Docs](https://nuxt.com)
- [Pinia Docs](https://pinia.vuejs.org)

---

**Last Updated:** January 20, 2025  
**Version:** 1.0  
**Maintainer:** TOSS Development Team

