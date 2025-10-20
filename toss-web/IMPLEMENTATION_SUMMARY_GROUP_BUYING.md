# TOSS Group Buying & Shared Logistics - Implementation Summary

## Overview

This document summarizes the Phase 1 implementation of Group Buying and Shared Logistics features for TOSS (Township Operations Support System), completed as per the architectural design plan.

**Implementation Date:** January 2025  
**Status:** ✅ Phase 1 Core Infrastructure Complete  
**Next Phase:** Phase 2 - Shared Logistics API completion & Integration

---

## 📦 What Has Been Implemented

### 1. TypeScript Type Definitions

#### `types/group-buying.ts`
Comprehensive type system for collective procurement:
- **Pool** - Main group buying entity with lifecycle states
- **PoolParticipant** - Shop participation records with payment tracking
- **PoolStatus** - State machine: open → pending → confirmed → fulfilled
- **DTOs** - Request/response types for all API operations
- **Analytics** - Types for reporting and metrics

**Key Features:**
- Single-SKU pools (MVP constraint)
- Flexible split rules (flat or by-units)
- Geographic zone support
- Payment link integration
- Extension tracking (one-time only)

#### `types/logistics.ts`
Complete shared delivery system types:
- **SharedRun** - Delivery run with multi-drop routes
- **DeliveryStop** - Individual shop drop-off points
- **POD (Proof of Delivery)** - PIN, photo, or signature capture
- **RouteOptimization** - Request/response for route planning
- **DeliveryCostBreakdown** - Transparent fee splitting

**Key Features:**
- Multiple fee split rules (by-stops, by-weight, by-distance, flat)
- Vehicle type support (bakkie, truck, van, motorcycle)
- Real-time tracking capability
- ETA calculations
- Driver rating system

---

### 2. Pinia State Management Stores

#### `stores/groupBuying.ts`
Centralized state management for pools:
- **State:** activePools, myLeadPools, myParticipations, analytics
- **Getters:** Smart computed properties for filtering and status checks
- **Actions:** Full CRUD operations with API integration

**Implemented Actions:**
- `fetchPools()` - List pools with filters
- `createPool()` - Start new group buy
- `joinPool()` - Participate in existing pool
- `leavePool()` - Exit before confirmation
- `confirmPool()` - Lock pool and create PO
- `cancelPool()` - Lead cancellation
- `generateInvite()` - WhatsApp invite links
- `calculateSavings()` - Real-time savings calculator
- `fetchAnalytics()` - Pool performance metrics

#### `stores/sharedLogistics.ts`
Delivery run and stop management:
- **State:** activeRuns, myDeliveries, driverLocation tracking
- **Getters:** Status filtering, capacity checks
- **Actions:** Run lifecycle management

**Implemented Actions:**
- `fetchRuns()` - List available runs
- `createRun()` - Schedule new shared delivery
- `addStopToRun()` - Join existing run
- `startRun()` - Driver action to begin delivery
- `capturePOD()` - Proof of delivery submission
- `optimizeRoute()` - Route planning
- `calculateCostBreakdown()` - Fee transparency
- `startTracking()` - Real-time location updates

---

### 3. API Endpoints

#### Pool Management API

**GET /api/pools**
- List all pools with comprehensive filters
- Filters: status, area, SKU, supplier, savings %, deadline
- Returns: pools array + totalCount

**POST /api/pools**
- Create new pool
- Auto-creates lead as first participant
- Calculates savings percentage
- Returns: created pool object

**GET /api/pools/:id**
- Get detailed pool information
- Includes all participants and metadata
- Returns: full pool object

**PATCH /api/pools/:id/join**
- Join existing pool
- Validates capacity and status
- Calculates cost share based on split rule
- Returns: updated pool

**PATCH /api/pools/:id/confirm**
- Confirm pool and trigger PO creation
- Generates payment links for participants
- Notifies supplier
- Returns: confirmed pool with PO ID

**GET /api/pools/:id/savings**
- Calculate participant savings
- Includes delivery savings if applicable
- Returns: detailed savings breakdown

#### Shared Logistics API

