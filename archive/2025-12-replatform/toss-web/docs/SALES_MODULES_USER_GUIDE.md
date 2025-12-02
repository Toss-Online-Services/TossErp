# TOSS Sales Modules - User Guide

## Overview

TOSS Sales modules provide a comprehensive suite of tools for managing the complete sales lifecycle, from quotations to delivery. Built with offline-first capabilities and optimized for township and rural SMMEs.

---

## Module Structure

### 1. Point of Sale (POS)
**Path**: `/sales/pos`

**Purpose**: Quick sales processing with offline support

**Key Features**:
- Barcode/QR scanning
- Quick product search
- Multiple payment methods
- Offline transaction queue
- Customer credit tracking
- Receipt generation (SMS/WhatsApp)

**Use Cases**:
- Walk-in customers
- Quick transactions
- Cash sales
- Credit sales to known customers

### 2. Quotations
**Path**: `/sales/quotations`

**Purpose**: Create and manage sales quotes for customers

**Key Features**:
- Draft quotations
- Convert to sales orders
- PDF generation
- Validity period tracking
- Customer history

**Use Cases**:
- Bulk orders
- Special pricing
- Customer requests
- Future orders

### 3. Sales Orders
**Path**: `/sales/orders`

**Purpose**: Manage confirmed customer orders

**Key Features**:
- Order tracking
- Inventory reservation
- Partial fulfillment
- Delivery coordination
- Order status updates

**Use Cases**:
- Pre-orders
- Scheduled deliveries
- Group buying coordination
- Supplier drop-shipping

### 4. Delivery Notes
**Path**: `/sales/delivery-notes`

**Purpose**: Track product deliveries to customers

**Key Features**:
- Delivery scheduling
- Driver assignment
- Proof of delivery
- GPS tracking
- Delivery status updates

**Use Cases**:
- Local deliveries
- Bulk order fulfillment
- Multi-stop routes
- Delivery verification

### 5. Sales Invoices
**Path**: `/sales/invoices`

**Purpose**: Generate and manage customer invoices

**Key Features**:
- Invoice generation from orders
- Payment tracking
- Credit terms
- Tax calculations
- Aging reports

**Use Cases**:
- Credit sales
- Formal invoicing
- Payment collection
- Financial records

### 6. Sales Reports
**Path**: `/sales/reports`

**Purpose**: Analytics and insights on sales performance

**Key Features**:
- Sales trends
- Product performance
- Customer analytics
- Profitability analysis
- AI-powered insights

**Use Cases**:
- Business planning
- Inventory decisions
- Customer targeting
- Growth tracking

---

## Common Workflows

### Workflow 1: Walk-in Cash Sale (POS)

```
1. Open POS → /sales/pos
2. Scan/search products
3. Add to cart
4. Select payment method (Cash)
5. Complete sale
6. Print/SMS receipt
```

**Offline Support**: ✅ Full offline capability

### Workflow 2: Customer Quotation → Order → Delivery → Invoice

```
1. Create Quotation → /sales/quotations/create
   - Add customer details
   - Add products and quantities
   - Set prices and discounts
   - Save as draft or submit

2. Customer Approves → Convert to Sales Order
   - Open quotation
   - Click "Convert to Order"
   - Confirm details

3. Prepare Delivery → Create Delivery Note
   - Open sales order
   - Click "Create Delivery Note"
   - Assign driver
   - Schedule delivery

4. Complete Delivery → Driver Updates Status
   - Mark items as delivered
   - Capture signature/proof
   - Update delivery status

5. Generate Invoice → Create Sales Invoice
   - From sales order or delivery note
   - Set payment terms
   - Send to customer

6. Receive Payment → Update Invoice
   - Record payment
   - Update accounts
   - Complete transaction
```

**Offline Support**: ⚠️ Partial (quotations, orders can be drafted offline; sync required for delivery tracking)

### Workflow 3: Credit Sale with Follow-up

```
1. POS Sale → Select "Credit" payment
2. Choose/create customer
3. Set credit limit and terms
4. Complete sale (creates invoice)
5. AI tracks payment due date
6. Automated reminders (SMS/WhatsApp)
7. Payment received → Update invoice
```

**Offline Support**: ✅ Full offline capability for sale; sync required for automated reminders

### Workflow 4: Group Buying Order

