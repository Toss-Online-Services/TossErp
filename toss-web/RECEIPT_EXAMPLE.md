# 🧾 TOSS ERP III - Receipt Example

## Printed Receipt Format

This is what customers receive when they make a purchase at Thabo's Spaza Shop using our POS system.

---

## 📄 Receipt Layout

```
================================
                                
      THABO'S SPAZA SHOP        
                                
    123 Main Street, Soweto     
      +27 11 123 4567           
                                
================================

Receipt: RCP-1728934567890
Date: 10/10/2025, 14:35:22
Cashier: Thabo

--------------------------------

Coca Cola 2L
2 x R35.00 = R70.00

White Bread 700g
3 x R18.00 = R54.00

Milk 1L
1 x R22.00 = R22.00

Simba Chips 125g
4 x R12.00 = R48.00

Sunlight Soap 250g
1 x R15.00 = R15.00

--------------------------------

TOTAL: R209.00

Payment: Cash

--------------------------------
                                
         Thank you!             
      Please come again!        
                                
================================
```

---

## 🎨 Receipt Features

### Header Section (Centered, Bold)
```
      THABO'S SPAZA SHOP        
    123 Main Street, Soweto     
      +27 11 123 4567           
```

**Features**:
- ✅ Store name in **bold**
- ✅ Center-aligned
- ✅ Store address
- ✅ Contact phone number

### Transaction Details (Left-aligned)
```
Receipt: RCP-1728934567890
Date: 10/10/2025, 14:35:22
Cashier: Thabo
```

**Features**:
- ✅ Unique receipt number
- ✅ Date and time stamp
- ✅ Cashier name

### Items Section
```
Coca Cola 2L
2 x R35.00 = R70.00
```

**Format**: 
- Line 1: Product name
- Line 2: `Quantity x Unit Price = Line Total`

**Features**:
- ✅ Clear product names
- ✅ Quantity shown
- ✅ Unit price displayed
- ✅ Line total calculated
- ✅ Currency symbol (R for ZAR)

### Total Section (Bold)
```
TOTAL: R209.00
Payment: Cash
```

**Features**:
- ✅ **Bold** total amount
- ✅ Payment method indicated
- ✅ Clear separation line

### Footer (Centered)
```
         Thank you!             
      Please come again!        
```

**Features**:
- ✅ Center-aligned
- ✅ Customer appreciation message
- ✅ Extra spacing before paper cut

---

## 🖨️ Technical Specifications

### ESC/POS Commands Used

| Command | Purpose | Code |
|---------|---------|------|
| `ESC @` | Initialize printer | `0x1B 0x40` |
| `ESC a 1` | Center align | `0x1B 0x61 0x01` |
| `ESC a 0` | Left align | `0x1B 0x61 0x00` |
| `ESC E 1` | Bold ON | `0x1B 0x45 0x01` |
| `ESC E 0` | Bold OFF | `0x1B 0x45 0x00` |
| `GS V 0` | Cut paper | `0x1D 0x56 0x00` |

### Paper Specifications
- **Width**: 80mm (standard thermal paper)
- **Characters per line**: ~32-48 (depending on font)
- **Paper type**: Thermal receipt paper
- **Print method**: ESC/POS thermal printing

### Printer Compatibility
✅ **Supported Printers**:
- Epson TM series (TM-T20, TM-T88, etc.)
- Star TSP series
- Bixolon SRP series
- Generic ESC/POS compatible printers

---

## 💡 Receipt Variations

### Example 1: Small Purchase
```
================================
                                
      THABO'S SPAZA SHOP        
                                
    123 Main Street, Soweto     
      +27 11 123 4567           
                                
================================

Receipt: RCP-1728934567891
Date: 10/10/2025, 15:20:10
Cashier: Thabo

--------------------------------

Maggi 2-Minute Noodles
1 x R8.00 = R8.00

--------------------------------

TOTAL: R8.00

Payment: Cash

--------------------------------
                                
         Thank you!             
      Please come again!        
                                
================================
```

