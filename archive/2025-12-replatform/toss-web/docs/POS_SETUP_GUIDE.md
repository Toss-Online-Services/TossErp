# POS Hardware Setup Guide for TOSS ERP

## Quick Start

### 1. Hardware Requirements

#### Essential Hardware:
- **Barcode Scanner**: USB keyboard wedge or HID scanner
- **Receipt Printer**: ESC/POS thermal printer (58mm or 80mm)
- **Cash Drawer**: RJ-11/RJ-12 connected to printer
- **Card Reader**: USB HID or serial card reader (optional)

#### Recommended Hardware:
- **Epson TM-T20II** or **TM-T88V** thermal printer (R 2,500 - R 4,500)
- **Symbol LS2208** or **Honeywell Voyager 1200g** barcode scanner (R 1,200 - R 2,500)
- **APG Vasario** cash drawer (R 1,800 - R 3,000)
- **ID TECH SecureMag** or **MagTek Dynamag** card reader (R 800 - R 1,500)

### 2. Physical Setup

#### Step 1: Connect Receipt Printer
1. Unbox the thermal printer
2. Connect power adapter
3. Connect USB cable to computer
4. Load thermal paper (58mm or 80mm roll)
5. Turn on printer
6. Print test page (usually a button on the printer)

#### Step 2: Connect Cash Drawer
1. Locate RJ-11/RJ-12 port on receipt printer (usually labeled "DK" or "Drawer Kick")
2. Connect cash drawer cable to printer
3. Test drawer opens when printer sends pulse command

#### Step 3: Connect Barcode Scanner
1. Connect USB cable to computer
2. Scanner should power on automatically (LED indicator)
3. Test by opening Notepad and scanning a barcode
4. Barcode should appear as typed text

#### Step 4: Connect Card Reader (Optional)
1. Connect USB cable to computer
2. Install any required drivers from manufacturer
3. Test with vendor software if available

### 3. Software Configuration

#### Browser Requirements:
- **Chrome 89+** or **Edge 89+** (required for Web Serial API and WebHID)
- **HTTPS enabled** (required in production, localhost works for development)

#### Enable Required APIs:

1. **Web Serial API** (for receipt printer):
   - Chrome: `chrome://flags/#enable-experimental-web-platform-features`
   - Enable "Experimental Web Platform features"

2. **WebHID API** (for card reader):
   - Usually enabled by default in Chrome 89+

3. **Web USB API** (for cash drawer):
   - Usually enabled by default in Chrome 61+

### 4. Grant Permissions

When you first access the POS page:

1. Navigate to `/sales/pos`
2. Hardware status indicators will show red (not connected)
3. Click "Request Hardware" or try to use a feature
4. Browser will prompt for device permissions
5. Select your hardware from the list
6. Click "Connect"
7. Status indicators should turn green

### 5. Testing

#### Test Barcode Scanner:
1. Focus on the search input field
2. Scan any product barcode
3. Product should be added to cart automatically
4. Green notification should appear

#### Test Card Reader:
1. Add items to cart
2. Click "Card Payment" button
3. Follow on-screen prompts
4. Insert/tap card when prompted
5. Wait for approval

#### Test Receipt Printer:
1. Complete a sale
2. Click "Print Receipt" button
3. Receipt should print automatically
4. Check formatting and content

#### Test Cash Drawer:
1. Click "Open Drawer" button
2. Drawer should open with audible click
3. If not working, check cable connection

## Troubleshooting

### Barcode Scanner Not Working

**Problem**: Scanned barcodes don't appear or add wrong products

**Solutions**:
1. Test scanner in Notepad - should type the barcode
2. Check USB connection
3. Ensure scanner is in keyboard wedge mode (not USB HID mode)
4. Check barcode format matches product SKUs
5. Verify scanner is configured for correct prefix/suffix

**Configuration**:
- Most scanners can be configured with special barcodes
- Set suffix to "Enter" (CR or CR+LF)
- Disable prefix if enabled
- Set to auto-sense barcode type

### Card Reader Not Detected

**Problem**: Card reader shows as disconnected

