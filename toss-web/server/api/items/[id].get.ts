import { defineEventHandler, getRouterParam, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  const all = await (await import('../index.get')).default({} as any)
  // fallback simple lookup on the static mock
  const items = [
    {
      id: '1', tenantId: 'tenant1', sku: 'BREAD-001', name: 'White Bread Loaf', description: 'Fresh white bread loaf',
      category: 'Bakery', unit: 'loaf', sellingPrice: 12.0, costPrice: 8.5, reorderLevel: 20, reorderQty: 50,
      isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T10:30:00Z', quantityOnHand: 45
    },
    { id: '2', tenantId: 'tenant1', sku: 'MILK-001', name: 'Fresh Milk 1L', description: 'Fresh full cream milk',
      category: 'Dairy', unit: 'liter', sellingPrice: 25.0, costPrice: 18.0, reorderLevel: 25, reorderQty: 30,
      isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T08:15:00Z', quantityOnHand: 15 },
  ]
  const item = items.find(i => i.id === id)
  if (!item) throw createError({ statusCode: 404, statusMessage: 'Item not found' })
  return item
})


