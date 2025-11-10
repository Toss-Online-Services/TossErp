# Queue-Based Orders Implementation Guide

## Overview

This document explains the implementation of queue-based orders in TOSS ERP - where orders placed through the POS system enter a preparation queue (like at a chisa nyama or kota shop) rather than being immediately completed.

## Business Context

**Use Case:** Customer walks up to a chisa nyama (barbecue stand) or kota shop, places an order via the POS, and waits while the food is prepared. The order moves through statuses:
- **Pending**  Order placed, waiting to start preparation
- **InProgress**  Food being cooked/assembled
- **Ready**  Order complete, customer can collect
- **Completed**  Customer has collected and paid

## Architecture Decision

### Chosen Approach: Single `Sale` Entity with Extended Workflow

Instead of maintaining separate `Sale` and `CustomerOrder` entities, we extend the `Sale` entity to support both:
1. **Immediate POS sales** (walk-in, pay-and-go)
2. **Queue-based orders** (place order, prepare, collect)
3. **Delivery orders** (place order, deliver later)
4. **Pre-orders** (order for future date)

**Why?** 
- Simpler architecture - one entity, one workflow
- Orders ARE sales - they're just sales with extended status lifecycle
- Avoids data duplication and sync issues
- Unified reporting and analytics

## Database Changes

### 1. Extended `SaleStatus` Enum

**File:** `backend/Toss/src/Domain/Enums/SaleStatus.cs`

```csharp
public enum SaleStatus
{
    Pending = 0,        // NEW: Order placed, awaiting preparation
    InProgress = 1,     // NEW: Order being prepared/cooked
    Ready = 2,          // NEW: Order ready for pickup
    Completed = 3,      // Sale completed and paid (was 1)
    Voided = 4,         // Sale voided/cancelled (was 2)
    Refunded = 5,       // Sale refunded (was 3)
    OnHold = 6          // Sale temporarily on hold (was 4)
}
```

**Note:** Enum values changed to accommodate new statuses. Migration will handle this.

### 2. New `SaleType` Enum

**File:** `backend/Toss/src/Domain/Enums/SaleType.cs`

```csharp
public enum SaleType
{
    POS = 0,            // Immediate POS sale (walk-in, pay-and-go)
    QueueOrder = 1,     // Queue-based order (chisa nyama, kota)
    Delivery = 2,       // Delivery order
    PreOrder = 3        // Pre-order for future pickup
}
```

### 3. Extended `Sale` Entity

**File:** `backend/Toss/src/Domain/Entities/Sales/Sale.cs`

**New Properties:**
```csharp
public SaleType SaleType { get; set; } = SaleType.POS;
public DateTimeOffset? ExpectedCompletionTime { get; set; }
public int? QueuePosition { get; set; }
public string? CustomerNotes { get; set; }
public string? CustomerName { get; set; }        // For walk-in orders
public string? CustomerPhone { get; set; }       // For notifications
```

### 4. Database Migration

**Migration:** `AddQueueBasedOrderSupport`

**Created:** 2025-11-07

**Changes:**
- Adds `SaleType` column (int, default 0)
- Adds `ExpectedCompletionTime` column (DateTimeOffset, nullable)
- Adds `QueuePosition` column (int, nullable)
- Adds `CustomerNotes` column (nvarchar, nullable)
- Adds `CustomerName` column (nvarchar, nullable)
- Adds `CustomerPhone` column (nvarchar, nullable)
- Updates existing `SaleStatus` values to match new enum numbering

