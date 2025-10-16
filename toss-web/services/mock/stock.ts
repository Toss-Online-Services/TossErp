/**
 * Mock Stock/Inventory Data Service
 * Simulates inventory management for offline and demo mode
 */

export interface StockItem {
  id: number
  name: string
  sku: string
  category: string
  currentStock: number
  minStock: number
  maxStock?: number
  unitPrice: number
  costPrice: number
  warehouseId: number
  warehouseName: string
  lastUpdated: Date
  supplier?: string
  barcode?: string
}

export interface StockMovement {
  id: number
  itemId: number
  itemName: string
  type: 'IN' | 'OUT' | 'TRANSFER' | 'ADJUSTMENT'
  quantity: number
  fromWarehouse?: string
  toWarehouse?: string
  date: Date
  reference: string
  user: string
  notes?: string
}

export interface Warehouse {
  id: number
  name: string
  location: string
  capacity: number
  currentUtilization: number
  manager: string
  phone: string
  isActive: boolean
}

export const mockStockItems: StockItem[] = [
  { id: 1, name: 'Coca Cola 2L', sku: 'BEV-001', category: 'Beverages', currentStock: 24, minStock: 12, unitPrice: 35.00, costPrice: 28.00, warehouseId: 1, warehouseName: 'Main Warehouse', lastUpdated: new Date(), supplier: 'Coca Cola Beverages SA' },
  { id: 2, name: 'White Bread 700g', sku: 'GRO-001', category: 'Groceries', currentStock: 15, minStock: 20, unitPrice: 18.00, costPrice: 14.00, warehouseId: 1, warehouseName: 'Main Warehouse', lastUpdated: new Date(), supplier: 'Albany Bakeries' },
  { id: 3, name: 'Milk 1L', sku: 'GRO-002', category: 'Groceries', currentStock: 12, minStock: 15, unitPrice: 22.00, costPrice: 18.50, warehouseId: 1, warehouseName: 'Main Warehouse', lastUpdated: new Date(), supplier: 'Clover SA' },
  { id: 4, name: 'Simba Chips 125g', sku: 'SNK-001', category: 'Snacks', currentStock: 30, minStock: 24, unitPrice: 12.00, costPrice: 9.00, warehouseId: 1, warehouseName: 'Main Warehouse', lastUpdated: new Date(), supplier: 'Simba' },
  { id: 5, name: 'Sunlight Soap 250g', sku: 'HOU-001', category: 'Household', currentStock: 20, minStock: 15, unitPrice: 15.00, costPrice: 11.50, warehouseId: 2, warehouseName: 'Store Front', lastUpdated: new Date(), supplier: 'Unilever SA' },
  { id: 6, name: 'Maggi 2-Minute Noodles', sku: 'GRO-003', category: 'Groceries', currentStock: 48, minStock: 30, unitPrice: 8.00, costPrice: 6.20, warehouseId: 1, warehouseName: 'Main Warehouse', lastUpdated: new Date(), supplier: 'Nestle SA' },
  { id: 7, name: 'Castle Lager 440ml', sku: 'BEV-002', category: 'Beverages', currentStock: 18, minStock: 24, unitPrice: 28.00, costPrice: 22.00, warehouseId: 1, warehouseName: 'Main Warehouse', lastUpdated: new Date(), supplier: 'SAB Miller' },
  { id: 8, name: 'Purity Baby Food', sku: 'GRO-004', category: 'Groceries', currentStock: 10, minStock: 12, unitPrice: 25.00, costPrice: 19.50, warehouseId: 2, warehouseName: 'Store Front', lastUpdated: new Date(), supplier: 'Purity' },
  { id: 9, name: 'Colgate Toothpaste', sku: 'PER-001', category: 'Personal Care', currentStock: 8, minStock: 10, unitPrice: 32.00, costPrice: 24.00, warehouseId: 2, warehouseName: 'Store Front', lastUpdated: new Date(), supplier: 'Colgate-Palmolive' },
  { id: 10, name: 'Frozen Chicken 1kg', sku: 'FRO-001', category: 'Frozen', currentStock: 6, minStock: 8, unitPrice: 65.00, costPrice: 52.00, warehouseId: 3, warehouseName: 'Cold Storage', lastUpdated: new Date(), supplier: 'Rainbow Chicken' }
]

