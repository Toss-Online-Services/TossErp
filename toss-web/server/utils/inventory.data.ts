import type { SKUItem, StockMovementInput } from '../../types/inventory'

// Temporary in-memory dataset (replace with DB later)
const DATA: SKUItem[] = [
  {
    id: '1', tenantId: 'tenant1', sku: 'BREAD-001', name: 'White Bread Loaf', description: 'Fresh white bread loaf',
    category: 'Bakery', unit: 'loaf', sellingPrice: 12, costPrice: 8.5, reorderLevel: 20, reorderQty: 50,
    isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T10:30:00Z', quantityOnHand: 45
  },
  {
    id: '2', tenantId: 'tenant1', sku: 'MILK-001', name: 'Fresh Milk 1L', description: 'Fresh full cream milk',
    category: 'Dairy', unit: 'liter', sellingPrice: 25, costPrice: 18, reorderLevel: 25, reorderQty: 30,
    isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T08:15:00Z', quantityOnHand: 15
  },
  {
    id: '3', tenantId: 'tenant1', sku: 'RICE-001', name: 'Basmati Rice 2kg', description: 'Premium basmati rice',
    category: 'Grains', unit: 'kg', sellingPrice: 48, costPrice: 35, reorderLevel: 10, reorderQty: 20,
    isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-14T16:45:00Z', quantityOnHand: 8
  }
]

export const getAll = () => [...DATA]
export const getById = (id: string) => DATA.find(d => d.id === id) || null
export const addItem = (item: SKUItem) => { DATA.push(item); return item }
export const applyMovement = (movement: StockMovementInput) => {
  const item = DATA.find(d => d.id === movement.itemId)
  if (!item) return null
  const qty = item.quantityOnHand || 0
  item.quantityOnHand = qty + (movement.quantity || 0)
  item.updatedAt = new Date().toISOString()
  return item
}
