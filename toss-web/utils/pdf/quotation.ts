import type { QuotationRecord } from '~/types/sales'
import { formatCurrencyValue } from '~/utils/quotations'

const formatDate = (value: string) =>
  new Date(value).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })

export const generateQuotationPdf = async (quotation: QuotationRecord) => {
  if (import.meta.server) {
    return
  }

  const [{ default: jsPDF }, autoTableModule] = await Promise.all([
    import('jspdf'),
    import('jspdf-autotable')
  ])
  const autoTable = (autoTableModule as any).default || autoTableModule

  const doc = new jsPDF()
  const marginLeft = 14

  doc.setFontSize(18)
  doc.text('Quotation', marginLeft, 18)
  doc.setFontSize(12)
  doc.text(`Quotation #: ${quotation.quotationNumber}`, marginLeft, 28)
  doc.text(`Date: ${formatDate(quotation.quotationDate)}`, marginLeft, 34)
  doc.text(`Valid Until: ${formatDate(quotation.validUntil)}`, marginLeft, 40)

  doc.text('Customer', marginLeft, 52)
  doc.text(quotation.customer.businessName, marginLeft, 58)
  doc.text(quotation.customer.address, marginLeft, 64)
  doc.text(quotation.customer.phone, marginLeft, 70)
  if (quotation.customer.email) {
    doc.text(quotation.customer.email, marginLeft, 76)
  }

  autoTable(doc, {
    startY: 90,
    head: [['#', 'Product', 'Qty', 'Rate', 'Disc %', 'Amount']],
    body: quotation.items.map((item, index) => [
      index + 1,
      item.productName,
      item.quantity,
      formatCurrencyValue(item.rate),
      item.discountPercent ? `${item.discountPercent}%` : '—',
      formatCurrencyValue(item.amount)
    ]),
    theme: 'striped',
    headStyles: { fillColor: [30, 64, 175], textColor: 255 },
    styles: { fontSize: 10, halign: 'right' },
    columnStyles: {
      1: { halign: 'left' }
    },
    margin: { left: marginLeft, right: marginLeft },
    didDrawPage: (data: any) => {
      doc.setFontSize(9)
      doc.text(
        `Generated ${new Date().toLocaleString('en-ZA')}`,
        data.settings.margin.left,
        doc.internal.pageSize.height - 10
      )
    }
  })

  const summaryStart = (doc as any).lastAutoTable.finalY + 10
  doc.setFontSize(12)
  doc.text('Summary', marginLeft, summaryStart)
  doc.setFontSize(10)
  const summaryLines = [
    `Subtotal: ${formatCurrencyValue(quotation.subtotal)}`,
    `Discount: -${formatCurrencyValue(quotation.discountAmount)}`,
    `Taxable: ${formatCurrencyValue(quotation.taxableAmount)}`,
    `VAT (15%): ${formatCurrencyValue(quotation.taxAmount)}`,
    `Grand Total: ${formatCurrencyValue(quotation.grandTotal)}`
  ]

  summaryLines.forEach((line, idx) => {
    const y = summaryStart + 8 + idx * 6
    doc.text(line, marginLeft, y)
  })

  doc.save(`${quotation.quotationNumber}.pdf`)
}
