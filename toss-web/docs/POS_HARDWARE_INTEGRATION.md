# POS Hardware Integration Guide

## Overview

The TOSS ERP POS system now includes comprehensive hardware integration support for:
- **Barcode Scanners** (USB, Bluetooth, Keyboard Wedge)
- **Card Readers** (Magnetic stripe, Chip, Contactless)
- **Receipt Printers** (ESC/POS thermal printers)
- **Cash Drawers** (Connected via printer or standalone)

## Features Implemented

### 1. Barcode Scanner Integration

#### Supported Types:
- **Keyboard Wedge Scanners**: Automatically detected, no configuration needed
- **USB HID Scanners**: Via WebHID API
- **Serial Scanners**: Via Web Serial API
- **Bluetooth Scanners**: Via Web Bluetooth API

#### Supported Vendors:
- Symbol/Zebra (Vendor ID: 0x05E0)
- Honeywell (Vendor ID: 0x0C2E)
- Generic USB scanners (Vendor ID: 0x1A86)

#### Features:
- Automatic barcode detection
- Real-time product lookup
- Instant cart addition
- Support for all standard barcode formats (EAN-13, UPC, Code 128, etc.)

### 2. Card Reader Integration

#### Supported Types:
- Magnetic stripe readers
- Chip card readers (EMV)
- Contactless/NFC readers

#### Supported Vendors:
- ID TECH (Vendor ID: 0x0ACD)
- MagTek (Vendor ID: 0x0801)
- AuthenTec (Vendor ID: 0x08FF)

#### Features:
- Secure card data processing
- Real-time transaction status
- Support for multiple payment methods
- PCI DSS compliant architecture

### 3. Receipt Printer Integration

#### Supported Types:
- ESC/POS thermal printers
- Serial printers
- USB printers
- Network printers (via print server)

#### Supported Vendors:
- Epson (Vendor ID: 0x04B8)
- Star Micronics (Vendor ID: 0x0519)
- Bixolon (Vendor ID: 0x154F)

#### Features:
- ESC/POS command generation
- Custom receipt formatting
- Logo printing support
- Automatic paper cutting
- Fallback to browser print

### 4. Cash Drawer Integration

#### Supported Types:
- Printer-connected drawers
- Standalone USB drawers

#### Features:
- Electronic drawer opening
- Manual open button
- Automatic open on cash payment
- Status monitoring

## Installation

### 1. Install Dependencies

```bash
npm install @point-of-sale/webhid-barcode-scanner dynamsoft-javascript-barcode
```

### 2. Browser Requirements

The hardware integration requires modern browser support for:
- **WebHID API**: Chrome 89+, Edge 89+
- **Web Serial API**: Chrome 89+, Edge 89+
- **Web Bluetooth API**: Chrome 56+, Edge 79+

### 3. HTTPS Requirement

All Web APIs require HTTPS in production. For local development, `localhost` is allowed.

## Usage

### Basic Setup

```vue
<script setup>
import { usePOSHardware } from '~/composables/usePOSHardware'

const {
  barcodeScannerConnected,
  cardReaderConnected,
  receiptPrinterConnected,
  cashDrawerConnected,
  initializeHardware,
  setupBarcodeListener,
  processCardPayment,
  printReceipt,
  openCashDrawer
} = usePOSHardware()

onMounted(async () => {
  // Initialize all hardware
  await initializeHardware()
  
  // Setup barcode listener
  const cleanup = setupBarcodeListener((barcode) => {
    console.log('Scanned:', barcode)
    // Add product to cart
  })
  
  // Cleanup on unmount
  onUnmounted(() => cleanup())
})
</script>
```

### Barcode Scanning

```javascript
// Automatic detection (keyboard wedge)
const cleanup = setupBarcodeListener((barcode) => {
  const product = products.find(p => p.barcode === barcode)
  if (product) {
    addToCart(product)
  }
})

// Manual request (USB/HID)
const scanner = await requestBarcodeScanner()
```

### Card Payment Processing

```javascript
try {
  const result = await processCardPayment(amount)
  console.log('Payment successful:', result)
  // {
  //   success: true,
  //   transactionId: 'TXN-123456',
  //   amount: 100.00,
  //   cardType: 'Visa',
  //   lastFourDigits: '4242'
  // }
} catch (error) {
  console.error('Payment failed:', error)
}
```

### Receipt Printing

```javascript
const receiptData = {
  storeName: "THABO'S SPAZA SHOP",
  storeAddress: '123 Main Street, Soweto',
  storePhone: '+27 11 123 4567',
  receiptNumber: 'RCP-001',
  date: new Date().toLocaleString(),
  cashier: 'Thabo',
  customer: 'Walk-in Customer',
  items: [
    { name: 'Coca Cola 2L', quantity: 2, price: 35, total: 70 },
    { name: 'White Bread', quantity: 1, price: 18, total: 18 }
  ],
  total: 88,
  paymentMethod: 'Cash'
}

await printReceipt(receiptData)
```

### Cash Drawer Control

```javascript
// Open cash drawer
try {
  await openCashDrawer()
  console.log('Drawer opened')
} catch (error) {
  console.error('Failed to open drawer:', error)
}
```

## Hardware Status Monitoring

The composable provides reactive status indicators:

