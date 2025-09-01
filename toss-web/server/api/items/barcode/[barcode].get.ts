import { defineEventHandler, getRouterParam, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const barcode = getRouterParam(event, 'barcode')
  if (!barcode) throw createError({ statusCode: 400, statusMessage: 'barcode required' })
  // Demo: map known barcode to item
  if (barcode === '6001234567890') {
    return {
      id: '2', tenantId: 'tenant1', sku: 'MILK-001', name: 'Fresh Milk 1L', description: 'Fresh full cream milk',
      category: 'Dairy', unit: 'liter', sellingPrice: 25.0, costPrice: 18.0, reorderLevel: 25, reorderQty: 30,
      isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T08:15:00Z', quantityOnHand: 15
    }
  }
  throw createError({ statusCode: 404, statusMessage: 'Item not found for barcode' })
})