export const mockStockMovements: StockMovement[] = [
  { id: 1, itemId: 1, itemName: 'Coca Cola 2L', type: 'OUT', quantity: 6, toWarehouse: 'Customer Sale', date: new Date(), reference: 'SALE-001', user: 'John Doe', notes: 'Cash sale' },
  { id: 2, itemId: 4, itemName: 'Simba Chips 125g', type: 'IN', quantity: 20, fromWarehouse: 'Supplier', date: new Date(Date.now() - 1800000), reference: 'PO-2024-001', user: 'Admin', notes: 'Stock replenishment' },
  { id: 3, itemId: 2, itemName: 'White Bread 700g', type: 'OUT', quantity: 8, toWarehouse: 'Customer Sale', date: new Date(Date.now() - 3600000), reference: 'SALE-002', user: 'Jane Smith', notes: 'Wholesale order' },
  { id: 4, itemId: 5, itemName: 'Sunlight Soap 250g', type: 'IN', quantity: 15, fromWarehouse: 'Supplier', date: new Date(Date.now() - 5400000), reference: 'PO-2024-002', user: 'Admin', notes: 'Monthly stock' },
  { id: 5, itemId: 3, itemName: 'Milk 1L', type: 'OUT', quantity: 4, toWarehouse: 'Customer Sale', date: new Date(Date.now() - 7200000), reference: 'SALE-003', user: 'John Doe', notes: 'Regular customer' }
]

export const mockWarehouses: Warehouse[] = [
  { id: 1, name: 'Main Warehouse', location: 'Soweto Central', capacity: 10000, currentUtilization: 6500, manager: 'Thabo Mbeki', phone: '+27 82 123 4567', isActive: true },
  { id: 2, name: 'Store Front', location: 'Alexandra Township', capacity: 2000, currentUtilization: 1200, manager: 'Sarah Nkosi', phone: '+27 83 234 5678', isActive: true },
  { id: 3, name: 'Cold Storage', location: 'Diepsloot', capacity: 5000, currentUtilization: 800, manager: 'Michael Dlamini', phone: '+27 84 345 6789', isActive: true }
]

export class MockStockService {
  static getAllItems(): StockItem[] {
    return mockStockItems
  }

  static getItemById(id: number): StockItem | undefined {
    return mockStockItems.find(item => item.id === id)
  }

  static getLowStockItems(): StockItem[] {
    return mockStockItems.filter(item => item.currentStock <= item.minStock)
  }

  static getStockLevels() {
    return mockStockItems.map(item => ({
      ...item,
      quantityAvailable: item.currentStock,
      reorderPoint: item.minStock,
      productId: item.id,
      productName: item.name,
      productSku: item.sku,
      isLowStock: item.currentStock <= item.minStock
    }))
  }

  static getMovements(): StockMovement[] {
    return mockStockMovements
  }

  static getWarehouses(): Warehouse[] {
    return mockWarehouses
  }

  static getWarehouseById(id: number): Warehouse | undefined {
    return mockWarehouses.find(w => w.id === id)
  }

  static addStockMovement(movement: Omit<StockMovement, 'id'>): StockMovement {
    const newMovement = {
      ...movement,
      id: mockStockMovements.length + 1
    }
    mockStockMovements.unshift(newMovement)
    
    // Update stock levels
    const item = mockStockItems.find(i => i.id === movement.itemId)
    if (item) {
      if (movement.type === 'IN') {
        item.currentStock += movement.quantity
      } else if (movement.type === 'OUT') {
        item.currentStock -= movement.quantity
      }
      item.lastUpdated = new Date()
    }
    
    return newMovement
  }

  static updateItemStock(itemId: number, quantity: number, type: 'IN' | 'OUT'): boolean {
    const item = mockStockItems.find(i => i.id === itemId)
    if (!item) return false
    
    if (type === 'IN') {
      item.currentStock += quantity
    } else {
      item.currentStock -= quantity
    }
    item.lastUpdated = new Date()
    return true
  }

  static getTotalValue(): number {
    return mockStockItems.reduce((sum, item) => sum + (item.currentStock * item.costPrice), 0)
  }

  static getItemsByCategory(category: string): StockItem[] {
    return mockStockItems.filter(item => item.category === category)
  }
}