**GET /api/runs**
- List delivery runs with filters
- Filters: status, zone, date range, driver, capacity
- Returns: runs array + totalCount

*(Additional endpoints planned for Phase 2)*

---

### 4. Utility Functions

#### `server/utils/whatsapp.ts`
WhatsApp Business API integration:

**Functions:**
- `sendPoolInvite()` - Invite shops to join pool
- `sendBulkPoolInvites()` - Multiple invites at once
- `notifyPoolProgress()` - Update participants on pool status
- `notifyPoolConfirmed()` - Payment link delivery
- `sendDeliveryUpdate()` - Delivery status notifications
- `notifyDriverArrival()` - ETA alerts
- `notifyPaymentReceived()` - Payment confirmations

**Features:**
- Formatted messages with emojis
- Deep linking to app
- Progress tracking
- Time-relative messaging

#### `server/utils/payments.ts`
South African payment provider integration:

**Functions:**
- `generatePayLink()` - Create payment link for pool participant
- `generateDeliveryPayLink()` - Payment for shared delivery
- `processPaymentWebhook()` - Handle payment callbacks
- `verifyPaymentWebhook()` - Signature validation
- `checkPaymentStatus()` - Query payment state
- `refundPayment()` - Process refunds
- `calculatePaymentFees()` - Fee transparency

**Supported Providers:**
- PayFast (primary)
- Yoco
- Ozow
- SnapScan

#### `server/utils/route-optimizer.ts`
Delivery route optimization:

**Functions:**
- `calculateDistance()` - Haversine formula for accurate distances
- `optimizeRoute()` - Nearest-neighbor greedy algorithm (MVP)
- `optimizeRouteAdvanced()` - Priority and time-window support
- `validateRoute()` - Check capacity and constraints
- `findNearbyStops()` - Geographic clustering
- `calculateETA()` - Arrival time estimation
- `estimateDeliveryWindow()` - Time window calculation

**Features:**
- Distance calculations in kilometers
- Sequence number assignment
- Fuel cost estimates
- Route validation
- Recommendations for improvement

---

### 5. Composables (Business Logic Layer)

#### `composables/useGroupBuying.ts`
Reusable pool management logic:

**Helper Functions:**
- `calculateFillPercentage()` - Pool completion %
- `hasReachedTarget()` - Target quantity check
- `isDeadlinePassed()` - Time validation
- `canExtend()` - Extension eligibility (70% filled, not extended before)
- `getTimeRemaining()` - Human-readable countdown
- `calculateParticipantSavings()` - Individual savings
- `canJoinPool()` - Eligibility check with reasons
- `getPoolStatusBadge()` - UI badge info (color, icon, label)
- `getPoolSuggestion()` - AI-powered recommendations
- `validatePoolData()` - Form validation

**Features:**
- Currency formatting (ZAR)
- Status badge styling
- Error messages
- Integration with store

#### `composables/useSharedDelivery.ts`
Delivery management logic:

**Helper Functions:**
- `calculateDeliverySavings()` - Solo vs shared comparison
- `getRunStatusBadge()` - Run status UI
- `getStopStatusBadge()` - Stop status UI
- `getCapacityUtilization()` - Load percentage
- `hasCapacityFor()` - Order fit check
- `getTimeUntilPickup()` - Countdown to pickup
- `getStopETA()` - Arrival estimate
- `isStopNearby()` - Within 10min check
- `getFeeShareExplanation()` - Cost transparency
- `getDriverRatingStars()` - Star display
- `canJoinRun()` - Eligibility validation
- `getDeliverySuggestion()` - AI recommendations

---

## 🏗️ Architecture Decisions

### 1. Pool as First-Class Entity

**Decision:** Pools are NOT purchase orders. Pools GENERATE purchase orders when confirmed.

**Rationale:**
- Cleaner separation of concerns
- Allows pool lifecycle independent of PO
- Enables multiple POs from single pool (if needed)
- Better tracking and analytics

**Implementation:**
```typescript
// Pool reaches threshold → status = 'confirmed'
// System creates consolidated PO
// PO.metadata.poolId = pool.id (link back)
```

### 2. State Machine for Pool Lifecycle

