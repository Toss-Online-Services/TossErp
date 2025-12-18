/**
 * Stock/Inventory API Composable
 * Connects to TOSS backend /api/inventory endpoints
 */

// Type definitions
export interface ItemDto {
  id: number
  shopId: number
  name: string
  sku?: string
  barcode?: string
  description?: string
  categoryId?: number
  category?: string // Display name
  unitPrice: number
  sellingPrice?: number // Alias for unitPrice
  costPrice?: number
  reorderLevel?: number
  reorderQty?: number // Alias for reorderLevel
  quantityOnHand?: number
  unit?: string
  isActive: boolean
  createdAt?: string
  updatedAt?: string
}

export interface CreateItemRequest {
  shopId: number
  name: string
  sku?: string
  barcode?: string
  description?: string
  categoryId?: number
  unitPrice: number
  costPrice?: number
  reorderLevel?: number
  isActive: boolean
  id?: string | number // For updates
}

export interface UpdateItemRequest {
  id?: number | string
  name?: string
  sku?: string
  barcode?: string
  description?: string
  categoryId?: number
  unitPrice?: number
  costPrice?: number
  reorderLevel?: number
  isActive?: boolean
}

export interface WarehouseDto {
  id: number
  shopId: number
  name: string
  address?: string
  isActive: boolean
  createdAt?: string
  updatedAt?: string
}

export const useStock = () => {
  const { get, post } = useApi()

  /**
   * Get products with pagination and filters
   */
  const getProducts = async (params?: {
    shopId?: number
    searchTerm?: string
    category?: string
    pageNumber?: number
    pageSize?: number
  }) => {
    return await get<any>('/api/inventory/products', params)
  }

  /**
   * Get product by ID
   */
  const getProduct = async (id: number) => {
    return await get<any>(`/api/inventory/products/${id}`)
  }

  /**
   * Create a new product
   */
  const createProduct = async (productData: {
    shopId: number
    name: string
    sku?: string
    barcode?: string
    description?: string
    categoryId?: number
    unitPrice: number
    costPrice?: number
    reorderLevel?: number
    isActive: boolean
  }) => {
    return await post<{ id: number }>('/api/inventory/products', productData)
  }

  /**
   * Get current stock levels for a shop
   */
  const getStockLevels = async (params?: {
    shopId?: number
    productId?: number
    pageNumber?: number
    pageSize?: number
  }) => {
    return await get<any>('/api/inventory/stock-levels', params)
  }

  /**
   * Get low stock alerts for a shop
   */
  const getLowStockAlerts = async (params?: {
    shopId?: number
    pageNumber?: number
    pageSize?: number
  }) => {
    return await get<any>('/api/inventory/low-stock-alerts', params)
  }

  /**
   * Adjust stock manually (increase or decrease)
   */
  const adjustStock = async (adjustmentData: {
    productId: number
    shopId: number
    quantity: number
    movementType: string
    reference?: string
    notes?: string
  }) => {
    return await post<{ id: number }>('/api/inventory/stock/adjust', adjustmentData)
  }

  /**
   * Get stock movement history
   */
  const getStockMovementHistory = async (params?: {
    shopId?: number
    productId?: number
    startDate?: string
    endDate?: string
    movementType?: string
    pageNumber?: number
    pageSize?: number
  }) => {
    return await get<any>('/api/inventory/stock/movements', params)
  }

  /**
   * Get product categories
   */
  const getCategories = async (shopId?: number) => {
    return await get<any>('/api/inventory/categories', { shopId })
  }

  /**
   * Get product by SKU
   */
  const getProductBySku = async (sku: string, shopId?: number) => {
    return await get<any>('/api/inventory/products/by-sku', { sku, shopId })
  }

  /**
   * Get product by barcode
   */
  const getProductByBarcode = async (barcode: string, shopId?: number) => {
    return await get<any>('/api/inventory/products/by-barcode', { barcode, shopId })
  }

  return {
    // Products
    getProducts,
    getProduct,
    createProduct,
    getProductBySku,
    getProductByBarcode,
    
    // Stock Levels
    getStockLevels,
    getLowStockAlerts,
    
    // Stock Operations
    adjustStock,
    getStockMovementHistory,
    
    // Categories
    getCategories
  }
}
