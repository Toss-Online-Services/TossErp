import { defineStore } from 'pinia'

export const useInventoryStore = defineStore('inventory', () => {
  const { 
    getProducts, 
    getStockLevels, 
    getLowStockAlerts,
    adjustStock,
    getStockMovementHistory,
    createProduct 
  } = useStock()

  // State
  const products = ref<any[]>([])
  const stockLevels = ref<any[]>([])
  const lowStockAlerts = ref<any[]>([])
  const stockMovements = ref<any[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Computed
  const productCount = computed(() => products.value.length)
  const lowStockCount = computed(() => lowStockAlerts.value.length)
  const totalStockValue = computed(() => {
    return stockLevels.value.reduce((sum, level) => sum + (level.totalValue || 0), 0)
  })

  // Actions
  const fetchProducts = async (params?: any) => {
    loading.value = true
    error.value = null
    try {
      const response = await getProducts(params)
      products.value = response.items || response || []
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch products'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchStockLevels = async (shopId: number) => {
    loading.value = true
    error.value = null
    try {
      const response = await getStockLevels({ shopId })
      stockLevels.value = response.items || response || []
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch stock levels'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchLowStockAlerts = async (shopId: number) => {
    loading.value = true
    error.value = null
    try {
      const response = await getLowStockAlerts({ shopId })
      lowStockAlerts.value = response.items || response || []
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch low stock alerts'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchStockMovements = async (params?: any) => {
    loading.value = true
    error.value = null
    try {
      const response = await getStockMovementHistory(params)
      stockMovements.value = response.items || response || []
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch stock movements'
      throw err
    } finally {
      loading.value = false
    }
  }

  const addProduct = async (productData: any) => {
    loading.value = true
    error.value = null
    try {
      const result = await createProduct(productData)
      await fetchProducts({ shopId: productData.shopId })
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to create product'
      throw err
    } finally {
      loading.value = false
    }
  }

  const performStockAdjustment = async (adjustmentData: any) => {
    loading.value = true
    error.value = null
    try {
      const result = await adjustStock(adjustmentData)
      await fetchStockLevels(adjustmentData.shopId)
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to adjust stock'
      throw err
    } finally {
      loading.value = false
    }
  }

  const clearError = () => {
    error.value = null
  }

  return {
    // State
    products,
    stockLevels,
    lowStockAlerts,
    stockMovements,
    loading,
    error,
    
    // Computed
    productCount,
    lowStockCount,
    totalStockValue,
    
    // Actions
    fetchProducts,
    fetchStockLevels,
    fetchLowStockAlerts,
    fetchStockMovements,
    addProduct,
    performStockAdjustment,
    clearError
  }
})

