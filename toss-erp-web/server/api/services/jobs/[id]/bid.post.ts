import { defineEventHandler, getRouterParam, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  const body = await readBody<{ providerId?: string; amount?: number; note?: string }>(event)
  if (!id || !body?.providerId || !body?.amount) {
    throw createError({ statusCode: 400, statusMessage: 'job id, providerId and amount required' })
  }
  return {
    jobId: id,
    providerId: body.providerId,
    amount: body.amount,
    note: body.note,
    status: 'submitted',
    createdAt: new Date().toISOString(),
  }
})


