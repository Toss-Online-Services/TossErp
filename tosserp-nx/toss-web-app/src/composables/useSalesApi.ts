import { ref } from 'vue'
import type { PaginatedResponse } from '../types/api'

export type PaymentType = 'Cash' | 'Card' | 'Mobile' | 'OnAccount'
export type SaleStatus = 'Pending' | 'Completed' | 'Voided' | 'Refunded'

export interface SaleItemDto {
  productId: number
  quantity: number
  unitPrice: number
}

export interface PosCheckoutRequest {
  shopId: number
  customerId?: number
  paymentMethod: PaymentType
  paymentReference?: string
  notes?: string
  items: SaleItemDto[]
  idempotencyKey?: string
}

export interface PosCheckoutResponse {
  saleId: number
  saleNumber: string
  total: number
  receiptId?: number
  receiptNumber?: string
  isNewSale: boolean
}

export interface SaleDto {
  id: number
  saleNumber: string
  saleDate: string
  status: SaleStatus
  total: number
  paymentMethod: PaymentType
}

export const useSalesApi = () => {
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
    // TODO: Get from auth store/composable
    const token = localStorage.getItem('auth_token')
    return {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    }
  }

  const posCheckout = async (request: PosCheckoutRequest): Promise<PosCheckoutResponse> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<PosCheckoutResponse>(
        `${getApiBaseUrl()}/api/sales/pos/checkout`,
        {
          method: 'POST',
          headers: getAuthHeaders(),
          body: request
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to process checkout'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getSales = async (params: {
    shopId: number
    fromDate?: string
    toDate?: string
    status?: SaleStatus
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<SaleDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      queryParams.append('shopId', params.shopId.toString())
      if (params.fromDate) queryParams.append('fromDate', params.fromDate)
      if (params.toDate) queryParams.append('toDate', params.toDate)
      if (params.status) queryParams.append('status', params.status)
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<SaleDto>>(
        `${getApiBaseUrl()}/api/sales?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch sales'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getSaleById = async (id: number): Promise<SaleDto> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<SaleDto>(
        `${getApiBaseUrl()}/api/sales/${id}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch sale'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const voidSale = async (id: number): Promise<void> => {
    isLoading.value = true
    error.value = null

    try {
      await $fetch(`${getApiBaseUrl()}/api/sales/${id}/void`, {
        method: 'POST',
        headers: getAuthHeaders()
      })
    } catch (err: any) {
      error.value = err.message || 'Failed to void sale'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    posCheckout,
    getSales,
    getSaleById,
    voidSale
  }
}

