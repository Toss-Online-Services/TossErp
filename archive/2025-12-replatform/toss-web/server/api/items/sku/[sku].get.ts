import { defineEventHandler, getRouterParam, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const sku = getRouterParam(event, 'sku')
  const item = sku === 'BREAD-001' ? {
    id: '1', tenantId: 'tenant1', sku: 'BREAD-001', name: 'White Bread Loaf', description: 'Fresh white bread loaf',
    category: 'Bakery', unit: 'loaf', sellingPrice: 12.0, costPrice: 8.5, reorderLevel: 20, reorderQty: 50,
    isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T10:30:00Z', quantityOnHand: 45
  } : undefined
  if (!item) throw createError({ statusCode: 404, statusMessage: 'Item not found for sku' })
  return item
})


