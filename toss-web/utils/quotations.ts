import type { QuotationFormItemInput, QuotationTotals, QuotationUpsertPayload } from '~/types/sales'

const DEFAULT_VAT_RATE = 15

const round = (value: number) => Number(value.toFixed(2))

export const formatCurrencyValue = (value: number, currency = 'R'): string => {
  const formatted = value.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
  return currency ? `${currency}${formatted}` : formatted
}

export const calculateLineAmount = (
  item: QuotationFormItemInput
): number => {
  const gross = (item.quantity || 0) * (item.rate || 0)
  const discountValue = (gross * (item.discountPercent || 0)) / 100
  return round(gross - discountValue)
}

export const calculateQuotationTotals = (
  items: QuotationFormItemInput[],
  vatRateDefault = DEFAULT_VAT_RATE
): QuotationTotals => {
  return items.reduce<QuotationTotals>(
    (acc, item) => {
      const gross = (item.quantity || 0) * (item.rate || 0)
      const discountValue = (gross * (item.discountPercent || 0)) / 100
      const net = gross - discountValue
      const vatRate = item.vatRate ?? vatRateDefault
      const vat = (net * vatRate) / 100

      return {
        subtotal: round(acc.subtotal + gross),
        discount: round(acc.discount + discountValue),
        taxableAmount: round(acc.taxableAmount + net),
        tax: round(acc.tax + vat),
        grandTotal: round(acc.grandTotal + net + vat)
      }
    },
    {
      subtotal: 0,
      discount: 0,
      taxableAmount: 0,
      tax: 0,
      grandTotal: 0
    }
  )
}

export const buildQuotationItemsPayload = (
  items: QuotationFormItemInput[]
): QuotationUpsertPayload['items'] =>
  items
    .filter((item) => item.productId && item.quantity > 0)
    .map((item) => ({
      productId: item.productId,
      quantity: item.quantity,
      discountPercent: item.discountPercent || 0
    }))

export const refreshLineAmounts = (
  items: QuotationFormItemInput[]
): QuotationFormItemInput[] =>
  items.map((item) => ({
    ...item,
    amount: calculateLineAmount(item)
  }))

export { DEFAULT_VAT_RATE }
