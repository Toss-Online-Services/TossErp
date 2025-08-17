// Stock Management API Service
import { apiGet, apiPost, apiPut, apiDelete, API_ENDPOINTS, USE_MOCK_DATA, mockDelay } from '~/utils/api'

// Types
export interface StockItem {
  id: string
  name: string
  description: string
  category: string
  sku: string
  quantity: number
  unitPrice: number
  totalValue: number
  status: 'in-stock' | 'low-stock' | 'out-of-stock'
  minStock: number
  createdAt: string
  updatedAt: string
}

export interface StockCategory {
  id: string
  name: string
  description: string
  itemCount: number
}

export interface StockAdjustment {
  id: string
  stockItemId: string
  type: 'increase' | 'decrease' | 'correction'
  quantity: number
  reason: string
  createdBy: string
  createdAt: string
}

export interface StockOverview {
  totalStockValue: number
  totalItems: number
  lowStockItems: number
  totalCategories: number
  recentAdjustments: StockAdjustment[]
}

export interface CreateStockItemRequest {
  name: string
  description: string
  category: string
  sku: string
  quantity: number
  unitPrice: number
  minStock: number
}

export interface UpdateStockItemRequest extends Partial<CreateStockItemRequest> {
  id: string
}

export interface StockAdjustmentRequest {
  stockItemId: string
  type: 'increase' | 'decrease' | 'correction'
  quantity: number
  reason: string
}

// Mock data for development
const mockStockItems: StockItem[] = [
  {
    id: '1',
    name: 'Samsung Galaxy S21',
    description: 'Latest Android smartphone',
    category: 'Electronics',
    sku: 'ELE-SAM-001',
    quantity: 25,
    unitPrice: 12999.00,
    totalValue: 324975.00,
    status: 'in-stock',
    minStock: 10,
    createdAt: '2024-01-15T10:00:00Z',
    updatedAt: '2024-01-15T10:00:00Z'
  },
  {
    id: '2',
    name: 'Nike Air Force 1',
    description: 'Classic white sneakers',
    category: 'Clothing',
    sku: 'CLO-NIK-001',
    quantity: 5,
    unitPrice: 1899.00,
    totalValue: 9495.00,
    status: 'low-stock',
    minStock: 10,
    createdAt: '2024-01-14T15:30:00Z',
    updatedAt: '2024-01-16T09:15:00Z'
  },
  {
    id: '3',
    name: 'Coca Cola 500ml',
    description: 'Refreshing soft drink',
    category: 'Food & Beverages',
    sku: 'FOD-COC-001',
    quantity: 120,
    unitPrice: 15.00,
    totalValue: 1800.00,
    status: 'in-stock',
    minStock: 50,
    createdAt: '2024-01-10T08:00:00Z',
    updatedAt: '2024-01-16T14:20:00Z'
  },
  {
    id: '4',
    name: 'iPhone 13 Pro',
    description: 'Premium Apple smartphone',
    category: 'Electronics',
    sku: 'ELE-APP-001',
    quantity: 0,
    unitPrice: 21999.00,
    totalValue: 0.00,
    status: 'out-of-stock',
    minStock: 5,
    createdAt: '2024-01-12T11:45:00Z',
    updatedAt: '2024-01-16T16:00:00Z'
  }
]

const mockCategories: StockCategory[] = [
  { id: '1', name: 'Electronics', description: 'Electronic devices and accessories', itemCount: 45 },
  { id: '2', name: 'Clothing', description: 'Apparel and fashion items', itemCount: 78 },
  { id: '3', name: 'Food & Beverages', description: 'Food items and drinks', itemCount: 156 },
  { id: '4', name: 'Home & Garden', description: 'Home improvement and garden supplies', itemCount: 89 }
]

const mockOverview: StockOverview = {
  totalStockValue: 45750.00,
  totalItems: 567,
  lowStockItems: 7,
  totalCategories: 12,
  recentAdjustments: [
    {
      id: '1',
      stockItemId: '2',
      type: 'decrease',
      quantity: 5,
      reason: 'Sales',
      createdBy: 'system',
      createdAt: '2024-01-16T14:30:00Z'
    }
  ]
}