```
1. Create Sales Order → From supplier catalog
2. Add products needed
3. Mark as "Group Buying"
4. System aggregates with other shops
5. Bulk order placed with supplier
6. Delivery coordinated
7. Delivery note created on arrival
8. Invoice generated
```

**Offline Support**: ⚠️ Requires connectivity for group aggregation

---

## Key Features by User Type

### Spaza Shop Owner (Single Location)

**Primary Modules**:
- POS for daily sales
- Sales invoices for credit customers
- Sales reports for insights

**Recommended Workflow**:
1. Start day with POS module
2. Process sales throughout day
3. End day with cash-up in reports
4. Review AI recommendations

### Multi-Outlet Retailer

**Primary Modules**:
- Sales orders for inter-store transfers
- Delivery notes for tracking shipments
- Consolidated reporting

**Recommended Workflow**:
1. Monitor orders across locations
2. Coordinate deliveries
3. Track inventory movement
4. Analyze performance by outlet

### Chisa Nyama/Restaurant

**Primary Modules**:
- POS with table management
- Sales orders for catering/events
- Delivery notes for food delivery

**Recommended Workflow**:
1. POS for walk-in customers
2. Orders for pre-bookings
3. Delivery tracking for takeaways
4. Daily sales analysis

---

## Offline Capabilities

### Fully Offline
✅ POS transactions
✅ Create quotations (draft)
✅ Create sales orders (draft)
✅ View cached data
✅ Add to queue for sync

### Requires Connectivity
❌ Real-time inventory sync
❌ AI recommendations
❌ Group buying aggregation
❌ SMS/email notifications
❌ Payment gateway integration
❌ GPS tracking

### Smart Sync
- Queues transactions offline
- Auto-syncs when online
- Conflict resolution
- Priority sync for critical data

---

## Payment Methods

### Cash
- Immediate completion
- No verification needed
- Records in cash register

### Mobile Money
- M-Pesa, TymeBank, etc.
- Verification via SMS
- Auto-reconciliation

### Card/QR
- Integrated payment terminals
- Instant verification
- Digital receipts

### Credit/Account
- Customer accounts
- Credit limit checks
- Payment terms (7/14/30 days)
- Automated reminders

### Mixed Payment
- Split across methods
- Partial cash + card
- Layaway tracking

---

## AI Copilot Features

### Smart Recommendations
- "Stock running low on bread - order 20 more loaves"
- "Friday cold drink sales up 40% - increase stock"
- "Customer typically orders on Mondays - prepare quote"

### Automated Tasks
- Generate restock orders from sales patterns
- Create delivery schedules
- Send payment reminders
- Flag overdue invoices

### Insights
- Best-selling products
- Profitable items
- Customer buying patterns
- Seasonal trends

### Automation
- Convert quotations to orders when approved
- Create delivery notes from orders
- Generate invoices on delivery
- Schedule payments

---

## Mobile Optimization

### Touch-Friendly
- Large tap targets
- Swipe gestures
- Minimal typing
- Voice input support

### Low Data Usage
- Compressed images
- Cached catalogs
- Incremental sync
- Offline-first design

### Works on Low-End Devices
- Optimized for Android Go
- Minimal RAM usage
- Progressive enhancement
- Lightweight assets

---

## Multi-Language Support

### Supported Languages
- English
- isiZulu
- isiXhosa
- Sesotho
- Afrikaans

### Dynamic Switching
- User preference
- Per-customer language
- Receipt language
- AI responds in user's language

---

## Integration Points

### Inventory Module
- Real-time stock updates
- Reorder alerts
- Stock reservation
- Batch/serial tracking

### Financial Module
- Auto-posting to accounts
- Tax calculations
- Profit tracking
- Aging reports

### Customer Module
- Customer profiles
- Purchase history
- Credit limits
- Loyalty points

### Supplier Module
- Group buying
- Purchase orders
- Delivery coordination
- Supplier invoicing

---

## Security & Permissions

### User Roles

**Shop Owner/Manager**:
- Full access to all modules
- Configure settings
- View all reports
- Manage users

**Cashier/Sales**:
- POS access
- Create quotations
- View orders
- Limited reporting

**Driver/Delivery**:
- View delivery notes
- Update delivery status
- Capture proof of delivery
- View assigned routes

**Accountant/Finance**:
- View invoices
- Receive payments
- Financial reports
- No sales creation