**States:** open → pending → confirmed → fulfilled | cancelled

**Transitions:**
- open → pending: Target met OR deadline reached
- pending → confirmed: All participants confirm + pay
- confirmed → fulfilled: Delivery complete + all PODs captured
- any → cancelled: Lead cancels OR pool fails

### 3. Split Rule Flexibility

**Flat Split:**
```typescript
costShare = totalCost / numberOfParticipants
```

**By-Units Split:**
```typescript
costShare = quantityCommitted * poolPrice
```

Future: by-weight, by-volume, custom formulas

### 4. Geographic Zones

Hard-coded for MVP, database-driven in production:
```typescript
const TOWNSHIP_ZONES = {
  'soweto-north': { center: [-26.2309, 27.8559], radius: 5 },
  'soweto-south': { center: [-26.2870, 27.8559], radius: 5 },
  'alexandra': { center: [-26.1023, 28.0897], radius: 3 }
}
```

Pools and Runs respect zone boundaries. Cross-zone requires opt-in.

---

## 📊 Data Flow Examples

### Creating and Joining a Pool

```
1. Lead creates pool:
   POST /api/pools
   → Pool status: 'open'
   → Lead added as first participant

2. Shop A sees pool in their area:
   GET /api/pools?area=soweto-north&status=open

3. Shop A joins:
   PATCH /api/pools/:id/join
   → Pool.currentCommitment += quantity
   → Pool.participantCount++
   → Participant status: 'joined'

4. Target reached:
   → Pool status: 'pending'
   → WhatsApp notifications sent

5. All participants confirm:
   PATCH /api/pools/:id/confirm
   → Pool status: 'confirmed'
   → PO created
   → Payment links generated

6. Delivery arranged:
   → SharedRun created
   → Stops added for each participant

7. Delivery complete:
   → PODs captured
   → Pool status: 'fulfilled'
```

### Shared Delivery Run

```
1. Pool confirmed OR solo PO placed:
   → Check for existing runs in same zone

2. If compatible run exists:
   POST /api/runs/:id/stops
   → Add stop to existing run
   → Recalculate fees

3. If no run exists:
   POST /api/runs
   → Create new run
   → Add first stop

4. Driver starts delivery:
   PATCH /api/runs/:id/start
   → Run status: 'out-for-delivery'
   → ETAs sent to shops

5. Each delivery:
   POST /api/runs/stops/:id/pod
   → Capture PIN/photo
   → Stop status: 'delivered'
   → WhatsApp confirmation

6. All stops complete:
   PATCH /api/runs/:id/complete
   → Run status: 'completed'
   → Calculate actual vs estimated
```

---

## 🔌 Integration Points

### Existing TOSS Systems

1. **Stock Management**
   - Pools linked to items via `itemId`
   - Stock receipt updates from PO fulfillment
   - Low-stock triggers pool suggestions

2. **Purchase Orders**
   - Pools generate consolidated POs
   - PO metadata includes `poolId`
   - PO receipt triggers delivery arrangement

3. **AI Copilot**
   - Suggests joining pools for low-stock items
   - Recommends creating pools for common SKUs
   - Proposes shared delivery for nearby orders

4. **WhatsApp System**
   - Reuses existing WhatsApp infrastructure
   - Adds pool-specific message templates
   - Delivery tracking notifications

5. **Payments**
   - Integrates with existing payment providers
   - Adds pool-specific reference format
   - Handles split payments

---

## 🎯 Success Metrics (Ready to Track)

The analytics types are in place to track:

1. **Pool Fill Rate:** % of pools that reach target
   - Target: ≥70%
2. **Average Delivery Cost:** Shared vs solo
   - Target: -30% reduction
3. **Savings Display Rate:** "You saved" shown
   - Target: ≥80% of transactions
4. **AI Acceptance:** Pool suggestions accepted
   - Target: ≥50%
5. **On-Time Delivery:** Shared runs on time
   - Target: ≥90%
6. **Payment Completion:** Pay links completed
   - Target: ≥70%

---

## ⚠️ Known Limitations (MVP)

