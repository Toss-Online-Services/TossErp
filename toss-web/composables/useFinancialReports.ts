import { ref } from 'vue'
import type { BalanceSheet, ProfitAndLoss, CashFlow, TrialBalance, FinancialRatio } from '~/types/accounting'

export const useFinancialReports = () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const getBalanceSheet = async (date: Date): Promise<BalanceSheet | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<BalanceSheet>('/api/accounting/reports/balance-sheet', {
        method: 'GET',
        params: { date: date.toISOString() },
      })

      return response
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch balance sheet'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getProfitAndLoss = async (startDate: Date, endDate: Date): Promise<ProfitAndLoss | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<ProfitAndLoss>('/api/accounting/reports/profit-loss', {
        method: 'GET',
        params: {
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString(),
        },
      })

      return response
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch profit and loss statement'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getCashFlow = async (startDate: Date, endDate: Date): Promise<CashFlow | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<CashFlow>('/api/accounting/reports/cash-flow', {
        method: 'GET',
        params: {
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString(),
        },
      })

      return response
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch cash flow statement'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getTrialBalance = async (date: Date): Promise<TrialBalance | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<TrialBalance>('/api/accounting/reports/trial-balance', {
        method: 'GET',
        params: { date: date.toISOString() },
      })

      return response
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch trial balance'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const getFinancialRatios = async (date: Date): Promise<FinancialRatio[] | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<FinancialRatio[]>('/api/accounting/reports/financial-ratios', {
        method: 'GET',
        params: { date: date.toISOString() },
      })

      return response
    } catch (e: any) {
      error.value = e.message || 'Failed to fetch financial ratios'
      return null
    } finally {
      isLoading.value = false
    }
  }

  const exportReport = async (
    reportType: 'balance-sheet' | 'profit-loss' | 'cash-flow' | 'trial-balance',
    format: 'pdf' | 'excel' | 'csv',
    params: Record<string, any>
  ): Promise<Blob | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<Blob>(`/api/accounting/reports/${reportType}/export`, {
        method: 'POST',
        body: { format, ...params },
        responseType: 'blob',
      })

      return response
    } catch (e: any) {
      error.value = e.message || 'Failed to export report'
      return null
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getBalanceSheet,
    getProfitAndLoss,
    getCashFlow,
    getTrialBalance,
    getFinancialRatios,
    exportReport,
  }
}