// Stock Service Class
export class StockService {
  /**
   * Get stock overview/summary
   */
  static async getOverview(): Promise<StockOverview> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return mockOverview
    }
    
    return await apiGet<StockOverview>(API_ENDPOINTS.stock)
  }
  
  /**
   * Get all stock items with optional filtering
   */
  static async getStockItems(params?: {
    category?: string
    status?: string
    search?: string
    page?: number
    limit?: number
  }): Promise<StockItem[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      let items = [...mockStockItems]
      
      if (params?.category) {
        items = items.filter(item => item.category === params.category)
      }
      
      if (params?.status) {
        items = items.filter(item => item.status === params.status)
      }
      
      if (params?.search) {
        const search = params.search.toLowerCase()
        items = items.filter(item => 
          item.name.toLowerCase().includes(search) ||
          item.description.toLowerCase().includes(search) ||
          item.sku.toLowerCase().includes(search)
        )
      }
      
      return items
    }
    
    return await apiGet<StockItem[]>(API_ENDPOINTS.stockItems, {
      query: params
    })
  }
  
  /**
   * Get a single stock item by ID
   */
  static async getStockItem(id: string): Promise<StockItem> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const item = mockStockItems.find(item => item.id === id)
      if (!item) {
        throw new Error(`Stock item with ID ${id} not found`)
      }
      return item
    }
    
    return await apiGet<StockItem>(`${API_ENDPOINTS.stockItems}/${id}`)
  }
  
  /**
   * Create a new stock item
   */
  static async createStockItem(data: CreateStockItemRequest): Promise<StockItem> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const newItem: StockItem = {
        id: Date.now().toString(),
        ...data,
        totalValue: data.quantity * data.unitPrice,
        status: data.quantity <= data.minStock ? 'low-stock' : 
               data.quantity === 0 ? 'out-of-stock' : 'in-stock',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
      mockStockItems.push(newItem)
      return newItem
    }
    
    return await apiPost<StockItem>(API_ENDPOINTS.stockItems, data)
  }
  
  /**
   * Update an existing stock item
   */
  static async updateStockItem(data: UpdateStockItemRequest): Promise<StockItem> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const index = mockStockItems.findIndex(item => item.id === data.id)
      if (index === -1) {
        throw new Error(`Stock item with ID ${data.id} not found`)
      }
      
      const updatedItem: StockItem = {
        ...mockStockItems[index],
        ...data,
        totalValue: (data.quantity || mockStockItems[index].quantity) * 
                   (data.unitPrice || mockStockItems[index].unitPrice),
        updatedAt: new Date().toISOString()
      }
      
      // Update status based on quantity
      if (updatedItem.quantity === 0) {
        updatedItem.status = 'out-of-stock'
      } else if (updatedItem.quantity <= updatedItem.minStock) {
        updatedItem.status = 'low-stock'
      } else {
        updatedItem.status = 'in-stock'
      }
      
      mockStockItems[index] = updatedItem
      return updatedItem
    }
    
    return await apiPut<StockItem>(`${API_ENDPOINTS.stockItems}/${data.id}`, data)
  }
  
  /**
   * Delete a stock item
   */
  static async deleteStockItem(id: string): Promise<void> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const index = mockStockItems.findIndex(item => item.id === id)
      if (index === -1) {
        throw new Error(`Stock item with ID ${id} not found`)
      }
      mockStockItems.splice(index, 1)
      return
    }
    
    return await apiDelete(`${API_ENDPOINTS.stockItems}/${id}`)
  }
  
  /**
   * Adjust stock quantity
   */
  static async adjustStock(data: StockAdjustmentRequest): Promise<StockAdjustment> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const adjustment: StockAdjustment = {
        id: Date.now().toString(),
        ...data,
        createdBy: 'current-user',
        createdAt: new Date().toISOString()
      }
      
      // Update the stock item
      const item = mockStockItems.find(item => item.id === data.stockItemId)
      if (item) {
        if (data.type === 'increase') {
          item.quantity += data.quantity
        } else if (data.type === 'decrease') {
          item.quantity = Math.max(0, item.quantity - data.quantity)
        } else {
          item.quantity = data.quantity
        }
        
        item.totalValue = item.quantity * item.unitPrice
        item.status = item.quantity === 0 ? 'out-of-stock' :
                     item.quantity <= item.minStock ? 'low-stock' : 'in-stock'
        item.updatedAt = new Date().toISOString()
      }
      
      return adjustment
    }
    
    return await apiPost<StockAdjustment>(API_ENDPOINTS.stockAdjustments, data)
  }
  
  /**
   * Get stock categories
   */
  static async getCategories(): Promise<StockCategory[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return mockCategories
    }
    
    return await apiGet<StockCategory[]>(API_ENDPOINTS.stockCategories)
  }
  
  /**
   * Get stock adjustments history
   */
  static async getAdjustments(params?: {
    stockItemId?: string
    type?: string
    page?: number
    limit?: number
  }): Promise<StockAdjustment[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return mockOverview.recentAdjustments
    }
    
    return await apiGet<StockAdjustment[]>(API_ENDPOINTS.stockAdjustments, {
      query: params
    })
  }
}

// Export default service
export default StockService
