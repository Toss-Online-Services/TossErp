import { ref } from 'vue'
import type { VATReturn, VATReport, VATTransaction } from '~/types/vat'
import { calculateVATFromSubtotal, extractVATFromTotal, VATType } from '~/types/vat'

export const useVAT = () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const calculateVAT = (subtotal: number, vatType: VATType = VATType.STANDARD) => {
    return calculateVATFromSubtotal(subtotal, vatType)
  }

  const extractVAT = (total: number, vatType: VATType = VATType.STANDARD) => {
    return extractVATFromTotal(total, vatType)
  }

  const getVATReturn = async (startDate: Date, endDate: Date): Promise<VATReturn | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<VATReturn>('/api/accounting/vat/return', {
        method: 'GET',
        params: {
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString(),
        },
      })

      return response
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch VAT return'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getVATReport = async (startDate: Date, endDate: Date): Promise<VATReport | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<VATReport>('/api/accounting/vat/report', {
        method: 'GET',
        params: {
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString(),
        },
      })

      return response
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch VAT report'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getVATTransactions = async (
    startDate: Date,
    endDate: Date,
    type?: 'sale' | 'purchase'
  ): Promise<VATTransaction[] | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<VATTransaction[]>('/api/accounting/vat/transactions', {
        method: 'GET',
        params: {
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString(),
          type,
        },
      })

      return response
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch VAT transactions'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const submitVATReturn = async (vatReturnId: string): Promise<boolean> => {
    isLoading.value = true
    error.value = null

    try {
      await $fetch('/api/accounting/vat/return/submit', {
        method: 'POST',
        body: { vatReturnId },
      })

      return true
    } catch (e: any) {
      error.value = e.message || 'Failed to submit VAT return'
      return false
    } finally {
      isLoading.value = false
    }
  }

  const exportVATReport = async (
    startDate: Date,
    endDate: Date,
    format: 'pdf' | 'excel' | 'csv'
  ): Promise<Blob | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<Blob>('/api/accounting/vat/export', {
        method: 'POST',
        body: {
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString(),
          format,
        },
        responseType: 'blob',
      })

      return response
    } catch (e: any) {
      error.value = e.message || 'Failed to export VAT report'
      return null
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    calculateVAT,
    extractVAT,
    getVATReturn,
    getVATReport,
    getVATTransactions,
    submitVATReturn,
    exportVATReport,
  }
}

