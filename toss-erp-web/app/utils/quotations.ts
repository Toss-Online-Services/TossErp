import type { QuotationFormItemInput, QuotationTotals, QuotationItem } from '~/types/sales'

export function buildQuotationItemsPayload(
  items: QuotationFormItemInput[],
  products: Array<{ id: string; price: number; vatRate: number }>
): Array<{ productId: string; quantity: number; discountPercent: number }> {
  return items.map((item) => ({
    productId: item.productId,
    quantity: item.quantity,
    discountPercent: item.discountPercent || 0
  }))
}

export function calculateLineAmount(
  quantity: number,
  rate: number,
  discountPercent: number
): number {
  const gross = quantity * rate
  const discountValue = (gross * discountPercent) / 100
  return Number((gross - discountValue).toFixed(2))
}

export function calculateQuotationTotals(items: QuotationItem[]): QuotationTotals {
  const subtotal = items.reduce((sum, item) => sum + item.quantity * item.rate, 0)
  const discountAmount = items.reduce(
    (sum, item) => sum + (item.quantity * item.rate * item.discountPercent) / 100,
    0
  )
  const taxableAmount = subtotal - discountAmount
  const taxAmount = items.reduce(
    (sum, item) =>
      sum +
      ((item.quantity * item.rate - (item.quantity * item.rate * item.discountPercent) / 100) *
        item.vatRate) /
        100,
    0
  )
  const grandTotal = taxableAmount + taxAmount

  return {
    subtotal: Number(subtotal.toFixed(2)),
    discountAmount: Number(discountAmount.toFixed(2)),
    taxableAmount: Number(taxableAmount.toFixed(2)),
    taxAmount: Number(taxAmount.toFixed(2)),
    grandTotal: Number(grandTotal.toFixed(2))
  }
}

