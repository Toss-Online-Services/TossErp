import { defineEventHandler, getQuery } from 'h3'

export default defineEventHandler(async (event) => {
  const items = [
    {
      id: '3', tenantId: 'tenant1', sku: 'RICE-001', name: 'Basmati Rice 2kg', category: 'Grains', unit: 'kg',
      sellingPrice: 48.0, costPrice: 35.0, reorderLevel: 10, reorderQty: 20, isActive: true,
      createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-14T16:45:00Z', quantityOnHand: 8
    },
    {
      id: '2', tenantId: 'tenant1', sku: 'MILK-001', name: 'Fresh Milk 1L', category: 'Dairy', unit: 'liter',
      sellingPrice: 25.0, costPrice: 18.0, reorderLevel: 25, reorderQty: 30, isActive: true,
      createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T08:15:00Z', quantityOnHand: 15
    }
  ]
  return { items, totalCount: items.length }
})


