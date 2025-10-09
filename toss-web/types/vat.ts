// South African VAT Types and Calculations

export const SA_VAT_RATE = 0.15 // 15% standard VAT rate in South Africa

export enum VATType {
  STANDARD = 'standard', // 15%
  ZERO_RATED = 'zero_rated', // 0% (exports, basic foods)
  EXEMPT = 'exempt', // No VAT (financial services, residential rent)
}

export interface VATCalculation {
  subtotal: number
  vatAmount: number
  total: number
  vatRate: number
  vatType: VATType
}

export interface VATReturn {
  id: string
  period: {
    startDate: Date
    endDate: Date
  }
  status: 'draft' | 'submitted' | 'approved'
  outputVAT: number // VAT on sales
  inputVAT: number // VAT on purchases
  netVAT: number // Output VAT - Input VAT
  adjustments: VATAdjustment[]
  totalPayable: number
  submittedAt?: Date
  approvedAt?: Date
}

export interface VATAdjustment {
  id: string
  description: string
  amount: number
  type: 'debit' | 'credit'
  reason: string
}

export interface VATTransaction {
  id: string
  date: Date
  reference: string
  description: string
  type: 'sale' | 'purchase'
  subtotal: number
  vatAmount: number
  total: number
  vatRate: number
  vatType: VATType
  accountId: string
}

export interface VATReport {
  period: {
    startDate: Date
    endDate: Date
  }
  sales: {
    standard: VATSummary
    zeroRated: VATSummary
    exempt: VATSummary
    total: VATSummary
  }
  purchases: {
    standard: VATSummary
    zeroRated: VATSummary
    exempt: VATSummary
    total: VATSummary
  }
  netVAT: number
  adjustments: VATAdjustment[]
  totalPayable: number
}

export interface VATSummary {
  count: number
  subtotal: number
  vatAmount: number
  total: number
}

/**
 * Calculate VAT amount from subtotal
 */
export function calculateVATFromSubtotal(subtotal: number, vatType: VATType = VATType.STANDARD): VATCalculation {
  const vatRate = vatType === VATType.STANDARD ? SA_VAT_RATE : 0
  const vatAmount = subtotal * vatRate
  const total = subtotal + vatAmount

  return {
    subtotal,
    vatAmount,
    total,
    vatRate,
    vatType,
  }
}

/**
 * Calculate VAT-inclusive amount (extract VAT from total)
 */
export function extractVATFromTotal(total: number, vatType: VATType = VATType.STANDARD): VATCalculation {
  const vatRate = vatType === VATType.STANDARD ? SA_VAT_RATE : 0
  const subtotal = total / (1 + vatRate)
  const vatAmount = total - subtotal

  return {
    subtotal,
    vatAmount,
    total,
    vatRate,
    vatType,
  }
}

/**
 * Format VAT amount for display
 */
export function formatVAT(amount: number): string {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
  }).format(amount)
}

/**
 * Get VAT rate as percentage string
 */
export function getVATRateString(vatType: VATType): string {
  switch (vatType) {
    case VATType.STANDARD:
      return '15%'
    case VATType.ZERO_RATED:
      return '0%'
    case VATType.EXEMPT:
      return 'Exempt'
    default:
      return '0%'
  }
}