**To Apply:**
```bash
cd backend/Toss
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

## API Changes

### New Endpoints (To Be Implemented)

**1. Get Queue Orders**
```
GET /api/sales/queue?shopId={shopId}
```
Returns all sales with SaleType=QueueOrder and Status=Pending/InProgress/Ready

**2. Create Queue Order**
```
POST /api/sales/queue
{
  "shopId": 1,
  "customerId": null,
  "customerName": "John Doe",
  "customerPhone": "+27123456789",
  "items": [...],
  "customerNotes": "Extra peri-peri",
  "saleType": "QueueOrder"
}
```

**3. Update Order Status**
```
PATCH /api/sales/{id}/status
{
  "status": "InProgress"  // or "Ready" or "Completed"
}
```

**4. Get Order by Sale Number**
```
GET /api/sales/queue/{saleNumber}
```

### Updated CreateSaleCommand

**File:** `backend/Toss/src/Application/Sales/Commands/CreateSale/CreateSaleCommand.cs`

**New Properties:**
```csharp
public SaleType SaleType { get; init; } = SaleType.POS;
public string? CustomerName { get; init; }
public string? CustomerPhone { get; init; }
public string? CustomerNotes { get; init; }
public int? EstimatedPreparationMinutes { get; init; }
```

## Frontend Changes

### 1. POS Page Updates

**File:** `toss-web/pages/sales/pos/index.vue`

**New Features:**
- **"Create Order" button** alongside "Complete Sale"
- When clicked, creates sale with:
  - `Status = Pending`
  - `SaleType = QueueOrder`
  - Captures customer name and phone
  - Adds to queue instead of completing immediately

**Updated Composable:**
```typescript
// toss-web/composables/useSalesAPI.ts

export const useSalesAPI = () => {
  const createQueueOrder = async (orderData: CreateOrderRequest) => {
    return await $fetch('/api/sales/queue', {
      method: 'POST',
      body: orderData
    })
  }

  const getQueueOrders = async (shopId: number) => {
    return await $fetch(`/api/sales/queue?shopId=${shopId}`)
  }

  const updateOrderStatus = async (saleId: number, status: string) => {
    return await $fetch(`/api/sales/${saleId}/status`, {
      method: 'PATCH',
      body: { status }
    })
  }

  return {
    // ... existing methods
    createQueueOrder,
    getQueueOrders,
    updateOrderStatus
  }
}
```

### 2. Queue Management Page

**New File:** `toss-web/pages/sales/pos/queue.vue`

**Features:**
- **Real-time queue display** - Shows all pending/in-progress orders
- **Status indicators** - Color-coded badges (Pending=yellow, InProgress=blue, Ready=green)
- **Quick actions** - Buttons to move orders through workflow:
  - "Start Preparation" (Pending  InProgress)
  - "Mark Ready" (InProgress  Ready)
  - "Complete" (Ready  Completed)
  - "Cancel" (Any  Voided)
- **Customer info** - Display customer name, phone, notes
- **Order details** - Expandable list of items
- **Audio alerts** - Optional notification sounds for new orders
- **Auto-refresh** - Poll every 5-10 seconds or use SignalR

**Layout:**
```

 Queue Dashboard          [Refresh] [Audio]  

  Stats: 3 Pending | 2 In Progress | 1 Ready

 PENDING (3)                                 
   
  #SAL-123 | John Doe | +2712...        
  2x Kota, 1x Coke                       
  "Extra peri-peri"                       
  [ Start Preparation] [ Cancel]       
   

 IN PROGRESS (2)                             
   
  #SAL-121 | Jane Smith | 5 min ago      
  1x Braai Plate, 2x Rolls               
  [ Mark Ready] [ Cancel]              
   

 READY (1)                                   
   
  #SAL-120 | Peter Jones | READY!        
  3x Kotas | R120.00                      
  [ Complete & Pay] [ Cancel]          
   

```

### 3. Navigation Updates

Add "Queue" button to POS main page:
```vue
<NuxtLink 
  to="/sales/pos/queue"
  class="btn-primary"
>
   Order Queue
  <span v-if="queueCount > 0" class="badge">{{ queueCount }}</span>