1. **Single-SKU Pools Only**
   - Multi-SKU baskets deferred to v1.1
   - Keeps MVP simple and testable

2. **Simple Split Rules**
   - Only flat and by-units
   - Complex rules (tiered, hybrid) in v1.2

3. **Mock Data**
   - API endpoints return mock data
   - Database integration required

4. **Route Optimization**
   - Basic nearest-neighbor algorithm
   - Google Maps integration planned for v1.2

5. **Payment Provider**
   - Mock implementation
   - Real provider integration needed

6. **WhatsApp**
   - Mock sending
   - Twilio/WhatsApp Business API integration needed

---

## 🚀 Next Steps (Phase 2)

### Week 5-6: Complete Shared Logistics API
- [ ] POST /api/runs (create run)
- [ ] PATCH /api/runs/:id/start (begin delivery)
- [ ] PATCH /api/runs/:id/complete (finish run)
- [ ] POST /api/runs/:id/stops (add stop)
- [ ] DELETE /api/runs/:id/stops/:stopId (remove stop)
- [ ] POST /api/runs/stops/:stopId/pod (capture POD)
- [ ] GET /api/runs/:id/cost-breakdown (fee transparency)
- [ ] GET /api/runs/savings (calculate savings)
- [ ] GET /api/runs/:id/track (real-time tracking SSE)

### Week 7: Database Integration
- [ ] Set up Prisma/Drizzle schema
- [ ] Create migration files
- [ ] Replace mock data with DB queries
- [ ] Add indexes for performance

### Week 8: Real Integrations
- [ ] WhatsApp Business API (Twilio)
- [ ] Payment provider (PayFast)
- [ ] Google Maps Directions API (optional)

### Week 9-10: Testing & Polish
- [ ] End-to-end pool flow
- [ ] End-to-end delivery flow
- [ ] Pool → Delivery integration
- [ ] Performance optimization
- [ ] Error handling refinement

---

## 📝 Files Created

```
types/
  ├── group-buying.ts      (387 lines)
  └── logistics.ts         (286 lines)

stores/
  ├── groupBuying.ts       (298 lines)
  └── sharedLogistics.ts   (267 lines)

server/
  ├── api/
  │   ├── pools/
  │   │   ├── index.get.ts         (List pools)
  │   │   ├── index.post.ts        (Create pool)
  │   │   ├── [id].get.ts          (Get pool)
  │   │   ├── [id]/
  │   │   │   ├── join.patch.ts    (Join pool)
  │   │   │   ├── confirm.patch.ts (Confirm pool)
  │   │   │   └── savings.get.ts   (Calculate savings)
  │   └── runs/
  │       └── index.get.ts         (List runs)
  └── utils/
      ├── whatsapp.ts       (242 lines)
      ├── payments.ts       (248 lines)
      └── route-optimizer.ts (241 lines)

composables/
  ├── useGroupBuying.ts     (358 lines)
  └── useSharedDelivery.ts  (364 lines)
```

**Total Lines of Code:** ~2,700 lines  
**Total Files:** 16 files

---

## 🎓 Key Learnings

1. **Type-First Development**
   - Defining comprehensive TypeScript types upfront made implementation smooth
   - DTOs provide clear API contracts

2. **Separation of Concerns**
   - Stores handle state
   - Composables handle business logic
   - API routes handle HTTP
   - Utils handle infrastructure

3. **Mock-First Approach**
   - Mock data enables frontend development before backend ready
   - Easy to swap for real data later

4. **Status-Driven UI**
   - Badge helper functions make UI consistent
   - State machines prevent invalid transitions

---

## 🔗 References

- [Original PRD](./TOSS_PRD_Group_Buying_Revised.md)
- [Implementation Plan](./toss-group-buying-prd-review.plan.md)
- [ERPNext Stock Module](https://docs.erpnext.com/docs/user/manual/en/stock)
- [Nuxt 3 Documentation](https://nuxt.com)
- [Pinia Documentation](https://pinia.vuejs.org)

---

**Implementation completed by:** AI Agent  
**Date:** January 20, 2025  
**Status:** ✅ Phase 1 Complete, Ready for Phase 2

