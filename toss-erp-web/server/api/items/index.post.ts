import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<any>(event)
  if (!body?.sku || !body?.name || !body?.category || !body?.unit || typeof body?.sellingPrice !== 'number') {
    throw createError({ statusCode: 400, statusMessage: 'sku, name, category, unit, sellingPrice required' })
  }
  return {
    id: Math.random().toString(36).slice(2),
    tenantId: 'tenant1',
    ...body,
    isActive: body.isActive ?? true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
    quantityOnHand: 0,
  }
})


