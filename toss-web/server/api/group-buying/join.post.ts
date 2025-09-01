import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ opportunityId?: string; quantity?: number }>(event)
  if (!body?.opportunityId || !body?.quantity || body.quantity <= 0) {
    throw createError({ statusCode: 400, statusMessage: 'opportunityId and positive quantity are required' })
  }

  // Pretend we joined successfully
  return {
    success: true,
    opportunityId: body.opportunityId,
    joinedQty: body.quantity,
    message: 'Joined group purchase successfully',
  }
})

