export const useProductsAPI = () => {
  const config = useRuntimeConfig()
  const baseURL = (config.public.apiBase || 'https://localhost:5001') + '/api'
  console.log('🌐 useProductsAPI initialized with baseURL:', baseURL)

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
      const params = { 
        ShopId: shopId,
        PageNumber: 1,
        PageSize: 1000,
        IsActive: true
      }
      console.log('🔍 useProductsAPI.getProducts() - Fetching with params:', params)
      console.log('🔍 useProductsAPI.getProducts() - Request URL:', `${baseURL}/Inventory/products`)
      
      try {
        const response: any = await $fetch(`${baseURL}/Inventory/products`, {
          method: 'GET',
          params
        })
        // Backend may return Items (PascalCase) or items (camelCase) or value
        const items = response?.items || response?.Items || response?.value || response?.Value || []
        console.log('✅ useProductsAPI.getProducts() - Items count:', Array.isArray(items) ? items.length : 0)
        // Normalize to array
        return Array.isArray(items) ? items : []
      } catch (error) {
        console.error('❌ useProductsAPI.getProducts() - Error:', error)
        throw error
      }
    },

    /**
     * Get all product categories
     */
  async getCategories(shopId: number) {
      console.log('🔍 useProductsAPI.getCategories() - Fetching for shopId:', shopId)
      console.log('🔍 useProductsAPI.getCategories() - Request URL:', `${baseURL}/Inventory/categories`)
      
      try {
        const response: any = await $fetch(`${baseURL}/Inventory/categories`, {
          method: 'GET',
          params: { shopId }
        })
        // Some backends wrap the array in { value: [...] }
        const list = response?.value || response?.items || response?.Items || response || []
        const categories = Array.isArray(list) ? list : []
        console.log('✅ useProductsAPI.getCategories() - Categories count:', categories.length)
        return categories
      } catch (error) {
        console.error('❌ useProductsAPI.getCategories() - Error:', error)
        throw error
      }
    }
  }
}

