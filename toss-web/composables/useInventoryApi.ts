import { useApi, useAuthApi } from './useApi'
import type { Item, StockMovement } from '~/stores/stock'

export function useInventoryApi() {
  const { getHeaders } = useAuthApi()
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase || 'http://localhost:5000/api'

  async function getProducts(shopId?: number, categoryId?: number, pageNumber = 1, pageSize = 50) {
    const params = new URLSearchParams()
    if (shopId) params.append('shopId', shopId.toString())
    if (categoryId) params.append('categoryId', categoryId.toString())
    params.append('pageNumber', pageNumber.toString())
    params.append('pageSize', pageSize.toString())
    
    const url = `/inventory/products?${params.toString()}`
    const { data, error, execute } = useApi<{ items: Item[], totalCount: number, pageNumber: number, totalPages: number }>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getProductById(id: number) {
    const { data, error, execute } = useApi<Item>(`/inventory/products/${id}`, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getProductBySku(sku: string, shopId: number) {
    const { data, error, execute } = useApi<Item>(`/inventory/products/by-sku?sku=${sku}&shopId=${shopId}`, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getProductByBarcode(barcode: string, shopId: number) {
    const { data, error, execute } = useApi<Item>(`/inventory/products/by-barcode?barcode=${barcode}&shopId=${shopId}`, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function searchProducts(searchTerm: string, shopId?: number, categoryId?: number, pageNumber = 1, pageSize = 50) {
    const { data, error, execute } = useApi<{ items: Item[], totalCount: number }>('/inventory/products/search', {
      method: 'POST',
      body: { searchTerm, shopId, categoryId, pageNumber, pageSize },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getLowStockItems(shopId: number, threshold = 10) {
    const { data, error, execute } = useApi<Item[]>(`/inventory/low-stock-items?shopId=${shopId}&threshold=${threshold}`, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getLowStockAlerts(shopId?: number) {
    const url = shopId ? `/inventory/low-stock-alerts?shopId=${shopId}` : '/inventory/low-stock-alerts'
    const { data, error, execute } = useApi<any[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getCategories() {
    const { data, error, execute } = useApi<any[]>('/inventory/categories', {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getStockLevels(shopId?: number, productId?: number) {
    const params = new URLSearchParams()
    if (shopId) params.append('shopId', shopId.toString())
    if (productId) params.append('productId', productId.toString())
    
    const url = `/inventory/stock-levels?${params.toString()}`
    const { data, error, execute } = useApi<any[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getStockMovementHistory(shopId?: number, productId?: number, pageNumber = 1, pageSize = 50) {
    const params = new URLSearchParams()
    if (shopId) params.append('shopId', shopId.toString())
    if (productId) params.append('productId', productId.toString())
    params.append('pageNumber', pageNumber.toString())
    params.append('pageSize', pageSize.toString())
    
    const url = `/inventory/stock/movements?${params.toString()}`
    const { data, error, execute } = useApi<{ items: StockMovement[], totalCount: number }>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function createProduct(product: Partial<Item>) {
    const { data, error, execute } = useApi<{ id: number }>('/inventory/products', {
      method: 'POST',
      body: product,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function updateProduct(id: number, updates: Partial<Item>) {
    const { data, error, execute } = useApi<Item>(`/inventory/products/${id}`, {
      method: 'PUT',
      body: updates,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function deleteProduct(id: number) {
    const { data, error, execute } = useApi(`/inventory/products/${id}`, {
      method: 'DELETE',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function adjustStock(productId: number, shopId: number, quantityChange: number, notes?: string) {
    const { data, error, execute } = useApi('/inventory/stock/adjust', {
      method: 'POST',
      body: { productId, shopId, quantityChange, notes },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  return {
    getProducts,
    getProductById,
    getProductBySku,
    getProductByBarcode,
    searchProducts,
    getLowStockItems,
    getLowStockAlerts,
    getCategories,
    getStockLevels,
    getStockMovementHistory,
    createProduct,
    updateProduct,
    deleteProduct,
    adjustStock
  }
}



