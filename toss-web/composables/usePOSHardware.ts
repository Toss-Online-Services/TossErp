/**
 * POS Hardware Integration Composable
 * Handles barcode scanners, card readers, receipt printers, and cash drawers
 */

export const usePOSHardware = () => {
  // Hardware status
  const barcodeScannerConnected = ref(false)
  const cardReaderConnected = ref(false)
  const receiptPrinterConnected = ref(false)
  const cashDrawerConnected = ref(false)

  // Barcode buffer for keyboard wedge scanners
  let barcodeBuffer = ''
  let barcodeTimeout: NodeJS.Timeout | null = null

  /**
   * Initialize all POS hardware
   */
  const initializeHardware = async () => {
    await checkBarcodeScanner()
    await checkCardReader()
    await checkReceiptPrinter()
    await checkCashDrawer()
  }

  /**
   * Barcode Scanner Functions
   */
  const checkBarcodeScanner = async () => {
    try {
      // Check for WebHID support (USB scanners)
      if ('hid' in navigator) {
        const devices = await (navigator as any).hid.getDevices()
        barcodeScannerConnected.value = devices.some((device: any) => 
          device.vendorId === 0x05E0 || // Symbol/Zebra
          device.vendorId === 0x0C2E || // Honeywell
          device.vendorId === 0x1A86    // Generic USB barcode scanner
        )
      }

      // Check for Serial API support
      if ('serial' in navigator) {
        const ports = await (navigator as any).serial.getPorts()
        if (ports.length > 0) {
          barcodeScannerConnected.value = true
        }
      }

      // Keyboard wedge scanners are always "connected" if USB keyboard is present
      if (!barcodeScannerConnected.value) {
        barcodeScannerConnected.value = true // Assume keyboard wedge
      }
    } catch (error) {
      console.error('Barcode scanner check failed:', error)
      barcodeScannerConnected.value = false
    }
  }

  const setupBarcodeListener = (callback: (barcode: string) => void) => {
    const handleKeyPress = (event: KeyboardEvent) => {
      // Ignore if user is typing in an input field (except our search field)
      const target = event.target as HTMLElement
      if (target.tagName === 'INPUT' && !target.classList.contains('barcode-input')) {
        return
      }

      // Clear timeout on each keypress
      if (barcodeTimeout) {
        clearTimeout(barcodeTimeout)
      }

      // Add character to buffer
      if (event.key.length === 1) {
        barcodeBuffer += event.key
      }

      // Process barcode on Enter key
      if (event.key === 'Enter' && barcodeBuffer.length >= 8) {
        callback(barcodeBuffer)
        barcodeBuffer = ''
        event.preventDefault()
        return
      }

      // Reset buffer after 100ms of inactivity (barcode scanners are fast)
      barcodeTimeout = setTimeout(() => {
        barcodeBuffer = ''
      }, 100)
    }

    document.addEventListener('keypress', handleKeyPress)

    // Return cleanup function
    return () => {
      document.removeEventListener('keypress', handleKeyPress)
      if (barcodeTimeout) {
        clearTimeout(barcodeTimeout)
      }
    }
  }

  const requestBarcodeScanner = async () => {
    try {
      if ('hid' in navigator) {
        const device = await (navigator as any).hid.requestDevice({
          filters: [
            { vendorId: 0x05E0 }, // Symbol/Zebra
            { vendorId: 0x0C2E }, // Honeywell
            { vendorId: 0x1A86 }  // Generic
          ]
        })
        if (device) {
          await device.open()
          barcodeScannerConnected.value = true
          return device
        }
      }
    } catch (error) {
      console.error('Failed to request barcode scanner:', error)
    }
    return null
  }

  /**
   * Card Reader Functions
   */
  const checkCardReader = async () => {
    try {
      if ('hid' in navigator) {
        const devices = await (navigator as any).hid.getDevices()
        cardReaderConnected.value = devices.some((device: any) => 
          device.vendorId === 0x0ACD || // ID TECH
          device.vendorId === 0x0801 || // MagTek
          device.vendorId === 0x08FF    // AuthenTec
        )
      }
    } catch (error) {
      console.error('Card reader check failed:', error)
      cardReaderConnected.value = false
    }
  }

  const requestCardReader = async () => {
    try {
      if ('hid' in navigator) {
        const device = await (navigator as any).hid.requestDevice({
          filters: [
            { vendorId: 0x0ACD }, // ID TECH
            { vendorId: 0x0801 }, // MagTek
            { vendorId: 0x08FF }  // AuthenTec
          ]
        })
        if (device) {
          await device.open()
          cardReaderConnected.value = true
          return device
        }
      }
    } catch (error) {
      console.error('Failed to request card reader:', error)
    }
    return null
  }

  const processCardPayment = async (amount: number) => {
    try {
      if (!cardReaderConnected.value) {
        throw new Error('Card reader not connected')
      }

      // Simulate card payment processing
      // In production, this would communicate with the actual card reader
      return new Promise((resolve, reject) => {
        setTimeout(() => {
          // Simulate successful payment
          resolve({
            success: true,
            transactionId: `TXN-${Date.now()}`,
            amount,
            cardType: 'Visa',
            lastFourDigits: '4242'
          })
        }, 3000)
      })
    } catch (error) {
      console.error('Card payment failed:', error)
      throw error
    }
  }

  /**
   * Receipt Printer Functions
   */
  const checkReceiptPrinter = async () => {
    try {
      if ('serial' in navigator) {
        const ports = await (navigator as any).serial.getPorts()
        receiptPrinterConnected.value = ports.some((port: any) => {
          const info = port.getInfo()
          return info.usbVendorId === 0x04B8 || // Epson
                 info.usbVendorId === 0x0519 || // Star Micronics
                 info.usbVendorId === 0x154F    // Bixolon
        })
      }
    } catch (error) {
      console.error('Receipt printer check failed:', error)
      receiptPrinterConnected.value = false
    }
  }

  const requestReceiptPrinter = async () => {
    try {
      if ('serial' in navigator) {
        const port = await (navigator as any).serial.requestPort({
          filters: [
            { usbVendorId: 0x04B8 }, // Epson
            { usbVendorId: 0x0519 }, // Star Micronics
            { usbVendorId: 0x154F }  // Bixolon
          ]
        })
        if (port) {
          await port.open({ baudRate: 9600 })
          receiptPrinterConnected.value = true
          return port
        }
      }
    } catch (error) {
      console.error('Failed to request receipt printer:', error)
    }
    return null
  }

  const printReceipt = async (receiptData: any) => {
    try {
      if (!receiptPrinterConnected.value) {
        // Fallback to browser print
        window.print()
        return
      }

      if ('serial' in navigator) {
        const ports = await (navigator as any).serial.getPorts()
        const printer = ports.find((port: any) => {
          const info = port.getInfo()
          return info.usbVendorId === 0x04B8 || 
                 info.usbVendorId === 0x0519 || 
                 info.usbVendorId === 0x154F
        })

        if (printer) {
          await printer.open({ baudRate: 9600 })
          
          // Generate ESC/POS commands
          const commands = generateESCPOSCommands(receiptData)
          
          const writer = printer.writable.getWriter()
          await writer.write(commands)
          writer.releaseLock()
          
          await printer.close()
        }
      }
    } catch (error) {
      console.error('Print receipt failed:', error)
      // Fallback to browser print
      window.print()
    }
  }

  const generateESCPOSCommands = (receiptData: any) => {
    const encoder = new TextEncoder()
    const ESC = 0x1B
    const GS = 0x1D
    
    let commands: number[] = []
    
    // Initialize printer
    commands.push(ESC, 0x40)
    
    // Set alignment to center
    commands.push(ESC, 0x61, 0x01)
    
    // Store name (bold)
    commands.push(ESC, 0x45, 0x01) // Bold on
    commands.push(...encoder.encode(receiptData.storeName + '\n'))
    commands.push(ESC, 0x45, 0x00) // Bold off
    
    // Store address
    commands.push(...encoder.encode(receiptData.storeAddress + '\n'))
    commands.push(...encoder.encode(receiptData.storePhone + '\n\n'))
    
    // Set alignment to left
    commands.push(ESC, 0x61, 0x00)
    
    // Receipt details
    commands.push(...encoder.encode(`Receipt #: ${receiptData.receiptNumber}\n`))
    commands.push(...encoder.encode(`Date: ${receiptData.date}\n`))
    commands.push(...encoder.encode(`Cashier: ${receiptData.cashier}\n`))
    commands.push(...encoder.encode(`Customer: ${receiptData.customer}\n\n`))
    
    // Separator
    commands.push(...encoder.encode('--------------------------------\n'))
    
    // Items
    receiptData.items.forEach((item: any) => {
      commands.push(...encoder.encode(`${item.name}\n`))
      commands.push(...encoder.encode(`${item.quantity} x R${item.price.toFixed(2)} = R${item.total.toFixed(2)}\n`))
    })
    
    // Separator
    commands.push(...encoder.encode('--------------------------------\n'))
    
    // Total (bold)
    commands.push(ESC, 0x45, 0x01) // Bold on
    commands.push(...encoder.encode(`TOTAL: R${receiptData.total.toFixed(2)}\n`))
    commands.push(ESC, 0x45, 0x00) // Bold off
    
    commands.push(...encoder.encode(`Payment: ${receiptData.paymentMethod}\n\n`))
    
    // Footer
    commands.push(ESC, 0x61, 0x01) // Center align
    commands.push(...encoder.encode('Thank you for your business!\n\n\n'))
    
    // Cut paper
    commands.push(GS, 0x56, 0x00)
    
    return new Uint8Array(commands)
  }

  /**
   * Cash Drawer Functions
   */
  const checkCashDrawer = async () => {
    try {
      if ('usb' in navigator) {
        const devices = await (navigator as any).usb.getDevices()
        cashDrawerConnected.value = devices.some((device: any) => 
          device.vendorId === 0x04B8 || // Epson (often connected to printer)
          device.vendorId === 0x0519    // Star Micronics
        )
      }
      
      // Cash drawer is often connected to receipt printer
      if (!cashDrawerConnected.value && receiptPrinterConnected.value) {
        cashDrawerConnected.value = true
      }
    } catch (error) {
      console.error('Cash drawer check failed:', error)
      cashDrawerConnected.value = false
    }
  }

  const openCashDrawer = async () => {
    try {
      if (!cashDrawerConnected.value) {
        throw new Error('Cash drawer not connected')
      }

      // Cash drawer is typically opened via printer port
      if ('serial' in navigator) {
        const ports = await (navigator as any).serial.getPorts()
        const printer = ports[0] // Usually first serial port
        
        if (printer) {
          await printer.open({ baudRate: 9600 })
          
          // ESC/POS command to open cash drawer
          const command = new Uint8Array([0x1B, 0x70, 0x00, 0x19, 0xFA])
          
          const writer = printer.writable.getWriter()
          await writer.write(command)
          writer.releaseLock()
          
          await printer.close()
        }
      }
    } catch (error) {
      console.error('Failed to open cash drawer:', error)
      throw error
    }
  }

  /**
   * Cleanup function
   */
  const cleanup = () => {
    if (barcodeTimeout) {
      clearTimeout(barcodeTimeout)
    }
    barcodeBuffer = ''
  }

  return {
    // Status
    barcodeScannerConnected,
    cardReaderConnected,
    receiptPrinterConnected,
    cashDrawerConnected,
    
    // Initialization
    initializeHardware,
    
    // Barcode Scanner
    checkBarcodeScanner,
    setupBarcodeListener,
    requestBarcodeScanner,
    
    // Card Reader
    checkCardReader,
    requestCardReader,
    processCardPayment,
    
    // Receipt Printer
    checkReceiptPrinter,
    requestReceiptPrinter,
    printReceipt,
    
    // Cash Drawer
    checkCashDrawer,
    openCashDrawer,
    
    // Cleanup
    cleanup
  }
}
