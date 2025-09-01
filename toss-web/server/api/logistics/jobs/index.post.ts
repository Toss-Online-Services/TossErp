import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ pickup?: any; dropoff?: any; weightKg?: number; reference?: string }>(event)
  if (!body?.pickup || !body?.dropoff || !body?.weightKg) {
    throw createError({ statusCode: 400, statusMessage: 'pickup, dropoff, weightKg are required' })
  }

  return {
    id: `job-${Math.random().toString(36).slice(2)}`,
    status: 'open',
    pickup: body.pickup,
    dropoff: body.dropoff,
    weightKg: body.weightKg,
    payout: Math.max(80, Math.round((body.weightKg || 1) * 2)),
    reference: body.reference,
    createdAt: new Date().toISOString(),
  }
})


