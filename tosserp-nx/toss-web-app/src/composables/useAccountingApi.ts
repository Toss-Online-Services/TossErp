import { ref } from 'vue'
import type { PaginatedResponse } from '~/types/api'

export interface AccountDto {
  id: number
  code: string
  name: string
  accountType: string
  balance: number
}

export interface CashbookEntryDto {
  id: number
  accountId: number
  accountName: string
  entryDate: string
  type: string
  amount: number
  sourceType?: string
  sourceId?: number
  description?: string
  reference?: string
}

export interface ProfitAndLossDto {
  fromDate: string
  toDate: string
  totalRevenue: number
  totalExpenses: number
  grossProfit: number
  netProfit: number
  revenueItems: RevenueItemDto[]
  expenseItems: ExpenseItemDto[]
}

export interface RevenueItemDto {
  source: string
  amount: number
  count: number
}

export interface ExpenseItemDto {
  category: string
  amount: number
  count: number
}

export interface CashflowSummaryDto {
  fromDate: string
  toDate: string
  openingBalance: number
  totalCashIn: number
  totalCashOut: number
  closingBalance: number
  cashInItems: CashflowItemDto[]
  cashOutItems: CashflowItemDto[]
}

export interface CashflowItemDto {
  source: string
  amount: number
  count: number
  lastEntryDate?: string
}

export interface DebtorDto {
  customerId: number
  customerName: string
  customerEmail?: string
  customerPhone?: string
  totalOutstanding: number
  invoiceCount: number
  oldestInvoiceDate?: string
  latestDueDate?: string
  outstandingInvoices: OutstandingInvoiceDto[]
}

export interface OutstandingInvoiceDto {
  invoiceId: number
  invoiceNumber: string
  invoiceDate: string
  dueDate?: string
  amount: number
  daysOverdue: number
  isOverdue: boolean
}

export interface CreditorDto {
  vendorId: number
  vendorName: string
  totalOutstanding: number
  invoiceCount: number
  oldestInvoiceDate?: string
  latestDueDate?: string
}

export const useAccountingApi = () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const getApiBaseUrl = () => {
    if (typeof window !== 'undefined') {
      const config = useRuntimeConfig()
      return config.public.apiBase || 'http://localhost:5000'
    }
    return 'http://localhost:5000'
  }

  const getAuthHeaders = () => {
    const token = localStorage.getItem('auth_token')
    return {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    }
  }

  const getAccounts = async (shopId?: number): Promise<AccountDto[]> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (shopId) queryParams.append('shopId', shopId.toString())

      const response = await $fetch<AccountDto[]>(
        `${getApiBaseUrl()}/api/accounting/accounts?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch accounts'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getCashbookEntries = async (params: {
    accountId?: number
    fromDate?: string
    toDate?: string
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<CashbookEntryDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.accountId) queryParams.append('accountId', params.accountId.toString())
      if (params.fromDate) queryParams.append('fromDate', params.fromDate)
      if (params.toDate) queryParams.append('toDate', params.toDate)
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<CashbookEntryDto>>(
        `${getApiBaseUrl()}/api/accounting/cashbook/entries?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch cashbook entries'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getProfitAndLoss = async (params: {
    fromDate: string
    toDate: string
    shopId?: number
  }): Promise<ProfitAndLossDto> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      queryParams.append('fromDate', params.fromDate)
      queryParams.append('toDate', params.toDate)
      if (params.shopId) queryParams.append('shopId', params.shopId.toString())

      const response = await $fetch<ProfitAndLossDto>(
        `${getApiBaseUrl()}/api/reports/pnl?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch profit and loss'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getCashflowSummary = async (params: {
    fromDate: string
    toDate: string
    accountId?: number
  }): Promise<CashflowSummaryDto> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      queryParams.append('fromDate', params.fromDate)
      queryParams.append('toDate', params.toDate)
      if (params.accountId) queryParams.append('accountId', params.accountId.toString())

      const response = await $fetch<CashflowSummaryDto>(
        `${getApiBaseUrl()}/api/reports/cashflow?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch cashflow summary'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getDebtors = async (params: {
    overdueOnly?: boolean
    customerId?: number
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<DebtorDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.overdueOnly) queryParams.append('overdueOnly', 'true')
      if (params.customerId) queryParams.append('customerId', params.customerId.toString())
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<DebtorDto>>(
        `${getApiBaseUrl()}/api/reports/debtors?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch debtors'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getCreditors = async (params: {
    overdueOnly?: boolean
    vendorId?: number
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<CreditorDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.overdueOnly) queryParams.append('overdueOnly', 'true')
      if (params.vendorId) queryParams.append('vendorId', params.vendorId.toString())
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<CreditorDto>>(
        `${getApiBaseUrl()}/api/reports/creditors?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch creditors'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getAccounts,
    getCashbookEntries,
    getProfitAndLoss,
    getCashflowSummary,
    getDebtors,
    getCreditors
  }
}