</NuxtLink>
```

## Workflow Examples

### Example 1: Chisa Nyama Order

1. **Customer arrives** at chisa nyama
2. **Cashier uses POS** - Scans/adds braai plate items
3. **Clicks "Create Order"** instead of "Complete Sale"
4. **Enter customer details** - Name: "Thabo", Phone: "+27821234567"
5. **Add special instructions** - "Well done, extra sauce"
6. **Order created** - Status: Pending, Sale #SAL-1-20251107-0042
7. **Kitchen sees order** on queue page
8. **Chef clicks "Start Preparation"** - Status: InProgress
9. **15 minutes later** - Chef clicks "Mark Ready"
10. **Customer collects** - Cashier clicks "Complete & Pay"
11. **Payment processed** - Status: Completed

### Example 2: Kota Shop Workflow

1. **Customer orders** 3 kotas with specific fillings
2. **Cashier creates order** with customer name and phone
3. **SMS sent** (optional) - "Your order #SAL-123 is being prepared"
4. **Kota preparer** sees order on tablet/phone
5. **Prepares kotas** - Marks InProgress
6. **Completes order** - Marks Ready
7. **SMS sent** (optional) - "Your order #SAL-123 is ready for collection"
8. **Customer arrives** - Collects and pays

## Integration with Existing Features

### POS Integration
- Existing POS workflow remains unchanged for immediate sales
- New "Create Order" button for queue-based orders
- Queue orders use same cart, products, pricing logic
- Payment still processed at completion

### Sales Reporting
- Queue orders appear in sales reports once completed
- Filter by SaleType to separate immediate vs queue sales
- Average preparation time analytics

### Inventory Management
- Stock reserved when order placed (Status=Pending)
- Stock deducted when order completed (Status=Completed)
- Stock released if order cancelled (Status=Voided)

### Customer Management
- Queue orders can link to existing customers OR use walk-in names
- Customer purchase history includes queue orders
- Phone numbers captured for SMS notifications

## Testing Plan

### Unit Tests
- [ ] SaleStatus enum serialization/deserialization
- [ ] SaleType enum handling
- [ ] CreateSaleCommand validation with new fields
- [ ] UpdateSaleStatus command validation

### Integration Tests
- [ ] Create queue order via API
- [ ] Update order status Pending  InProgress  Ready  Completed
- [ ] Cancel order at each status
- [ ] Filter sales by SaleType
- [ ] Get queue orders for specific shop

### E2E Tests
- [ ] Complete workflow from POS to queue to completion
- [ ] Multiple orders in queue simultaneously
- [ ] Real-time updates when status changes
- [ ] Cancel order from queue

## Deployment Steps

1. **Backup database** - Always backup before migration
2. **Apply migration** - `dotnet ef database update`
3. **Verify migration** - Check new columns exist
4. **Deploy backend** - Update API with new endpoints
5. **Deploy frontend** - Update POS and add queue page
6. **Test workflow** - Create test order through full lifecycle
7. **Train users** - Show staff how to use queue system

## Future Enhancements

### Phase 2
- [ ] SMS notifications for order status changes
- [ ] Kitchen display system (KDS) - Dedicated screen for kitchen
- [ ] Estimated preparation time calculations
- [ ] Priority flagging (urgent orders)
- [ ] Queue analytics - Average prep time, bottlenecks

### Phase 3
- [ ] Customer self-service kiosk
- [ ] Online ordering integration
- [ ] WhatsApp order updates
- [ ] Delivery rider assignment
- [ ] Route optimization

## Troubleshooting

**Issue:** Orders not appearing in queue
- Check SaleType is set to QueueOrder (not POS)
- Check Status is Pending/InProgress/Ready (not Completed)
- Verify shopId filter matches

**Issue:** Status not updating
- Check UpdateSaleStatusCommand validation
- Verify state machine allows transition
- Check frontend is calling correct endpoint

**Issue:** Stock not releasing on cancellation
- Implement VoidSaleCommand to handle stock adjustment
- Check inventory transactions are created

## Related Documentation
- [Sales API Documentation](./SALES_API_INTEGRATION_COMPLETE.md)
- [POS Feature Implementation](./POS_FEATURE_IMPLEMENTATION_COMPLETE.md)
- [Database Schema](./backend/Toss/src/Infrastructure/Data/Migrations/)

---
**Created:** 2025-11-07
**Last Updated:** 2025-11-07
**Status:**  Backend Complete |  Frontend In Progress
