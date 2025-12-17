// VAT Management composable for TOSS ERP III (South African VAT - 15%)
import { ref } from 'vue'

// Types
export interface VATLineItem {
  count: number
  subtotal: number
  vatAmount: number
  total: number
}

export interface VATReport {
  startDate: Date
  endDate: Date
  sales: {
    standard: VATLineItem    // 15% VAT
    zeroRated: VATLineItem   // 0% VAT
    exempt: VATLineItem      // No VAT
    total: VATLineItem
  }
  purchases: {
    standard: VATLineItem    // 15% VAT
    zeroRated: VATLineItem   // 0% VAT
    total: VATLineItem
  }
  netVAT: number            // Output VAT - Input VAT
  totalPayable: number      // Amount to pay to SARS (if positive)
}

export const useVAT = () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // South African VAT rate
  const VAT_RATE = 0.15 // 15%

  // Calculate VAT amount
  const calculateVAT = (amount: number, rate: number = VAT_RATE): number => {
    return amount * rate
  }

  // Calculate amount excluding VAT
  const calculateExcludingVAT = (totalAmount: number, rate: number = VAT_RATE): number => {
    return totalAmount / (1 + rate)
  }

  // Mock VAT Report
  const getVATReport = async (startDate: Date, endDate: Date): Promise<VATReport> => {
    isLoading.value = true
    error.value = null

    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 500))

      // Mock sales data
      const salesStandardSubtotal = 850000
      const salesStandardVAT = calculateVAT(salesStandardSubtotal)
      
      const salesZeroRatedSubtotal = 125000
      const salesZeroRatedVAT = 0
      
      const salesExemptSubtotal = 25000
      const salesExemptVAT = 0

      // Mock purchase data
      const purchasesStandardSubtotal = 450000
      const purchasesStandardVAT = calculateVAT(purchasesStandardSubtotal)
      
      const purchasesZeroRatedSubtotal = 75000
      const purchasesZeroRatedVAT = 0

      const totalOutputVAT = salesStandardVAT
      const totalInputVAT = purchasesStandardVAT
      const netVAT = totalOutputVAT - totalInputVAT

      return {
        startDate,
        endDate,
        sales: {
          standard: {
            count: 245,
            subtotal: salesStandardSubtotal,
            vatAmount: salesStandardVAT,
            total: salesStandardSubtotal + salesStandardVAT
          },
          zeroRated: {
            count: 45,
            subtotal: salesZeroRatedSubtotal,
            vatAmount: salesZeroRatedVAT,
            total: salesZeroRatedSubtotal
          },
          exempt: {
            count: 12,
            subtotal: salesExemptSubtotal,
            vatAmount: salesExemptVAT,
            total: salesExemptSubtotal
          },
          total: {
            count: 302,
            subtotal: salesStandardSubtotal + salesZeroRatedSubtotal + salesExemptSubtotal,
            vatAmount: totalOutputVAT,
            total: salesStandardSubtotal + salesStandardVAT + salesZeroRatedSubtotal + salesExemptSubtotal
          }
        },
        purchases: {
          standard: {
            count: 128,
            subtotal: purchasesStandardSubtotal,
            vatAmount: purchasesStandardVAT,
            total: purchasesStandardSubtotal + purchasesStandardVAT
          },
          zeroRated: {
            count: 23,
            subtotal: purchasesZeroRatedSubtotal,
            vatAmount: purchasesZeroRatedVAT,
            total: purchasesZeroRatedSubtotal
          },
          total: {
            count: 151,
            subtotal: purchasesStandardSubtotal + purchasesZeroRatedSubtotal,
            vatAmount: totalInputVAT,
            total: purchasesStandardSubtotal + purchasesStandardVAT + purchasesZeroRatedSubtotal
          }
        },
        netVAT,
        totalPayable: Math.max(0, netVAT) // Only payable if positive
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load VAT report'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    VAT_RATE,
    calculateVAT,
    calculateExcludingVAT,
    getVATReport
  }
}
