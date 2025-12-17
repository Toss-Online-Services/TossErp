/**
 * Receipt Generation Composable
 * Handles receipt creation, printing, and sharing
 */

import { ref } from 'vue'

export interface ReceiptData {
  receiptNumber: string
  shopName: string
  shopAddress?: string
  shopPhone?: string
  shopEmail?: string
  vatNumber?: string
  items: ReceiptItem[]
  subtotal: number
  tax?: number
  discount?: number
  total: number
  paymentMethod: string
  cashReceived?: number
  changeGiven?: number
  date: Date
  cashierName?: string
  customerName?: string
  notes?: string
}

export interface ReceiptItem {
  name: string
  quantity: number
  unitPrice: number
  total: number
  sku?: string
}

export const useReceipt = () => {
  const isGenerating = ref(false)
  const error = ref<string | null>(null)

  /**
   * Generate receipt HTML for display or printing
   */
  const generateHTML = (data: ReceiptData): string => {
    const { apiBase } = useRuntimeConfig().public
    
    return `
<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Receipt #${data.receiptNumber}</title>
  <style>
    @media print {
      body { margin: 0; padding: 0; }
      @page { margin: 0mm; }
    }
    
    * { margin: 0; padding: 0; box-sizing: border-box; }
    
    body {
      font-family: 'Courier New', monospace;
      font-size: 12px;
      line-height: 1.4;
      padding: 10mm;
      max-width: 80mm;
      margin: 0 auto;
    }
    
    .receipt {
      background: white;
      padding: 5mm;
    }
    
    .header {
      text-align: center;
      border-bottom: 2px dashed #000;
      padding-bottom: 5mm;
      margin-bottom: 5mm;
    }
    
    .shop-name {
      font-size: 16px;
      font-weight: bold;
      margin-bottom: 2mm;
    }
    
    .shop-details {
      font-size: 10px;
      color: #333;
    }
    
    .receipt-info {
      margin-bottom: 5mm;
      padding-bottom: 3mm;
      border-bottom: 1px solid #ddd;
    }
    
    .receipt-info div {
      display: flex;
      justify-content: space-between;
      margin-bottom: 1mm;
    }
    
    .items-table {
      width: 100%;
      margin-bottom: 5mm;
      border-bottom: 1px solid #ddd;
      padding-bottom: 3mm;
    }
    
    .item-row {
      display: flex;
      justify-content: space-between;
      margin-bottom: 2mm;
    }
    
    .item-name {
      flex: 1;
    }
    
    .item-qty {
      width: 40px;
      text-align: center;
    }
    
    .item-price {
      width: 60px;
      text-align: right;
    }
    
    .totals {
      margin-top: 3mm;
      padding-top: 3mm;
      border-top: 2px solid #000;
    }
    
    .total-row {
      display: flex;
      justify-content: space-between;
      margin-bottom: 2mm;
    }
    
    .total-row.grand-total {
      font-size: 14px;
      font-weight: bold;
      margin-top: 2mm;
      padding-top: 2mm;
      border-top: 1px dashed #000;
    }
    
    .payment-info {
      margin-top: 5mm;
      padding-top: 3mm;
      border-top: 1px dashed #000;
    }
    
    .footer {
      margin-top: 5mm;
      padding-top: 3mm;
      border-top: 2px dashed #000;
      text-align: center;
      font-size: 10px;
    }
    
    .thank-you {
      font-weight: bold;
      margin-bottom: 2mm;
    }
    
    .barcode {
      margin: 3mm auto;
      text-align: center;
    }
  </style>
</head>
<body>
  <div class="receipt">
    <!-- Header -->
    <div class="header">
      <div class="shop-name">${data.shopName}</div>
      ${data.shopAddress ? `<div class="shop-details">${data.shopAddress}</div>` : ''}
      ${data.shopPhone ? `<div class="shop-details">Tel: ${data.shopPhone}</div>` : ''}
      ${data.shopEmail ? `<div class="shop-details">${data.shopEmail}</div>` : ''}
      ${data.vatNumber ? `<div class="shop-details">VAT: ${data.vatNumber}</div>` : ''}
    </div>
    
    <!-- Receipt Info -->
    <div class="receipt-info">
      <div><span>Receipt #:</span><span>${data.receiptNumber}</span></div>
      <div><span>Date:</span><span>${formatDate(data.date)}</span></div>
      <div><span>Time:</span><span>${formatTime(data.date)}</span></div>
      ${data.cashierName ? `<div><span>Cashier:</span><span>${data.cashierName}</span></div>` : ''}
      ${data.customerName ? `<div><span>Customer:</span><span>${data.customerName}</span></div>` : ''}
    </div>
    
    <!-- Items -->
    <div class="items-table">
      ${data.items.map(item => `
        <div class="item-row">
          <div class="item-name">${item.name}</div>
          <div class="item-qty">x${item.quantity}</div>
          <div class="item-price">R${item.total.toFixed(2)}</div>
        </div>
        ${item.unitPrice !== item.total / item.quantity ? `
        <div class="item-row" style="font-size: 10px; color: #666;">
          <div class="item-name" style="padding-left: 5mm;">@ R${item.unitPrice.toFixed(2)} each</div>
          <div class="item-qty"></div>
          <div class="item-price"></div>
        </div>
        ` : ''}
      `).join('')}
    </div>
    
    <!-- Totals -->
    <div class="totals">
      <div class="total-row">
        <span>Subtotal:</span>
        <span>R${data.subtotal.toFixed(2)}</span>
      </div>
      ${data.discount && data.discount > 0 ? `
      <div class="total-row">
        <span>Discount:</span>
        <span>-R${data.discount.toFixed(2)}</span>
      </div>
      ` : ''}
      ${data.tax && data.tax > 0 ? `
      <div class="total-row">
        <span>VAT (15%):</span>
        <span>R${data.tax.toFixed(2)}</span>
      </div>
      ` : ''}
      <div class="total-row grand-total">
        <span>TOTAL:</span>
        <span>R${data.total.toFixed(2)}</span>
      </div>
    </div>
    
    <!-- Payment Info -->
    <div class="payment-info">
      <div class="total-row">
        <span>Payment Method:</span>
        <span>${data.paymentMethod}</span>
      </div>
      ${data.cashReceived ? `
      <div class="total-row">
        <span>Cash Received:</span>
        <span>R${data.cashReceived.toFixed(2)}</span>
      </div>
      ` : ''}
      ${data.changeGiven ? `
      <div class="total-row">
        <span>Change:</span>
        <span>R${data.changeGiven.toFixed(2)}</span>
      </div>
      ` : ''}
    </div>
    
    ${data.notes ? `
    <div class="payment-info">
      <div style="font-size: 10px; font-style: italic;">${data.notes}</div>
    </div>
    ` : ''}
    
    <!-- Footer -->
    <div class="footer">
      <div class="thank-you">THANK YOU FOR YOUR BUSINESS!</div>
      <div>Powered by TOSS</div>
      <div>Township One-Stop Solution</div>
      ${data.shopPhone ? `<div>Support: ${data.shopPhone}</div>` : ''}
    </div>
  </div>
</body>
</html>
    `
  }

  /**
   * Generate plain text receipt (for thermal printers)
   */
  const generatePlainText = (data: ReceiptData): string => {
    const width = 32 // Character width for thermal printers

    const center = (text: string) => {
      const padding = Math.max(0, Math.floor((width - text.length) / 2))
      return ' '.repeat(padding) + text
    }

    const line = (char = '=') => char.repeat(width)

    let receipt = ''
    
    // Header
    receipt += center(data.shopName) + '\n'
    if (data.shopAddress) receipt += center(data.shopAddress) + '\n'
    if (data.shopPhone) receipt += center(`Tel: ${data.shopPhone}`) + '\n'
    if (data.vatNumber) receipt += center(`VAT: ${data.vatNumber}`) + '\n'
    receipt += line('-') + '\n'
    
    // Receipt info
    receipt += `Receipt #: ${data.receiptNumber}\n`
    receipt += `Date: ${formatDate(data.date)}\n`
    receipt += `Time: ${formatTime(data.date)}\n`
    if (data.cashierName) receipt += `Cashier: ${data.cashierName}\n`
    receipt += line('-') + '\n'
    
    // Items
    data.items.forEach(item => {
      receipt += `${item.name}\n`
      receipt += `  ${item.quantity} x R${item.unitPrice.toFixed(2)}`.padEnd(width - 8)
      receipt += `R${item.total.toFixed(2)}`.padStart(8) + '\n'
    })
    receipt += line('-') + '\n'
    
    // Totals
    receipt += `${'Subtotal:'.padEnd(width - 10)}R${data.subtotal.toFixed(2)}`.padStart(10) + '\n'
    if (data.discount && data.discount > 0) {
      receipt += `${'Discount:'.padEnd(width - 10)}-R${data.discount.toFixed(2)}`.padStart(10) + '\n'
    }
    if (data.tax && data.tax > 0) {
      receipt += `${'VAT (15%):'.padEnd(width - 10)}R${data.tax.toFixed(2)}`.padStart(10) + '\n'
    }
    receipt += line('=') + '\n'
    receipt += `${'TOTAL:'.padEnd(width - 10)}R${data.total.toFixed(2)}`.padStart(10) + '\n'
    receipt += line('=') + '\n'
    
    // Payment
    receipt += `Payment: ${data.paymentMethod}\n`
    if (data.cashReceived) {
      receipt += `Cash: R${data.cashReceived.toFixed(2)}\n`
      receipt += `Change: R${(data.changeGiven || 0).toFixed(2)}\n`
    }
    receipt += line('-') + '\n'
    
    // Footer
    receipt += '\n' + center('THANK YOU!') + '\n'
    receipt += center('Powered by TOSS') + '\n'
    if (data.shopPhone) receipt += center(data.shopPhone) + '\n'
    receipt += '\n'

    return receipt
  }

  /**
   * Print receipt
   */
  const print = async (data: ReceiptData): Promise<boolean> => {
    isGenerating.value = true
    error.value = null

    try {
      // Generate HTML
      const html = generateHTML(data)
      
      // Open print dialog
      const printWindow = window.open('', '_blank')
      if (!printWindow) {
        throw new Error('Could not open print window. Please allow popups.')
      }

      printWindow.document.write(html)
      printWindow.document.close()
      
      // Wait for content to load
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Print
      printWindow.print()
      
      // Close after printing (optional)
      setTimeout(() => {
        printWindow.close()
      }, 1000)

      return true
    } catch (err: any) {
      error.value = err.message
      console.error('Print error:', err)
      return false
    } finally {
      isGenerating.value = false
    }
  }

  /**
   * Generate PDF receipt
   */
  const generatePDF = async (data: ReceiptData): Promise<Blob | null> => {
    try {
      // This would use a library like jsPDF
      // For now, return null and use print instead
      console.warn('PDF generation not yet implemented')
      return null
    } catch (err: any) {
      error.value = err.message
      return null
    }
  }

  /**
   * Share receipt via WhatsApp
   */
  const shareViaWhatsApp = async (data: ReceiptData, phoneNumber: string): Promise<boolean> => {
    try {
      const text = generateWhatsAppMessage(data)
      const encodedText = encodeURIComponent(text)
      const url = `https://wa.me/${phoneNumber.replace(/\D/g, '')}?text=${encodedText}`
      
      window.open(url, '_blank')
      return true
    } catch (err: any) {
      error.value = err.message
      return false
    }
  }

  /**
   * Share receipt via Email
   */
  const shareViaEmail = async (data: ReceiptData, email: string): Promise<boolean> => {
    try {
      const subject = `Receipt #${data.receiptNumber} - ${data.shopName}`
      const body = generateEmailBody(data)
      
      const mailtoLink = `mailto:${email}?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`
      window.location.href = mailtoLink
      
      return true
    } catch (err: any) {
      error.value = err.message
      return false
    }
  }

  /**
   * Download receipt as text file
   */
  const downloadAsText = (data: ReceiptData): void => {
    const text = generatePlainText(data)
    const blob = new Blob([text], { type: 'text/plain' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `receipt-${data.receiptNumber}.txt`
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
    URL.revokeObjectURL(url)
  }

  /**
   * Generate WhatsApp message
   */
  const generateWhatsAppMessage = (data: ReceiptData): string => {
    let message = `*${data.shopName}*\n`
    message += `Receipt #${data.receiptNumber}\n`
    message += `Date: ${formatDate(data.date)}\n`
    message += `━━━━━━━━━━━━━━━━\n\n`

    data.items.forEach(item => {
      message += `${item.name}\n`
      message += `${item.quantity} x R${item.unitPrice.toFixed(2)} = R${item.total.toFixed(2)}\n\n`
    })

    message += `━━━━━━━━━━━━━━━━\n`
    message += `Subtotal: R${data.subtotal.toFixed(2)}\n`
    if (data.tax) message += `VAT: R${data.tax.toFixed(2)}\n`
    if (data.discount) message += `Discount: -R${data.discount.toFixed(2)}\n`
    message += `*TOTAL: R${data.total.toFixed(2)}*\n`
    message += `━━━━━━━━━━━━━━━━\n\n`
    message += `Payment: ${data.paymentMethod}\n\n`
    message += `Thank you for your business! 🙏\n`
    message += `_Powered by TOSS_`

    return message
  }

  /**
   * Generate email body
   */
  const generateEmailBody = (data: ReceiptData): string => {
    let body = `Dear ${data.customerName || 'Valued Customer'},\n\n`
    body += `Thank you for your purchase at ${data.shopName}!\n\n`
    body += `Receipt #: ${data.receiptNumber}\n`
    body += `Date: ${formatDate(data.date)}\n`
    body += `Time: ${formatTime(data.date)}\n\n`
    body += `ITEMS PURCHASED:\n`
    body += `${'='.repeat(50)}\n`

    data.items.forEach(item => {
      body += `${item.name} - ${item.quantity} x R${item.unitPrice.toFixed(2)} = R${item.total.toFixed(2)}\n`
    })

    body += `${'='.repeat(50)}\n`
    body += `Subtotal: R${data.subtotal.toFixed(2)}\n`
    if (data.tax) body += `VAT (15%): R${data.tax.toFixed(2)}\n`
    if (data.discount) body += `Discount: -R${data.discount.toFixed(2)}\n`
    body += `TOTAL: R${data.total.toFixed(2)}\n\n`
    body += `Payment Method: ${data.paymentMethod}\n\n`
    
    if (data.notes) {
      body += `Notes: ${data.notes}\n\n`
    }
    
    body += `Thank you for your business!\n\n`
    body += `${data.shopName}\n`
    if (data.shopAddress) body += `${data.shopAddress}\n`
    if (data.shopPhone) body += `Tel: ${data.shopPhone}\n`
    body += `\nPowered by TOSS - Township One-Stop Solution`

    return body
  }

  // Helper functions
  const formatDate = (date: Date): string => {
    return new Date(date).toLocaleDateString('en-ZA', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    })
  }

  const formatTime = (date: Date): string => {
    return new Date(date).toLocaleTimeString('en-ZA', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    })
  }

  return {
    // State
    isGenerating,
    error,

    // Methods
    generateHTML,
    generatePlainText,
    print,
    generatePDF,
    shareViaWhatsApp,
    shareViaEmail,
    downloadAsText
  }
}

