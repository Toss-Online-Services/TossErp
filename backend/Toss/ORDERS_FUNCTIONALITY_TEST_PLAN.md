# Orders Functionality Test Plan

## Overview
This document outlines the comprehensive testing plan for the Sales Orders functionality.

## Backend Endpoints to Test

### 1. GET /api/CustomerOrders
**Purpose**: Retrieve a list of customer orders

**Query Parameters**:
- `shopId` (int, optional): Filter orders by shop ID
- `customerId` (int, optional): Filter orders by customer ID
- `status` (OrderStatus enum, optional): Filter by order status
- `pageNumber` (int, default: 1): Page number for pagination
- `pageSize` (int, default: 50): Number of items per page

**Expected Response**:
```json
[
  {
    "id": 1,
    "orderGuid": "...",
    "orderNumber": "ORD-000001",
    "customerId": 1,
    "customerName": "John Doe",
    "orderDate": "2024-01-01T00:00:00Z",
    "orderStatus": "Processing",
    "shippingStatus": "Shipped",
    "paymentStatus": "Completed",
    "orderTotal": 150.00,
    "itemCount": 3
  }
]
```

**Test Cases**:
- [ ] Get all orders without filters
- [ ] Filter by shopId
- [ ] Filter by customerId
- [ ] Filter by status (Pending, Processing, Complete, Cancelled)
- [ ] Test pagination (pageNumber and pageSize)
- [ ] Verify orders are ordered by Created date (descending)
- [ ] Verify customerName is populated correctly
- [ ] Verify orderNumber format is correct (ORD-XXXXXX)
- [ ] Verify itemCount is accurate

### 2. POST /api/CustomerOrders
**Purpose**: Create a new customer order

**Request Body**:
```json
{
  "customerId": 1,
  "orderItems": [
    {
      "productId": 1,
      "quantity": 2,
      "unitPriceExclTax": 50.00
    }
  ]
}
```

**Test Cases**:
- [ ] Create order with valid data
- [ ] Verify order is created with correct status (Pending)
- [ ] Verify order totals are calculated correctly
- [ ] Verify order items are associated correctly
- [ ] Test with invalid customerId (should fail)
- [ ] Test with invalid productId (should fail)
- [ ] Test with zero quantity (should fail)

### 3. POST /api/CustomerOrders/{id}/status
**Purpose**: Update order status

**Request Body**:
```json
{
  "status": "Processing"
}
```

**Test Cases**:
- [ ] Update status from Pending to Processing
- [ ] Update status from Processing to Complete
- [ ] Update status to Cancelled
- [ ] Test invalid status transitions (should fail)
- [ ] Test with non-existent order ID (should fail)

### 4. POST /api/CustomerOrders/{id}/cancel
**Purpose**: Cancel an order

**Test Cases**:
- [ ] Cancel a pending order
- [ ] Cancel a processing order
- [ ] Cancel an already completed order (should fail)
- [ ] Cancel a non-existent order (should fail)

## Frontend Functionality to Test

### 1. Orders List Page (`/sales/orders`)

**Features to Test**:
- [ ] Orders load correctly on page mount
- [ ] Loading state displays while fetching orders
- [ ] Error handling displays error message if API fails
- [ ] Orders are displayed in a list/grid format
- [ ] Order details are displayed correctly:
  - Order number
  - Customer name
  - Order date
  - Order status (with correct badges/colors)
  - Order total
  - Item count

### 2. Order Statistics Cards

**Test Cases**:
- [ ] Total Orders count is accurate
- [ ] Pending Orders count is accurate
- [ ] In Progress Orders count is accurate
- [ ] Ready Orders count is accurate
- [ ] Completed Orders count is accurate
- [ ] Total Order Value is calculated correctly
- [ ] Statistics update when orders are filtered

### 3. Order Filtering

**Test Cases**:
- [ ] Filter by status (clicking status cards)
- [ ] Filter by customer name (dropdown)
- [ ] Search by order number
- [ ] Search by customer name
- [ ] Clear filters
- [ ] Multiple filters work together (AND logic)
- [ ] Filtered results update correctly

### 4. Order Expansion/Details

**Test Cases**:
- [ ] Click order to expand details
- [ ] Order details display correctly:
  - Order items
  - Payment status
  - Shipping status
  - Customer information
  - Delivery address (if available)
- [ ] Multiple orders can be expanded simultaneously
- [ ] Expand/collapse toggle works correctly

### 5. Create Order Button

**Test Cases**:
- [ ] "Create Order" button navigates to create order page
- [ ] Button is visible and accessible
- [ ] Button styling matches design system

## Data Mapping Verification

### Backend to Frontend Mapping

**OrderStatus Enum Mapping**:
- `Pending` → `'pending'`
- `Processing` → `'in-progress'`
- `Complete` → `'completed'`
- `Cancelled` → `'cancelled'`

**Verify**:
- [ ] Backend OrderStatus enum values are correctly mapped to frontend status strings
- [ ] Status badges display correct colors for each status
- [ ] Statistics filter correctly by status

## Error Handling

**Test Cases**:
- [ ] API returns 404 for non-existent order
- [ ] API returns 400 for invalid request data
- [ ] API returns 500 for server errors
- [ ] Frontend displays user-friendly error messages
- [ ] Frontend handles network errors gracefully
- [ ] Frontend handles timeout errors

## Performance Tests

**Test Cases**:
- [ ] Page loads within acceptable time (< 2 seconds)
- [ ] Orders load efficiently with pagination
- [ ] Filtering is responsive (no lag)
- [ ] Large datasets (100+ orders) load correctly
- [ ] Infinite scroll or pagination works smoothly

## Browser Compatibility

**Test Cases**:
- [ ] Chrome
- [ ] Firefox
- [ ] Edge
- [ ] Safari (if available)
- [ ] Mobile responsive (if applicable)

## Edge Cases

**Test Cases**:
- [ ] Orders with no items
- [ ] Orders with very large totals
- [ ] Orders with very long customer names
- [ ] Orders with special characters in customer names
- [ ] Orders with missing customer information
- [ ] Orders with null/empty values
- [ ] Empty order list (no orders found)
- [ ] Orders from different shops (verify shop filtering)

## Accessibility

**Test Cases**:
- [ ] Keyboard navigation works
- [ ] Screen reader compatibility
- [ ] Focus indicators are visible
- [ ] Color contrast meets WCAG standards
- [ ] Buttons have accessible labels

## Integration Tests

**Test Cases**:
- [ ] Create order → Verify it appears in list
- [ ] Update order status → Verify status updates in UI
- [ ] Cancel order → Verify order is removed/hidden
- [ ] Filter by shop → Verify only that shop's orders appear
- [ ] Multiple users can view orders simultaneously

## Checklist Summary

- [ ] Backend endpoints are working correctly
- [ ] Frontend displays orders correctly
- [ ] Filtering and search work correctly
- [ ] Order details expand/collapse correctly
- [ ] Status mapping is correct
- [ ] Error handling works correctly
- [ ] Performance is acceptable
- [ ] Edge cases are handled
- [ ] Accessibility requirements are met

## Notes

- Ensure backend server is running before testing
- Ensure database has seeded orders (100 orders seeded)
- Verify DateTimeInterceptor is working (all DateTime values are UTC)
- Check browser console for any JavaScript errors
- Check backend logs for any errors












