export const useProductsAPI = () => {
  const config = useRuntimeConfig()
  const baseURL = (config.public.apiBase || 'https://localhost:5001') + '/api'

  return {
    /**
     * Search products with filters
     */
    async searchProducts(params: {
      shopId: number
      searchTerm?: string
      categoryId?: number
      inStock?: boolean
      lowStock?: boolean
      minPrice?: number
      maxPrice?: number
      pageNumber?: number
      pageSize?: number
    }) {
      return await $fetch<{
        products: Array<{
          id: number
          name: string
          sku: string
          barcode: string | null
          basePrice: number
          imageUrl: string | null
          categoryId: number
          categoryName: string
          availableStock: number
          isActive: boolean
          isTaxable: boolean
        }>
        totalCount: number
        totalPages: number
        currentPage: number
      }>(`${baseURL}/Inventory/products/search`, {
        method: 'POST',
        body: params
      })
    },

    /**
     * Get low stock items
     */
    async getLowStockItems(shopId: number, threshold: number = 10) {
      return await $fetch<Array<{
        productId: number
        productName: string
        sku: string
        currentStock: number
        reorderPoint: number
        lastPurchasePrice: number
        preferredVendor: string
        vendorId: number
      }>>(`${baseURL}/Inventory/low-stock-items`, {
        method: 'GET',
        params: { shopId, threshold }
      })
    },

    /**
     * Get product by barcode
     */
    async getProductByBarcode(barcode: string, shopId: number) {
      return await $fetch<{
        id: number
        name: string
        sku: string
        barcode: string
        basePrice: number
        categoryId: number
        categoryName: string
        availableStock: number
        unit: string
        isTaxable: boolean
      }>(`${baseURL}/Inventory/products/by-barcode`, {
        method: 'GET',
        params: { barcode, shopId }
      })
    },

    /**
     * Get all products
     */
    async getProducts(shopId?: number) {
      return await $fetch<Array<{
        id: number
        name: string
        sku: string
        basePrice: number
        categoryId: number
        isActive: boolean
      }>>(`${baseURL}/Inventory/products`, {
        method: 'GET',
        params: { shopId }
      })
    }
  }
}

