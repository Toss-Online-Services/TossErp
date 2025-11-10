# Queue Orders - Data Loading Fixed

## Issue Resolved
The '/sales/orders/queue' page was showing "data not loaded" because it was calling the wrong API methods.

## Changes Made

### 1. Fixed API Calls in Queue Page
**File**: 'toss-web/pages/sales/orders/queue.vue'

**Problem**: The page was calling generic 'getOrders()' method which returns CustomerOrder data, not queue orders based on Sales.

**Solution**: Updated to call the proper queue-specific methods:
- Changed 'salesAPI.getOrders()'  'salesAPI.getQueueOrders(shopId)'
- Changed 'salesAPI.updateOrderStatus()'  'salesAPI.updateQueueOrderStatus()'  
- Changed 'salesAPI.completeOrder()'  'salesAPI.completeQueueOrder()'
- Changed 'salesAPI.cancelOrder()'  'salesAPI.voidSale()' (queue orders use Sales void)

### 2. Data Transformation
Added proper transformation of backend queue data to match frontend expectations:
```typescript
const queueData = await salesAPI.getQueueOrders(shopId)

queueOrders.value = queueData.map((order: any) => ({
  id: order.id,
  orderNumber: order.saleNumber,
  customer: order.customerName || 'Walk-in Customer',
  customerPhone: order.customerPhone,
  status: order.status.toLowerCase(),
  total: order.total,
  orderItems: order.items || [],
  createdAt: new Date(order.saleDate),
  notes: order.customerNotes,
  expectedCompletion: order.expectedCompletionTime ? new Date(order.expectedCompletionTime) : null,
  queuePosition: order.queuePosition
}))
```

### 3. Status Mapping
Added proper mapping between frontend status strings and backend SaleStatus enum:
```typescript
const statusMap: Record<string, string> = {
  'pending': 'Pending',
  'in-progress': 'InProgress',
  'ready': 'Ready'
}
```

## System Architecture Clarification

### Two Separate Systems

#### 1. CustomerOrders System (Legacy)
- **Location**: '/sales/orders/*'
- **API**: 'useCustomerOrdersAPI()'  '/api/CustomerOrders'
- **Entity**: 'CustomerOrder' table
- **Purpose**: Formal customer orders with delivery, shipping, etc.
- **Pages**: 
  - '/sales/orders/index.vue' - Order management
  - '/sales/orders/create-order.vue' - Create new orders
  - **Note**: This folder has its own 'queue.vue' but it uses CustomerOrders API

#### 2. Queue-Based Orders System (New/Extended)
- **Location**: Queue functionality uses Sale entity
- **API**: 'useSalesAPI()'  '/api/Sales/queue'
- **Entity**: 'Sale' table with 'SaleType = QueueOrder'
- **Purpose**: Fast-service queue workflow (chisa nyama, kota shops)
- **Key Features**:
  - Extends Sale entity (not separate CustomerOrder)
  - Status workflow: Pending  InProgress  Ready  Completed
  - Includes: customerName, customerPhone, customerNotes, expectedCompletionTime, queuePosition
  - Lives in database as Sales records with 'SaleType = 1' (QueueOrder enum)

## Available Methods in useSalesAPI

### Queue-Specific Methods
```typescript
// Create queue order (status starts as Pending)
createQueueOrder(orderData: {
  shopId, customerName, customerPhone, 
  customerNotes, items, paymentType, 
  totalAmount, estimatedPreparationMinutes
})

// Get active queue orders (Pending, InProgress, Ready)
getQueueOrders(shopId: number)

// Update order status through workflow
updateQueueOrderStatus(saleId, newStatus: 'Pending'|'InProgress'|'Ready'|'Completed')

// Complete and finalize order
completeQueueOrder(saleId: number)

// Cancel/void order
voidSale(saleId: number, reason: string)
```

## Testing Instructions

### Option 1: Via POS Page
1. Open: 'http://localhost:3000/sales/pos'
2. Add products to cart
3. Click "Create Order" button
4. Fill in customer details:
   - Customer Name: "John Doe"
   - Phone: "+27123456789"
   - Notes: "Extra cheese"
   - Estimated time: 15 minutes
