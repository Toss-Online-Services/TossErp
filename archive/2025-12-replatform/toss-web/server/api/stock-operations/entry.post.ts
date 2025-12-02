import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ itemId?: string; warehouseId?: string; movementType?: string; quantity?: number }>(event)
  if (!body?.itemId || !body?.warehouseId || !body?.movementType || !body?.quantity) {
    throw createError({ statusCode: 400, statusMessage: 'itemId, warehouseId, movementType, quantity required' })
  }
  return { success: true, reference: `ENT-${Math.random().toString(36).slice(2)}` }
})