### Example 2: Large Purchase
```
================================
                                
      THABO'S SPAZA SHOP        
                                
    123 Main Street, Soweto     
      +27 11 123 4567           
                                
================================

Receipt: RCP-1728934567892
Date: 10/10/2025, 16:45:33
Cashier: Thabo

--------------------------------

Coca Cola 2L
3 x R35.00 = R105.00

White Bread 700g
5 x R18.00 = R90.00

Milk 1L
2 x R22.00 = R44.00

Simba Chips 125g
6 x R12.00 = R72.00

Sunlight Soap 250g
3 x R15.00 = R45.00

Maggi 2-Minute Noodles
10 x R8.00 = R80.00

Castle Lager 440ml
6 x R28.00 = R168.00

Purity Baby Food
2 x R25.00 = R50.00

Colgate Toothpaste
1 x R32.00 = R32.00

Frozen Chicken 1kg
2 x R65.00 = R130.00

--------------------------------

TOTAL: R816.00

Payment: Card

--------------------------------
                                
         Thank you!             
      Please come again!        
                                
================================
```

### Example 3: Walk-in Customer
```
================================
                                
      THABO'S SPAZA SHOP        
                                
    123 Main Street, Soweto     
      +27 11 123 4567           
                                
================================

Receipt: RCP-1728934567893
Date: 10/10/2025, 17:10:05
Cashier: Thabo
Customer: Walk-in Customer

--------------------------------

Coca Cola 2L
1 x R35.00 = R35.00

--------------------------------

TOTAL: R35.00

Payment: Mobile Money

--------------------------------
                                
         Thank you!             
      Please come again!        
                                
================================
```

---

## 🎯 Receipt Customization Options

### Available Data Fields

```typescript
interface ReceiptData {
  storeName: string          // "THABO'S SPAZA SHOP"
  storeAddress: string       // "123 Main Street, Soweto"
  storePhone: string         // "+27 11 123 4567"
  receiptNumber: string      // "RCP-1728934567890"
  date: string               // "10/10/2025, 14:35:22"
  cashier: string            // "Thabo"
  customer?: string          // Optional customer name
  items: Array<{
    name: string             // Product name
    quantity: number         // Quantity purchased
    price: number            // Unit price
    total: number            // Line total
  }>
  total: number              // Grand total
  paymentMethod: string      // "Cash", "Card", "Mobile", etc.
}
```

### Customizable Elements

1. **Store Information**
   - Store name
   - Address
   - Phone number
   - Logo (if printer supports graphics)

2. **Transaction Details**
   - Receipt number format
   - Date/time format
   - Cashier name
   - Customer name (optional)

3. **Footer Message**
   - Thank you message
   - Promotional text
   - Store hours
   - Website/social media

4. **Formatting**
   - Font size (if printer supports)
   - Bold/underline
   - Alignment
   - Line spacing

---

## 🔧 Implementation Details

### Receipt Generation Code

```typescript
const receiptData = {
  storeName: "THABO'S SPAZA SHOP",
  storeAddress: '123 Main Street, Soweto',
  storePhone: '+27 11 123 4567',
  receiptNumber: `RCP-${Date.now()}`,
  date: new Date().toLocaleString('en-ZA'),
  cashier: 'Thabo',
  customer: selectedCustomer.value || 'Walk-in Customer',
  items: cartItems.value.map(item => ({
    name: item.name,
    quantity: item.quantity,
    price: item.price,
    total: item.price * item.quantity
  })),
  total: cartTotal.value,
  paymentMethod: selectedPaymentMethod.value
}

await printReceipt(receiptData)
```

### Browser Print Fallback

If ESC/POS printer is not available, the system falls back to browser print dialog:

