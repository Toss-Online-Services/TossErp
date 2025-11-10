#  Queue Orders - Quick Reference Card

## Problem Solved
 Fixed "data not loaded" error on '/sales/orders/queue' page  
 Page now calls correct API endpoints for queue-based orders

## What Changed
The queue page was calling generic 'CustomerOrders' API methods instead of the queue-specific 'Sales' API methods.

## Test It Now!

###  Quick Test (2 minutes)
1. **Open POS**: 'http://localhost:3000/sales/pos'
2. **Add items**: Click any products to add to cart
3. **Create order**: Click "Create Order" button
4. **Fill form**:
   - Name: John Doe
   - Phone: +27123456789
   - Notes: Extra cheese
   - Time: 15 min
5. **Submit**: Click submit
6. **View queue**: Click " Order Queue" button
7. ** Success**: Order should appear in "Pending" section

###  What You Should See
- **Stats cards** showing: Total, Pending, In Progress, Ready counts
- **Order cards** with customer name, phone, notes
- **Action buttons**:
  - " Start" (Pending  InProgress)
  - " Mark Ready" (InProgress  Ready)
  - " Complete" (Ready  Completed + removed from queue)
  - " Cancel" (Voids the order)

## URLs

| Page | URL |
|------|-----|
| Queue Management | 'http://localhost:3000/sales/orders/queue' |
| POS (Create Order) | 'http://localhost:3000/sales/pos' |
| API Endpoint | 'http://localhost:5000/api/Sales/queue?shopId=1' |

## API Quick Reference

### Create Queue Order
```http
POST http://localhost:5000/api/Sales
Content-Type: application/json

{
  "shopId": 1,
  "customerName": "John Doe",
  "customerPhone": "+27123456789",
  "customerNotes": "Extra cheese",
  "items": [{ "productId": 1, "quantity": 2, "unitPrice": 50.00 }],
  "paymentType": "Cash",
  "totalAmount": 100.00,
  "saleType": 1,
  "estimatedPreparationMinutes": 15
}
```

### Get Queue Orders
```http
GET http://localhost:5000/api/Sales/queue?shopId=1
```

### Update Status
```http
POST http://localhost:5000/api/Sales/{saleId}/status
Content-Type: application/json

{ "newStatus": "InProgress" }
```

## Status Workflow

```
     Start        Mark Ready   
 Pending   >  InProgress   >   Ready  
                                    
                                                                     
                                                                       Complete
                                                                     
                                                              
                                                               Completed 
                                                               (removed) 
                                                              
```

## Files Modified

| File | Change |
|------|--------|
| 'toss-web/pages/sales/orders/queue.vue' | Fixed API method calls |
| | Changed 'getOrders()'  'getQueueOrders(shopId)' |
| | Changed 'updateOrderStatus()'  'updateQueueOrderStatus()' |
| | Changed 'completeOrder()'  'completeQueueOrder()' |
| | Changed 'cancelOrder()'  'voidSale()' |

## Troubleshooting

###  Queue shows "No data"
1. Check browser console (F12) for API errors
2. Verify backend is running (should see terminal output)
3. Create test order via POS first
4. Refresh the queue page

###  Can't create order
1. Make sure you have products in the system
2. Check that cart has items before clicking "Create Order"
3. Fill all required fields (Name is required)
4. Check browser console for validation errors

###  Status buttons don't work
1. Verify backend is responding (check terminal)
2. Look for error messages in browser console
3. Confirm sale exists in database
4. Try refreshing the page

## Architecture

### Two Order Systems (Important!)

#### 1. CustomerOrders (Existing)
- **Path**: '/sales/orders/index.vue'
- **API**: '/api/CustomerOrders'
- **Use**: Formal orders with delivery/shipping
- **Entity**: 'CustomerOrder' table

#### 2. Queue Orders (New/Fixed)
- **Path**: '/sales/orders/queue.vue'
- **API**: '/api/Sales/queue'
- **Use**: Fast-service queue workflow
- **Entity**: 'Sale' table with 'SaleType=1'

## Key Points

 Queue orders are stored as 'Sale' records (not 'CustomerOrder')  
 They have 'SaleType = 1' (QueueOrder enum value)  
 Status starts at 'Pending' (not 'Completed' like normal POS sales)  
 Orders disappear from queue when marked 'Completed'  
 Can include customer info (name, phone, notes)  
 Can track expected completion time  

## Next Steps

1.  **Test the fixed queue page** (follow Quick Test above)
2.  **Verify workflow** (Pending  InProgress  Ready  Completed)
3.  **Test with multiple orders** (create 3-5 orders)
4.  **Test edge cases** (cancel order, empty queue, etc.)

## Documentation

Full details in:
- 'QUEUE_ORDERS_FIXED_SUMMARY.md' - Complete fix documentation
- 'QUEUE_IMPLEMENTATION_CHECKLIST.md' - Full implementation checklist
- 'QUEUE_ORDERS_COMPLETE_SUMMARY.md' - Original implementation summary

---
**Status**:  FIXED - Ready for testing  
**Backend**:  Running (process 32412)  
**Frontend**:  Running on port 3000  
**Issue**:  API methods corrected  
