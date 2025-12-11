import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useInventoryApi } from '~/composables/useInventoryApi'

export interface Item {
  id: string
  code: string
  name: string
  description?: string
  category: string
  unit: string
  costPrice: number
  sellingPrice: number
  currentStock: number
  minStock: number
  maxStock?: number
  warehouse: string
  barcode?: string
  supplier?: string
  imageUrl?: string
  isActive: boolean
  createdAt: Date
  updatedAt: Date
}

export interface StockMovement {
  id: string
  itemId: string
  itemName: string
  type: 'purchase' | 'sale' | 'adjustment' | 'transfer' | 'return'
  quantity: number
  warehouse: string
  referenceType?: string
  referenceId?: string
  supplierId?: string
  supplierName?: string
  notes?: string
  createdBy: string
  createdAt: Date
}

export interface Warehouse {
  id: string
  name: string
  code: string
  address?: string
  isActive: boolean
}

export const useStockStore = defineStore('stock', () => {
  // State
  const items = ref<Item[]>([])
  const movements = ref<StockMovement[]>([])
  const warehouses = ref<Warehouse[]>([])
  const loading = ref(false)
  const selectedWarehouse = ref<string>('main')

  // Computed
  const lowStockItems = computed(() => {
    return items.value.filter(item => item.currentStock <= item.minStock && item.isActive)
  })

  const outOfStockItems = computed(() => {
    return items.value.filter(item => item.currentStock === 0 && item.isActive)
  })

  const totalStockValue = computed(() => {
    return items.value.reduce((total, item) => {
      return total + (item.currentStock * item.costPrice)
    }, 0)
  })

  const itemsByCategory = computed(() => {
    const categories: Record<string, Item[]> = {}
    items.value.forEach(item => {
      if (!categories[item.category]) {
        categories[item.category] = []
      }
      categories[item.category].push(item)
    })
    return categories
  })

  // Actions
  async function fetchItems(shopId?: number) {
    loading.value = true
    try {
      const inventoryApi = useInventoryApi()
      const { data, error } = await inventoryApi.getProducts(shopId)
      
      if (error) {
        console.error('Failed to fetch items:', error)
        // Fallback to empty array on error
        items.value = []
        return
      }
      
      if (data?.value?.items) {
        // Map backend data to frontend format
        items.value = data.value.items.map((item: any) => ({
          id: item.id?.toString() || item.productId?.toString() || '',
          code: item.code || item.sku || '',
          name: item.name || '',
          description: item.description,
          category: item.categoryName || item.category || '',
          unit: item.unit || 'unit',
          costPrice: item.costPrice || item.purchasePrice || 0,
          sellingPrice: item.sellingPrice || item.price || 0,
          currentStock: item.stockOnHand || item.quantity || 0,
          minStock: item.minStock || item.reorderLevel || 0,
          maxStock: item.maxStock,
          warehouse: item.warehouseName || item.warehouse || 'main',
          barcode: item.barcode,
          supplier: item.supplierName,
          imageUrl: item.imageUrl,
          isActive: item.isActive !== false,
          createdAt: item.created ? new Date(item.created) : new Date(),
          updatedAt: item.lastModified ? new Date(item.lastModified) : new Date()
        }))
      } else {
        items.value = []
      }
    } catch (error) {
      console.error('Failed to fetch items:', error)
      items.value = []
    } finally {
      loading.value = false
    }
  }

  async function fetchMovements(itemId?: string, shopId?: number) {
    loading.value = true
    try {
      const inventoryApi = useInventoryApi()
      const productId = itemId ? parseInt(itemId) : undefined
      const { data, error } = await inventoryApi.getStockMovementHistory(shopId, productId)
      
      if (error) {
        console.error('Failed to fetch movements:', error)
        movements.value = []
        return
      }
      
      if (data?.value?.items) {
        movements.value = data.value.items.map((mov: any) => ({
          id: mov.id?.toString() || '',
          itemId: mov.productId?.toString() || mov.itemId?.toString() || '',
          itemName: mov.productName || mov.itemName || '',
          type: mov.movementType?.toLowerCase() || mov.type || 'adjustment',
          quantity: mov.quantityChange || mov.quantity || 0,
          warehouse: mov.warehouseName || mov.warehouse || 'main',
          referenceType: mov.referenceType,
          referenceId: mov.referenceId?.toString(),
          supplierId: mov.supplierId?.toString(),
          supplierName: mov.supplierName,
          notes: mov.notes,
          createdBy: mov.createdBy || 'system',
          createdAt: mov.created ? new Date(mov.created) : new Date()
        }))
      } else {
        movements.value = []
      }
    } catch (error) {
      console.error('Failed to fetch movements:', error)
      movements.value = []
    } finally {
      loading.value = false
    }
  }

  async function createItem(item: Omit<Item, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true
    try {
      const inventoryApi = useInventoryApi()
      const { data, error } = await inventoryApi.createProduct({
        name: item.name,
        code: item.code,
        sku: item.code,
        description: item.description,
        categoryId: undefined, // TODO: Map category name to ID
        unit: item.unit,
        costPrice: item.costPrice,
        sellingPrice: item.sellingPrice,
        minStock: item.minStock,
        maxStock: item.maxStock,
        barcode: item.barcode,
        isActive: item.isActive
      })
      
      if (error || !data?.value) {
        throw error || new Error('Failed to create item')
      }
      
      // Refresh items list
      await fetchItems()
      
      const newItem = items.value.find(i => i.id === data.value.id.toString())
      return newItem
    } catch (error) {
      console.error('Failed to create item:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function updateItem(id: string, updates: Partial<Item>) {
    loading.value = true
    try {
      const inventoryApi = useInventoryApi()
      const productId = parseInt(id)
      const { data, error } = await inventoryApi.updateProduct(productId, {
        name: updates.name,
        code: updates.code,
        sku: updates.code,
        description: updates.description,
        unit: updates.unit,
        costPrice: updates.costPrice,
        sellingPrice: updates.sellingPrice,
        minStock: updates.minStock,
        maxStock: updates.maxStock,
        barcode: updates.barcode,
        isActive: updates.isActive
      })
      
      if (error) {
        throw error
      }
      
      // Refresh items list
      await fetchItems()
    } catch (error) {
      console.error('Failed to update item:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function adjustStock(itemId: string, quantity: number, notes?: string, shopId?: number) {
    loading.value = true
    try {
      const inventoryApi = useInventoryApi()
      const productId = parseInt(itemId)
      const { data, error } = await inventoryApi.adjustStock(productId, shopId || 1, quantity, notes)
      
      if (error) {
        throw error
      }
      
      // Refresh items and movements
      await fetchItems(shopId)
      await fetchMovements(itemId, shopId)
    } catch (error) {
      console.error('Failed to adjust stock:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  function getItemById(id: string): Item | undefined {
    return items.value.find(item => item.id === id)
  }

  async function searchItems(query: string, shopId?: number): Promise<Item[]> {
    try {
      const inventoryApi = useInventoryApi()
      const { data, error } = await inventoryApi.searchProducts(query, shopId)
      
      if (error || !data?.value) {
        // Fallback to local search
        const lowerQuery = query.toLowerCase()
        return items.value.filter(item => 
          item.name.toLowerCase().includes(lowerQuery) ||
          item.code.toLowerCase().includes(lowerQuery) ||
          item.barcode?.includes(query)
        )
      }
      
      // Map backend results to frontend format
      return data.value.items.map((item: any) => ({
        id: item.id?.toString() || '',
        code: item.code || item.sku || '',
        name: item.name || '',
        description: item.description,
        category: item.categoryName || item.category || '',
        unit: item.unit || 'unit',
        costPrice: item.costPrice || 0,
        sellingPrice: item.sellingPrice || 0,
        currentStock: item.stockOnHand || 0,
        minStock: item.minStock || 0,
        maxStock: item.maxStock,
        warehouse: item.warehouseName || 'main',
        barcode: item.barcode,
        supplier: item.supplierName,
        imageUrl: item.imageUrl,
        isActive: item.isActive !== false,
        createdAt: item.created ? new Date(item.created) : new Date(),
        updatedAt: item.lastModified ? new Date(item.lastModified) : new Date()
      }))
    } catch (error) {
      console.error('Failed to search items:', error)
      // Fallback to local search
      const lowerQuery = query.toLowerCase()
      return items.value.filter(item => 
        item.name.toLowerCase().includes(lowerQuery) ||
        item.code.toLowerCase().includes(lowerQuery) ||
        item.barcode?.includes(query)
      )
    }
  }

  return {
    // State
    items,
    movements,
    warehouses,
    loading,
    selectedWarehouse,
    // Computed
    lowStockItems,
    outOfStockItems,
    totalStockValue,
    itemsByCategory,
    // Actions
    fetchItems,
    fetchMovements,
    createItem,
    updateItem,
    adjustStock,
    getItemById,
    searchItems
  }
})

