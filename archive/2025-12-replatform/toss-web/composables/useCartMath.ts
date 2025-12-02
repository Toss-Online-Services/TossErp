/**
 * Cart Mathematics Composable
 * Pure calculation functions for POS cart operations including:
 * - Line-level discounts and taxes
 * - Subtotal, discount total, tax total, grand total
 * - Change calculation
 * - ZAR currency formatting
 */

export interface CartLine {
  productId: string | number
  name: string
  quantity: number
  unitPrice: number
  discountPercent?: number
  discountAmount?: number
  taxRate?: number
}

export interface CartTotals {
  subtotal: number
  discountTotal: number
  taxTotal: number
  grandTotal: number
}

export interface CartPaymentResult {
  paid: number
  change: number
  insufficient: boolean
  shortfall: number
}

export const useCartMath = () => {
  /**
   * Calculate line total before discount
   */
  const calculateLineSubtotal = (line: CartLine): number => {
    if (line.quantity < 0 || line.unitPrice < 0) {
      return 0
    }
    return line.quantity * line.unitPrice
  }

  /**
   * Calculate line discount amount
   * Supports both percentage and fixed amount discounts
   */
  const calculateLineDiscount = (line: CartLine): number => {
    const subtotal = calculateLineSubtotal(line)
    
    if (line.discountAmount !== undefined && line.discountAmount > 0) {
      // Fixed amount discount (capped at subtotal)
      return Math.min(line.discountAmount, subtotal)
    }
    
    if (line.discountPercent !== undefined && line.discountPercent > 0) {
      // Percentage discount (capped at 100%)
      const percent = Math.min(line.discountPercent, 100)
      return (subtotal * percent) / 100
    }
    
    return 0
  }

  /**
   * Calculate line total after discount (taxable amount)
   */
  const calculateLineTaxableAmount = (line: CartLine): number => {
    const subtotal = calculateLineSubtotal(line)
    const discount = calculateLineDiscount(line)
    return Math.max(subtotal - discount, 0)
  }

  /**
   * Calculate line tax amount
   * Applied to discounted amount
   */
  const calculateLineTax = (line: CartLine): number => {
    const taxableAmount = calculateLineTaxableAmount(line)
    const taxRate = line.taxRate || 0
    
    if (taxRate <= 0) {
      return 0
    }
    
    return (taxableAmount * taxRate) / 100
  }

  /**
   * Calculate line grand total (after discount, with tax)
   */
  const calculateLineTotal = (line: CartLine): number => {
    const taxableAmount = calculateLineTaxableAmount(line)
    const tax = calculateLineTax(line)
    return taxableAmount + tax
  }

  /**
   * Calculate cart-level totals from all lines
   */
  const calculateCartTotals = (lines: CartLine[]): CartTotals => {
    const subtotal = lines.reduce((sum, line) => sum + calculateLineSubtotal(line), 0)
    const discountTotal = lines.reduce((sum, line) => sum + calculateLineDiscount(line), 0)
    const taxTotal = lines.reduce((sum, line) => sum + calculateLineTax(line), 0)
    const grandTotal = lines.reduce((sum, line) => sum + calculateLineTotal(line), 0)

    return {
      subtotal: roundToZAR(subtotal),
      discountTotal: roundToZAR(discountTotal),
      taxTotal: roundToZAR(taxTotal),
      grandTotal: roundToZAR(grandTotal)
    }
  }

  /**
   * Calculate payment change
   */
  const calculateChange = (grandTotal: number, paid: number): CartPaymentResult => {
    const totalDue = roundToZAR(grandTotal)
    const amountPaid = roundToZAR(paid)
    
    if (amountPaid >= totalDue) {
      return {
        paid: amountPaid,
        change: roundToZAR(amountPaid - totalDue),
        insufficient: false,
        shortfall: 0
      }
    }
    
    return {
      paid: amountPaid,
      change: 0,
      insufficient: true,
      shortfall: roundToZAR(totalDue - amountPaid)
    }
  }

  /**
   * Round to ZAR (2 decimal places)
   */
  const roundToZAR = (amount: number): number => {
    return Math.round(amount * 100) / 100
  }

  /**
   * Format currency for ZAR
   * Uses existing useMoney for consistency
   */
  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('en-ZA', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(amount)
  }

  /**
   * Apply standard VAT rate (15% for ZA)
   */
  const applyStandardVAT = (lines: CartLine[]): CartLine[] => {
    return lines.map(line => ({
      ...line,
      taxRate: line.taxRate ?? 15
    }))
  }

  return {
    calculateLineSubtotal,
    calculateLineDiscount,
    calculateLineTaxableAmount,
    calculateLineTax,
    calculateLineTotal,
    calculateCartTotals,
    calculateChange,
    roundToZAR,
    formatCurrency,
    applyStandardVAT
  }
}
