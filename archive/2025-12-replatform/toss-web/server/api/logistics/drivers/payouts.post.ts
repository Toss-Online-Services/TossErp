import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ driverId?: string; amount?: number; method?: string }>(event)
  if (!body?.driverId || !body?.amount || body.amount <= 0) {
    throw createError({ statusCode: 400, statusMessage: 'driverId and positive amount are required' })
  }
  return {
    id: `payout-${Math.random().toString(36).slice(2)}`,
    driverId: body.driverId,
    amount: body.amount,
    method: body.method ?? 'eft',
    status: 'processing',
    createdAt: new Date().toISOString(),
  }
})