### Data Protection
- Encrypted storage
- Secure transmission
- Audit trails
- Regular backups
- POPIA compliance

---

## Getting Started

### Initial Setup

1. **Configure Shop Details**
   - Shop name and location
   - Contact information
   - Tax registration
   - Operating hours

2. **Set Up Product Catalog**
   - Import common products
   - Set pricing
   - Configure inventory
   - Add custom items

3. **Create Customer Accounts**
   - Regular customers
   - Credit limits
   - Contact details
   - Preferences

4. **Configure Payment Methods**
   - Enable cash
   - Set up mobile money
   - Card terminal integration
   - Credit terms

5. **Train Staff**
   - POS basics
   - Credit sales
   - Delivery process
   - Reporting

### First Week Checklist

**Day 1-2: POS Only**
- [ ] Process cash sales
- [ ] Learn product search
- [ ] Practice cash-up

**Day 3-4: Add Credit**
- [ ] Create customer accounts
- [ ] First credit sale
- [ ] Track payments

**Day 5-7: Full Features**
- [ ] Create first quotation
- [ ] Convert to order
- [ ] Generate invoice
- [ ] Review reports

---

## Troubleshooting

### POS Not Loading
1. Check internet connection (or confirm offline mode)
2. Clear browser cache
3. Refresh page
4. Check device storage

### Items Not Syncing
1. Verify connectivity
2. Check sync queue in settings
3. Manually trigger sync
4. Contact support if persistent

### Payment Not Processing
1. Verify payment method is enabled
2. Check customer credit limit
3. Ensure sufficient stock
4. Review error messages

### Reports Not Generating
1. Ensure data has synced
2. Check date range
3. Verify permissions
4. Try different report type

---

## Best Practices

### Daily Operations
- Start day with POS open
- Review overnight sync status
- Check low stock alerts
- Process queued transactions
- End day with cash-up

### Weekly Tasks
- Review sales reports
- Follow up on credit accounts
- Reconcile payments
- Plan restocking
- Review AI recommendations

### Monthly Tasks
- Generate financial reports
- Analyze trends
- Update pricing
- Review customer accounts
- Archive old data

---

## Support & Training

### In-App Help
- Context-sensitive tooltips
- Video tutorials
- Quick start guides
- AI assistant

### Community
- User forums
- WhatsApp groups
- Local meetups
- Success stories

### Professional Support
- Phone support (business hours)
- Email support (24hr response)
- On-site training (paid)
- Custom integrations

---

## Roadmap & Coming Features

### Q1 2026
- [ ] Table management for restaurants
- [ ] Loyalty program integration
- [ ] Advanced pricing rules
- [ ] Multi-currency support

### Q2 2026
- [ ] E-commerce integration
- [ ] WhatsApp ordering
- [ ] Voice-activated POS
- [ ] Predictive ordering

### Q3 2026
- [ ] Route optimization for delivery
- [ ] Customer app
- [ ] Subscription sales
- [ ] Equipment rental module

---

## Glossary

**Quotation**: Price proposal to customer before sale

**Sales Order**: Confirmed customer order awaiting fulfillment

**Delivery Note**: Record of goods delivered to customer

**Invoice**: Payment request for goods/services provided

**Credit Sale**: Sale with deferred payment

**Group Buying**: Collective purchasing for better pricing

**Stock Reservation**: Inventory allocated to specific order

**Cash-up**: Daily reconciliation of cash and sales

**Proof of Delivery**: Signature/photo confirming delivery

**Aging Report**: Analysis of overdue invoices

---

## Conclusion

TOSS Sales modules are designed to be simple yet powerful, enabling township SMMEs to operate with the same capabilities as large retailers while maintaining the flexibility and personal touch that makes them community lifelines.

Start small with POS, expand to quotations and orders as you grow, and let the AI Copilot guide you every step of the way.

**Remember**: You're not alone. Thousands of shop owners across South Africa are using TOSS to transform their businesses. Join the network, learn from peers, and grow together.

---

For detailed technical documentation, see:
- [Sales Module Implementation](./SALES_MODULE_COMPLETE.md)
- [POS Integration Guide](./POS_CART_INTEGRATION_COMPLETE.md)
- [API Documentation](../docs/api/)
- [Developer Guide](../docs/development/)