5. Submit order
6. Click " Order Queue" navigation link
7. Verify order appears in queue

### Option 2: Via Direct API Test
Since the backend is already running (process 32412), you can test the API directly using the browser:

1. **Create a queue order via POS** (see Option 1 above)
2. **View API response in browser**:
   - Open: 'http://localhost:5000/api/Sales/queue?shopId=1'
   - Or: 'https://localhost:5001/api/Sales/queue?shopId=1'
3. **Expected JSON response**:
```json
[
  {
    "id": 123,
    "saleNumber": "SALE-00123",
    "shopId": 1,
    "customerName": "John Doe",
    "customerPhone": "+27123456789",
    "saleDate": "2025-01-11T10:00:00Z",
    "status": "Pending",
    "saleType": 1,
    "total": 100.00,
    "paymentMethod": "Cash",
    "customerNotes": "Extra cheese",
    "expectedCompletionTime": "2025-01-11T10:15:00Z",
    "queuePosition": 1,
    "items": [
      {
        "id": 1,
        "productName": "Chicken Burger",
        "productSKU": "CHKB-001",
        "quantity": 2,
        "unitPrice": 50.00,
        "lineTotal": 100.00
      }
    ]
  }
]
```

### Option 3: PowerShell Test Script
A test script 'test-create-queue-order.ps1' is available, but requires:
- Backend running on 'http://localhost:5000' (HTTP, not HTTPS)
- PowerShell 7+ for '-SkipCertificateCheck' parameter

**Note**: Since your backend is running on HTTPS (5001), you'll need to either:
1. Use the POS page to create orders (recommended)
2. Configure backend to also listen on HTTP port 5000
3. Use Postman/Insomnia for API testing

## URLs

### Frontend
- **Queue Page**: 'http://localhost:3000/sales/orders/queue'
- **POS Page**: 'http://localhost:3000/sales/pos'
- **Orders List**: 'http://localhost:3000/sales/orders'

### Backend API
- **Get Queue Orders**: 'GET http://localhost:5000/api/Sales/queue?shopId=1'
- **Create Sale (Queue)**: 'POST http://localhost:5000/api/Sales' with 'saleType=1'
- **Update Status**: 'POST http://localhost:5000/api/Sales/{id}/status'

## What's Fixed

 Queue page now calls correct API endpoints  
 Data transformation matches backend response format  
 Status updates use proper queue methods  
 Complete/cancel actions use appropriate methods  
 Frontend running on port 3000  
 Backend already running (process 32412)

## Next Steps

1. **Test the queue page**: Navigate to 'http://localhost:3000/sales/orders/queue'
2. **Create test order**: Use POS page to create a queue order
3. **Verify workflow**: Test status changes (Pending  InProgress  Ready  Completed)
4. **Check database**: Verify records in 'Sales' table have 'SaleType = 1'

## Troubleshooting

### If queue page shows "No data"
1. Check browser console (F12) for API errors
2. Verify backend is accessible: Open 'http://localhost:5000/api/Sales/queue?shopId=1' in browser
3. Create a test order via POS first
4. Check that 'shopId' parameter matches your shop

### If API calls fail
1. Verify backend process is running (should be process ID 32412)
2. Check terminal for backend errors
3. Confirm database migration was applied ('AddQueueBasedOrderSupport')
4. Test with Postman to isolate frontend vs backend issues

### If status updates don't work
1. Check that status values match enum: 'Pending', 'InProgress', 'Ready', 'Completed'
2. Verify sale exists in database
3. Check backend logs for validation errors

## Summary

The issue was that '/sales/orders/queue.vue' was calling generic order methods ('getOrders', 'updateOrderStatus') instead of queue-specific methods ('getQueueOrders', 'updateQueueOrderStatus'). This has been fixed, and the page now properly interfaces with the Sale-based queue system we implemented in the backend.

The queue orders are stored as 'Sale' records with 'SaleType = 1' (QueueOrder), allowing them to flow through the Pending  InProgress  Ready  Completed workflow while leveraging the existing Sales infrastructure.