```
┌─────────────────────────────────────┐
│  Browser Print Preview              │
├─────────────────────────────────────┤
│                                     │
│  THABO'S SPAZA SHOP                 │
│  123 Main Street, Soweto            │
│  +27 11 123 4567                    │
│                                     │
│  Receipt: RCP-1728934567890         │
│  Date: 10/10/2025, 14:35:22         │
│  Cashier: Thabo                     │
│                                     │
│  Items:                             │
│  • Coca Cola 2L                     │
│    2 x R35.00 = R70.00              │
│                                     │
│  TOTAL: R70.00                      │
│  Payment: Cash                      │
│                                     │
│  Thank you!                         │
│                                     │
└─────────────────────────────────────┘
```

---

## 📱 Email Receipt Format

When customers request email receipts, they receive:

```html
<!DOCTYPE html>
<html>
<head>
  <style>
    body { font-family: 'Courier New', monospace; }
    .receipt { max-width: 400px; margin: 0 auto; }
    .center { text-align: center; }
    .bold { font-weight: bold; }
    .line { border-top: 1px dashed #000; margin: 10px 0; }
  </style>
</head>
<body>
  <div class="receipt">
    <div class="center bold">THABO'S SPAZA SHOP</div>
    <div class="center">123 Main Street, Soweto</div>
    <div class="center">+27 11 123 4567</div>
    
    <div class="line"></div>
    
    <div>Receipt: RCP-1728934567890</div>
    <div>Date: 10/10/2025, 14:35:22</div>
    <div>Cashier: Thabo</div>
    
    <div class="line"></div>
    
    <div>Coca Cola 2L</div>
    <div>2 x R35.00 = R70.00</div>
    
    <div class="line"></div>
    
    <div class="bold">TOTAL: R70.00</div>
    <div>Payment: Cash</div>
    
    <div class="line"></div>
    
    <div class="center">Thank you!</div>
  </div>
</body>
</html>
```

---

## ✅ Receipt Quality Checklist

### Print Quality
- ✅ Clear, readable text
- ✅ Proper alignment
- ✅ Consistent spacing
- ✅ Bold text for emphasis
- ✅ Clean paper cut

### Information Accuracy
- ✅ Correct store details
- ✅ Accurate date/time
- ✅ Correct item names
- ✅ Accurate quantities
- ✅ Correct prices
- ✅ Accurate total

### Professional Appearance
- ✅ Neat formatting
- ✅ Consistent layout
- ✅ Professional header
- ✅ Clear item separation
- ✅ Friendly footer message

### Compliance
- ✅ Receipt number for tracking
- ✅ Date and time stamp
- ✅ Cashier identification
- ✅ Payment method recorded
- ✅ Store contact information

---

## 🎨 Future Enhancements

### Planned Features
- ⚠️ QR code for digital receipt
- ⚠️ Barcode for returns/exchanges
- ⚠️ Loyalty points display
- ⚠️ Store logo printing
- ⚠️ Promotional messages
- ⚠️ Tax breakdown (VAT)
- ⚠️ Multi-language support

### Advanced Options
- ⚠️ Receipt templates
- ⚠️ Custom branding
- ⚠️ Variable font sizes
- ⚠️ Graphics support
- ⚠️ Duplicate receipt printing
- ⚠️ Receipt history/reprint

---

## 📊 Receipt Statistics

### Average Receipt
- **Items**: 3-5 products
- **Total**: R50-R200
- **Print time**: 2-3 seconds
- **Paper used**: 10-15cm

### Daily Volume
- **Receipts printed**: ~50-100 per day
- **Paper rolls used**: 1-2 per week
- **Ink/toner**: Thermal (no ink needed)

---

**Receipt Format Version**: 1.0  
**Last Updated**: {{ new Date().toLocaleDateString() }}  
**Status**: ✅ Production Ready  
**Printer Compatibility**: ESC/POS Standard

---

**🧾 PROFESSIONAL RECEIPT SYSTEM - READY FOR USE 🧾**