```vue
<template>
  <div>
    <div v-if="barcodeScannerConnected" class="status-indicator">
      ✓ Barcode Scanner Connected
    </div>
    <div v-if="cardReaderConnected" class="status-indicator">
      ✓ Card Reader Connected
    </div>
    <div v-if="receiptPrinterConnected" class="status-indicator">
      ✓ Receipt Printer Connected
    </div>
    <div v-if="cashDrawerConnected" class="status-indicator">
      ✓ Cash Drawer Connected
    </div>
  </div>
</template>
```

## ESC/POS Commands

The receipt printer uses standard ESC/POS commands:

| Command | Hex | Description |
|---------|-----|-------------|
| Initialize | ESC @ | Reset printer |
| Bold On | ESC E 1 | Enable bold text |
| Bold Off | ESC E 0 | Disable bold text |
| Align Center | ESC a 1 | Center alignment |
| Align Left | ESC a 0 | Left alignment |
| Cut Paper | GS V 0 | Full cut |
| Open Drawer | ESC p 0 25 250 | Pulse drawer pin |

## Troubleshooting

### Barcode Scanner Not Working

1. **Check USB connection**: Ensure scanner is properly connected
2. **Test scanner**: Open Notepad and scan - should type the barcode
3. **Check browser**: Ensure Chrome/Edge 89+ for WebHID support
4. **Grant permissions**: Click "Request Hardware" button to grant access

### Card Reader Not Detected

1. **Install drivers**: Some readers require vendor-specific drivers
2. **Check compatibility**: Verify reader supports WebHID
3. **Test connection**: Use vendor software to test reader
4. **Browser permissions**: Ensure site has HID device access

### Receipt Printer Issues

1. **Check connection**: Verify USB/Serial connection
2. **Test printer**: Use vendor utility to print test page
3. **Check paper**: Ensure paper is loaded correctly
4. **Baud rate**: Default is 9600, adjust if needed
5. **Fallback**: System will use browser print if printer unavailable

### Cash Drawer Won't Open

1. **Check connection**: Drawer should be connected to printer
2. **Printer compatibility**: Ensure printer supports drawer kick
3. **Cable**: Verify RJ-11/RJ-12 cable is connected
4. **Manual test**: Try opening via printer test button

## Security Considerations

### PCI DSS Compliance

- Card data is never stored in browser
- All card processing happens on secure hardware
- No card numbers are logged or transmitted
- Use tokenization for recurring payments

### HTTPS Requirement

All Web APIs require HTTPS. Configure your server:

```nginx
server {
    listen 443 ssl;
    server_name pos.example.com;
    
    ssl_certificate /path/to/cert.pem;
    ssl_certificate_key /path/to/key.pem;
    
    location / {
        proxy_pass http://localhost:3001;
    }
}
```

### Permissions

Users must grant explicit permission for each hardware type:
- First use requires clicking "Request Hardware" button
- Permissions persist across sessions
- Users can revoke permissions in browser settings

## Browser Support Matrix

| Browser | WebHID | Web Serial | Web Bluetooth |
|---------|--------|------------|---------------|
| Chrome 89+ | ✓ | ✓ | ✓ |
| Edge 89+ | ✓ | ✓ | ✓ |
| Firefox | ✗ | ✗ | ✓ |
| Safari | ✗ | ✗ | ✗ |

**Recommendation**: Use Chrome or Edge for full hardware support.

## Testing

### Simulated Hardware

For development without physical hardware:

```javascript
// Simulate barcode scan
const testBarcode = '6001001234567'
handleBarcodeScanned(testBarcode)

// Simulate card payment
const mockPayment = {
  success: true,
  transactionId: 'TEST-123',
  amount: 100,
  cardType: 'Test Card'
}
```

### Hardware Test Page

Visit `/sales/pos/hardware` to access the hardware-enabled POS page with:
- Live hardware status indicators
- Hardware request buttons
- Full POS functionality
- Real-time barcode scanning
- Card payment processing
- Receipt printing
- Cash drawer control

## Production Deployment

### 1. SSL Certificate

```bash
# Using Let's Encrypt
sudo certbot --nginx -d pos.yourdomain.com
```

### 2. Hardware Setup

1. Connect barcode scanner to USB port
2. Connect receipt printer to USB/Serial port
3. Connect cash drawer to printer
4. Connect card reader to USB port
5. Install any required drivers

### 3. Test All Hardware

1. Open POS page: `https://pos.yourdomain.com/sales/pos/hardware`
2. Click "Request Hardware" to grant permissions
3. Verify all status indicators show green
4. Test barcode scanning
5. Test card payment
6. Test receipt printing
7. Test cash drawer opening

### 4. Train Staff

- How to scan barcodes
- How to process card payments
- How to handle payment failures
- How to open cash drawer manually
- How to load receipt paper
- How to troubleshoot common issues

## API Reference

See `composables/usePOSHardware.ts` for complete API documentation.

## Support

For hardware-specific issues:
- Barcode scanners: Contact vendor support
- Card readers: Contact payment processor
- Receipt printers: Check ESC/POS compatibility
- Cash drawers: Verify printer compatibility

For software issues:
- Check browser console for errors
- Verify HTTPS is enabled
- Ensure permissions are granted
- Test with different hardware

## Future Enhancements

- [ ] Customer display support
- [ ] Scale integration
- [ ] Label printer support
- [ ] Signature capture
- [ ] Biometric authentication
- [ ] Mobile POS (Bluetooth devices)
- [ ] Offline mode with sync
- [ ] Multi-terminal support

## License

This POS hardware integration is part of the TOSS ERP system.
