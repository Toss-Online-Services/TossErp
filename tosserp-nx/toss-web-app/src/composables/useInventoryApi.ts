import { ref } from 'vue'
import type { PaginatedResponse } from '~/types/api'

export interface ProductDto {
  id: number
  sku: string
  barcode?: string
  name: string
  categoryId?: number
  categoryName?: string
  basePrice: number
  isActive: boolean
  availableStock: number
}

export interface StockAlertDto {
  id: number
  productId: number
  productName: string
  productSKU: string
  currentStock: number
  minimumStock: number
  reorderQuantity?: number
  createdDate: string
}

export interface StockOnHandDto {
  productId: number
  productName: string
  productSKU: string
  shopId: number
  shopName: string
  currentStock: number
  reservedStock: number
  availableStock: number
  averageCost: number
  lastStockDate: string
}

export interface AdjustStockRequest {
  productId: number
  shopId: number
  quantity: number
  reason: string
  notes?: string
}

export interface TransferStockRequest {
  fromShopId: number
  toShopId: number
  productId: number
  quantity: number
  notes?: string
}

export const useInventoryApi = () => {
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

  const getProducts = async (params: {
    shopId?: number
    searchTerm?: string
    categoryId?: number
    isActive?: boolean
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<ProductDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.shopId) queryParams.append('shopId', params.shopId.toString())
      if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm)
      if (params.categoryId) queryParams.append('categoryId', params.categoryId.toString())
      if (params.isActive !== undefined) queryParams.append('isActive', params.isActive.toString())
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<ProductDto>>(
        `${getApiBaseUrl()}/api/inventory/products?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch products'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getLowStockAlerts = async (shopId: number, onlyUnacknowledged = true): Promise<StockAlertDto[]> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<StockAlertDto[]>(
        `${getApiBaseUrl()}/api/inventory/low-stock-alerts?shopId=${shopId}&onlyUnacknowledged=${onlyUnacknowledged}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch low stock alerts'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getStockOnHand = async (params: {
    productId?: number
    shopId?: number
  }): Promise<StockOnHandDto[]> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.productId) queryParams.append('productId', params.productId.toString())
      if (params.shopId) queryParams.append('shopId', params.shopId.toString())

      const response = await $fetch<StockOnHandDto[]>(
        `${getApiBaseUrl()}/api/inventory/stock/soh?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch stock on hand'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const adjustStock = async (request: AdjustStockRequest): Promise<{ id: number }> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<{ id: number }>(
        `${getApiBaseUrl()}/api/inventory/stock/adjust`,
        {
          method: 'POST',
          headers: getAuthHeaders(),
          body: request
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to adjust stock'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const transferStock = async (request: TransferStockRequest): Promise<{ id: number }> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<{ id: number }>(
        `${getApiBaseUrl()}/api/inventory/stock/transfer`,
        {
          method: 'POST',
          headers: getAuthHeaders(),
          body: request
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to transfer stock'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const acknowledgeStockAlert = async (alertId: number): Promise<void> => {
    isLoading.value = true
    error.value = null

    try {
      await $fetch(
        `${getApiBaseUrl()}/api/inventory/stock-alerts/${alertId}/acknowledge`,
        {
          method: 'POST',
          headers: getAuthHeaders()
        }
      )
    } catch (err: any) {
      error.value = err.message || 'Failed to acknowledge alert'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getProducts,
    getLowStockAlerts,
    getStockOnHand,
    adjustStock,
    transferStock,
    acknowledgeStockAlert
  }
}

