import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

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
  async function fetchItems() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      items.value = [
        {
          id: '1',
          code: 'CEM-001',
          name: 'Cement 50kg',
          category: 'Building Materials',
          unit: 'bag',
          costPrice: 85,
          sellingPrice: 100,
          currentStock: 45,
          minStock: 20,
          maxStock: 200,
          warehouse: 'main',
          barcode: '1234567890123',
          supplier: 'PPC Cement',
          isActive: true,
          createdAt: new Date(),
          updatedAt: new Date()
        },
        {
          id: '2',
          code: 'SUG-001',
          name: 'Sugar 2.5kg',
          category: 'Groceries',
          unit: 'pack',
          costPrice: 28,
          sellingPrice: 35,
          currentStock: 15,
          minStock: 30,
          maxStock: 100,
          warehouse: 'main',
          barcode: '2345678901234',
          supplier: 'Tongaat Hulett',
          isActive: true,
          createdAt: new Date(),
          updatedAt: new Date()
        },
        {
          id: '3',
          code: 'OIL-001',
          name: 'Cooking Oil 750ml',
          category: 'Groceries',
          unit: 'bottle',
          costPrice: 22,
          sellingPrice: 28,
          currentStock: 8,
          minStock: 20,
          maxStock: 80,
          warehouse: 'main',
          barcode: '3456789012345',
          supplier: 'Willowton Oil',
          isActive: true,
          createdAt: new Date(),
          updatedAt: new Date()
        }
      ]
    } catch (error) {
      console.error('Failed to fetch items:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchMovements(itemId?: string) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      movements.value = [
        {
          id: '1',
          itemId: '1',
          itemName: 'Cement 50kg',
          type: 'purchase',
          quantity: 50,
          warehouse: 'main',
          referenceType: 'PO',
          referenceId: 'PO-001',
          notes: 'Received from PPC',
          createdBy: 'admin',
          createdAt: new Date()
        }
      ]
    } catch (error) {
      console.error('Failed to fetch movements:', error)
    } finally {
      loading.value = false
    }
  }

  async function createItem(item: Omit<Item, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const newItem: Item = {
        ...item,
        id: `item_${Date.now()}`,
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      items.value.push(newItem)
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
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const index = items.value.findIndex(item => item.id === id)
      if (index !== -1) {
        items.value[index] = {
          ...items.value[index],
          ...updates,
          updatedAt: new Date()
        }
      }
    } catch (error) {
      console.error('Failed to update item:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function adjustStock(itemId: string, quantity: number, notes?: string) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const item = items.value.find(i => i.id === itemId)
      if (item) {
        item.currentStock += quantity
        
        const movement: StockMovement = {
          id: `mov_${Date.now()}`,
          itemId,
          itemName: item.name,
          type: 'adjustment',
          quantity,
          warehouse: item.warehouse,
          notes,
          createdBy: 'admin',
          createdAt: new Date()
        }
        
        movements.value.unshift(movement)
      }
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

  function searchItems(query: string): Item[] {
    const lowerQuery = query.toLowerCase()
    return items.value.filter(item => 
      item.name.toLowerCase().includes(lowerQuery) ||
      item.code.toLowerCase().includes(lowerQuery) ||
      item.barcode?.includes(query)
    )
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