**Solutions**:
1. Install manufacturer drivers
2. Check USB connection
3. Grant browser permissions (chrome://settings/content/usbDevices)
4. Try different USB port
5. Restart browser after connecting

**Supported Readers**:
- ID TECH SecureMag (Vendor ID: 0x0ACD)
- MagTek Dynamag (Vendor ID: 0x0801)
- Most USB HID card readers

### Receipt Printer Issues

**Problem**: Receipts don't print or print incorrectly

**Solutions**:
1. Check paper is loaded correctly
2. Verify USB/Serial connection
3. Test with vendor utility software
4. Check baud rate (default: 9600)
5. Ensure printer supports ESC/POS commands
6. Grant serial port permissions in browser

**Paper Loading**:
1. Open printer cover
2. Insert paper roll (thermal side facing print head)
3. Pull paper through slot
4. Close cover
5. Press feed button to test

### Cash Drawer Won't Open

**Problem**: Drawer doesn't open when button clicked

**Solutions**:
1. Check RJ-11/RJ-12 cable connection to printer
2. Verify printer supports drawer kick
3. Test drawer with printer test button
4. Check drawer lock is not engaged
5. Verify cable is not damaged

**Manual Test**:
- Most printers have a "drawer kick" test in their menu
- Use printer buttons to navigate to test menu
- Select "Drawer Kick" option
- Drawer should open

## Advanced Configuration

### Custom Barcode Formats

If your products use custom barcodes:

```javascript
// In products.value, add custom barcode field
{ 
  id: '1', 
  name: 'Product', 
  sku: 'PROD001',
  barcode: '6001001234567', // EAN-13
  customCode: 'CUST001'      // Custom format
}

// Update processBarcode function
const processBarcode = (barcode: string) => {
  const product = products.value.find(p => 
    p.barcode === barcode ||
    p.customCode === barcode ||
    p.sku === barcode
  )
  // ... rest of function
}
```

### Multiple Card Readers

If you have multiple card readers:

```javascript
const selectCardReader = async () => {
  const devices = await navigator.hid.getDevices()
  // Show selection dialog
  // Connect to selected device
}
```

### Network Printers

For network printers (not USB):

```javascript
// Use print server or cloud printing
const printToNetwork = async (receiptData) => {
  await fetch('/api/print', {
    method: 'POST',
    body: JSON.stringify(receiptData)
  })
}
```

## Hardware Vendor Contacts

### South African Suppliers:

**Barcode Scanners**:
- Barcode Warehouse: www.barcodewarehouse.co.za
- POS Direct: www.posdirect.co.za
- Scan Shop: www.scanshop.co.za

**Receipt Printers**:
- Epson South Africa: www.epson.co.za
- Star Micronics: www.star-emea.com
- Bixolon: www.bixolon.com

**POS Bundles**:
- POS Solutions SA: Complete POS kits from R 8,000
- Retail Systems: Integrated POS hardware from R 12,000

## Security Best Practices

### PCI DSS Compliance:

1. **Never store card data** in browser or database
2. **Use tokenization** for recurring payments
3. **Encrypt all transmissions** (HTTPS required)
4. **Log all transactions** for audit trail
5. **Restrict access** to POS terminals

### Physical Security:

1. **Lock cash drawer** when not in use
2. **Secure POS terminal** to counter
3. **Use cable locks** for hardware
4. **Install security cameras** covering POS area
5. **Implement dual control** for large transactions

## Maintenance

### Daily:
- [ ] Count cash drawer at start/end of shift
- [ ] Check receipt paper level
- [ ] Test barcode scanner
- [ ] Clean scanner window
- [ ] Verify card reader connection

### Weekly:
- [ ] Clean receipt printer print head
- [ ] Check all cable connections
- [ ] Review transaction logs
- [ ] Test backup procedures
- [ ] Update product database

### Monthly:
- [ ] Replace receipt printer cleaning card
- [ ] Check for software updates
- [ ] Review hardware warranty status
- [ ] Test disaster recovery
- [ ] Audit transaction records

## Support

### Technical Support:
- **TOSS ERP Support**: support@toss-erp.co.za
- **Hardware Issues**: Contact vendor directly
- **Software Issues**: Check browser console for errors

### Emergency Procedures:

**If POS system fails**:
1. Switch to manual sales recording
2. Use backup calculator and receipt book
3. Record all transactions on paper
4. Enter into system when back online
5. Contact support immediately

**If hardware fails**:
1. Check connections and power
2. Restart computer/browser
3. Test with vendor software
4. Use backup hardware if available
5. Continue with manual process

## Training Checklist

Train all staff on:
- [ ] Basic POS operation
- [ ] Barcode scanning
- [ ] Card payment processing
- [ ] Receipt printing
- [ ] Cash drawer management
- [ ] Handling payment failures
- [ ] Voiding transactions
- [ ] End-of-day procedures
- [ ] Emergency procedures
- [ ] Security protocols

## Compliance

### South African Requirements:

1. **POPIA Compliance**: Protect customer payment data
2. **SARS Requirements**: Keep transaction records for 5 years
3. **Consumer Protection Act**: Provide clear receipts
4. **FICA**: Verify customer identity for large transactions

### Receipt Requirements:

Must include:
- [ ] Business name and address
- [ ] VAT registration number (if applicable)
- [ ] Date and time of transaction
- [ ] Description of goods/services
- [ ] Quantity and price
- [ ] Total amount including VAT
- [ ] Payment method
- [ ] Receipt number (for tracking)

## Cost Breakdown

### Initial Investment:

| Item | Cost (ZAR) |
|------|-----------|
| Barcode Scanner | R 1,500 - R 2,500 |
| Receipt Printer | R 2,500 - R 4,500 |
| Cash Drawer | R 1,800 - R 3,000 |
| Card Reader | R 800 - R 1,500 |
| **Total** | **R 6,600 - R 11,500** |

### Monthly Costs:

| Item | Cost (ZAR) |
|------|-----------|
| Thermal Paper Rolls | R 200 - R 400 |
| Printer Maintenance | R 100 - R 200 |
| Card Processing Fees | 2.5% - 3.5% of card sales |
| Software License | R 0 (TOSS ERP) |

### ROI Calculation:

**Time Savings**:
- Manual checkout: ~3-5 minutes per transaction
- POS checkout: ~30-60 seconds per transaction
- **Savings**: 2-4 minutes per transaction

**For 50 transactions/day**:
- Time saved: 100-200 minutes (1.5-3 hours)
- Labor cost saved: R 50-100/day
- **Monthly savings**: R 1,500-3,000

**Payback period**: 2-8 months

## Next Steps

1. ✅ Review hardware requirements
2. ✅ Purchase recommended hardware
3. ✅ Set up physical hardware
4. ✅ Configure software
5. ✅ Test all features
6. ✅ Train staff
7. ✅ Go live!

For questions or support, contact: support@toss-erp.co.za
