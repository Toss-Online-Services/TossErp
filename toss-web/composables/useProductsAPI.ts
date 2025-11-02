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
        const response = await $fetch<{
          items: Array<{
            id: number
            name: string
            sku: string
            barcode?: string
            categoryName?: string
            basePrice: number
            categoryId: number
            isActive: boolean
            availableStock: number
          }>
          pageNumber: number
          totalPages: number
          totalCount: number
        }>(`${baseURL}/Inventory/products`, {
          method: 'GET',
          params
        })
        console.log('✅ useProductsAPI.getProducts() - Response received:', response)
        console.log('✅ useProductsAPI.getProducts() - Items count:', response.items?.length || 0)
        // Return just the items array for backwards compatibility
        return response.items
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
        const response = await $fetch<Array<{
          id: number
          name: string
          description?: string
          parentCategoryId?: number
          productCount: number
        }>>(`${baseURL}/Inventory/categories`, {
          method: 'GET',
          params: { shopId }
        })
        console.log('✅ useProductsAPI.getCategories() - Response:', response)
        console.log('✅ useProductsAPI.getCategories() - Categories count:', response?.length || 0)
        return response
      } catch (error) {
        console.error('❌ useProductsAPI.getCategories() - Error:', error)
        throw error
      }
    }
  }
}

